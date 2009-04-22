using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BuildOpenCVProxy
{
    class HeaderConverter
    {
        const string ProcSig = "CVAPI(";
        const string ProcSigEnd = ");";
        const string DefaultParamSig = "CV_DEFAULT(";
        const string ConstSig = "const";
        const string CommentStart = "/*";
        const string CommentEnd = "*/";

        static string ClassTemplate = File.ReadAllText("ClassTempl.txt");
        static string ClassTemplate2 = File.ReadAllText("ClassTempl2.txt");
        static string ClassTemplateEnd = "\t}\r\n}";

        public void Convert(string file, string output, string ns, string prefix)
        {
            string[] lines = File.ReadAllLines(file);
            using (StreamWriter w = new StreamWriter(output))
            {
                w.WriteLine(ClassTemplate2, prefix, ns);
                ExtractDefines(lines, w);
                w.WriteLine(ClassTemplateEnd);
            }
        }

        public void Convert(string file, string output, string ns, string prefix, string dllName)
        {
            string[] lines = File.ReadAllLines(file);

            using (StreamWriter w = new StreamWriter(output))
            {
                w.WriteLine(ClassTemplate, prefix, dllName, ns);
                ExtractDefines(lines, w);
                ExtractApiMethods(prefix, lines, w);
                w.WriteLine(ClassTemplateEnd);
            }
        }

        private void ExtractApiMethods(string prefix, string[] lines, StreamWriter w)
        {
            List<string> tempComment = null;
            string comment = null;

            int i = 0;
            while (i < lines.Length)
            {                                
                if (lines[i].StartsWith(ProcSig))
                {
                    string body = lines[i];
                    int j = i;
                    while (lines[i].IndexOf(ProcSigEnd) < 0)
                    {
                        i++;
                        body += lines[i];
                    }

                    ProcessBody(w, body, prefix, comment);
                    comment = null;
                }
                if (lines[i].IndexOf(CommentStart) >= 0)
                {
                    tempComment = new List<string>();
                    tempComment.Add("<summary>");
                }
                if (tempComment != null) tempComment.Add(lines[i]);
                if (lines[i].IndexOf(CommentEnd) >= 0)
                {
                    tempComment.Add("</summary>");
                    comment = String.Join("\r\n/// ", tempComment.ToArray());
                    comment = "/// " + comment.Replace(CommentStart, "").Replace(CommentEnd, "");
                    tempComment = null;
                }
                
                i++;
            }
        }

        const string DefineSig = "#define";
        static Regex defineValidator = new Regex(@"^#define\s+([A-Za-z0-9_]+)\s+((0x[A-F0-9]+)|([\d]+))\s*$");        
//        static Regex defineValidator = new Regex(@"^#define\s+([A-Za-z0-9_]+)\s+([^\\]+)$");
        static Regex bodyContent = new Regex(@"^[\(\)\|\-\+\&\<\>\sA-Za-z0-9_]+$");
        static Regex bodyWithNumberOrOperator = new Regex(@"\b((0x[A-Fa-f0-9]+)|(\d+))\b");
        static char[] bodyOperators = { '+', '|' };


        private static void ExtractDefines(string[] lines, StreamWriter w)
        {
            for (int q = 0; q < lines.Length; q++)
            {
                string line = lines[q];
                if (line.StartsWith(DefineSig))
                {
                    Match m = defineValidator.Match(line);
                    if (m.Success)
                    {
                        string body = m.Groups[2].Value;
                        if (bodyContent.IsMatch(body) && 
                            (bodyWithNumberOrOperator.IsMatch(body) || body.IndexOfAny(bodyOperators) >= 0 ))
                        {
                            if (body.StartsWith("0x") && body[2] >= '8')
                                body = "unchecked((int)" + body + ")";
                            w.WriteLine("\t\tpublic const int {0} = {1};",
                                m.Groups[1].Value, body);
                        }
                    }
                }
            }
        }

        void ProcessBody(StreamWriter w, string body, string prefix, string comment)
        {
            string methodName = "(unknown)";
            try
            {
                int i1 = body.IndexOf('(');
                int i2 = body.IndexOf(')', i1);
                int i3 = body.IndexOf('(', i2);
                int i4 = body.LastIndexOf(')');
                string returnType = body.Substring(i1 + 1, i2 - i1 - 1);
                methodName = body.Substring(i2 + 1, i3 - i2 - 1).Trim();

                string[] p = ProcessParameters(body.Substring(i3 + 1, i4 - i3 - 1));
                string ret = TypeConverter.Instance.GetConvertedType(returnType, 3);
                string returnAttr = null;
                if (ret.StartsWith("["))
                {
                    int j = ret.LastIndexOf(']');
                    returnAttr = ret.Substring(0, j + 1);
                    ret = ret.Substring(j + 1).TrimStart();
                }
                w.WriteLine();
                if(comment != null)
                {
                w.WriteLine(comment.Replace("///", "\t\t///"));
                }
                w.WriteLine("\t\t[DllImport({0}DllName, EntryPoint = \"{1}\", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]", prefix, methodName);
                if (returnAttr != null)
                {
                    w.WriteLine("\t\t" + returnAttr);
                }
                w.WriteLine("\t\tpublic static extern {0} {1}({2});",
                    ret, methodName, String.Join(", ", p));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Unable to process: {0} {1}", methodName, ex.Message);
            }
        }

        static Regex re = new Regex(@"\s+");

        string[] ProcessParameters(string parameters)
        {
            parameters = parameters.Trim();
            if (parameters == "void" || parameters.Length == 0)
                return new string[0];

            List<string> p = new List<string>();
            int pars = 0;
            int start = 0;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] == ',' && pars == 0)
                {
                    p.Add(ProcessParameter(parameters.Substring(start, i - start)));
                    start = i + 1;
                }
                else if (parameters[i] == '(')
                    pars++;
                else if (parameters[i] == ')')
                    pars--;
            }
            p.Add(ProcessParameter(parameters.Substring(start)));
            return p.ToArray();
        }

        string ProcessParameter(string t)
        {
            string[] parts = re.Split(t.Trim());
            int i = 0;
            int j = 0;
            while (j < parts.Length - 1 && !parts[j + 1].StartsWith(DefaultParamSig))
            {
                j++;
            }

            bool isConst = false;
            if (parts[i] == ConstSig) { i++; isConst = true; }
            string paramName = parts[j--];
            string type = parts[i++];
            while (i <= j) type += parts[i++];
            if (paramName.StartsWith("*"))
            {
                paramName = paramName.Substring(1);
                type += "*";
            }
            else if (paramName.EndsWith("]"))
            {
                paramName = paramName.Substring(0, paramName.IndexOf('['));
                isConst = true;
                type += "*";
            }

            return TypeConverter.Instance.GetConvertedType(type, isConst ? 2 : 1) +
                " " + paramName;
        }
    }
}
