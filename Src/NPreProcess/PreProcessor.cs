using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NPreProcess
{
    public static class PreProcessor
    {
        public static void PreProcessFileTo(Context context, string inputFilePath, string outputFilePath, string type = null)
        {
            if (string.IsNullOrEmpty(type))
            {
                type = System.IO.Path.GetExtension(inputFilePath).Substring(1).ToUpperInvariant();
            }

            var outputContent = PreProcessFile(context, inputFilePath, type);

            if (outputContent != null)
                System.IO.File.WriteAllText(outputFilePath, outputContent);
        }

        public static string PreProcessFile(Context context, string inputFilePath, string type = null)
        {
            if (string.IsNullOrEmpty(type))
            {
                type = System.IO.Path.GetExtension(inputFilePath).Substring(1).ToUpperInvariant();
            }

            string fileContent = System.IO.File.ReadAllText(inputFilePath);

            string processedContent = PreProcess(context, fileContent, type);

            return string.Equals(fileContent, processedContent) ? null : processedContent;
        }

        public static string PreProcess(Context context, string content, string type)
        {
            TypeDefinition fileType = definitions[type];

            content = fileType.Include.Definition.Replace(content, delegate(Match match)
            {
                var fileName = match.Groups[1].Value;

                var includedFileContent = System.IO.File.ReadAllText(fileName);

                return PreProcess(context, includedFileContent, type);
            });

            content = fileType.Exclude.Definition.Replace(content, delegate(Match match)
            {
                return context.Test(match.Groups[1].Value) ? "" : match.Groups[2].Value.Trim();
            });

            content = fileType.IfDef.Definition.Replace(content, delegate(Match match)
            {
                var test = match.Groups[1].Value.Trim();

                return context.Contains(test) ? match.Groups[2].Value.Trim() : "";
            });

            content = fileType.IfNDef.Definition.Replace(content, delegate(Match match)
            {
                var test = match.Groups[1].Value.Trim();

                return context.Contains(test) ? "" : match.Groups[2].Value.Trim();
            });

            content = fileType.If.Definition.Replace(content, delegate(Match match)
            {
                return context.Test(match.Groups[1].Value) ? match.Groups[2].Value.Trim() : "";
            });

            content = fileType.Echo.Definition.Replace(content, delegate(Match match)
            {
                var key = match.Value.Trim();

                return context.Contains(key) ? context[key].Trim() : "";
            });

            return content;
        }

        private static Definitions definitions = new Definitions();

        class Definitions
        {
            private Dictionary<string, TypeDefinition> types = new Dictionary<string, TypeDefinition>();

            public Definitions()
            {
                types["JS"] = new TypeDefinition()
                {
                    Echo = new RegexRule(@"(?://|/\\*)[ \t]*@echo[ \t]*([^\n*]*)[ \t]*(?:\\*/)?"),
                    Include = new RegexRule(@"(?://|/\\*)[ \t]*@include[ \t]*([^\n*]*)[ \t]*(?:\\*/)?"),
                    Exclude = new RegexRule(@"(?://|/\\*)[ \t]*@exclude[ \t]*([^\n*]*)[ \t]*(?:\\*/)?", @"(?://|/\\*)[ \t]*@endexclude[ \t]*(?:\\*/)?"),
                    If = new RegexRule(@"(?://|/\\*)[ \t]*@if[ \t]*([^\n*]*)[ \t]*(?:\\*/)?", @"(?://|/\\*)[ \t]*@endif[ \t]*(?:\\*/)?"),
                    IfDef = new RegexRule(@"(?://|/\\*)[ \t]*@ifdef[ \t]*([^\n*]*)[ \t]*(?:\\*/)?", @"(?://|/\\*)[ \t]*@endif[ \t]*(?:\\*/)?"),
                    IfNDef = new RegexRule(@"(?://|/\\*)[ \t]*@ifndef[ \t]*([^\n*]*)[ \t]*(?:\\*/)?", @"(?://|/\\*)[ \t]*@endif[ \t]*(?:\\*/)?")
                };

                types["HTML"] = new TypeDefinition()
                {
                    Echo = new RegexRule(@"<!--[ \t]*@echo[ \t]*([^\n-]*)[ \t]*-->"),
                    Include = new RegexRule(@"<!--[ \t]*@include[ \t]*([^\n]*)[ \t]*-->"),
                    Exclude = new RegexRule(@"<!--[ \t]*@exclude[ \t]*([^\n]*)[ \t]*-->", @"<!--[ \t]*@endexclude[ \t]*-->"),
                    If = new RegexRule(@"<!--[ \t]*@if[ \t]*(.*?)(?:(?!-->))[ \t]*-->", @"<!--[ \t]*@endif[ \t]*-->"),
                    IfDef = new RegexRule(@"<!--[ \t]*@ifdef[ \t]*(.*?)(?:(?!-->))[ \t]*-->", @"<!--[ \t]*@endif[ \t]*-->"),
                    IfNDef = new RegexRule(@"<!--[ \t]*@ifndef[ \t]*(.*?)(?:(?!-->))[ \t]*-->", @"<!--[ \t]*@endif[ \t]*-->")
                };

                types["CSS"] = types["JS"];
                types["C"] = types["JS"];
                types["CPP"] = types["JS"];
                types["CS"] = types["JS"];
                types["JAVA"] = types["JS"];
                types["CSS"] = types["JS"];
                types["PHP"] = types["JS"];

                types["XMl"] = types["HTML"];
                types["HTM"] = types["HTML"];
            }

            public TypeDefinition this[string type]
            {
                get
                {
                    TypeDefinition fileType;

                    if (!this.types.TryGetValue(type, out fileType))
                    {
                        throw new Exception("Unknown file type.");
                    }

                    return fileType;
                }
            }
        }

        class TypeDefinition
        {
            public RegexRule Echo { get; set; }
            public RegexRule Include { get; set; }
            public RegexRule If { get; set; }
            public RegexRule Exclude { get; set; }
            public RegexRule IfDef { get; set; }
            public RegexRule IfNDef { get; set; }
        }

        class RegexRule
        {
            public RegexRule(string def)
            {
                this.Definition = new Regex(def, RegexOptions.ECMAScript);
            }

            public RegexRule(string start, string end)
            {
                this.Definition = new Regex(start + "((?:.|\\n|\\r)*?)" + end, RegexOptions.ECMAScript);
            }

            public Regex Definition { get; private set; }
        }
    }
}
