using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NPreProcess
{
    [TestFixture]
    public class UnitTests
    {
        private string Process(string input, string type)
        {
            var context = new Context();

            context["NODE_ENV"] = "production";

            return PreProcessor.PreProcess(context, input, type);
        }

        [Test]
        [TestCase(@"//@if NODE_ENV='production'
function()
//@endif", "JS", @"function()")]
        [TestCase(@"//@if NODE_ENV='test'
function()
//@endif", "JS", @"")]
        [TestCase(@"//@if NODE_ENV!='test'
function()
//@endif", "JS", @"")]
        public void IfTests(string input, string type, string expected)
        {
            var result = Process(input, type);

            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCase(@"//@exclude NODE_ENV='production'
function()
//@endif", "JS", @"")]
        [TestCase(@"//@exclude NODE_ENV='test'
function()
//@endif", "JS", @"function()")]
        [TestCase(@"//@exclude
function()
//@endif", "JS", @"")]
        public void ExcludeTests(string input, string type, string expected)
        {
            var result = Process(input, type);

            Assert.AreEqual(expected, result);
        }


    }
}
