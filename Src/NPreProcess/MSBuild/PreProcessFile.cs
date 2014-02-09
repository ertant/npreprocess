using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace NPreProcess.MSBuild
{
    public class PreProcessFile : Task
    {
        public PreProcessFile()
        {
            this.Type = "JS";
        }

        [Required]
        public ITaskItem[] InputFiles { get; set; }

        public string Type { get; set; }

        public string Properties { get; set; }

        public override bool Execute()
        {
            this.Log.LogMessage(MessageImportance.Normal, "Processing {0} file", this.InputFiles.Length);

            var context = Context.FromEnvironment();

            if (!string.IsNullOrEmpty(this.Properties))
            {
                foreach (var property in this.Properties.Split(';'))
                {
                    var parts = property.Split('=');

                    context[parts[0]] = parts.Length > 1 ? parts[1] : parts[0];
                }
            }

            var files = this.InputFiles.ToArray();

            files.AsParallel().ForAll(delegate(ITaskItem input)
            {
                this.Log.LogMessage(MessageImportance.Low, "Processing " + input.ItemSpec);

                var output = PreProcessor.PreProcessFile(context, input.ItemSpec, this.Type);

                if (output != null)
                    System.IO.File.WriteAllText(input.ItemSpec, output, Encoding.UTF8);
            });

            return true;
        }
    }
}
