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
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Transpiler
{
    public static class Util
    {
        public static string[] Preprocess(string[] lines, string languageDefine)
        {
            List<string> output = new List<string>();

            // Process the lines
            bool included = true;
            bool langChecked = false;
            foreach (string line in lines)
            {
                // Poor mans preprocessor handling
                if (line.StartsWith("#"))
                {
                    Debug.Assert(line == line.Trim(), "Must not have whitespace on preprocessor lines");
                    if (line == $"#if {languageDefine}" || line == $"#elif {languageDefine}")
                    {
                        included = true; // matching language block
                        langChecked = true;
                    }
                    else if (line.StartsWith("#if ") || line.StartsWith("#elif "))
                        included = false; // non-matching language block
                    else if (line == "#if !TRANSPILE")
                        included = false;
                    else if (line.StartsWith("#else"))
                        included = !langChecked;
                    else if (line.StartsWith("#endif"))
                    {
                        included = true;
                        langChecked = false;
                    }
                    else
                        Debug.Fail("Unrecognized preprocessor line: '{0}'", line);

                    continue;
                }

                if (!included)
                    continue;

                output.Add(line);
            }

            return output.ToArray();
        }

        public static string[] Unindent(string[] lines, int amount)
        {
            return lines.Select(line =>
            {
                for (int i = 0; i < amount; i++)
                {
                    if (line.StartsWith("    "))
                        line = line.Substring(4);
                    else if (line.StartsWith("\t"))
                        line = line.Substring(1);
                }

                return line;
            }).ToArray();
        }

        // Replaces all occurances of the whole word (in the C# sense) with the given text
        public static string ReplaceWholeWord(string str, string old_word, string new_word)
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
                bool is_ws_start = (next_hit == 0) || (IsWordBoundary(str[next_hit - 1]));
                bool is_ws_end = ((next_hit + old_len) == str.Length) || (IsWordBoundary(str[next_hit + old_len]));

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

        // Tests if the character is a valid C# identifier boundary character
        private static bool IsWordBoundary(char c)
        {
            if (Char.IsLetterOrDigit(c) || c == '_')
                return false;
            return true;
        }
    }
}