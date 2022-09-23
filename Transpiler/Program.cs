//
// FixPointCS
//
// Copyright(c) Jere Sanisalo, Petri Kero
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
using System.IO;

namespace Transpiler
{
    class Program
    {
        const string InputPath      = "../FixPointCS";
        const string CppOutputPath  = "../Cpp";
        const string JavaOutputPath = "../Java";

        static void Main(string[] args)
        {
            // Generate C++ files
            GenerateCpp.ConvertFile(Path.Combine(InputPath, "FixedUtil.cs"), Path.Combine(CppOutputPath, "FixedUtil.h"), GenerateCpp.Mode.Util);
            GenerateCpp.ConvertFile(Path.Combine(InputPath, "Fixed32.cs"),   Path.Combine(CppOutputPath, "Fixed32.h"), GenerateCpp.Mode.Fp32);
            GenerateCpp.ConvertFile(Path.Combine(InputPath, "Fixed64.cs"),   Path.Combine(CppOutputPath, "Fixed64.h"), GenerateCpp.Mode.Fp64);

            // Generate Java files
            GenerateJava.ConvertFile(Path.Combine(InputPath, "FixedUtil.cs"), Path.Combine(JavaOutputPath, "FixedUtil.java"));
            GenerateJava.ConvertFile(Path.Combine(InputPath, "Fixed32.cs"),   Path.Combine(JavaOutputPath, "Fixed32.java"));
            GenerateJava.ConvertFile(Path.Combine(InputPath, "Fixed64.cs"),   Path.Combine(JavaOutputPath, "Fixed64.java"));
        }
    }
}
