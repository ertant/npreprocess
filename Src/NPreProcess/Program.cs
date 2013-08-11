using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NPreProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Usage();
            }
            else
            {
                var exec = new Execution();

                foreach (var arg in args)
                {
                    if (arg.StartsWith("/out:"))
                    {
                        exec.OutputFile = arg.Substring(5);
                    }
                    else if (arg.StartsWith("/type:"))
                    {
                        exec.Type = arg.Substring(6);
                    }
                    else if (arg.StartsWith("/v:"))
                    {
                        var v = arg.Substring(3).Split('=');

                        exec.Context[v[0]] = v[1];
                    }
                    else
                    {
                        exec.InputFile = arg;
                    }
                }

                if (string.IsNullOrEmpty(exec.InputFile))
                {
                    Usage();
                }
                else
                {
                    if (string.IsNullOrEmpty(exec.OutputFile))
                    {
                        exec.OutputFile = Path.GetFileNameWithoutExtension(exec.InputFile) + ".processed" + Path.GetExtension(exec.InputFile);
                    }

                    try
                    {
                        PreProcessor.PreProcessFileTo(exec.Context, exec.InputFile, exec.OutputFile, exec.Type);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

            }
        }

        static void Usage()
        {
            Console.WriteLine("JS/HTML/CSS Preprocessor.");
            Console.WriteLine("Usage:");
            Console.WriteLine(" npreprocess inputFile [/out:outputFile] [/type:fileType] [/v:name=value]");
        }

        class Execution
        {
            public Execution()
            {
                this.Context = Context.FromEnvironment();
            }

            public string InputFile { get; set; }

            public string OutputFile { get; set; }

            public string Type { get; set; }

            public Context Context { get; set; }
        }

    }
}
