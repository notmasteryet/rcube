using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BuildOpenCVProxy
{
    class TypeConverter
    {
        public static TypeConverter Instance = new TypeConverter();

        private Dictionary<string, string[]> d = new Dictionary<string, string[]>();

        private TypeConverter()
        {
            foreach (string line in File.ReadAllLines("Types.txt"))
            {
                string[] p = line.Trim().Split(',');
                if (p.Length == 4 && !p[0].StartsWith("#"))
                {
                    d.Add(p[0], p);
                }
            }
        }

        public string GetConvertedType(string name, int position)
        {
            string[] conv;
            string res = null;
            if (d.TryGetValue(name, out conv))
            {
                res = conv[position];
            }
            if (String.IsNullOrEmpty(res))
            {
                throw new Exception("Unknown type: " + name);
            }
            else
                return res;
        }
    }
}
