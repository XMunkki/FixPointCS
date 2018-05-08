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
using System.Threading;
using System.Linq;
using System.Diagnostics;
using FixPointCS;

namespace FixPointCSTest
{
    struct OperationError
    {
        public double measured;     // Measured error
        public double reference;    // Error reference
        public double minPossible;  // Minimum representable error

        public OperationError(double measured, double reference, double minPossible) { this.measured = measured; this.reference = reference; this.minPossible = minPossible; }
    }

    delegate double[] ValueGenerator(Random rnd, int count);
    delegate OperationError UnaryErrorEvaluator(double i0, double o, double reference, double ulpScale);
    delegate OperationError BinaryErrorEvaluator(double i0, double i1, double o, double reference, double ulpScale);

    abstract class ValueBounds
    {
        public abstract double InputPosMax { get; }
        public abstract double InputNegMax { get; }
        public abstract double InputPosMin { get; }
        public abstract double InputNegMin { get; }

        public abstract double OutputPosMax { get; }
        public abstract double OutputNegMax { get; }
    }

    class ValueBoundsF64 : ValueBounds
    {
        // \todo [petri] expand to about 0.9 * MaxValue ?
        public override double InputPosMax { get { return 0.5 * (1L << 31); } }
        public override double InputNegMax { get { return -0.5 * (1L << 31); } }
        public override double InputPosMin { get { return 1.0 / (1L << 32); } }
        public override double InputNegMin { get { return -1.0 / (1L << 32); } }

        public override double OutputPosMax { get { return 0.99 * (1L << 31); } }
        public override double OutputNegMax { get { return -0.99 * (1L << 31); } }
    }

    class ValueBoundsF32 : ValueBounds
    {
        // \todo [petri] expand to about 0.9 * MaxValue ?
        public override double InputPosMax { get { return 0.5 * (1 << 15); } }
        public override double InputNegMax { get { return -0.5 * (1 << 15); } }
        public override double InputPosMin { get { return 1.0 / (1 << 15); } }
        public override double InputNegMin { get { return -1.0 / (1 << 15); } }

        public override double OutputPosMax { get { return 0.99 * (1 << 15); } }
        public override double OutputNegMax { get { return -0.99 * (1 << 15); } }
    }

    class Input
    {
        public enum SignMode
        {
            Direct,
            Negate,
            Random
        }

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

        static public ValueGenerator Exponential(double mn, double mx, SignMode signMode = SignMode.Direct)
        {
            Debug.Assert(Math.Sign(mn) == Math.Sign(mx));
            Debug.Assert(Math.Abs(mn) < Math.Abs(mx));

            double ratio = Math.Log(mx / mn, 2.0);
            return (Random rnd, int count) =>
            {
                double[] values = new double[count];
                switch (signMode)
                {
                    case SignMode.Direct:
                        for (int i = 0; i < count; i++)
                            values[i] = mn * Math.Pow(2.0, ratio * rnd.NextDouble());
                        break;

                    case SignMode.Negate:
                        for (int i = 0; i < count; i++)
                            values[i] = -mn * Math.Pow(2.0, ratio * rnd.NextDouble());
                        break;

                    case SignMode.Random:
                        for (int i = 0; i < count; i++)
                            values[i] = ((i & 2) - 1) * mn * Math.Pow(2.0, ratio * rnd.NextDouble());
                        break;
                }
                return values;
            };
        }
    };

    class InputGenerator
    {
        public ValueGenerator[] generators;

        public InputGenerator(ValueGenerator[] generators)
        {
            this.generators = generators;
        }

        public static InputGenerator Unary(ValueGenerator gen)
        {
            return new InputGenerator(new ValueGenerator[]{ gen });
        }

        public static InputGenerator Binary(ValueGenerator gen0, ValueGenerator gen1)
        {
            return new InputGenerator(new ValueGenerator[]{ gen0, gen1 });
        }
    }

    delegate InputGenerator[] InputProducerFactory(ValueBounds bounds);

    class Operation
    {
        public string                           Name;
        public ValueBounds                      ValueBounds;
        public Type[]                           InputTypes;
        public Type[]                           OutputTypes;
        public int                              PrecisionBits;
        public Action<int, Array[], Array[]>    ArrayExecute;

        public Operation(string name, ValueBounds valueBounds, Type[] inputTypes, Type[] outputTypes, Action<int, Array[], Array[]> arrayExecute)
        {
            this.Name = name;
            this.ValueBounds = valueBounds;
            this.InputTypes = inputTypes;
            this.OutputTypes = outputTypes;
            this.PrecisionBits = outputTypes.Select(type => GetTypePrecisionBits(type)).Min();
            this.ArrayExecute = arrayExecute;
        }

        public static int GetTypePrecisionBits(Type type)
        {
            if (type == typeof(F64))
                return 32;
            else if (type == typeof(F32))
                return 16;
            else
                throw new InvalidOperationException("GetTypePrecisionBits(): Unknown type: " + type);
        }

        public static Operation F64_F64(string name, Action<int, F64[], F64[]> execute)
        {
            return new Operation(name, new ValueBoundsF64(), new[] { typeof(F64) }, new[] { typeof(F64) },
                (int count, Array[] inputs, Array[] outputs) => { execute(count, (F64[])inputs[0], (F64[])outputs[0]); }
            );
        }

        public static Operation F64_F64_F64(string name, Action<int, F64[], F64[], F64[]> execute)
        {
            return new Operation(name, new ValueBoundsF64(), new[] { typeof(F64), typeof(F64) }, new[] { typeof(F64) }, (int count, Array[] inputs, Array[] outputs) => { execute(count, (F64[])inputs[0], (F64[])inputs[1], (F64[])outputs[0]); });
        }

        public static Operation F32_F32(string name, Action<int, F32[], F32[]> execute)
        {
            return new Operation(name, new ValueBoundsF32(), new[] { typeof(F32) }, new[] { typeof(F32) }, (int count, Array[] inputs, Array[] outputs) => { execute(count, (F32[])inputs[0], (F32[])outputs[0]); });
        }

        public static Operation F32_F32_F32(string name, Action<int, F32[], F32[], F32[]> execute)
        {
            return new Operation(name, new ValueBoundsF32(), new[] { typeof(F32), typeof(F32) }, new[] { typeof(F32) }, (int count, Array[] inputs, Array[] outputs) => { execute(count, (F32[])inputs[0], (F32[])inputs[1], (F32[])outputs[0]); });
        }

        public static Operation[] Multi(params Operation[] list)
        {
            return list;
        }
    }

    abstract class OpFamilyBase
    {
        public Operation[]              operations;
        public InputProducerFactory     inputFactory;

        public abstract OperationError[] EvaluateErrors(int count, double[][] inputs, double[][] outputs, double ulpScale);
        public abstract double[] ComputeReferences(double[][] inputs);

        public static double[] ConvertToDouble(Array values)
        {
            Type fromType = values.GetType().GetElementType();
            double[] output = new double[values.Length];

            if (fromType == typeof(F64))
            {
                F64[] input = (F64[])values;
                for (int i = 0; i < values.Length; i++)
                    output[i] = input[i].Double;
            }
            else if (fromType == typeof(F32))
            {
                F32[] input = (F32[])values;
                for (int i = 0; i < values.Length; i++)
                    output[i] = input[i].Double;
            }
            else
                throw new InvalidOperationException("Unsupported type conversion from " + fromType);

            return output;
        }
    };

    class UnaryOpFamily : OpFamilyBase
    {
        public Func<double, double>     refExecute;
        public UnaryErrorEvaluator      evaluateError;

        // \todo [petri] way to express exact input-output values
        // \todo [petri] way to express monotonicity requirement

        public UnaryOpFamily(Func<double, double> refExec, UnaryErrorEvaluator evaluateError, Operation operation, InputProducerFactory inputFactory)
        {
            this.refExecute     = refExec;
            this.evaluateError  = evaluateError;
            this.operations     = new Operation[] { operation };
            this.inputFactory   = inputFactory;
        }

        public UnaryOpFamily(Func<double, double> refExec, UnaryErrorEvaluator evaluateError, Operation[] operations, InputProducerFactory inputFactory)
        {
            this.refExecute     = refExec;
            this.evaluateError  = evaluateError;
            this.operations     = operations;
            this.inputFactory   = inputFactory;
        }

        public override OperationError[] EvaluateErrors(int count, double[][] inputs, double[][] outputs, double ulpScale)
        {
            double[] input0 = inputs[0];
            double[] output = outputs[0];

            // Evaluate errors for all inputs.
            OperationError[] errors = new OperationError[count];
            for (int i = 0; i < count; i++)
            {
                double reference = refExecute(input0[i]);
                errors[i] = evaluateError(input0[i], output[i], reference, ulpScale);
            }
            return errors;
        }

        public override double[] ComputeReferences(double[][] inputs)
        {
            Debug.Assert(inputs.Length == 1);

            // Allocate results.
            double[] input0 = inputs[0];
            int count = input0.Length;
            double[] results = new double[count];

            for (int ndx = 0; ndx < count; ndx++)
                results[ndx] = refExecute(input0[ndx]);

            return results;
        }
    }

    class BinaryOpFamily : OpFamilyBase
    {
        public Func<double, double, double> refExecute;
        public BinaryErrorEvaluator         evaluateError;

        // \todo [petri] way to express exact input-output values
        // \todo [petri] way to express monotonicity requirement

        public BinaryOpFamily(Func<double, double, double> refExec, BinaryErrorEvaluator evaluateError, Operation operation, InputProducerFactory inputFactory)
        {
            this.refExecute     = refExec;
            this.evaluateError  = evaluateError;
            this.operations     = new Operation[] { operation };
            this.inputFactory   = inputFactory;
        }

        public BinaryOpFamily(Func<double, double, double> refExec, BinaryErrorEvaluator evaluateError, Operation[] operations, InputProducerFactory inputFactory)
        {
            this.refExecute     = refExec;
            this.evaluateError  = evaluateError;
            this.operations     = operations;
            this.inputFactory   = inputFactory;
        }

        public override OperationError[] EvaluateErrors(int count, double[][] inputs, double[][] outputs, double ulpScale)
        {
            double[] input0 = inputs[0];
            double[] input1 = inputs[1];
            double[] output = outputs[0];

            OperationError[] errors = new OperationError[count];
            for (int i = 0; i < count; i++)
            {
                double reference = refExecute(input0[i], input1[i]);
                errors[i] = evaluateError(input0[i], input1[i], output[i], reference, ulpScale);
            }
            return errors;
        }

        public override double[] ComputeReferences(double[][] inputs)
        {
            Debug.Assert(inputs.Length == 2);

            // Allocate results.
            double[] input0 = inputs[0];
            double[] input1 = inputs[1];
            int count = input0.Length;
            double[] results = new double[count];

            for (int ndx = 0; ndx < count; ndx++)
                results[ndx] = refExecute(input0[ndx], input1[ndx]);

            return results;
        }
    }

    class TestRunner
    {
        private const int       PRECISION_NUM_ITERS         = 1000;
        private const int       PRECISION_CHUNK_SIZE        = 1024;
        private const int       BENCHMARK_NUM_REPEATS       = 9;
        private const int       BENCHMARK_NUM_ITERS         = 10000;
        private const int       BENCHMARK_CHUNK_SIZE        = 128;
        private const double    BENCHMARK_DURATION_SECONDS  = 0.5;

        private static Random rnd = new Random(12345678);

        public struct PrecisionResult
        {
            public double   avgError;
            public double   maxError;
            public double   numPrecisionBits;
            public double[] worstInput;

            public PrecisionResult(double avgError, double maxError, double numPrecisionBits, double[] worstInput)
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

            public BenchmarkResult(double opsPerSec)
            {
                this.opsPerSec      = opsPerSec;
            }
        }

        static string DoubleToString(double val, int numChars)
        {
            string str = val.ToString("F" + numChars, System.Globalization.CultureInfo.InvariantCulture);
            return str.Substring(0, Math.Min(str.Length, numChars));
        }

        public static Array[] GenerateOpInputs(Operation opImpl, InputGenerator inputGenerator, int count)
        {
            Debug.Assert(opImpl.InputTypes.Length == inputGenerator.generators.Length);

            Array[] result = new Array[opImpl.InputTypes.Length];
            for (int ndx = 0; ndx < opImpl.InputTypes.Length; ndx++)
            {
                Type type = opImpl.InputTypes[ndx];
                F64[] values = inputGenerator.generators[ndx](rnd, count).Select(d => F64.FromDouble(d)).ToArray();
                if (type == typeof(F64))
                    result[ndx] = values;
                else if (type == typeof(F32))
                    result[ndx] = values.Select(v => F32.FromRaw((int)(v.raw >> 16))).ToArray();
                else
                    throw new InvalidOperationException("Unknown input data type: " + type);
            }
            return result;
        }

        public static Array[] GenerateOpOutputs(Operation opImpl, int count)
        {
            return opImpl.OutputTypes.Select(type => { return Array.CreateInstance(type, count); }).ToArray();
        }

        public static Array ConvertToType(Array values, Type toType)
        {
            // Check if types already match.
            Type fromType = values.GetType().GetElementType();
            if (fromType == toType)
                return values;

            // Conversions
            if (fromType == typeof(F64))
            {
                F64[] inputs = (F64[])values;
                if (toType == typeof(Double))
                    return inputs.Select(a => a.Double).ToArray();
            }
            else if (fromType == typeof(F32))
            {
                F32[] inputs = (F32[])values;
                if (toType == typeof(Double))
                    return inputs.Select(a => a.Double).ToArray();
            }

            throw new InvalidOperationException("Unsupported type conversion: " + fromType + " to " + toType);
        }

        public static Array ConcatArrays(Array a, Array b)
        {
            Array result = new object[a.Length + b.Length];
            a.CopyTo(result, 0);
            b.CopyTo(result, a.Length);
            return result;
        }

        public static PrecisionResult MeasureOpPrecision(OpFamilyBase opFamily, Operation opImpl)
        {
            int         numTested   = 0;
            double      totalErr    = 0.0;
            double      maxErr      = 0.0;
            double[]    worstInput  = new double[] {};

            // Input generators for operation's data type.
            InputGenerator[] inputGenerators = opFamily.inputFactory(opImpl.ValueBounds);
            foreach (InputGenerator inputGenerator in inputGenerators)
            {
                for (int iter = 0; iter < PRECISION_NUM_ITERS; iter++)
                {
                    // Generate inputs & execute operation.
                    Array[] inputs = GenerateOpInputs(opImpl, inputGenerator, PRECISION_CHUNK_SIZE);
                    Array[] outputs = GenerateOpOutputs(opImpl, PRECISION_CHUNK_SIZE);
                    opImpl.ArrayExecute(PRECISION_CHUNK_SIZE, inputs, outputs);

                    // Evaluate error against reference (using doubles as inputs/outputs).
                    double[][] dblInputs = inputs.Select(arr => OpFamilyBase.ConvertToDouble(arr)).ToArray();
                    double[][] dblOutputs = outputs.Select(arr => OpFamilyBase.ConvertToDouble(arr)).ToArray();
                    double ulpScale = 1.0 / Math.Pow(2.0, opImpl.PrecisionBits);
                    OperationError[] errors = opFamily.EvaluateErrors(PRECISION_CHUNK_SIZE, dblInputs, dblOutputs, ulpScale);

                    // Compute reference values.
                    double[] references = opFamily.ComputeReferences(dblInputs);

                    // Compute error stats.
                    for (int i = 0; i < PRECISION_CHUNK_SIZE; i++)
                    {
                        double reference = references[i];

                        ValueBounds bounds = opImpl.ValueBounds;
                        if (reference >= bounds.InputNegMax && reference <= bounds.InputPosMax)
                        {
                            if (errors[i].measured >= errors[i].minPossible)
                            {
                                double err = errors[i].measured / errors[i].reference;
                                numTested++;
                                totalErr += err;

                                if (err > maxErr)
                                {
                                    maxErr = err;
                                    worstInput = dblInputs.Select(arr => arr[i]).ToArray();
                                }
                            }
                        }
                    }
                }
            }

            //Debug.Assert(numTested > 1000);
            double avgErr = totalErr / (double)numTested;
            double numBits = -Math.Log(maxErr, 2.0);
            return new PrecisionResult(avgErr, maxErr, numBits, worstInput);
        }

        private static double BenchmarkFunction(Action func)
        {
            // Measure execution time X times (pick fastest).
            double bestOpsPerSec = 0.0;
            for (int repeat = 0; repeat < BENCHMARK_NUM_REPEATS; repeat++)
            {
                // Figure out how many loops to run
                var watch = System.Diagnostics.Stopwatch.StartNew();
                int numLoops = 0;
                for (;;)
                {
                    // Execute once.
                    numLoops++;
                    func();

                    // Check if enough iterations done
                    double elapsedSeconds = watch.ElapsedMilliseconds / 1000.0;
                    if (elapsedSeconds >= BENCHMARK_DURATION_SECONDS)
                    {
                        long numTotalOps = (long)numLoops * (long)BENCHMARK_NUM_ITERS * (long)BENCHMARK_CHUNK_SIZE;
                        double opsPerSec = numTotalOps / elapsedSeconds;
                        bestOpsPerSec = Math.Max(bestOpsPerSec, opsPerSec);
                        break;
                    }
                }
            }

            return bestOpsPerSec;
        }

        public static BenchmarkResult BenchmarkOperation(OpFamilyBase opFamily, Operation opImpl)
        {
            // Generate inputs and output arrays.
            // \todo [petri] use all input generators?
            InputGenerator[] inputGenerators = opFamily.inputFactory(opImpl.ValueBounds);
            Array[] inputs = GenerateOpInputs(opImpl, inputGenerators[0], BENCHMARK_CHUNK_SIZE);
            Array[] outputs = GenerateOpOutputs(opImpl, BENCHMARK_CHUNK_SIZE);

            // Measure execution time.
            double opsPerSec = BenchmarkFunction(() =>
            {
                for (int iter = 0; iter < BENCHMARK_NUM_ITERS; iter++)
                    opImpl.ArrayExecute(BENCHMARK_CHUNK_SIZE, inputs, outputs);
            });

            // Return result.
            return new BenchmarkResult(opsPerSec);
        }

        public static void PrintOperationSummary(string name, PrecisionResult precision, BenchmarkResult benchmark)
        {
            if (precision.maxError == 0.0)
                Console.WriteLine("| {0,22} | {1,10:0.00} |     exact | {2,16} | {3,16} |", name, benchmark.opsPerSec / 1000000.0, "0.0", "0.0");
            else
            {
                string coordsStr = String.Join(" ", precision.worstInput.Select(v => DoubleToString(v, 16)));
                Console.WriteLine("| {0,22} | {1,10:0.00} | {2,9:0.00} | {3} | {4} | {5}", name, benchmark.opsPerSec / 1000000.0, precision.numPrecisionBits, DoubleToString(precision.maxError, 16), DoubleToString(precision.avgError, 16), coordsStr);
            }
        }

        public static void MeasureOperation(OpFamilyBase opFamily, Operation opImpl)
        {
            PrecisionResult precision = MeasureOpPrecision(opFamily, opImpl);
            BenchmarkResult benchmark = BenchmarkOperation(opFamily, opImpl);
            PrintOperationSummary(opImpl.Name, precision, benchmark);
        }
    }

    class Program
    {
        static UnaryErrorEvaluator AbsoluteUnaryErrorEvaluator()
        {
            return (double input, double output, double reference, double ulpScale) =>
            {
                return new OperationError(Math.Abs(output - reference), 1.0, 0.0);
            };
        }

        static UnaryErrorEvaluator RelativeUnaryErrorEvaluator(double ulps = 4.0)
        {
            return (double input, double output, double reference, double ulpScale) =>
            {
                double minErr = ulps * ulpScale;
                double absRef = Math.Max(Math.Abs(reference), 256.0 * ulpScale);
                double err = Math.Abs(output - reference);
                return new OperationError(err, absRef, minErr);
                //return (err >= minErr) ? err / absRef : -1.0;
            };
        }

        static BinaryErrorEvaluator AbsoluteBinaryErrorEvaluator()
        {
            return (double a, double b, double output, double reference, double ulpScale) =>
            {
                return new OperationError(Math.Abs(output - reference), 1.0, 0.0);
            };
        }

        static BinaryErrorEvaluator RelativeBinaryErrorEvaluator(double ulps = 4.0)
        {
            return (double a, double b, double output, double reference, double ulpScale) =>
            {
                double minErr = ulps * ulpScale;
                double absRef = Math.Max(Math.Abs(reference), 256 * ulpScale);
                double err = Math.Abs(output - reference);
                //return (err >= minErr) ? err / absRef : -1.0;
                return new OperationError(err, absRef, minErr);
            };
        }

        static UnaryErrorEvaluator SinCosErrorEvaluator()
        {
            return (double input, double output, double reference, double ulpScale) =>
            {
                double inputScale = Math.Max(1.0, Math.Abs(input) / (2.0 * Math.PI));
                double err = Math.Abs(output - reference) / inputScale;
                return new OperationError(err, 1.0, 4.0 * ulpScale);
            };
        }

        static BinaryErrorEvaluator DivisionErrorEvaluator()
        {
            return (double a, double b, double output, double reference, double ulpScale) =>
            {
                const double minErr = 4.0 / 4294967296.0;
                double absRef = Math.Max(Math.Abs(reference), Math.Abs(b));
                double err = Math.Abs(output - reference);
                //return (err >= minErr) ? err / absRef : -1.0;
                return new OperationError(err, absRef, minErr);
            };
        }

        static OpFamilyBase[] operations =
        {
            new UnaryOpFamily(
                (double i0) => { return i0; },
                AbsoluteUnaryErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64("Identity F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i]; } }),
                    Operation.F32_F32("Identity F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i]; } })
                ),
                bounds => new[] {
                    InputGenerator.Unary(Input.Uniform(bounds.InputNegMax, bounds.InputPosMax))
                }
            ),

            new BinaryOpFamily(
                (double i0, double i1) => { return i0 + i1; },
                AbsoluteBinaryErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64_F64("a+b F64", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] + i1[i]; } }),
                    Operation.F32_F32_F32("a+b F32", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] + i1[i]; } })
                ),
                bounds => new[] {
                    InputGenerator.Binary(Input.Uniform(-10.0, 10.0), Input.Uniform(-10.0, 10.0)),
                    InputGenerator.Binary(Input.Uniform(bounds.InputNegMax, bounds.InputPosMax), Input.Uniform(bounds.InputNegMax, bounds.InputPosMax)),
                    InputGenerator.Binary(Input.Exponential(1.0, bounds.InputPosMax), Input.Exponential(bounds.InputPosMin, 1.0)),
                }
            ),

            new BinaryOpFamily(
                (double i0, double i1) => { return i0 - i1; },
                AbsoluteBinaryErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64_F64("a-b F64", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] - i1[i]; } }),
                    Operation.F32_F32_F32("a-b F32", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] - i1[i]; } })
                ),
                bounds => new[] {
                    InputGenerator.Binary(Input.Uniform(-10.0, 10.0), Input.Uniform(-10.0, 10.0)),
                    InputGenerator.Binary(Input.Uniform(bounds.InputNegMax, bounds.InputPosMax), Input.Uniform(bounds.InputNegMax, bounds.InputPosMax)),
                    InputGenerator.Binary(Input.Exponential(1.0, bounds.InputPosMax), Input.Exponential(bounds.InputPosMin, 1.0)),
                }
            ),

            new BinaryOpFamily(
                (double i0, double i1) => { return i0 * i1; },
                RelativeBinaryErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64_F64("a*b F64", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] * i1[i]; } }),
                    Operation.F32_F32_F32("a*b F32", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] * i1[i]; } })
                ),
                bounds => new[] {
                    InputGenerator.Binary(Input.Uniform(-1e3, 1e3), Input.Uniform(-1e3, 1e3)),
                    InputGenerator.Binary(Input.Uniform(-1e5, 1e5), Input.Uniform(-1.0, 1.0)),
                    InputGenerator.Binary(Input.Uniform(-1e9, 1e9), Input.Uniform(-1e-3, 1e-3))
                }
            ),

            new BinaryOpFamily(
                (double i0, double i1) => { return i0 / i1; },
                DivisionErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64_F64("a/b F64", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] / i1[i]; } }),
                    Operation.F32_F32_F32("a/b F32", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] / i1[i]; } })
                ),
                bounds => new[] {
                    InputGenerator.Binary(Input.Uniform(-1000.0, 1000.0), Input.Exponential(1.0, bounds.InputPosMax, Input.SignMode.Random)),
                    InputGenerator.Binary(Input.Uniform(bounds.InputNegMax, bounds.InputPosMax), Input.Uniform(bounds.InputNegMax, bounds.InputPosMax)),
                    InputGenerator.Binary(Input.Uniform(999.0, 1000.0), Input.Uniform(-999.0, -1000.0)),
                }
            ),

            new BinaryOpFamily(
                (double i0, double i1) => { return i0 % i1; },
                DivisionErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64_F64("a%b F64", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] % i1[i]; } }),
                    Operation.F32_F32_F32("a%b F32", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] % i1[i]; } })
                ),
                bounds => new[] {
                    // \note performance highly dependent on inputs
                    InputGenerator.Binary(Input.Uniform(-1000.0, 1000.0), Input.Exponential(1.0, 10.0)),
                    InputGenerator.Binary(Input.Uniform(-1000.0, 1000.0), Input.Exponential(1.0, bounds.InputPosMax, Input.SignMode.Random)),
                    InputGenerator.Binary(Input.Uniform(bounds.InputNegMax, bounds.InputPosMax), Input.Uniform(bounds.InputNegMax, bounds.InputPosMax)),
                    InputGenerator.Binary(Input.Uniform(999.0, 1000.0), Input.Uniform(-999.0, -1000.0)),
                }
            ),

            new BinaryOpFamily(
                (double i0, double i1) => { return Math.Min(i0, i1); },
                AbsoluteBinaryErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64_F64("Min(a,b) F64", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Min(i0[i], i1[i]); } }),
                    Operation.F32_F32_F32("Min(a,b) F32", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Min(i0[i], i1[i]); } })
                ),
                bounds => new[] {
                    InputGenerator.Binary(Input.Uniform(-1e5, 1e5), Input.Uniform(-1e5, 1e5)),
                    InputGenerator.Binary(Input.Uniform(bounds.InputNegMax, bounds.InputPosMax), Input.Uniform(bounds.InputNegMax, bounds.InputPosMax)),
                }
            ),

            new BinaryOpFamily(
                (double i0, double i1) => { return Math.Max(i0, i1); },
                AbsoluteBinaryErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64_F64("Max(a,b) F64", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Max(i0[i], i1[i]); } }),
                    Operation.F32_F32_F32("Max(a,b) F32", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Max(i0[i], i1[i]); } })
                ),
                bounds => new[] {
                    InputGenerator.Binary(Input.Uniform(-1.0, 1.0), Input.Uniform(-1.0, 1.0)),
                    InputGenerator.Binary(Input.Uniform(-1e5, 1e5), Input.Uniform(-1e5, 1e5)),
                    InputGenerator.Binary(Input.Uniform(bounds.InputNegMax, bounds.InputPosMax), Input.Uniform(bounds.InputNegMax, bounds.InputPosMax)),
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return Math.Ceiling(i0); },
                AbsoluteUnaryErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64("Ceil() F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Ceil(i0[i]); } }),
                    Operation.F32_F32("Ceil() F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Ceil(i0[i]); } })
                ),
                bounds => new[] {
                    InputGenerator.Unary(Input.Uniform(-1.0, 1.0)),
                    InputGenerator.Unary(Input.Uniform(-1e5, 1e5)),
                    InputGenerator.Unary(Input.Uniform(bounds.InputNegMax, bounds.InputPosMax))
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return Math.Floor(i0); },
                AbsoluteUnaryErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64("Floor() F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Floor(i0[i]); } }),
                    Operation.F32_F32("Floor() F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Floor(i0[i]); } })
                ),
                bounds => new[] {
                    InputGenerator.Unary(Input.Uniform(-1.0, 1.0)),
                    InputGenerator.Unary(Input.Uniform(-1e5, 1e5)),
                    InputGenerator.Unary(Input.Uniform(bounds.InputNegMax, bounds.InputPosMax))
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return Math.Floor(i0 + 0.5); },
                AbsoluteUnaryErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64("Round() F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Round(i0[i]); } }),
                    Operation.F32_F32("Round() F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Round(i0[i]); } })
                ),
                bounds => new[] {
                    InputGenerator.Unary(Input.Uniform(-1.0, 1.0)),
                    InputGenerator.Unary(Input.Uniform(-1e5, 1e5)),
                    InputGenerator.Unary(Input.Uniform(bounds.InputNegMax, bounds.InputPosMax))
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return i0 - Math.Floor(i0); },
                AbsoluteUnaryErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64("Fract() F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Fract(i0[i]); } }),
                    Operation.F32_F32("Fract() F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Fract(i0[i]); } })
                ),
                bounds => new[] {
                    InputGenerator.Unary(Input.Uniform(-1.0, 1.0)),
                    InputGenerator.Unary(Input.Uniform(-1e5, 1e5)),
                    InputGenerator.Unary(Input.Uniform(bounds.InputNegMax, bounds.InputPosMax))
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return Math.Abs(i0); },
                AbsoluteUnaryErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64("Abs() F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Abs(i0[i]); } }),
                    Operation.F32_F32("Abs() F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Abs(i0[i]); } })
                ),
                bounds => new[] {
                    InputGenerator.Unary(Input.Uniform(-1.0, 1.0)),
                    InputGenerator.Unary(Input.Uniform(-1e5, 1e5)),
                    InputGenerator.Unary(Input.Uniform(bounds.InputNegMax, bounds.InputPosMax))
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return -Math.Abs(i0); },
                AbsoluteUnaryErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64("Nabs() F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Nabs(i0[i]); } }),
                    Operation.F32_F32("Nabs() F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Nabs(i0[i]); } })
                ),
                bounds => new[] {
                    InputGenerator.Unary(Input.Uniform(-1.0, 1.0)),
                    InputGenerator.Unary(Input.Uniform(-1e5, 1e5)),
                    InputGenerator.Unary(Input.Uniform(bounds.InputNegMax, bounds.InputPosMax))
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return 1.0 / i0; },
                RelativeUnaryErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64("1/x F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.One / i0[i]; } }),
                    Operation.F32_F32("1/x F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.One / i0[i]; } })
                ),
                bounds => new[] {
                    InputGenerator.Unary(Input.Exponential(bounds.InputPosMin, bounds.InputPosMax)),
                    InputGenerator.Unary(Input.Exponential(bounds.InputNegMin, bounds.InputNegMax))
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return 1.0 / i0; },
                RelativeUnaryErrorEvaluator(),
                new[] {
                    Operation.F64_F64("Rcp(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Rcp(i0[i]); } }),
                    Operation.F64_F64("RcpFast(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.RcpFast(i0[i]); } }),
                    Operation.F64_F64("RcpFastest(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.RcpFastest(i0[i]); } }),
                    Operation.F32_F32("Rcp(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Rcp(i0[i]); } }),
                    Operation.F32_F32("RcpFast(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.RcpFast(i0[i]); } }),
                    Operation.F32_F32("RcpFastest(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.RcpFastest(i0[i]); } }),
                },
                bounds => new[] {
                    InputGenerator.Unary(Input.Exponential(bounds.InputPosMin, bounds.InputPosMax, Input.SignMode.Random)),
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return Math.Sqrt(i0); },
                RelativeUnaryErrorEvaluator(),
                new[] {
                    Operation.F64_F64("SqrtPrecise(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.SqrtPrecise(i0[i]); } }),
                    Operation.F64_F64("Sqrt(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Sqrt(i0[i]); } }),
                    Operation.F64_F64("SqrtFast(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.SqrtFast(i0[i]); } }),
                    Operation.F64_F64("SqrtFastest(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.SqrtFastest(i0[i]); } }),
                    // Operation.F32_F32("SqrtPrecise(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.SqrtPrecise(i0[i]); } }),
                    Operation.F32_F32("Sqrt(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Sqrt(i0[i]); } }),
                    Operation.F32_F32("SqrtFast(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.SqrtFast(i0[i]); } }),
                    Operation.F32_F32("SqrtFastest(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.SqrtFastest(i0[i]); } }),
                },
                bounds => new[] {
                    InputGenerator.Unary(Input.Exponential(bounds.InputPosMin, bounds.InputPosMax)),
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return 1.0 / Math.Sqrt(i0); },
                RelativeUnaryErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64("RSqrt(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.RSqrt(i0[i]); } }),
                    Operation.F64_F64("RSqrtFast(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.RSqrtFast(i0[i]); } }),
                    Operation.F64_F64("RSqrtFastest(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.RSqrtFastest(i0[i]); } }),
                    Operation.F32_F32("RSqrt(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.RSqrt(i0[i]); } }),
                    Operation.F32_F32("RSqrtFast(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.RSqrtFast(i0[i]); } }),
                    Operation.F32_F32("RSqrtFastest(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.RSqrtFastest(i0[i]); } })
                ),
                bounds => new[] {
                    InputGenerator.Unary(Input.Exponential(bounds.InputPosMin, bounds.InputPosMax))
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return Math.Exp(i0); },
                RelativeUnaryErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64("Exp(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Exp(i0[i]); } }),
                    Operation.F64_F64("ExpFast(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.ExpFast(i0[i]); } }),
                    Operation.F64_F64("ExpFastest(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.ExpFastest(i0[i]); } }),
                    Operation.F32_F32("Exp(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Exp(i0[i]); } }),
                    Operation.F32_F32("ExpFast(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.ExpFast(i0[i]); } }),
                    Operation.F32_F32("ExpFastest(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.ExpFastest(i0[i]); } })
                ),
                bounds => new[] {
                    InputGenerator.Unary(Input.Uniform(-100.0, 100.0)),
                    InputGenerator.Unary(Input.Exponential(bounds.InputPosMin, 10.0)),
                    InputGenerator.Unary(Input.Exponential(bounds.InputNegMin, -10.0))
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return Math.Pow(2.0, i0); },
                RelativeUnaryErrorEvaluator(),
                new[] {
                    Operation.F64_F64("Exp2(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Exp2(i0[i]); } }),
                    Operation.F64_F64("Exp2Fast(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Exp2Fast(i0[i]); } }),
                    Operation.F64_F64("Exp2Fastest(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Exp2Fastest(i0[i]); } }),
                    Operation.F32_F32("Exp2(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Exp2(i0[i]); } }),
                    Operation.F32_F32("Exp2Fast(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Exp2Fast(i0[i]); } }),
                    Operation.F32_F32("Exp2Fastest(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Exp2Fastest(i0[i]); } }),
                },
                bounds => new[] {
                    InputGenerator.Unary(Input.Uniform(-100.0, 100.0)),
                    InputGenerator.Unary(Input.Exponential(bounds.InputPosMin, 10.0)),
                    InputGenerator.Unary(Input.Exponential(bounds.InputNegMin, -10.0))
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return Math.Log(i0); },
                RelativeUnaryErrorEvaluator(16.0),
                new[] {
                    Operation.F64_F64("Log(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Log(i0[i]); } }),
                    Operation.F64_F64("LogFast(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.LogFast(i0[i]); } }),
                    Operation.F64_F64("LogFastest(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.LogFastest(i0[i]); } }),
                    Operation.F32_F32("Log(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Log(i0[i]); } }),
                    Operation.F32_F32("LogFast(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.LogFast(i0[i]); } }),
                    Operation.F32_F32("LogFastest(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.LogFastest(i0[i]); } }),
                },
                bounds => new[] {
                    InputGenerator.Unary(Input.Exponential(bounds.InputPosMin, bounds.InputPosMax)),
                    InputGenerator.Unary(Input.Exponential(0.999, 1.001)),
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return Math.Log(i0, 2.0); },
                RelativeUnaryErrorEvaluator(16.0),
                new[] {
                    Operation.F64_F64("Log2(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Log2(i0[i]); } }),
                    Operation.F64_F64("Log2Fast(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Log2Fast(i0[i]); } }),
                    Operation.F64_F64("Log2Fastest(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Log2Fastest(i0[i]); } }),
                    Operation.F32_F32("Log2(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Log2(i0[i]); } }),
                    Operation.F32_F32("Log2Fast(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Log2Fast(i0[i]); } }),
                    Operation.F32_F32("Log2Fastest(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Log2Fastest(i0[i]); } }),
                },
                bounds => new[] {
                    InputGenerator.Unary(Input.Exponential(bounds.InputPosMin, bounds.InputPosMax)),
                    InputGenerator.Unary(Input.Exponential(0.999, 1.001)),
                }
            ),

            new BinaryOpFamily(
                (double i0, double i1) => { return Math.Pow(i0, i1); },
                RelativeBinaryErrorEvaluator(),
                new[] {
                    Operation.F64_F64_F64("Pow(a,b) F64", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Pow(i0[i], i1[i]); } }),
                    Operation.F64_F64_F64("PowFast(a,b) F64", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.PowFast(i0[i], i1[i]); } }),
                    Operation.F64_F64_F64("PowFastest(a,b) F64", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.PowFastest(i0[i], i1[i]); } }),
                    Operation.F32_F32_F32("Pow(a,b) F32", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Pow(i0[i], i1[i]); } }),
                    Operation.F32_F32_F32("PowFast(a,b) F32", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.PowFast(i0[i], i1[i]); } }),
                    Operation.F32_F32_F32("PowFastest(a,b) F32", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.PowFastest(i0[i], i1[i]); } }),
                },
                bounds => new[] {
                    InputGenerator.Binary(Input.Exponential(1e-6, 1.0), Input.Exponential(1e-3, 20.0)),
                    InputGenerator.Binary(Input.Exponential(1.0, 16.0), Input.Exponential(1e-3, 1.0)),
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return Math.Sin(i0); },
                SinCosErrorEvaluator(),
                new[] {
                    Operation.F64_F64("Sin(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Sin(i0[i]); } }),
                    Operation.F64_F64("SinFast(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.SinFast(i0[i]); } }),
                    Operation.F64_F64("SinFastest(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.SinFastest(i0[i]); } }),
                    Operation.F32_F32("Sin(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Sin(i0[i]); } }),
                    Operation.F32_F32("SinFast(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.SinFast(i0[i]); } }),
                    Operation.F32_F32("SinFastest(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.SinFastest(i0[i]); } }),
                },
                bounds => new[] {
                    InputGenerator.Unary(Input.Uniform(-100.0, 100.0)),
                    InputGenerator.Unary(Input.Uniform(-1e5, 1e5)),
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return Math.Cos(i0); },
                SinCosErrorEvaluator(),
                new[] {
                    Operation.F64_F64("Cos(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Cos(i0[i]); } }),
                    Operation.F64_F64("CosFast(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.CosFast(i0[i]); } }),
                    Operation.F64_F64("CosFastest(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.CosFastest(i0[i]); } }),
                    Operation.F32_F32("Cos(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Cos(i0[i]); } }),
                    Operation.F32_F32("CosFast(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.CosFast(i0[i]); } }),
                    Operation.F32_F32("CosFastest(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.CosFastest(i0[i]); } }),
                },
                bounds => new[] {
                    InputGenerator.Unary(Input.Uniform(-100.0, 100.0)),
                    InputGenerator.Unary(Input.Uniform(-1e5, 1e5)),
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return Math.Tan(i0); },
                RelativeUnaryErrorEvaluator(16.0),
                new[] {
                    Operation.F64_F64("Tan(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Tan(i0[i]); } }),
                    Operation.F64_F64("TanFast(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.TanFast(i0[i]); } }),
                    Operation.F64_F64("TanFastest(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.TanFastest(i0[i]); } }),
                    Operation.F32_F32("Tan(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Tan(i0[i]); } }),
                    Operation.F32_F32("TanFast(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.TanFast(i0[i]); } }),
                    Operation.F32_F32("TanFastest(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.TanFastest(i0[i]); } }),
                },
                bounds => new[] {
                    InputGenerator.Unary(Input.Exponential(-0.1, -0.9999)),
                    InputGenerator.Unary(Input.Uniform(-0.1, 0.1)),
                    InputGenerator.Unary(Input.Exponential(0.1, 0.9999)),
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return Math.Asin(i0); },
                AbsoluteUnaryErrorEvaluator(),
                new[] {
                    Operation.F64_F64("Asin(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Asin(i0[i]); } }),
                    Operation.F64_F64("AsinFast(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.AsinFast(i0[i]); } }),
                    Operation.F64_F64("AsinFastest(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.AsinFastest(i0[i]); } }),
                    Operation.F32_F32("Asin(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Asin(i0[i]); } }),
                    Operation.F32_F32("AsinFast(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.AsinFast(i0[i]); } }),
                    Operation.F32_F32("AsinFastest(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.AsinFastest(i0[i]); } }),
                },
                bounds => new[] {
                    InputGenerator.Unary(Input.Uniform(-1.0, 1.0))
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return Math.Acos(i0); },
                AbsoluteUnaryErrorEvaluator(),
                new[] {
                    Operation.F64_F64("Acos(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Acos(i0[i]); } }),
                    Operation.F64_F64("AcosFast(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.AcosFast(i0[i]); } }),
                    Operation.F64_F64("AcosFastest(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.AcosFastest(i0[i]); } }),
                    Operation.F32_F32("Acos(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Acos(i0[i]); } }),
                    Operation.F32_F32("AcosFast(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.AcosFast(i0[i]); } }),
                    Operation.F32_F32("AcosFastest(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.AcosFastest(i0[i]); } }),
                },
                bounds => new[] {
                    InputGenerator.Unary(Input.Uniform(-1.0, 1.0))
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return Math.Atan(i0); },
                AbsoluteUnaryErrorEvaluator(),
                new[] {
                    Operation.F64_F64("Atan(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Atan(i0[i]); } }),
                    Operation.F64_F64("AtanFast(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.AtanFast(i0[i]); } }),
                    Operation.F64_F64("AtanFastest(x) F64", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.AtanFastest(i0[i]); } }),
                    Operation.F32_F32("Atan(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Atan(i0[i]); } }),
                    Operation.F32_F32("AtanFast(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.AtanFast(i0[i]); } }),
                    Operation.F32_F32("AtanFastest(x) F32", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.AtanFastest(i0[i]); } }),
                },
                bounds => new[] {
                    InputGenerator.Unary(Input.Uniform(-1.0, 1.0)),
                    InputGenerator.Unary(Input.Uniform(-1000.0, 1000.0)),
                    InputGenerator.Unary(Input.Exponential(1.0, bounds.InputPosMax)),
                    InputGenerator.Unary(Input.Exponential(-1.0, bounds.InputNegMax)),
                }
            ),

            new BinaryOpFamily(
                (double i0, double i1) => { return Math.Atan2(i0, i1); },
                AbsoluteBinaryErrorEvaluator(),
                new[] {
                    Operation.F64_F64_F64("Atan2(y,x) F64", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Atan2(i0[i], i1[i]); } }),
                    Operation.F64_F64_F64("Atan2Fast(y,x) F64", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Atan2Fast(i0[i], i1[i]); } }),
                    Operation.F64_F64_F64("Atan2Fastest(y,x) F64", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Atan2Fastest(i0[i], i1[i]); } }),
                    Operation.F32_F32_F32("Atan2(y,x) F32", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Atan2(i0[i], i1[i]); } }),
                    Operation.F32_F32_F32("Atan2Fast(y,x) F32", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Atan2Fast(i0[i], i1[i]); } }),
                    Operation.F32_F32_F32("Atan2Fastest(y,x) F32", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Atan2Fastest(i0[i], i1[i]); } }),
                },
                bounds => new[] {
                    InputGenerator.Binary(Input.Exponential(1e-6, 1e3, Input.SignMode.Random), Input.Exponential(1e-3, 1e5, Input.SignMode.Random)),
                    InputGenerator.Binary(Input.Exponential(1e-3, 1e5, Input.SignMode.Random), Input.Exponential(1e-6, 1e3, Input.SignMode.Random)),
                    InputGenerator.Binary(Input.Exponential(1.0, bounds.InputPosMax, Input.SignMode.Random), Input.Exponential(1.0, bounds.InputPosMax, Input.SignMode.Random)),
                }
            ),
        };

        static void TestOperations(string testFilter)
        {
            Console.WriteLine("OPERATIONS SUMMARY:");
            Console.WriteLine("|              Operation |     Mops/s | Precision |        Max error |        Avg error | Worst input");
            Console.WriteLine("|------------------------|-----------:|----------:|-----------------:|-----------------:|-----------------");

            foreach (OpFamilyBase opFamily in operations)
            {
                foreach (var opImpl in opFamily.operations)
                {
                    if (opImpl.Name.StartsWith(testFilter))
                        TestRunner.MeasureOperation(opFamily, opImpl);
                }
            }

            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            // Run on second core only, set process/thread priority to high.
            Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(2);
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            // Simple executions for debugging.
            // Console.WriteLine("{0} vs {1}", F32.Atan2(F32.FromDouble(0.0), F32.FromDouble(-30023.340087890)), Math.Atan2(0.0, -300.0));
            // Console.WriteLine("{0} vs {1}", F64.Atan2(F64.FromDouble(0.0), F64.FromDouble(-30023.340087890)), Math.Atan2(0.0, -300.0));
            // Console.WriteLine("{0} vs {1}", F64.Div(F64.FromDouble(994.866242629709), F64.FromDouble(-1.0099070169962)), 994.866242629709 / -1.0099070169962);

            // Name prefix of operations to test. Empty matches all operations.
            // Set this to, eg, "Atan2" or "Rcp(x)" to only measure that operation.
            string testFilter = "";

            // Run precision and performance tests.
            TestOperations(testFilter);

            /*Console.WriteLine("-ENTER-");
            Console.ReadLine();*/
        }
    }
}
