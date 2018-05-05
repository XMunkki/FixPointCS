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

namespace GenerateCpp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Designed to be run from visual studio; paths fixed for that
            string in_file = "../../../FixPointCS/Fixed64.cs";
            string out_file = "../../../Cpp/Fixed64.h";

            // Read the file
            string[] source_lines = File.ReadAllLines(in_file);

            // Prepare output
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(g_prefix);

            // Process the lines
            bool output_enabled = true;

            foreach (string line_in in source_lines)
            {
                string line = line_in;
                string line_trimmed = line.Trim();

                // Poor mans preprocessor handling; ignore "#if !CPP" -blocks
                if (line_trimmed.StartsWith("#"))
                {
                    if (line_trimmed.StartsWith("#if !CPP"))
                        output_enabled = false;
                    if (line_trimmed.StartsWith("#else"))
                        output_enabled = !output_enabled;
                    if (line_trimmed.StartsWith("#endif"))
                        output_enabled = true;
                    continue;
                }

                if (!output_enabled)
                    continue;

                // Ignore [...] -lines (attributes)
                if (line_trimmed.StartsWith("["))
                    continue;

                // Undent for better looking output
                if (line.StartsWith("    "))
                    line = line.Substring(4);
                else if (line.StartsWith("\t"))
                    line = line.Substring(1);

                // Process the line
                line = ReplaceWholeWordCpp(line, "public", "");
                line = ReplaceWholeWordCpp(line, "private", "");
                line = ReplaceWholeWordCpp(line, "const", "static const");
                line = ReplaceWholeWordCpp(line, "int", "FP_INT");
                line = ReplaceWholeWordCpp(line, "long", "FP_LONG");
                line = ReplaceWholeWordCpp(line, "ulong", "FP_ULONG");
                line = ReplaceWholeWordCpp(line, "Debug.Assert", "FP_ASSERT");
                line = ReplaceWholeWordCpp(line, "Math.PI", "FP_PI");
                line = line.Replace("Util.", "");
                line = ConvertArrays(line);
                line = Convert64bitConstants(line);
                
                // Add the line
                sb.AppendLine(line);
            }

            // Finalize the output
            sb.AppendLine(g_suffix);
            string dest_text = sb.ToString();

            // Save the file
            File.WriteAllText(out_file, dest_text, Encoding.ASCII);
        }

        // Replaces all occurances of the whole word (int the c++ sense) with the given text
        private static string ReplaceWholeWordCpp(string str, string old_word, string new_word)
        {
            int old_len = old_word.Length;

            // Early exits
            if (old_len == 0)
                return str;
            if (!str.Contains(old_word))
                return str;

            StringBuilder sb = new StringBuilder();
            int search_pos = 0;

            while (true)
            {
                // Find the next hit
                search_pos = Math.Min(search_pos, str.Length); // Clamp the search area
                int next_hit = str.IndexOf(old_word, search_pos);

                // No more hits?
                if (next_hit == -1)
                {
                    sb.Append(str.Substring(search_pos));
                    return sb.ToString();
                }

                // Copy to the hit
                if (next_hit > search_pos)
                {
                    sb.Append(str.Substring(search_pos, next_hit - search_pos));
                    search_pos = next_hit;
                }

                // Check if the hit is on a word boundary
                bool is_ws_start = (next_hit == 0) || (IsWordBoundaryCpp(str[next_hit - 1]));
                bool is_ws_end = ((next_hit + old_len) == str.Length) || (IsWordBoundaryCpp(str[next_hit + old_len]));

                if (is_ws_start && is_ws_end)
                { // On a word boundary
                    if (new_word.Length > 0)
                    { // Fit the new word in
                        sb.Append(new_word);
                    }
                    else
                    { // Replacing with empty; skip the next whitespace in the source
                        search_pos += 1;
                    }
                }
                else
                { // Not on a word boundary
                    sb.Append(str.Substring(next_hit, old_len));
                }
                search_pos += old_len;
            }
        }

        // Tests if the character is a valid c++ identifier boundary character
        private static bool IsWordBoundaryCpp(char c)
        {
            if (Char.IsLetterOrDigit(c) || c == '_')
                return false;
            return true;
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

        // Prefix for the 
        private static string g_prefix = @"//
// GENERATED FILE!!!
//
// Generated from Fixed64.cs, part of the FixPointCS project (MIT license).
//
#pragma once
#ifndef __FIXED64_H
#define __FIXED64_H

// Include 64bit numeric types
#include <stdint.h>

// If FP_ASSERT is not custom-defined, then use the standard one
#ifndef FP_ASSERT
#include <assert.h>
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

    #ifndef FP_ASSERT
    #define FP_ASSERT(x) assert(x)
    #endif

    static const double FP_PI = 3.14159265359;
";

        private static string g_suffix = @"

    #undef FP_ASSERT
};
#endif
";
    }
}
