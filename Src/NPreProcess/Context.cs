using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NPreProcess
{
    public class Context
    {
        Dictionary<string, string> variables = new Dictionary<string, string>();

        public Context()
        {
        }

        public static Context FromEnvironment()
        {
            var context = new Context();

            foreach (System.Collections.DictionaryEntry pair in Environment.GetEnvironmentVariables())
            {
                context[Convert.ToString(pair.Key)] = Convert.ToString(pair.Value);
            }

            return context;
        }

        public string this[string key]
        {
            get
            {
                string value = "";

                variables.TryGetValue(key, out value);

                return value;
            }
            set
            {
                variables[key] = value;
            }
        }

        public bool Contains(string key)
        {
            return variables.ContainsKey(key);
        }

        Regex equality = new Regex("(.*[^=])=(.*[^=])", RegexOptions.ECMAScript);

        public bool Test(string input)
        {
            var match = equality.Match(input);

            if (match.Success)
            {
                var k1 = match.Groups[1].Value.Trim();
                var k2 = match.Groups[2].Value.Trim();

                k2 = k2.Trim(new[] { '\'', '\"' });

                return this[k1] == k2;
            }

            return true;
        }
    }
}
