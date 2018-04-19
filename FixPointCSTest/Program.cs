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
using System.Text;
using System.Linq;
using System.Diagnostics;
using FixPointCS;

namespace FixPointCSTest
{
    using UnaryExecutor = Action<int, F64[], F64[]>;
    using BinaryExecutor = Action<int, F64[], F64[], F64[]>;
    using UnaryErrorEvaluator = Func<F64, F64, double, double>;
    using BinaryErrorEvaluator = Func<F64, F64, F64, double, double>;
    using ValueGenerator = Func<Random, int, double[]>;
    using BinaryInputGenerator = Tuple<Func<Random, int, double[]>, Func<Random, int, double[]>>;

    class ValueBounds
    {
        public static readonly double Max = 65536.0 * 65536.0 * 0.99;
        public static readonly double Min = -Max;
    }

    class Input
    {
        static public ValueGenerator Linear(double mn, double mx)
        {
            return (Random rnd, int count) =>
            {
                double[] values = new double[count];
                for (int i = 0; i < count; i++)
                    values[i] = mn + (mx - mn) * rnd.NextDouble();
                return values;
            };
        }

        static public ValueGenerator Exponential(double mn, double mx)
        {
            // \todo [petri] actually make exponential!
            return (Random rnd, int count) =>
            {
                double[] values = new double[count];
                for (int i = 0; i < count; i++)
                    values[i] = mn + (mx - mn) * rnd.NextDouble();
                return values;
            };
        }
    };

    class UnaryOp
    {
        public string                   name;
        public int                      numBenchmarkIters;
        public UnaryExecutor            arrayExecute;
        public Func<double, double>     refExecute;
        public UnaryErrorEvaluator      evaluateError;
        public ValueGenerator[]         inputGenerators;
        public double[]                 basicValues;

        // \todo [petri] way to express exact input-output values
        // \todo [petri] way to express monotonicity requirement

        public UnaryOp (string name, int numIters, UnaryExecutor arrExec, Func<double, double> refExec, UnaryErrorEvaluator evaluateError, ValueGenerator[] inputGenerators, double[] basicValues)
        {
            this.name               = name;
            this.numBenchmarkIters  = numIters;
            this.arrayExecute       = arrExec;
            this.refExecute         = refExec;
            this.evaluateError      = evaluateError;
            this.inputGenerators    = inputGenerators;
            this.basicValues        = basicValues;
        }
    }

    class BinaryOp
    {
        public string                       name;
        public int                          numBenchmarkIters;
        public BinaryExecutor               arrayExecute;
        public Func<double, double, double> refExecute;
        public BinaryErrorEvaluator         evaluateError;
        public BinaryInputGenerator[]       inputGenerators;
        public Tuple<double, double>[]      basicValues;

        // \todo [petri] way to express exact input-output values
        // \todo [petri] way to express monotonicity requirement

        public BinaryOp(string name, int numIters, BinaryExecutor arrExec, Func<double, double, double> refExec, BinaryErrorEvaluator evaluateError, BinaryInputGenerator[] inputGenerators, Tuple<double, double>[] basicValues)
        {
            this.name               = name;
            this.numBenchmarkIters  = numIters;
            this.arrayExecute       = arrExec;
            this.refExecute         = refExec;
            this.evaluateError      = evaluateError;
            this.inputGenerators    = inputGenerators;
            this.basicValues        = basicValues;
        }
    }

    class RegressionTest
    {
        static string DoubleToString(double val, int numChars)
        {
            return val.ToString("F" + numChars, System.Globalization.CultureInfo.InvariantCulture).Substring(0, numChars);
        }

        public static void UnaryBasicValues(UnaryOp op)
        {
            int numValues = op.basicValues.Length;
            if (numValues > 0)
            {
                F64[] inputs = new F64[numValues];
                F64[] outputs = new F64[numValues];
                double[] reference = new double[numValues];

                for (int i = 0; i < numValues; i++)
                {
                    F64 i0 = F64.FromDouble(op.basicValues[i]);
                    inputs[i] = i0;
                    reference[i] = op.refExecute(i0.Double);
                }

                op.arrayExecute(numValues, inputs, outputs);

                Console.WriteLine("{0,-16}    {1,12} {2,12} {3,12}", op.name, "reference", "result", "error");
                for (int i = 0; i < numValues; i++)
                {
                    double input = inputs[i].Double;
                    double res = outputs[i].Double;
                    double err = op.evaluateError(inputs[i], outputs[i], reference[i]);
                    Console.WriteLine("{0} -> {1} {2} {3}", DoubleToString(input, 16), DoubleToString(reference[i], 12), DoubleToString(res, 12), DoubleToString(err, 12));
                }
                Console.WriteLine();
            }
        }

        public static void BinaryBasicValues(BinaryOp op)
        {
            int numValues = op.basicValues.Length;
            if (numValues > 0)
            {
                F64[] inputs0 = new F64[numValues];
                F64[] inputs1 = new F64[numValues];
                F64[] outputs = new F64[numValues];
                double[] reference = new double[numValues];

                for (int i = 0; i < numValues; i++)
                {
                    F64 i0 = F64.FromDouble(op.basicValues[i].Item1);
                    F64 i1 = F64.FromDouble(op.basicValues[i].Item2);
                    inputs0[i] = i0;
                    inputs1[i] = i1;
                    reference[i] = op.refExecute(i0.Double, i1.Double);
                }

                op.arrayExecute(numValues, inputs0, inputs1, outputs);

                Console.WriteLine("{0,-16}                      {1,12} {2,12} {3,12}", op.name, "reference", "result", "error");
                for (int i = 0; i < numValues; i++)
                {
                    double input0 = inputs0[i].Double;
                    double input1 = inputs1[i].Double;
                    double res = outputs[i].Double;
                    double err = op.evaluateError(inputs0[i], inputs1[i], outputs[i], reference[i]);
                    Console.WriteLine("{0}, {1} -> {2} {3} {4}", DoubleToString(input0, 16), DoubleToString(input1, 16), DoubleToString(reference[i], 12), DoubleToString(res, 12), DoubleToString(err, 12));
                }
                Console.WriteLine();
            }
        }
    }

    class PrecisionTest
    {
        private static readonly int NUM_VALUES = 1024 * 1024;

        private static Random rnd = new Random();

        static string DoubleToString(double val, int numChars)
        {
            string str = val.ToString("F" + numChars, System.Globalization.CultureInfo.InvariantCulture);
            return str.Substring(0, Math.Min(str.Length, numChars));
        }

        public static void TestUnary(UnaryOp op)
        {
            int     numTested   = 0;
            double  totalErr    = 0.0;
            double  maxErr      = 0.0;
            F64     worstInput  = F64.Zero;

            foreach (ValueGenerator inputGenerator in op.inputGenerators)
            {
                F64[] inputs = inputGenerator(rnd, NUM_VALUES).Select(d => F64.FromDouble(d)).ToArray();
                F64[] outputs = new F64[NUM_VALUES];

                op.arrayExecute(NUM_VALUES, inputs, outputs);

                for (int i = 0; i < NUM_VALUES; i++)
                {
                    double input = inputs[i].Double;
                    double res = outputs[i].Double;
                    double reference = op.refExecute(input);

                    if (reference >= ValueBounds.Min && reference <= ValueBounds.Max)
                    {
                        double err = op.evaluateError(inputs[i], outputs[i], reference);
                        numTested++;
                        totalErr += err;

                        if (err >= maxErr)
                        {
                            maxErr = Math.Max(maxErr, err);
                            worstInput = inputs[i];
                        }
                    }
                }
            }

            Debug.Assert(numTested > 1000);
            double avgErr = totalErr / (double)numTested;
            if (maxErr == 0.0)
                Console.WriteLine("{0,16} {1,16} {2,16}", op.name, "exact", "exact");
            else
                Console.WriteLine("{0,16} {1} {2} [x={3}]", op.name, DoubleToString(totalErr / NUM_VALUES, 16), DoubleToString(maxErr, 16), DoubleToString(worstInput.Double, 16));
        }

        public static void TestBinary(BinaryOp op)
        {
            int             numTested   = 0;
            double          totalErr    = 0.0;
            double          maxErr      = 0.0;
            Tuple<F64, F64> worstInput  = Tuple.Create(F64.Zero, F64.Zero);

            foreach (BinaryInputGenerator inputGenerator in op.inputGenerators)
            {
                F64[] inputs0 = inputGenerator.Item1(rnd, NUM_VALUES).Select(d => F64.FromDouble(d)).ToArray();
                F64[] inputs1 = inputGenerator.Item2(rnd, NUM_VALUES).Select(d => F64.FromDouble(d)).ToArray();
                F64[] outputs = new F64[NUM_VALUES];

                op.arrayExecute(NUM_VALUES, inputs0, inputs1, outputs);

                for (int i = 0; i < NUM_VALUES; i++)
                {
                    double input0 = inputs0[i].Double;
                    double input1 = inputs1[i].Double;
                    double res = outputs[i].Double;
                    double reference = op.refExecute(input0, input1);

                    if (reference >= ValueBounds.Min && reference <= ValueBounds.Max)
                    {
                        double err = op.evaluateError(inputs0[i], inputs1[i], outputs[i], reference);
                        numTested++;
                        totalErr += err;

                        if (err >= maxErr)
                        {
                            maxErr = Math.Max(maxErr, err);
                            worstInput = Tuple.Create(inputs0[i], inputs1[i]);
                        }
                    }
                }
            }

            Debug.Assert(numTested > 1000);
            double avgErr = totalErr / (double)numTested;
            if (maxErr == 0.0)
                Console.WriteLine("{0,16} {1,16} {2,16}", op.name, "exact", "exact");
            else
                Console.WriteLine("{0,16} {1} {2} [a={3}, b={4}]", op.name, DoubleToString(totalErr / NUM_VALUES, 16), DoubleToString(maxErr, 16), DoubleToString(worstInput.Item1.Double, 16), DoubleToString(worstInput.Item2.Double, 16));
        }
    }

    class Benchmark
    {
        private const int CHUNK_SIZE = 128;
        private static Random rnd = new Random();

        private static double Measure(Action func)
        {
            // Measure execution time X times (pick fastest).
            const int NUM_ITERS = 7;
            double elapsed = Double.PositiveInfinity;
            for (int iter = 0; iter < NUM_ITERS; iter++)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var elapsedMs = watch.ElapsedMilliseconds;
                func();
                watch.Stop();
                elapsed = Math.Min(elapsed, watch.ElapsedMilliseconds / 1000.0);
            }

            return elapsed;
        }

        private static void MeasureReport(string opName, int numIters, Action func)
        {
            // Measure execution time.
            double elapsed = Measure(func);

            // Report result.
            long numOps = numIters * CHUNK_SIZE;
            double opsPerS = numOps / elapsed;
            Console.WriteLine("{0,-13}{1,10:0.0000} Mops/s ({2:0.0000}s elapsed)", opName, opsPerS / 1000000.0, elapsed);
        }

        public static void ExecuteUnary(UnaryOp op)
        {
            // \todo [petri] use all generators?
            F64[] inputs = op.inputGenerators[0](rnd, CHUNK_SIZE).Select(d => F64.FromDouble(d)).ToArray();
            F64[] outputs = new F64[CHUNK_SIZE];

            // Measure execution time.
            double elapsed = Measure(() =>
            {
                for (int iter = 0; iter < op.numBenchmarkIters; iter++)
                    op.arrayExecute(CHUNK_SIZE, inputs, outputs);
            });

            // Report result.
            long numOps = op.numBenchmarkIters * CHUNK_SIZE;
            double opsPerS = numOps / elapsed;
            Console.WriteLine("{0,-13}{1,10:0.0000} Mops/s ({2:0.0000}s elapsed)", op.name, opsPerS / 1000000.0, elapsed);
        }

        public static void ExecuteBinary(BinaryOp op)
        {
            // \todo [petri] use all generators?
            F64[] inputs0 = op.inputGenerators[0].Item1(rnd, CHUNK_SIZE).Select(d => F64.FromDouble(d)).ToArray();
            F64[] inputs1 = op.inputGenerators[0].Item2(rnd, CHUNK_SIZE).Select(d => F64.FromDouble(d)).ToArray();
            F64[] outputs = new F64[CHUNK_SIZE];

            // Measure execution time.
            double elapsed = Measure(() =>
            {
                for (int iter = 0; iter < op.numBenchmarkIters; iter++)
                    op.arrayExecute(CHUNK_SIZE, inputs0, inputs1, outputs);
            });

            // Report result.
            long numOps = op.numBenchmarkIters * CHUNK_SIZE;
            double opsPerS = numOps / elapsed;
            Console.WriteLine("{0,-13}{1,10:0.0000} Mops/s ({2:0.0000}s elapsed)", op.name, opsPerS / 1000000.0, elapsed);
        }
    }

    class Program
    {
        static UnaryErrorEvaluator AbsoluteUnaryErrorEvaluator(double relativeTo = 1.0)
        {
            return (F64 input, F64 output, double reference) =>
            {
                return Math.Abs(output.Double - reference) / relativeTo;
            };
        }

        static UnaryErrorEvaluator RelativeUnaryErrorEvaluator(double threshold = 1.0 / 65536.0)
        {
            return (F64 input, F64 output, double reference) =>
            {
                double absRef = Math.Abs(reference);
                double err = Math.Abs(output.Double - reference);
                if (absRef < threshold)
                    return err / threshold;
                else
                    return err / absRef;
            };
        }

        static BinaryErrorEvaluator AbsoluteBinaryErrorEvaluator(double relativeTo = 1.0)
        {
            return (F64 a, F64 b, F64 output, double reference) =>
            {
                return Math.Abs(output.Double - reference) / relativeTo;
            };
        }

        static BinaryErrorEvaluator RelativeBinaryErrorEvaluator(double threshold = 1.0 / 65536.0)
        {
            return (F64 a, F64 b, F64 output, double reference) =>
            {
                double relativeTo = Math.Max(threshold, Math.Abs(reference));
                double err = Math.Abs(output.Double - reference);
                return err / relativeTo;
            };
        }

        static BinaryErrorEvaluator DivisionErrorEvaluator(double threshold = 1.0 / 65536.0)
        {
            return (F64 a, F64 b, F64 output, double reference) =>
            {
                double relativeTo = Math.Max(threshold, Math.Abs(b.Double));
                double err = Math.Abs(output.Double - reference);
                return err / relativeTo;
            };
        }

        static double[] rcpValues = new double[] {
            0.03,
            0.125,
            0.5,
            1.0,
            2.0,
            3.0,
            3.999,
            4.0,
            7.777,
            11.12345,
            12.0,
            256.0,
            65535.0,
            123544.0,
        };

        static double[] sinCosValues = new double[] {
            -16.1234,
            -4.444,
            -0.5,
            0.0,
            0.12345,
            0.5,
            1.2,
            2.1,
            Math.PI,
            1.9999 * Math.PI,
            2.0001 * Math.PI,
            4.56 * Math.PI,
            16.0 * Math.PI,
        };

        static double[] asinCosValues = new double[] {
            -0.9999972383957,
            -0.99,
            -0.95,
            -0.9,
            -0.8,
            -0.75,
            -0.73,
            -0.71,
            -0.7071059781592,
            -0.7,
            -0.65,
            -0.5,
            -0.321,
            -0.11211,
            -0.000014884,
            0.0,
            0.00001,
            0.321,
            0.5521,
            0.7071059781592,
            0.99,
            0.9999972383957,
        };

        static UnaryOp[] unaryOps = new UnaryOp[] {
            new UnaryOp(
                "Ceil()",
                1000000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Ceil(); } },
                (double i0) => { return Math.Ceiling(i0); },
                AbsoluteUnaryErrorEvaluator(), // \todo [petri] exact error?
                new[] {
                    Input.Linear(-1e6, 1e6),
                    Input.Linear(-1.0, 1.0)
                },
                rcpValues
            ),

            new UnaryOp(
                "Floor()",
                1000000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Floor(); } },
                (double i0) => { return Math.Floor(i0); },
                AbsoluteUnaryErrorEvaluator(), // \todo [petri] exact error?
                new[] {
                    Input.Linear(-1e6, 1e6),
                    Input.Linear(-1.0, 1.0)
                },
                rcpValues
            ),

            new UnaryOp(
                "Round()",
                1000000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Round(); } },
                (double i0) => { return Math.Round(i0); }, // \todo [petri] round to nearest-even
                AbsoluteUnaryErrorEvaluator(), // \todo [petri] exact error?
                new[] {
                    Input.Linear(-1e6, 1e6),
                    Input.Linear(-1.0, 1.0)
                },
                rcpValues
            ),

            new UnaryOp(
                "Abs()",
                1000000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Abs(); } },
                (double i0) => { return Math.Abs(i0); },
                AbsoluteUnaryErrorEvaluator(), // \todo [petri] exact error?
                new[] {
                    Input.Linear(-1e6, 1e6),
                    Input.Linear(-1.0, 1.0)
                },
                rcpValues
            ),

            new UnaryOp(
                "Nabs()",
                1000000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Nabs(); } },
                (double i0) => { return -Math.Abs(i0); },
                AbsoluteUnaryErrorEvaluator(), // \todo [petri] exact error?
                new[] {
                    Input.Linear(-1e6, 1e6),
                    Input.Linear(-1.0, 1.0)
                },
                rcpValues
            ),

            new UnaryOp(
                "1/x",
                100000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.One / i0[i]; } },
                (double i0) => { return 1.0 / i0; },
                RelativeUnaryErrorEvaluator(), // \todo [petri] param
                new[] { Input.Exponential(0.01, 40.0) },
                rcpValues
            ),

            new UnaryOp(
                "Rcp(x)",
                100000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Rcp(); } },
                (double i0) => { return 1.0 / i0; },
                RelativeUnaryErrorEvaluator(), // \todo [petri] param
                new[] { Input.Exponential(0.01, 40.0) },
                rcpValues
            ),

            new UnaryOp(
                "Sqrt(x)",
                10000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Sqrt(); } },
                (double i0) => { return Math.Sqrt(i0); },
                RelativeUnaryErrorEvaluator(), // \todo [petri] param
                new[] { Input.Exponential(0.01, 40.0) },
                rcpValues
            ),

            new UnaryOp(
                "Rcp(RSqrt(x))",
                100000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].RSqrt().Rcp(); } },
                (double i0) => { return Math.Sqrt(i0); },
                RelativeUnaryErrorEvaluator(), // \todo [petri] param
                new[] { Input.Exponential(0.01, 40.0) },
                rcpValues
            ),

            new UnaryOp(
                "SqrtFast(x)",
                100000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].SqrtFast(); } },
                (double i0) => { return Math.Sqrt(i0); },
                RelativeUnaryErrorEvaluator(), // \todo [petri] param
                new[] { Input.Exponential(0.01, 40.0) },
                rcpValues
            ),

            new UnaryOp(
                "RSqrt(x)",
                100000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].RSqrt(); } },
                (double i0) => { return 1.0 / Math.Sqrt(i0); },
                RelativeUnaryErrorEvaluator(), // \todo [petri] param
                new[] { Input.Exponential(0.01, 40.0) },
                rcpValues
            ),

            new UnaryOp(
                "1/Sqrt(x)",
                10000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.FromInt(1) / i0[i].Sqrt(); } },
                (double i0) => { return 1.0 / Math.Sqrt(i0); },
                RelativeUnaryErrorEvaluator(), // \todo [petri] param
                new[] { Input.Exponential(0.01, 40.0) },
                rcpValues
            ),

            new UnaryOp(
                "Rcp(Sqrt(x))",
                10000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Sqrt().Rcp(); } },
                (double i0) => { return 1.0 / Math.Sqrt(i0); },
                RelativeUnaryErrorEvaluator(), // \todo [petri] param
                new[] { Input.Exponential(0.01, 40.0) },
                new double[] { 0.5, 1.0 }
            ),

            new UnaryOp(
                "Exp(x)",
                100000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Exp(); } },
                (double i0) => { return Math.Exp(i0); },
                RelativeUnaryErrorEvaluator(), // \todo [petri] param
                new[] { Input.Linear(-10.0, 10.0) },
                new double[] { 0.5, 1.0, 4.85202256566845 }
            ),

            new UnaryOp(
                "Exp2(x)",
                100000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Exp2(); } },
                (double i0) => { return Math.Pow(2.0, i0); },
                RelativeUnaryErrorEvaluator(), // \todo [petri] param
                new[] { Input.Linear(-10.0, 10.0) },
                new double[] { 0.5, 1.0, 6.9999888928141445 }
            ),

            new UnaryOp(
                "Log(x)",
                100000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Log(); } },
                (double i0) => { return Math.Log(i0); },
                RelativeUnaryErrorEvaluator(), // \todo [petri] param
                new[] { Input.Exponential(0.001, 1e6) },
                new double[] { 0.5, 1.0 }
            ),

            new UnaryOp(
                "Log2(x)",
                100000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Log2(); } },
                (double i0) => { return Math.Log(i0, 2.0); },
                RelativeUnaryErrorEvaluator(), // \todo [petri] param
                new[] { Input.Exponential(0.001, 1e6) },
                new double[] { 0.5, 1.0 }
            ),

            new UnaryOp(
                "Sin(x)",
                500000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Sin(); } },
                (double i0) => { return Math.Sin(i0); },
                AbsoluteUnaryErrorEvaluator(),
                new[]
                {
                    Input.Linear(-10.0, 10.0),
                    Input.Linear(-100.0, 100.0),
                    Input.Linear(-1e6, 1e6),
                },
                sinCosValues
            ),

            new UnaryOp(
                "Cos(x)",
                500000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Cos(); } },
                (double i0) => { return Math.Cos(i0); },
                AbsoluteUnaryErrorEvaluator(),
                new[]
                {
                    Input.Linear(-10.0, 10.0),
                    Input.Linear(-100.0, 100.0),
                    Input.Linear(-1e6, 1e6),
                },
                sinCosValues
            ),

            new UnaryOp(
                "Tan(x)",
                100000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Tan(); } },
                (double i0) => { return Math.Tan(i0); },
                RelativeUnaryErrorEvaluator(), // \todo [petri] param
                new[] {
                    Input.Linear(-0.99, -0.1),
                    Input.Linear(-0.1, 0.1),
                    Input.Linear(0.1, 0.99),
                },
                new double[] { 0.5, 1.0 }
            ),

            new UnaryOp(
                "Asin(x)",
                50000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Asin(); } },
                (double i0) => { return Math.Asin(i0); },
                AbsoluteUnaryErrorEvaluator(),
                new[] { Input.Linear(-1.0, 1.0) },
                asinCosValues
            ),

            new UnaryOp(
                "Acos(x)",
                50000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Acos(); } },
                (double i0) => { return Math.Acos(i0); },
                AbsoluteUnaryErrorEvaluator(),
                new[] { Input.Linear(-1.0, 1.0) },
                asinCosValues
            ),

            new UnaryOp(
                "Atan(x)",
                100000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Atan(); } },
                (double i0) => { return Math.Atan(i0); },
                AbsoluteUnaryErrorEvaluator(),
                new[] {
                    Input.Linear(-1.0, 1.0),
                    Input.Linear(-1000.0, 1000.0)
                },
                new double[] { 0.5, 1.0 }
            ),
        };

        static BinaryOp[] binaryOps = new BinaryOp[] {
            new BinaryOp(
                "a+b",
                1000000,
                (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] + i1[i]; } },
                (double i0, double i1) => { return i0 + i1; },
                AbsoluteBinaryErrorEvaluator(),
                new[] { Tuple.Create(Input.Linear(-1e6, 1e6), Input.Linear(-1e6, 1e6)) },
                new Tuple<double, double>[] { }
            ),

            new BinaryOp(
                "a-b",
                1000000,
                (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] - i1[i]; } },
                (double i0, double i1) => { return i0 - i1; },
                AbsoluteBinaryErrorEvaluator(),
                new[] { Tuple.Create(Input.Linear(-1e6, 1e6), Input.Linear(-1e6, 1e6)) },
                new Tuple<double, double>[] { }
            ),

            new BinaryOp(
                "a*b",
                1000000,
                (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] * i1[i]; } },
                (double i0, double i1) => { return i0 * i1; },
                RelativeBinaryErrorEvaluator(), // \todo [petri] param
                new[] {
                    Tuple.Create(Input.Linear(-1e3, 1e3), Input.Linear(-1e3, 1e3)),
                    Tuple.Create(Input.Linear(-1e6, 1e6), Input.Linear(-1.0, 1.0)),
                    Tuple.Create(Input.Linear(-1e9, 1e9), Input.Linear(-1e-3, 1e-3))
                },
                new Tuple<double, double>[] { }
            ),

            new BinaryOp(
                "a/b",
                1000000,
                (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] / i1[i]; } },
                (double i0, double i1) => { return i0 / i1; },
                DivisionErrorEvaluator(), // \todo [petri] param
                new[] {
                    // \todo [petri] add more test ranges -- need solution for error being relative to divisor & can wrap
                    Tuple.Create(Input.Linear(0.0, 1e3), Input.Exponential(1e3, 1e9)),
                },
                new [] {
                    Tuple.Create(0.0, 1.0),
                    Tuple.Create(4.0, 2.0),
                    Tuple.Create(6.0, 0.5),
                    Tuple.Create(39.483308344613761, 87595497.47375004)
                }
            ),

            new BinaryOp(
                "Min(a,b)",
                1000000,
                (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Min(i0[i], i1[i]); } },
                (double i0, double i1) => { return Math.Min(i0, i1); },
                AbsoluteBinaryErrorEvaluator(),
                new[] {
                    Tuple.Create(Input.Linear(-1e6, 1e6), Input.Linear(-1e6, 1e6)),
                    Tuple.Create(Input.Linear(-1e6, 1e6), Input.Linear(-1e6, -1e6)),
                },
                new Tuple<double, double>[] { }
            ),

            new BinaryOp(
                "Max(a,b)",
                1000000,
                (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Max(i0[i], i1[i]); } },
                (double i0, double i1) => { return Math.Max(i0, i1); },
                AbsoluteBinaryErrorEvaluator(),
                new[] {
                    Tuple.Create(Input.Linear(-1e6, 1e6), Input.Linear(-1e6, 1e6)),
                    Tuple.Create(Input.Linear(-1e6, 1e6), Input.Linear(-1e6, -1e6)),
                },
                new Tuple<double, double>[] { }
            ),

            new BinaryOp(
                "Pow(a, b)",
                100000,
                (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Pow(i0[i], i1[i]); } },
                (double i0, double i1) => { return Math.Pow(i0, i1); },
                RelativeBinaryErrorEvaluator(1.0 / 256), // \todo [petri] pretty high threshold
                new[] {
                    // \todo [petri] very small numbers need absolute error!
                    // \todo [petri] add more range!
                    Tuple.Create(Input.Exponential(1e-6, 1.0), Input.Exponential(1e-3, 20.0)),
                    Tuple.Create(Input.Exponential(1.0, 16.0), Input.Exponential(1e-3, 1.0)),
                },
                new[] {
                    Tuple.Create(0.03431271179579, 8.52991297887638),
                    Tuple.Create(0.00991570530459, 9.13006472284906),
                    Tuple.Create(0.16317143756896, 12.2345450441353),
                    Tuple.Create(0.28775308770127, 17.8064287465531)
                }
            ),

            new BinaryOp(
                "Atan2(a, b)",
                100000,
                (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Atan2(i0[i], i1[i]); } },
                (double i0, double i1) => { return Math.Atan2(i0, i1); },
                AbsoluteBinaryErrorEvaluator(),
                new[] {
                    Tuple.Create(Input.Exponential(1e-6, 1.0), Input.Exponential(1e-6, 1.0)),
                    Tuple.Create(Input.Exponential(1.0, 1e6), Input.Exponential(1.0, 1e6)),
                    // \todo [petri] all sign combinations
                },
                new Tuple<double, double>[] { }
            ),
        };
/*
        private static double[] g_test_vals =
        {
            13.0, 1.2,
            13.2, 2.4,
            -13.4, 5.5,
            -13.3, -8.8,
            19.0, -7.5,
            13.0, 0.5,
            13.0, 0.05,
            1.234, 2.345,
        };
*/
        static void BasicTests()
        {
            Console.WriteLine("One in raw: {0}", Fixed64.One);
            Console.WriteLine("Pi in raw: {0}, value: {1}", Fixed64.Pi, Fixed64.ToString(Fixed64.Pi));
            Console.WriteLine();

            Console.WriteLine("Basic tests:");

            foreach (UnaryOp op in unaryOps)
                RegressionTest.UnaryBasicValues(op);

            foreach (BinaryOp op in binaryOps)
                RegressionTest.BinaryBasicValues(op);
        }

        static void PrecisionTests()
        {
            Console.WriteLine("Precision tests:          average            worst [coord]");

            foreach (UnaryOp op in unaryOps)
                PrecisionTest.TestUnary(op);

            foreach (BinaryOp op in binaryOps)
                PrecisionTest.TestBinary(op);

            Console.WriteLine();
        }

        static void PerfTests()
        {
            Console.WriteLine("Performance benchmark:");
            foreach (UnaryOp op in unaryOps)
                Benchmark.ExecuteUnary(op);

            foreach (BinaryOp op in binaryOps)
                Benchmark.ExecuteBinary(op);

            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            BasicTests();
            PrecisionTests();
            PerfTests();

            /*Console.WriteLine("-ENTER-");
            Console.ReadLine();*/
        }
    }
}
