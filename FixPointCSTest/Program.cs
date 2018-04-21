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
        public static readonly double PosMax = 0.99 * (1L << 31);
        public static readonly double NegMax = -PosMax;

        // \todo [petri] expand to about 0.9 * MaxValue ?
        public static readonly double TestPosMax = 0.5 * (1L << 31);
        public static readonly double TestNegMax = -TestPosMax;
        public static readonly double TestPosMin = 1.0 / TestPosMax;
        public static readonly double TestNegMin = -TestPosMin;
    }

    class Input
    {
        static public ValueGenerator Uniform(double mn, double mx)
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
            Debug.Assert(Math.Sign(mn) == Math.Sign(mx));
            Debug.Assert(Math.Abs(mn) < Math.Abs(mx));

            double ratio = Math.Log(mx / mn, 2.0);
            return (Random rnd, int count) =>
            {
                double[] values = new double[count];
                for (int i = 0; i < count; i++)
                    values[i] = mn * Math.Pow(2.0, ratio * rnd.NextDouble());
                return values;
            };
        }
    };

    class OpBase
    {
        public string   name;
        public int      numBenchmarkIters;
    };

    class UnaryOp : OpBase
    {
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

    class BinaryOp : OpBase
    {
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

    class TestRunner
    {
        private const int NUM_PRECISION_TESTS = 1024 * 1024;
        private const int BENCHMARK_CHUNK_SIZE = 128;

        private static Random rnd = new Random(12345678);

        public struct PrecisionResult
        {
            public double   avgError;
            public double   maxError;
            public double   numPrecisionBits;
            public F64[]    worstInput;

            public PrecisionResult(double avgError, double maxError, double numPrecisionBits, F64[] worstInput)
            {
                this.avgError           = avgError;
                this.maxError           = maxError;
                this.numPrecisionBits   = numPrecisionBits;
                this.worstInput         = worstInput;
            }
        }

        public struct BenchmarkResult
        {
            public double   opsPerSec;
            public double   elapsedSeconds;

            public BenchmarkResult(double opsPerSec, double elapsedSeconds)
            {
                this.opsPerSec      = opsPerSec;
                this.elapsedSeconds = elapsedSeconds;
            }
        }

        static string DoubleToString(double val, int numChars)
        {
            string str = val.ToString("F" + numChars, System.Globalization.CultureInfo.InvariantCulture);
            return str.Substring(0, Math.Min(str.Length, numChars));
        }

        public static void TestUnaryBasicValues(UnaryOp op)
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

                Console.WriteLine("{0,-16}    {1,16} {2,16} {3,16}", op.name, "reference", "result", "error");
                for (int i = 0; i < numValues; i++)
                {
                    double input = inputs[i].Double;
                    double res = outputs[i].Double;
                    double err = op.evaluateError(inputs[i], outputs[i], reference[i]);
                    Console.WriteLine("{0} -> {1} {2} {3}", DoubleToString(input, 16), DoubleToString(reference[i], 16), DoubleToString(res, 16), DoubleToString(err, 16));
                }
                Console.WriteLine();
            }
        }

        public static void TestBinaryBasicValues(BinaryOp op)
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

                Console.WriteLine("{0,-16}                      {1,16} {2,16} {3,16}", op.name, "reference", "result", "error");
                for (int i = 0; i < numValues; i++)
                {
                    double input0 = inputs0[i].Double;
                    double input1 = inputs1[i].Double;
                    double res = outputs[i].Double;
                    double err = op.evaluateError(inputs0[i], inputs1[i], outputs[i], reference[i]);
                    Console.WriteLine("{0}, {1} -> {2} {3} {4}", DoubleToString(input0, 16), DoubleToString(input1, 16), DoubleToString(reference[i], 16), DoubleToString(res, 16), DoubleToString(err, 16));
                }
                Console.WriteLine();
            }
        }

        public static PrecisionResult TestUnaryPrecision(UnaryOp op)
        {
            int     numTested   = 0;
            double  totalErr    = 0.0;
            double  maxErr      = 0.0;
            F64     worstInput  = F64.Zero;

            foreach (ValueGenerator inputGenerator in op.inputGenerators)
            {
                F64[] inputs = inputGenerator(rnd, NUM_PRECISION_TESTS).Select(d => F64.FromDouble(d)).ToArray();
                F64[] outputs = new F64[NUM_PRECISION_TESTS];

                op.arrayExecute(NUM_PRECISION_TESTS, inputs, outputs);

                for (int i = 0; i < NUM_PRECISION_TESTS; i++)
                {
                    double input = inputs[i].Double;
                    double res = outputs[i].Double;
                    double reference = op.refExecute(input);

                    if (reference >= ValueBounds.NegMax && reference <= ValueBounds.PosMax)
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
            double numBits = -Math.Log(maxErr, 2.0);
            return new PrecisionResult(avgErr, maxErr, numBits, new[]{ worstInput });
        }

        public static PrecisionResult TestBinaryPrecision(BinaryOp op)
        {
            int             numTested   = 0;
            double          totalErr    = 0.0;
            double          maxErr      = 0.0;
            Tuple<F64, F64> worstInput  = Tuple.Create(F64.Zero, F64.Zero);

            foreach (BinaryInputGenerator inputGenerator in op.inputGenerators)
            {
                F64[] inputs0 = inputGenerator.Item1(rnd, NUM_PRECISION_TESTS).Select(d => F64.FromDouble(d)).ToArray();
                F64[] inputs1 = inputGenerator.Item2(rnd, NUM_PRECISION_TESTS).Select(d => F64.FromDouble(d)).ToArray();
                F64[] outputs = new F64[NUM_PRECISION_TESTS];

                op.arrayExecute(NUM_PRECISION_TESTS, inputs0, inputs1, outputs);

                for (int i = 0; i < NUM_PRECISION_TESTS; i++)
                {
                    double input0 = inputs0[i].Double;
                    double input1 = inputs1[i].Double;
                    double res = outputs[i].Double;
                    double reference = op.refExecute(input0, input1);

                    if (reference >= ValueBounds.NegMax && reference <= ValueBounds.PosMax)
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
            double numBits = -Math.Log(maxErr, 2.0);
            return new PrecisionResult(avgErr, maxErr, numBits, new[] { worstInput.Item1, worstInput.Item2});
        }

        private static double BenchmarkFunction(Action func)
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

        public static BenchmarkResult BenchmarkUnaryOperation(UnaryOp op)
        {
            // \todo [petri] use all generators?
            F64[] inputs = op.inputGenerators[0](rnd, BENCHMARK_CHUNK_SIZE).Select(d => F64.FromDouble(d)).ToArray();
            F64[] outputs = new F64[BENCHMARK_CHUNK_SIZE];

            // Measure execution time.
            double elapsedSeconds = BenchmarkFunction(() =>
            {
                for (int iter = 0; iter < op.numBenchmarkIters; iter++)
                    op.arrayExecute(BENCHMARK_CHUNK_SIZE, inputs, outputs);
            });

            // Return result.
            long numTotalOps = op.numBenchmarkIters * BENCHMARK_CHUNK_SIZE;
            double opsPerSec = numTotalOps / elapsedSeconds;
            return new BenchmarkResult(opsPerSec, elapsedSeconds);
        }

        public static BenchmarkResult BenchmarkBinaryOperation(BinaryOp op)
        {
            // \todo [petri] use all generators?
            F64[] inputs0 = op.inputGenerators[0].Item1(rnd, BENCHMARK_CHUNK_SIZE).Select(d => F64.FromDouble(d)).ToArray();
            F64[] inputs1 = op.inputGenerators[0].Item2(rnd, BENCHMARK_CHUNK_SIZE).Select(d => F64.FromDouble(d)).ToArray();
            F64[] outputs = new F64[BENCHMARK_CHUNK_SIZE];

            // Measure execution time.
            double elapsedSeconds = BenchmarkFunction(() =>
            {
                for (int iter = 0; iter < op.numBenchmarkIters; iter++)
                    op.arrayExecute(BENCHMARK_CHUNK_SIZE, inputs0, inputs1, outputs);
            });

            // Return reuslt.
            long numTotalOps = op.numBenchmarkIters * BENCHMARK_CHUNK_SIZE;
            double opsPerSec = numTotalOps / elapsedSeconds;
            return new BenchmarkResult(opsPerSec, elapsedSeconds);
        }

        public static void PrintOperationSummary(string name, PrecisionResult precision, BenchmarkResult benchmark)
        {
            if (benchmark.elapsedSeconds < 0.1 || benchmark.elapsedSeconds > 1.0)
                Console.WriteLine("WARNING: {0} took {1:0.00}s to execute, tweak its iteration count so that it executes in roughly 0.5s", name, benchmark.elapsedSeconds);

            if (precision.maxError == 0.0)
                Console.WriteLine("| {0,16} | {1,10:0.0000} |     exact | {2,16} | {3,16} |", name, benchmark.opsPerSec / 1000000.0, "0.0", "0.0");
            else
            {
                string coordsStr = String.Join(" ", precision.worstInput.Select(v => DoubleToString(v.Double, 16)));
                Console.WriteLine("| {0,16} | {1,10:0.0000} | {2,9:0.00} | {3} | {4} | {5}", name, benchmark.opsPerSec / 1000000.0, precision.numPrecisionBits, DoubleToString(precision.avgError, 16), DoubleToString(precision.maxError, 16), coordsStr);
            }
        }

        public static void TestUnaryOperation(UnaryOp op)
        {
            PrecisionResult precision = TestUnaryPrecision(op);
            BenchmarkResult benchmark = BenchmarkUnaryOperation(op);
            PrintOperationSummary(op.name, precision, benchmark);
        }

        public static void TestBinaryOperation(BinaryOp op)
        {
            PrecisionResult precision = TestBinaryPrecision(op);
            BenchmarkResult benchmark = BenchmarkBinaryOperation(op);
            PrintOperationSummary(op.name, precision, benchmark);
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
            // -1.0,
            // -128.0,
            // -220.0,
            // -250.0,
            // -255.0,
            // -255.99966072314,
            0.0000005565656,
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
            1644181663.39180,
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

        static OpBase[] operations = new OpBase[]
        {
            new UnaryOp(
                "Ceil()",
                1000000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Ceil(); } },
                (double i0) => { return Math.Ceiling(i0); },
                AbsoluteUnaryErrorEvaluator(),
                new[] {
                    Input.Uniform(-1.0, 1.0),
                    Input.Uniform(-1e6, 1e6),
                    Input.Uniform(ValueBounds.TestNegMax, ValueBounds.TestPosMax)
                },
                rcpValues
            ),

            new UnaryOp(
                "Floor()",
                1000000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Floor(); } },
                (double i0) => { return Math.Floor(i0); },
                AbsoluteUnaryErrorEvaluator(),
                new[] {
                    Input.Uniform(-1.0, 1.0),
                    Input.Uniform(-1e6, 1e6),
                    Input.Uniform(ValueBounds.TestNegMax, ValueBounds.TestPosMax)
                },
                rcpValues
            ),

            new UnaryOp(
                "Round()",
                1000000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Round(); } },
                (double i0) => { return Math.Floor(i0 + 0.5); },
                AbsoluteUnaryErrorEvaluator(),
                new[] {
                    Input.Uniform(-1.0, 1.0),
                    Input.Uniform(-1e6, 1e6),
                    Input.Uniform(ValueBounds.TestNegMax, ValueBounds.TestPosMax)
                },
                rcpValues
            ),

            new UnaryOp(
                "Abs()",
                1000000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Abs(); } },
                (double i0) => { return Math.Abs(i0); },
                AbsoluteUnaryErrorEvaluator(),
                new[] {
                    Input.Uniform(-1.0, 1.0),
                    Input.Uniform(-1e6, 1e6),
                    Input.Uniform(ValueBounds.TestNegMax, ValueBounds.TestPosMax)
                },
                rcpValues
            ),

            new UnaryOp(
                "Nabs()",
                1000000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Nabs(); } },
                (double i0) => { return -Math.Abs(i0); },
                AbsoluteUnaryErrorEvaluator(),
                new[] {
                    Input.Uniform(-1.0, 1.0),
                    Input.Uniform(-1e6, 1e6),
                    Input.Uniform(ValueBounds.TestNegMax, ValueBounds.TestPosMax)
                },
                rcpValues
            ),

            new UnaryOp(
                "1/x",
                100000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.One / i0[i]; } },
                (double i0) => { return 1.0 / i0; },
                RelativeUnaryErrorEvaluator(),
                new[] {
                    Input.Exponential(ValueBounds.TestPosMin, ValueBounds.PosMax),
                    Input.Exponential(ValueBounds.TestNegMin, ValueBounds.NegMax)
                },
                rcpValues
            ),

            new UnaryOp(
                "Rcp(x)",
                100000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Rcp(); } },
                (double i0) => { return 1.0 / i0; },
                RelativeUnaryErrorEvaluator(),
                new[] {
                    Input.Exponential(ValueBounds.TestPosMin, ValueBounds.TestPosMax),
                    Input.Exponential(ValueBounds.TestNegMin, ValueBounds.TestNegMax)
                },
                rcpValues
            ),

            new UnaryOp(
                "Sqrt(x)",
                10000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Sqrt(); } },
                (double i0) => { return Math.Sqrt(i0); },
                RelativeUnaryErrorEvaluator(),
                new[] {
                    Input.Exponential(ValueBounds.TestPosMin, ValueBounds.TestPosMax),
                    Input.Exponential(ValueBounds.TestNegMin, ValueBounds.TestNegMax)
                },
                rcpValues
            ),

            new UnaryOp(
                "Rcp(RSqrt(x))",
                100000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].RSqrt().Rcp(); } },
                (double i0) => { return Math.Sqrt(i0); },
                RelativeUnaryErrorEvaluator(),
                new[] {
                    Input.Exponential(ValueBounds.TestPosMin, ValueBounds.TestPosMax)
                },
                rcpValues
            ),

            new UnaryOp(
                "SqrtFast(x)",
                100000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].SqrtFast(); } },
                (double i0) => { return Math.Sqrt(i0); },
                RelativeUnaryErrorEvaluator(),
                new[] {
                    Input.Exponential(ValueBounds.TestPosMin, ValueBounds.TestPosMax)
                },
                rcpValues
            ),

            new UnaryOp(
                "RSqrt(x)",
                500000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].RSqrt(); } },
                (double i0) => { return 1.0 / Math.Sqrt(i0); },
                RelativeUnaryErrorEvaluator(),
                new[] {
                    Input.Exponential(ValueBounds.TestPosMin, ValueBounds.TestPosMax)
                },
                rcpValues
            ),

            new UnaryOp(
                "1/Sqrt(x)",
                10000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.FromInt(1) / i0[i].Sqrt(); } },
                (double i0) => { return 1.0 / Math.Sqrt(i0); },
                RelativeUnaryErrorEvaluator(),
                new[] {
                    Input.Exponential(ValueBounds.TestPosMin, ValueBounds.TestPosMax)
                },
                rcpValues
            ),

            new UnaryOp(
                "Rcp(Sqrt(x))",
                10000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Sqrt().Rcp(); } },
                (double i0) => { return 1.0 / Math.Sqrt(i0); },
                RelativeUnaryErrorEvaluator(),
                new[] {
                    Input.Exponential(ValueBounds.TestPosMin, ValueBounds.TestPosMax)
                },
                rcpValues
            ),

            new UnaryOp(
                "Exp(x)",
                500000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Exp(); } },
                (double i0) => { return Math.Exp(i0); },
                RelativeUnaryErrorEvaluator(),
                new[] {
                    Input.Uniform(-10.0, 10.0),
                    Input.Exponential(ValueBounds.TestPosMin, 10.0),
                    Input.Exponential(ValueBounds.TestNegMin, -10.0)
                },
                new double[] { 0.5, 1.0, 4.85202256566845 }
            ),

            new UnaryOp(
                "Exp2(x)",
                500000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Exp2(); } },
                (double i0) => { return Math.Pow(2.0, i0); },
                RelativeUnaryErrorEvaluator(),
                new[] { Input.Uniform(-10.0, 10.0) },
                new double[] { 0.5, 1.0, 6.9999888928141445 }
            ),

            new UnaryOp(
                "Log(x)",
                100000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Log(); } },
                (double i0) => { return Math.Log(i0); },
                RelativeUnaryErrorEvaluator(),
                new[] { Input.Exponential(0.001, 1e6) },
                new double[] { 0.5, 0.99994553346186, 1.0 }
            ),

            new UnaryOp(
                "Log2(x)",
                100000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Log2(); } },
                (double i0) => { return Math.Log(i0, 2.0); },
                RelativeUnaryErrorEvaluator(),
                new[] { Input.Exponential(0.001, 1e6) },
                new double[] { 0.5, 0.99994553346186, 1.0 }
            ),

            new UnaryOp(
                "Sin(x)",
                500000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Sin(); } },
                (double i0) => { return Math.Sin(i0); },
                AbsoluteUnaryErrorEvaluator(),
                new[]
                {
                    Input.Uniform(-10.0, 10.0),
                    Input.Uniform(-100.0, 100.0),
                    Input.Uniform(-1e6, 1e6),
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
                    Input.Uniform(-10.0, 10.0),
                    Input.Uniform(-100.0, 100.0),
                    Input.Uniform(-1e6, 1e6),
                },
                sinCosValues
            ),

            new UnaryOp(
                "Tan(x)",
                100000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Tan(); } },
                (double i0) => { return Math.Tan(i0); },
                RelativeUnaryErrorEvaluator(),
                new[] {
                    Input.Uniform(-0.99, -0.1),
                    Input.Uniform(-0.1, 0.1),
                    Input.Uniform(0.1, 0.99),
                },
                new double[] { 0.5, 1.0 }
            ),

            new UnaryOp(
                "Asin(x)",
                50000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Asin(); } },
                (double i0) => { return Math.Asin(i0); },
                AbsoluteUnaryErrorEvaluator(),
                new[] { Input.Uniform(-1.0, 1.0) },
                asinCosValues
            ),

            new UnaryOp(
                "Acos(x)",
                50000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Acos(); } },
                (double i0) => { return Math.Acos(i0); },
                AbsoluteUnaryErrorEvaluator(),
                new[] { Input.Uniform(-1.0, 1.0) },
                asinCosValues
            ),

            new UnaryOp(
                "Atan(x)",
                100000,
                (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i].Atan(); } },
                (double i0) => { return Math.Atan(i0); },
                AbsoluteUnaryErrorEvaluator(),
                new[] {
                    Input.Uniform(-1.0, 1.0),
                    Input.Uniform(-1000.0, 1000.0)
                },
                new double[] { 0.5, 1.0 }
            ),

            new BinaryOp(
                "a+b",
                1000000,
                (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] + i1[i]; } },
                (double i0, double i1) => { return i0 + i1; },
                AbsoluteBinaryErrorEvaluator(),
                new[] { Tuple.Create(Input.Uniform(-1e6, 1e6), Input.Uniform(-1e6, 1e6)) },
                new Tuple<double, double>[] { }
            ),

            new BinaryOp(
                "a-b",
                1000000,
                (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] - i1[i]; } },
                (double i0, double i1) => { return i0 - i1; },
                AbsoluteBinaryErrorEvaluator(),
                new[] { Tuple.Create(Input.Uniform(-1e6, 1e6), Input.Uniform(-1e6, 1e6)) },
                new Tuple<double, double>[] { }
            ),

            new BinaryOp(
                "a*b",
                1000000,
                (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] * i1[i]; } },
                (double i0, double i1) => { return i0 * i1; },
                RelativeBinaryErrorEvaluator(), // \todo [petri] param
                new[] {
                    Tuple.Create(Input.Uniform(-1e3, 1e3), Input.Uniform(-1e3, 1e3)),
                    Tuple.Create(Input.Uniform(-1e6, 1e6), Input.Uniform(-1.0, 1.0)),
                    Tuple.Create(Input.Uniform(-1e9, 1e9), Input.Uniform(-1e-3, 1e-3))
                },
                new Tuple<double, double>[] { }
            ),

            new BinaryOp(
                "a/b",
                100000,
                (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] / i1[i]; } },
                (double i0, double i1) => { return i0 / i1; },
                DivisionErrorEvaluator(), // \todo [petri] param
                new[] {
                    // \todo [petri] add more test ranges -- need solution for error being relative to divisor & can wrap
                    Tuple.Create(Input.Uniform(0.0, 1e3), Input.Exponential(1e3, 1e9)),
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
                    Tuple.Create(Input.Uniform(-1e6, 1e6), Input.Uniform(-1e6, 1e6)),
                    Tuple.Create(Input.Uniform(-1e6, 1e6), Input.Uniform(-1e6, -1e6)),
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
                    Tuple.Create(Input.Uniform(-1e6, 1e6), Input.Uniform(-1e6, 1e6)),
                    Tuple.Create(Input.Uniform(-1e6, 1e6), Input.Uniform(-1e6, -1e6)),
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
                new[] {
                    Tuple.Create(1.0, 1.0),
                    Tuple.Create(0.0, 0.0),
                    Tuple.Create(6.99465692322701, 3.93830072996207),
                }
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
        static void BasicTests(string testFilter)
        {
            Console.WriteLine("Basic tests:");

            foreach (OpBase op in operations)
            {
                if (op.name.StartsWith(testFilter))
                {
                    if (op is UnaryOp)
                        TestRunner.TestUnaryBasicValues((UnaryOp)op);
                    else if (op is BinaryOp)
                        TestRunner.TestBinaryBasicValues((BinaryOp)op);
                }
            }
        }

        static void TestOperations(string testFilter)
        {
            Console.WriteLine("OPERATIONS SUMMARY:");
            Console.WriteLine("|        Operation |     Mops/s | Precision |        Max error |        Avg error | Worst input");
            Console.WriteLine("|------------------|-----------:|----------:|-----------------:|-----------------:|-----------------");

            foreach (OpBase op in operations)
            {
                if (op.name.StartsWith(testFilter))
                {
                    if (op is UnaryOp)
                        TestRunner.TestUnaryOperation((UnaryOp)op);
                    else if (op is BinaryOp)
                        TestRunner.TestBinaryOperation((BinaryOp)op);
                }
            }

            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            // Name prefix of operations to test.
            // Set this to, eg, "Atan2" or "Rcp(x)" to only measure that operation
            string testFilter = "";

            BasicTests(testFilter);
            TestOperations(testFilter);

            /*Console.WriteLine("-ENTER-");
            Console.ReadLine();*/
        }
    }
}
