//
// FixPointCS
//
// Copyright(c) 2018 Jere Sanisalo, Petri Kero
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
    public static class GenerateCpp
    {
        public static void ConvertFile(string inPath, string outPath)
        {
            Console.WriteLine("Generating {0}..", outPath);

            // Read the file
            string[] lines = File.ReadAllLines(inPath);

            // Prepare output
            StringBuilder sb = new StringBuilder();

            // Run hacky preprocessor
            lines = Util.Preprocess(lines, "CPP");
            lines = Util.Unindent(lines, 1);

            foreach (string line_in in lines)
            {
                string line = line_in;
                string line_trimmed = line.Trim();

                // Ignore [...] -lines (attributes)
                if (line_trimmed.StartsWith("["))
                    continue;

                // Process the line
                line = Util.ReplaceWholeWord(line, "public", "");
                line = Util.ReplaceWholeWord(line, "private", "");
                line = Util.ReplaceWholeWord(line, "const", "static const");
                line = Util.ReplaceWholeWord(line, "out int", "int&");
                line = Util.ReplaceWholeWord(line, "out long", "long&");
                line = Util.ReplaceWholeWord(line, "out ulong", "ulong&");
                line = Util.ReplaceWholeWord(line, "out", "");
                line = Util.ReplaceWholeWord(line, "int", "FP_INT");
                line = Util.ReplaceWholeWord(line, "long", "FP_LONG");
                line = Util.ReplaceWholeWord(line, "ulong", "FP_ULONG");
                line = Util.ReplaceWholeWord(line, "Debug.Assert", "FP_ASSERT");
                line = line.Replace("Util.", "");
                line = ConvertArrays(line);
                line = Convert64bitConstants(line);
                line = Util.ReplaceWholeWord(line, "// PREFIX", g_prefix);
                line = Util.ReplaceWholeWord(line, "// SUFFIX", g_suffix);

                // Add the line
                sb.AppendLine(line);
            }

            // Finalize the output
            string dest_text = sb.ToString();

            // Save the file
            File.WriteAllText(outPath, dest_text, Encoding.ASCII);
        }

        private static string ConvertArrays(string str)
        {
            return Regex.Replace(str, "static readonly FP_INT\\[\\] ([a-zA-Z0-9_]+)", "static FP_INT $1[]");
        }

        // Converts all 64bit constants into C++ form.
        // A bit hacky, requires a leading white space
        private static string Convert64bitConstants(string str)
        {
            return Regex.Replace(str, " (-?0?x?[0-9a-fA-F]+)L", " INT64_C($1)", RegexOptions.Singleline);
        }

        // Prefix for the generated file
        private static string g_prefix = @"//
// GENERATED FILE!!!
//
// Generated from Fixed64.cs, part of the FixPointCS project (MIT license).
//
#pragma once
#ifndef __FIXED64_H
#define __FIXED64_H

// Include numeric types
#include <stdint.h>

// If FP_ASSERT is not custom-defined, then use the standard one
#ifndef FP_ASSERT
#   include <assert.h>
#   define FP_ASSERT(x) assert(x)
#endif

namespace Fixed64
{
    typedef int FP_INT;
    typedef unsigned int FP_UINT;
    typedef int64_t FP_LONG;
    typedef uint64_t FP_ULONG;

    static_assert(sizeof(FP_INT) == 4, ""Wrong bytesize for FP_INT"");
    static_assert(sizeof(FP_UINT) == 4, ""Wrong bytesize for FP_UINT"");
    static_assert(sizeof(FP_LONG) == 8, ""Wrong bytesize for FP_LONG"");
    static_assert(sizeof(FP_ULONG) == 8, ""Wrong bytesize for FP_ULONG"");
";

        private static string g_suffix = @"

    #undef FP_ASSERT
};
#endif // __FIXED64_H
";
    }
}
