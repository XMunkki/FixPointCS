//
// FixPointCS
//
// Copyright(c) 2018-2019 Jere Sanisalo, Petri Kero
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Transpiler
{
    public static class GenerateJava
    {
        public static void ConvertFile(string inPath, string outPath)
        {
            Console.WriteLine("Generating {0}..", outPath);

            // Read the file
            string[] lines = File.ReadAllLines(inPath);

            // Preprocess
            lines = Util.Preprocess(lines, "JAVA");
            lines = Util.Unindent(lines, 1);

            // Prepare output
            StringBuilder sb = new StringBuilder();

            // \todo [petri] actual conversion!
            foreach (string lineIn in lines)
            {
                string line = lineIn;
                string trimmed = line.Trim();

                // Ignore [...] -lines (attributes)
                if (trimmed.StartsWith("["))
                    continue;

                // Process line
                line = Util.ReplaceWholeWord(line, "// PREFIX", g_prefix);
                line = Util.ReplaceWholeWord(line, "// SUFFIX", g_suffix);
                line = Util.ReplaceWholeWord(line, "static class", "class");                    // static classes
                line = Util.ReplaceWholeWord(line, "static readonly", "static final");          // const arrays
                line = Util.ReplaceWholeWord(line, "public const", "public static final");      // global variables
                line = Util.ReplaceWholeWord(line, "private const", "private static final");    // global variables
                line = Util.ReplaceWholeWord(line, "const", "final");                           // method-local constants
                line = Util.ReplaceWholeWord(line, "Debug.Assert", "assert");
                line = line.Replace("Nlz((ulong)", "Nlz(");
                line = line.Replace("Nlz((uint)", "Nlz(");

                sb.AppendLine(line);
            }

            // Save the file
            File.WriteAllText(outPath, sb.ToString().Replace("\r", ""), Encoding.ASCII);
        }

        // Prefix for the generated file
        private static string g_prefix = @"//
// GENERATED FILE!!!
//
// Generated from Fixed64.cs, part of the FixPointCS project (MIT license).
//
";

        private static string g_suffix = @"// END GENERATED FILE";
    }
}
