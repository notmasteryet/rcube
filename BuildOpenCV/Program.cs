using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.IO;

namespace BuildOpenCVProxy
{
    class Program
    {
        static string OutputPath;

        static string FileCore;
        static string FileCoreTypes;
        static string FileCv;
        static string FileCvType;
        static string FileHighGUI;

        const string CodeNamespaceName = "OpenCVProxy.Interop";

        static Program()
        {
            string openCVRoot = ConfigurationManager.AppSettings["OpenCVRoot"] ;

            openCVRoot = Environment.ExpandEnvironmentVariables(openCVRoot);
            
            FileCore = Path.Combine(openCVRoot, @"cxcore\include\cxcore.h");
            FileCoreTypes = Path.Combine(openCVRoot, @"cxcore\include\cxtypes.h");
            FileCv = Path.Combine(openCVRoot, @"cv\include\cv.h");
            FileCvType = Path.Combine(openCVRoot, @"cv\include\cvtypes.h");
            FileHighGUI = Path.Combine(openCVRoot, @"otherlibs\highgui\highgui.h");

            string outputPath = ConfigurationManager.AppSettings["OpenCVProxyDir"];
            OutputPath = Path.Combine(Path.GetFullPath(outputPath), @"Interop");
        }

        static void Main(string[] args)
        {
            HeaderConverter hc = new HeaderConverter();
            Console.WriteLine("Core");
            hc.Convert(FileCore, Path.Combine(OutputPath, "CxCore.Functions.cs"), CodeNamespaceName, "CxCore", "cxcore110.dll");
            hc.Convert(FileCoreTypes, Path.Combine(OutputPath, "CxCore.Constants.cs"), CodeNamespaceName, "CxCore");
            Console.WriteLine("Cv");
            hc.Convert(FileCv, Path.Combine(OutputPath, "Cv.Functions.cs"), CodeNamespaceName, "Cv", "cv110.dll");
            hc.Convert(FileCvType, Path.Combine(OutputPath, "Cv.Constants.cs"), CodeNamespaceName, "Cv");
            Console.WriteLine("Higui");
            hc.Convert(FileHighGUI, Path.Combine(OutputPath, "HighGui.Functions.cs"), CodeNamespaceName, "HighGui", "highgui110.dll");
        }
    }
}
