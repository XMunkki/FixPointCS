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
using System.IO;
using System.Threading;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

using FixMath;

namespace FixPointCSTest
{
    enum TargetLanguage
    {
        Java,
        Cpp
    }

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

        static public ValueGenerator Constant(double v)
        {
            return (Random rnd, int count) =>
            {
                double[] values = new double[count];
                for (int i = 0; i < count; i++)
                    values[i] = v;
                return values;
            };
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
        public string                           FuncName;           // Name of function (eg, "Fixed64.Mul")
        public ValueBounds                      ValueBounds;
        public Type[]                           InputTypes;
        public Type[]                           OutputTypes;
        public int                              PrecisionBits;
        public Action<int, Array[], Array[]>    ArrayExecute;

        public Operation(string funcName, ValueBounds valueBounds, Type[] inputTypes, Type[] outputTypes, Action<int, Array[], Array[]> arrayExecute)
        {
            this.FuncName       = funcName;
            this.ValueBounds    = valueBounds;
            this.InputTypes     = inputTypes;
            this.OutputTypes    = outputTypes;
            this.PrecisionBits  = outputTypes.Select(type => GetTypePrecisionBits(type)).Min();
            this.ArrayExecute   = arrayExecute;
        }

        public static int GetTypePrecisionBits(Type type)
        {
            if (type == typeof(F64))
                return 32;
            else if (type == typeof(F32))
                return 16;
            else if (type == typeof(int))
                return 0;
            else
                throw new InvalidOperationException("GetTypePrecisionBits(): Unknown type: " + type);
        }

        public static Operation F64_F64(string funcName, Action<int, F64[], F64[]> execute)
        {
            return new Operation(funcName, new ValueBoundsF64(), new[] { typeof(F64) }, new[] { typeof(F64) },
                (int count, Array[] inputs, Array[] outputs) => { execute(count, (F64[])inputs[0], (F64[])outputs[0]); }
            );
        }

        public static Operation F64_Int(string funcName, Action<int, F64[], int[]> execute)
        {
            return new Operation(funcName, new ValueBoundsF64(), new[] { typeof(F64) }, new[] { typeof(int) },
                (int count, Array[] inputs, Array[] outputs) => { execute(count, (F64[])inputs[0], (int[])outputs[0]); }
            );
        }

        public static Operation F64_F64_F64(string funcName, Action<int, F64[], F64[], F64[]> execute)
        {
            return new Operation(funcName, new ValueBoundsF64(), new[] { typeof(F64), typeof(F64) }, new[] { typeof(F64) }, (int count, Array[] inputs, Array[] outputs) => { execute(count, (F64[])inputs[0], (F64[])inputs[1], (F64[])outputs[0]); });
        }

        public static Operation F32_F32(string funcName, Action<int, F32[], F32[]> execute)
        {
            return new Operation(funcName, new ValueBoundsF32(), new[] { typeof(F32) }, new[] { typeof(F32) }, (int count, Array[] inputs, Array[] outputs) => { execute(count, (F32[])inputs[0], (F32[])outputs[0]); });
        }

        public static Operation F32_Int(string funcName, Action<int, F32[], int[]> execute)
        {
            return new Operation(funcName, new ValueBoundsF32(), new[] { typeof(F32) }, new[] { typeof(int) },
                (int count, Array[] inputs, Array[] outputs) => { execute(count, (F32[])inputs[0], (int[])outputs[0]); }
            );
        }

        public static Operation F32_F32_F32(string funcName, Action<int, F32[], F32[], F32[]> execute)
        {
            return new Operation(funcName, new ValueBoundsF32(), new[] { typeof(F32), typeof(F32) }, new[] { typeof(F32) }, (int count, Array[] inputs, Array[] outputs) => { execute(count, (F32[])inputs[0], (F32[])inputs[1], (F32[])outputs[0]); });
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

        public static string ValuesToString(TargetLanguage language, Array values)
        {
            string longPostfix = (language == TargetLanguage.Java) ? "L" : "LL";
            Type fromType = values.GetType().GetElementType();
            switch (language)
            {
                case TargetLanguage.Java:
                    if (fromType == typeof(F32))
                        return String.Join(", ", ((F32[])values).Select(v => v.Raw.ToString()));
                    else if (fromType == typeof(int))
                        return String.Join(", ", ((int[])values).Select(v => v.ToString()));
                    else if (fromType == typeof(F64))
                        return String.Join(", ", ((F64[])values).Select(v => v.Raw.ToString() + longPostfix));
                    else if (fromType == typeof(long))
                        return String.Join(", ", ((long[])values).Select(v => v.ToString() + longPostfix));
                    else
                        throw new InvalidOperationException("Invalid array type: " + fromType);

                case TargetLanguage.Cpp:
                    if (fromType == typeof(F32))
                        return String.Join(", ", ((F32[])values).Select(v => "(int32_t)0x" + v.Raw.ToString("X")));
                    else if (fromType == typeof(int))
                        return String.Join(", ", ((int[])values).Select(v => "(int32_t)0x" + v.ToString("X")));
                    else if (fromType == typeof(F64))
                        return String.Join(", ", ((F64[])values).Select(v => "(int64_t)0x" + v.Raw.ToString("X") + longPostfix));
                    else if (fromType == typeof(long))
                        return String.Join(", ", ((long[])values).Select(v => "(int64_t)0x" + v.ToString("X") + longPostfix));
                    else
                        throw new InvalidOperationException("Invalid array type: " + fromType);

                default:
                    throw new InvalidOperationException("Invalid language: " + language);
            }
        }

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
            else if (fromType == typeof(int))
            {
                int[] input = (int[])values;
                for (int i = 0; i < values.Length; i++)
                    output[i] = (double)input[i];
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
        private const int       UNITTEST_NUM_CASES          = 100;
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

        public static Array[] AllocateOpInputs(Operation opImpl, int count)
        {
            Array[] result = new Array[opImpl.InputTypes.Length];
            for (int ndx = 0; ndx < opImpl.InputTypes.Length; ndx++)
            {
                Type type = opImpl.InputTypes[ndx];

                if (type == typeof(F64))
                    result[ndx] = new F64[count];
                else if (type == typeof(F32))
                    result[ndx] = new F32[count];
                else
                    throw new InvalidOperationException("Unknown input data type: " + type);
            }
            return result;
        }

        public static void GenerateOpInputRange(Array[] result, Operation opImpl, InputGenerator inputGenerator, int offset, int count)
        {
            Debug.Assert(opImpl.InputTypes.Length == inputGenerator.generators.Length);

            // Generate values
            for (int ndx = 0; ndx < opImpl.InputTypes.Length; ndx++)
            {
                ValueGenerator generator = inputGenerator.generators[ndx];
                Type type = opImpl.InputTypes[ndx];

                if (type == typeof(F64))
                {
                    F64[] tmp = generator(rnd, count).Select(d => F64.FromDouble(d)).ToArray();
                    Array.Copy(tmp, 0, result[ndx], offset, count);
                }
                else if (type == typeof(F32))
                {
                    F32[] tmp = generator(rnd, count).Select(d => F32.FromDouble(d)).ToArray();
                    Array.Copy(tmp, 0, result[ndx], offset, count);
                }
                else
                    throw new InvalidOperationException("Unknown input data type: " + type);
            }
        }

        public static Array[] GenerateOpInputs(Operation opImpl, InputGenerator inputGenerator, int count)
        {
            Debug.Assert(opImpl.InputTypes.Length == inputGenerator.generators.Length);

            // Allocate & generate inputs
            Array[] result = AllocateOpInputs(opImpl, count);
            GenerateOpInputRange(result, opImpl, inputGenerator, 0, count);
            return result;
        }

        public static Array[] GenerateOpInputs(Operation opImpl, InputGenerator[] inputGenerators, int count)
        {
            // Allocate result
            Array[] result = AllocateOpInputs(opImpl, count);

            // Generate subblock with each generator
            int numGenerators = inputGenerators.Length;
            int countPerGenerator = (count + numGenerators - 1) / numGenerators;
            int offset = 0;
            foreach (InputGenerator inputGenerator in inputGenerators)
            {
                int blockSize = Math.Min(count - offset, countPerGenerator);
                GenerateOpInputRange(result, opImpl, inputGenerator, offset, blockSize);
                offset += blockSize;
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
                    // \todo [petri] also computed in EvaluateErrors() !
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

        private static string MapTypeName(TargetLanguage language, Type type)
        {
            switch (language)
            {
                case TargetLanguage.Java:
                    if (type == typeof(F64))
                        return "long";
                    else if (type == typeof(F32))
                        return "int";
                    else if (type == typeof(long))
                        return "long";
                    else if (type == typeof(int))
                        return "int";
                    else
                        throw new InvalidOperationException("Invalid type: " + type);

                case TargetLanguage.Cpp:
                    if (type == typeof(F64))
                        return "int64_t";
                    else if (type == typeof(F32))
                        return "int32_t";
                    else if (type == typeof(long))
                        return "int64_t";
                    else if (type == typeof(int))
                        return "int32_t";
                    else
                        throw new InvalidOperationException("Invalid type: " + type);

                default:
                    throw new InvalidOperationException("Invalid language: " + language);
            }
        }

        public static void GenerateUnitTestCases(TargetLanguage language, StreamWriter file, OpFamilyBase opFamily, Operation opImpl)
        {
            file.WriteLine($"\t// {opImpl.FuncName}()");

            string funcType = (language == TargetLanguage.Java) ? "public static void" : "static void";
            string checkFuncName = (language == TargetLanguage.Java) ? "Util.Check" : "Util::Check";

            // Input generators for operation's data type.
            InputGenerator[] inputGenerators = opFamily.inputFactory(opImpl.ValueBounds);

            // Generate inputs & execute operation.
            Array[] inputs = GenerateOpInputs(opImpl, inputGenerators, UNITTEST_NUM_CASES);
            Array[] outputs = GenerateOpOutputs(opImpl, UNITTEST_NUM_CASES);
            opImpl.ArrayExecute(UNITTEST_NUM_CASES, inputs, outputs);

            string testFuncName = opImpl.FuncName.Replace(".", "_");
            string opName = (language == TargetLanguage.Java) ? opImpl.FuncName : opImpl.FuncName.Replace(".", "::");

            file.WriteLine($"\t{funcType} {testFuncName}()");
            file.WriteLine("\t{");

            for (int inputNdx = 0; inputNdx < inputs.Length; inputNdx++)
            {
                string typeName = MapTypeName(language, inputs[inputNdx].GetType().GetElementType());
                string values = OpFamilyBase.ValuesToString(language, inputs[inputNdx]);

                if (language == TargetLanguage.Java)
                    file.WriteLine($"\t\t{typeName}[] input{inputNdx} = new {typeName}[]{{ {values} }};");
                else
                    file.WriteLine($"\t\tstatic const {typeName} input{inputNdx}[] = {{ {values} }};");
            }

            for (int outputNdx = 0; outputNdx < outputs.Length; outputNdx++)
            {
                string typeName = MapTypeName(language, outputs[outputNdx].GetType().GetElementType());
                string values = OpFamilyBase.ValuesToString(language, outputs[outputNdx]);
                if (language == TargetLanguage.Java)
                    file.WriteLine($"\t\t{typeName}[] output{outputNdx} = new {typeName}[]{{ {values} }};");
                else
                    file.WriteLine($"\t\tstatic const {typeName} output{outputNdx}[] = {{ {values} }};");
            }

            string inputArgs = String.Join(", ", Enumerable.Range(0, inputs.Length).Select(inputNdx => $"input{inputNdx}[ndx]"));

            file.WriteLine($"\t\tfor (int ndx = 0; ndx < {UNITTEST_NUM_CASES}; ndx++)");
            file.WriteLine($"\t\t\t{checkFuncName}(\"{opName}\", {opName}({inputArgs}), output0[ndx], {inputArgs});");
            file.WriteLine("\t}");
            file.WriteLine();
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
            PrintOperationSummary(opImpl.FuncName, precision, benchmark);
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
                double minErr = 4.0 * ulpScale;
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
                    Operation.F64_F64("Fixed64.Nop", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i]; } }),
                    Operation.F32_F32("Fixed32.Nop", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i]; } })
                ),
                bounds => new[] {
                    InputGenerator.Unary(Input.Uniform(bounds.InputNegMax, bounds.InputPosMax))
                }
            ),

            new BinaryOpFamily(
                (double i0, double i1) => { return i0 + i1; },
                AbsoluteBinaryErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64_F64("Fixed64.Add", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] + i1[i]; } }),
                    Operation.F32_F32_F32("Fixed32.Add", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] + i1[i]; } })
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
                    Operation.F64_F64_F64("Fixed64.Sub", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] - i1[i]; } }),
                    Operation.F32_F32_F32("Fixed32.Sub", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] - i1[i]; } })
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
                    Operation.F64_F64_F64("Fixed64.Mul", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] * i1[i]; } }),
                    Operation.F32_F32_F32("Fixed32.Mul", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] * i1[i]; } })
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
                    Operation.F64_F64_F64("Fixed64.DivPrecise", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] / i1[i]; } }),
                    Operation.F32_F32_F32("Fixed32.DivPrecise", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] / i1[i]; } })
                ),
                bounds => new[] {
                    InputGenerator.Binary(Input.Uniform(-1000.0, 1000.0), Input.Exponential(1.0, bounds.InputPosMax, Input.SignMode.Random)),
                    InputGenerator.Binary(Input.Uniform(bounds.InputNegMax, bounds.InputPosMax), Input.Uniform(bounds.InputNegMax, -0.001)),
                    InputGenerator.Binary(Input.Uniform(bounds.InputNegMax, bounds.InputPosMax), Input.Uniform(0.001, bounds.InputPosMax)),
                    InputGenerator.Binary(Input.Uniform(999.0, 1000.0), Input.Uniform(-999.0, -1000.0)),
                }
            ),

            new BinaryOpFamily(
                (double i0, double i1) => { return i0 % i1; },
                DivisionErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64_F64("Fixed64.Mod", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] % i1[i]; } }),
                    Operation.F32_F32_F32("Fixed32.Mod", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = i0[i] % i1[i]; } })
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
                    Operation.F64_F64_F64("Fixed64.Min", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Min(i0[i], i1[i]); } }),
                    Operation.F32_F32_F32("Fixed32.Min", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Min(i0[i], i1[i]); } })
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
                    Operation.F64_F64_F64("Fixed64.Max", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Max(i0[i], i1[i]); } }),
                    Operation.F32_F32_F32("Fixed32.Max", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Max(i0[i], i1[i]); } })
                ),
                bounds => new[] {
                    InputGenerator.Binary(Input.Uniform(-1.0, 1.0), Input.Uniform(-1.0, 1.0)),
                    InputGenerator.Binary(Input.Uniform(-1e5, 1e5), Input.Uniform(-1e5, 1e5)),
                    InputGenerator.Binary(Input.Uniform(bounds.InputNegMax, bounds.InputPosMax), Input.Uniform(bounds.InputNegMax, bounds.InputPosMax)),
                }
            ),

            // \todo [petri] implement clamp
            //new TernaryOpFamily(
            //    (double i0, double i1, double i2) => { return Math.Clamp(i0, i1, i2); },
            //    AbsoluteBinaryErrorEvaluator(),
            //    Operation.Multi(
            //        Operation.F64_F64_F64_F64("Fixed64.Clamp", (int n, F64[] i0, F64[] i1, F64[] i2, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Clamp(i0[i], i1[i], i2[i]); } }),
            //        Operation.F32_F32_F32_F32("Fixed32.Clamp", (int n, F32[] i0, F32[] i1, F32[] i2, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Clamp(i0[i], i1[i], i2[i]); } })
            //    ),
            //    bounds => new[] {
            //        InputGenerator.Ternary(Input.Uniform(-1.0, 1.0), Input.Uniform(-1.0, 1.0), Input.Uniform(-1.0, 1.0)),
            //        InputGenerator.Ternary(Input.Uniform(-1e5, 1e5), Input.Uniform(-1e5, 1e5), Input.Uniform(-1e5, 1e5)),
            //        InputGenerator.Ternary(Input.Uniform(bounds.InputNegMax, bounds.InputPosMax), Input.Uniform(bounds.InputNegMax, bounds.InputPosMax), Input.Uniform(bounds.InputNegMax, bounds.InputPosMax)),
            //    }
            //),

            new UnaryOpFamily(
                (double i0) => { return Math.Ceiling(i0); },
                AbsoluteUnaryErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64("Fixed64.Ceil", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Ceil(i0[i]); } }),
                    Operation.F32_F32("Fixed32.Ceil", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Ceil(i0[i]); } })
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
                    Operation.F64_F64("Fixed64.Floor", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Floor(i0[i]); } }),
                    Operation.F32_F32("Fixed32.Floor", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Floor(i0[i]); } })
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
                    Operation.F64_F64("Fixed64.Round", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Round(i0[i]); } }),
                    Operation.F32_F32("Fixed32.Round", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Round(i0[i]); } })
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
                    Operation.F64_F64("Fixed64.Fract", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Fract(i0[i]); } }),
                    Operation.F32_F32("Fixed32.Fract", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Fract(i0[i]); } })
                ),
                bounds => new[] {
                    InputGenerator.Unary(Input.Uniform(-1.0, 1.0)),
                    InputGenerator.Unary(Input.Uniform(-1e5, 1e5)),
                    InputGenerator.Unary(Input.Uniform(bounds.InputNegMax, bounds.InputPosMax))
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return Math.Ceiling(i0); },
                AbsoluteUnaryErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_Int("Fixed64.CeilToInt", (int n, F64[] i0, int[] o) => { for (int i=0; i<n; i++) { o[i] = F64.CeilToInt(i0[i]); } }),
                    Operation.F32_Int("Fixed32.CeilToInt", (int n, F32[] i0, int[] o) => { for (int i=0; i<n; i++) { o[i] = F32.CeilToInt(i0[i]); } })
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
                    Operation.F64_Int("Fixed64.FloorToInt", (int n, F64[] i0, int[] o) => { for (int i=0; i<n; i++) { o[i] = F64.FloorToInt(i0[i]); } }),
                    Operation.F32_Int("Fixed32.FloorToInt", (int n, F32[] i0, int[] o) => { for (int i=0; i<n; i++) { o[i] = F32.FloorToInt(i0[i]); } })
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
                    Operation.F64_Int("Fixed64.RoundToInt", (int n, F64[] i0, int[] o) => { for (int i=0; i<n; i++) { o[i] = F64.RoundToInt(i0[i]); } }),
                    Operation.F32_Int("Fixed32.RoundToInt", (int n, F32[] i0, int[] o) => { for (int i=0; i<n; i++) { o[i] = F32.RoundToInt(i0[i]); } })
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
                    Operation.F64_F64("Fixed64.Abs", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Abs(i0[i]); } }),
                    Operation.F32_F32("Fixed32.Abs", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Abs(i0[i]); } })
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
                    Operation.F64_F64("Fixed64.Nabs", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Nabs(i0[i]); } }),
                    Operation.F32_F32("Fixed32.Nabs", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Nabs(i0[i]); } })
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
                new[] {
                    Operation.F64_F64("Fixed64.Rcp", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Rcp(i0[i]); } }),
                    Operation.F64_F64("Fixed64.RcpFast", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.RcpFast(i0[i]); } }),
                    Operation.F64_F64("Fixed64.RcpFastest", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.RcpFastest(i0[i]); } }),
                    Operation.F32_F32("Fixed32.Rcp", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Rcp(i0[i]); } }),
                    Operation.F32_F32("Fixed32.RcpFast", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.RcpFast(i0[i]); } }),
                    Operation.F32_F32("Fixed32.RcpFastest", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.RcpFastest(i0[i]); } }),
                },
                bounds => new[] {
                    InputGenerator.Unary(Input.Exponential(bounds.InputPosMin, bounds.InputPosMax, Input.SignMode.Random)),
                }
            ),

            new BinaryOpFamily(
                (double i0, double i1) => { return i0 / i1; },
                DivisionErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64_F64("Fixed64.Div", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Div(i0[i], i1[i]); } }),
                    Operation.F64_F64_F64("Fixed64.DivFast", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.DivFast(i0[i], i1[i]); } }),
                    Operation.F64_F64_F64("Fixed64.DivFastest", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.DivFastest(i0[i], i1[i]); } }),
                    Operation.F32_F32_F32("Fixed32.Div", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Div(i0[i], i1[i]); } }),
                    Operation.F32_F32_F32("Fixed32.DivFast", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.DivFast(i0[i], i1[i]); } }),
                    Operation.F32_F32_F32("Fixed32.DivFastest", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.DivFastest(i0[i], i1[i]); } })
                ),
                bounds => new[] {
                    InputGenerator.Binary(Input.Constant(0.06715393066406), Input.Constant(-1.0237274169921)),
                    InputGenerator.Binary(Input.Uniform(-1000.0, 1000.0), Input.Exponential(1.0, bounds.InputPosMax, Input.SignMode.Random)),
                    InputGenerator.Binary(Input.Uniform(bounds.InputNegMax, bounds.InputPosMax), Input.Uniform(bounds.InputNegMax, bounds.InputPosMax)),
                    InputGenerator.Binary(Input.Uniform(999.0, 1000.0), Input.Uniform(-999.0, -1000.0)),
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return Math.Sqrt(i0); },
                RelativeUnaryErrorEvaluator(),
                new[] {
                    Operation.F64_F64("Fixed64.SqrtPrecise", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.SqrtPrecise(i0[i]); } }),
                    Operation.F64_F64("Fixed64.Sqrt", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Sqrt(i0[i]); } }),
                    Operation.F64_F64("Fixed64.SqrtFast", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.SqrtFast(i0[i]); } }),
                    Operation.F64_F64("Fixed64.SqrtFastest", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.SqrtFastest(i0[i]); } }),
                    Operation.F32_F32("Fixed32.SqrtPrecise", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.SqrtPrecise(i0[i]); } }),
                    Operation.F32_F32("Fixed32.Sqrt", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Sqrt(i0[i]); } }),
                    Operation.F32_F32("Fixed32.SqrtFast", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.SqrtFast(i0[i]); } }),
                    Operation.F32_F32("Fixed32.SqrtFastest", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.SqrtFastest(i0[i]); } }),
                },
                bounds => new[] {
                    InputGenerator.Unary(Input.Exponential(bounds.InputPosMin, bounds.InputPosMax)),
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return 1.0 / Math.Sqrt(i0); },
                RelativeUnaryErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64("Fixed64.RSqrt", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.RSqrt(i0[i]); } }),
                    Operation.F64_F64("Fixed64.RSqrtFast", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.RSqrtFast(i0[i]); } }),
                    Operation.F64_F64("Fixed64.RSqrtFastest", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.RSqrtFastest(i0[i]); } }),
                    Operation.F32_F32("Fixed32.RSqrt", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.RSqrt(i0[i]); } }),
                    Operation.F32_F32("Fixed32.RSqrtFast", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.RSqrtFast(i0[i]); } }),
                    Operation.F32_F32("Fixed32.RSqrtFastest", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.RSqrtFastest(i0[i]); } })
                ),
                bounds => new[] {
                    InputGenerator.Unary(Input.Exponential(bounds.InputPosMin, bounds.InputPosMax))
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return Math.Exp(i0); },
                RelativeUnaryErrorEvaluator(),
                Operation.Multi(
                    Operation.F64_F64("Fixed64.Exp", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Exp(i0[i]); } }),
                    Operation.F64_F64("Fixed64.ExpFast", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.ExpFast(i0[i]); } }),
                    Operation.F64_F64("Fixed64.ExpFastest", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.ExpFastest(i0[i]); } }),
                    Operation.F32_F32("Fixed32.Exp", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Exp(i0[i]); } }),
                    Operation.F32_F32("Fixed32.ExpFast", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.ExpFast(i0[i]); } }),
                    Operation.F32_F32("Fixed32.ExpFastest", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.ExpFastest(i0[i]); } })
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
                    Operation.F64_F64("Fixed64.Exp2", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Exp2(i0[i]); } }),
                    Operation.F64_F64("Fixed64.Exp2Fast", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Exp2Fast(i0[i]); } }),
                    Operation.F64_F64("Fixed64.Exp2Fastest", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Exp2Fastest(i0[i]); } }),
                    Operation.F32_F32("Fixed32.Exp2", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Exp2(i0[i]); } }),
                    Operation.F32_F32("Fixed32.Exp2Fast", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Exp2Fast(i0[i]); } }),
                    Operation.F32_F32("Fixed32.Exp2Fastest", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Exp2Fastest(i0[i]); } }),
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
                    Operation.F64_F64("Fixed64.Log", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Log(i0[i]); } }),
                    Operation.F64_F64("Fixed64.LogFast", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.LogFast(i0[i]); } }),
                    Operation.F64_F64("Fixed64.LogFastest", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.LogFastest(i0[i]); } }),
                    Operation.F32_F32("Fixed32.Log", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Log(i0[i]); } }),
                    Operation.F32_F32("Fixed32.LogFast", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.LogFast(i0[i]); } }),
                    Operation.F32_F32("Fixed32.LogFastest", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.LogFastest(i0[i]); } }),
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
                    Operation.F64_F64("Fixed64.Log2", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Log2(i0[i]); } }),
                    Operation.F64_F64("Fixed64.Log2Fast", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Log2Fast(i0[i]); } }),
                    Operation.F64_F64("Fixed64.Log2Fastest", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Log2Fastest(i0[i]); } }),
                    Operation.F32_F32("Fixed32.Log2", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Log2(i0[i]); } }),
                    Operation.F32_F32("Fixed32.Log2Fast", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Log2Fast(i0[i]); } }),
                    Operation.F32_F32("Fixed32.Log2Fastest", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Log2Fastest(i0[i]); } }),
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
                    Operation.F64_F64_F64("Fixed64.Pow", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Pow(i0[i], i1[i]); } }),
                    Operation.F64_F64_F64("Fixed64.PowFast", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.PowFast(i0[i], i1[i]); } }),
                    Operation.F64_F64_F64("Fixed64.PowFastest", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.PowFastest(i0[i], i1[i]); } }),
                    Operation.F32_F32_F32("Fixed32.Pow", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Pow(i0[i], i1[i]); } }),
                    Operation.F32_F32_F32("Fixed32.PowFast", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.PowFast(i0[i], i1[i]); } }),
                    Operation.F32_F32_F32("Fixed32.PowFastest", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.PowFastest(i0[i], i1[i]); } }),
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
                    Operation.F64_F64("Fixed64.Sin", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Sin(i0[i]); } }),
                    Operation.F64_F64("Fixed64.SinFast", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.SinFast(i0[i]); } }),
                    Operation.F64_F64("Fixed64.SinFastest", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.SinFastest(i0[i]); } }),
                    Operation.F32_F32("Fixed32.Sin", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Sin(i0[i]); } }),
                    Operation.F32_F32("Fixed32.SinFast", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.SinFast(i0[i]); } }),
                    Operation.F32_F32("Fixed32.SinFastest", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.SinFastest(i0[i]); } }),
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
                    Operation.F64_F64("Fixed64.Cos", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Cos(i0[i]); } }),
                    Operation.F64_F64("Fixed64.CosFast", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.CosFast(i0[i]); } }),
                    Operation.F64_F64("Fixed64.CosFastest", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.CosFastest(i0[i]); } }),
                    Operation.F32_F32("Fixed32.Cos", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Cos(i0[i]); } }),
                    Operation.F32_F32("Fixed32.CosFast", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.CosFast(i0[i]); } }),
                    Operation.F32_F32("Fixed32.CosFastest", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.CosFastest(i0[i]); } }),
                },
                bounds => new[] {
                    InputGenerator.Unary(Input.Uniform(-100.0, 100.0)),
                    InputGenerator.Unary(Input.Uniform(-1e4, 1e4)),
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return Math.Tan(i0); },
                RelativeUnaryErrorEvaluator(16.0),
                new[] {
                    Operation.F64_F64("Fixed64.Tan", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Tan(i0[i]); } }),
                    Operation.F64_F64("Fixed64.TanFast", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.TanFast(i0[i]); } }),
                    Operation.F64_F64("Fixed64.TanFastest", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.TanFastest(i0[i]); } }),
                    Operation.F32_F32("Fixed32.Tan", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Tan(i0[i]); } }),
                    Operation.F32_F32("Fixed32.TanFast", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.TanFast(i0[i]); } }),
                    Operation.F32_F32("Fixed32.TanFastest", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.TanFastest(i0[i]); } }),
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
                    Operation.F64_F64("Fixed64.Asin", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Asin(i0[i]); } }),
                    Operation.F64_F64("Fixed64.AsinFast", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.AsinFast(i0[i]); } }),
                    Operation.F64_F64("Fixed64.AsinFastest", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.AsinFastest(i0[i]); } }),
                    Operation.F32_F32("Fixed32.Asin", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Asin(i0[i]); } }),
                    Operation.F32_F32("Fixed32.AsinFast", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.AsinFast(i0[i]); } }),
                    Operation.F32_F32("Fixed32.AsinFastest", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.AsinFastest(i0[i]); } }),
                },
                bounds => new[] {
                    InputGenerator.Unary(Input.Uniform(-1.0, 1.0))
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return Math.Acos(i0); },
                AbsoluteUnaryErrorEvaluator(),
                new[] {
                    Operation.F64_F64("Fixed64.Acos", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Acos(i0[i]); } }),
                    Operation.F64_F64("Fixed64.AcosFast", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.AcosFast(i0[i]); } }),
                    Operation.F64_F64("Fixed64.AcosFastest", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.AcosFastest(i0[i]); } }),
                    Operation.F32_F32("Fixed32.Acos", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Acos(i0[i]); } }),
                    Operation.F32_F32("Fixed32.AcosFast", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.AcosFast(i0[i]); } }),
                    Operation.F32_F32("Fixed32.AcosFastest", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.AcosFastest(i0[i]); } }),
                },
                bounds => new[] {
                    InputGenerator.Unary(Input.Uniform(-1.0, 1.0))
                }
            ),

            new UnaryOpFamily(
                (double i0) => { return Math.Atan(i0); },
                AbsoluteUnaryErrorEvaluator(),
                new[] {
                    Operation.F64_F64("Fixed64.Atan", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Atan(i0[i]); } }),
                    Operation.F64_F64("Fixed64.AtanFast", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.AtanFast(i0[i]); } }),
                    Operation.F64_F64("Fixed64.AtanFastest", (int n, F64[] i0, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.AtanFastest(i0[i]); } }),
                    Operation.F32_F32("Fixed32.Atan", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Atan(i0[i]); } }),
                    Operation.F32_F32("Fixed32.AtanFast", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.AtanFast(i0[i]); } }),
                    Operation.F32_F32("Fixed32.AtanFastest", (int n, F32[] i0, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.AtanFastest(i0[i]); } }),
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
                    Operation.F64_F64_F64("Fixed64.Atan2", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Atan2(i0[i], i1[i]); } }),
                    Operation.F64_F64_F64("Fixed64.Atan2Fast", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Atan2Fast(i0[i], i1[i]); } }),
                    Operation.F64_F64_F64("Fixed64.Atan2Fastest", (int n, F64[] i0, F64[] i1, F64[] o) => { for (int i=0; i<n; i++) { o[i] = F64.Atan2Fastest(i0[i], i1[i]); } }),
                    Operation.F32_F32_F32("Fixed32.Atan2", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Atan2(i0[i], i1[i]); } }),
                    Operation.F32_F32_F32("Fixed32.Atan2Fast", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Atan2Fast(i0[i], i1[i]); } }),
                    Operation.F32_F32_F32("Fixed32.Atan2Fastest", (int n, F32[] i0, F32[] i1, F32[] o) => { for (int i=0; i<n; i++) { o[i] = F32.Atan2Fastest(i0[i], i1[i]); } }),
                },
                bounds => new[] {
                    InputGenerator.Binary(Input.Exponential(1e-6, 1e3, Input.SignMode.Random), Input.Exponential(1e-3, 1e4, Input.SignMode.Random)),
                    InputGenerator.Binary(Input.Exponential(1e-3, 1e4, Input.SignMode.Random), Input.Exponential(1e-6, 1e3, Input.SignMode.Random)),
                    InputGenerator.Binary(Input.Exponential(1.0, bounds.InputPosMax, Input.SignMode.Random), Input.Exponential(1.0, bounds.InputPosMax, Input.SignMode.Random)),
                }
            ),
        };

        static void TestOperations(string testFilter)
        {
            Console.WriteLine("|              Operation |     Mops/s | Precision |        Max error |        Avg error | Worst input");
            Console.WriteLine("|------------------------|-----------:|----------:|-----------------:|-----------------:|-----------------");

            foreach (OpFamilyBase opFamily in operations)
            {
                foreach (Operation opImpl in opFamily.operations)
                {
                    if (opImpl.FuncName.Contains(testFilter))
                        TestRunner.MeasureOperation(opFamily, opImpl);
                }
            }

            Console.WriteLine();
        }

        static void GenerateUnitTestsJava(string testFilter)
        {
            Console.WriteLine("Generating Java unit tests..");

            using (StreamWriter file = new StreamWriter("../../../Java/UnitTest.java"))
            {
                file.WriteLine("package fixpointcs.test;");
                file.WriteLine("");
                file.WriteLine("import fixpointcs.*;");
                file.WriteLine("");
                file.WriteLine("class UnitTest");
                file.WriteLine("{");

                List<string> funcNames = new List<string>();

                foreach (OpFamilyBase opFamily in operations)
                {
                    foreach (Operation opImpl in opFamily.operations)
                    {
                        // Skip Nop()
                        if (opImpl.FuncName.EndsWith(".Nop"))
                            continue;

                        if (opImpl.FuncName != null && opImpl.FuncName.Contains(testFilter))
                        {
                            Console.WriteLine("  {0}", opImpl.FuncName);
                            TestRunner.GenerateUnitTestCases(TargetLanguage.Java, file, opFamily, opImpl);
                            funcNames.Add(opImpl.FuncName.Replace(".", "_"));
                        }
                    }
                }

                file.WriteLine("\tpublic static void TestAll()");
                file.WriteLine("\t{");
                foreach (string funcName in funcNames)
                    file.WriteLine($"\t\t{funcName}();");
                file.WriteLine("\t}");

                file.WriteLine("}");
            }

            Console.WriteLine("");
        }

        static void GenerateUnitTestsCpp(string testFilter)
        {
            Console.WriteLine("Generating C++ unit tests..");

            using (StreamWriter file = new StreamWriter("../../../Cpp/UnitTest.cpp"))
            {
                file.WriteLine("#include \"UnitTest.h\"");
                file.WriteLine("#include \"Fixed32.h\"");
                file.WriteLine("#include \"Fixed64.h\"");
                file.WriteLine("");

                List<string> funcNames = new List<string>();

                file.WriteLine("namespace UnitTest");
                file.WriteLine("{");

                foreach (OpFamilyBase opFamily in operations)
                {
                    foreach (Operation opImpl in opFamily.operations)
                    {
                        // Skip Nop()
                        if (opImpl.FuncName.EndsWith(".Nop"))
                            continue;

                        if (opImpl.FuncName != null && opImpl.FuncName.Contains(testFilter))
                        {
                            Console.WriteLine("  {0}", opImpl.FuncName);
                            TestRunner.GenerateUnitTestCases(TargetLanguage.Cpp, file, opFamily, opImpl);
                            funcNames.Add(opImpl.FuncName.Replace(".", "_"));
                        }
                    }
                }

                file.WriteLine("}");
                file.WriteLine("");

                file.WriteLine("void UnitTest_TestAll()");
                file.WriteLine("{");
                foreach (string funcName in funcNames)
                    file.WriteLine($"\t\tUnitTest::{funcName}();");
                file.WriteLine("}");
            }

            Console.WriteLine("");
        }

        static void GenerateUnitTests(string testFilter)
        {
            GenerateUnitTestsJava(testFilter);
            GenerateUnitTestsCpp(testFilter);
        }

        static void Main(string[] args)
        {
            // Run on second core only, set process/thread priority to high.
            // Produces more stable results from benchmarks.
            Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(2);
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            //CompareBenchmark();

            // Simple executions for debugging.
            // Console.WriteLine("{0} vs {1}", F32.Atan2(F32.FromDouble(0.0), F32.FromDouble(-30023.340087890)), Math.Atan2(0.0, -300.0));
            // Console.WriteLine("{0} vs {1}", F64.Atan2(F64.FromDouble(0.0), F64.FromDouble(-30023.340087890)), Math.Atan2(0.0, -300.0));
            // Console.WriteLine("{0} vs {1}", F64.Div(F64.FromDouble(994.866242629709), F64.FromDouble(-1.0099070169962)), 994.866242629709 / -1.0099070169962);
            // Console.WriteLine("{0}", F64.CeilToInt(F64.FromDouble(-1073741494.0000)));
            // Console.WriteLine("Cos(): {0} vs {1}", F32.Cos(F32.FromDouble(32766.4527587891)), Math.Cos(32766.4527587891));
            // Console.WriteLine("Asin(): {0} vs {1}", F32.Asin(F32.FromDouble(0.99998474121093)), Math.Asin(0.99998474121093));
            // Console.WriteLine("Atan2(): {0} vs {1}", F32.Atan2(F32.FromDouble(-4.4691772460937), F32.FromDouble(-3.9427642822265)), Math.Atan2(-4.4691772460937, -3.9427642822265));
            // Console.WriteLine();

            // Generate unit tests for Java & C++
            GenerateUnitTests("");

            // Filter for choosing which tests to run. Empty runs all tests
            // Examples:
            // - "Atan2" runs the whole Atan2 family
            // - "ToInt" runs all the XxxToInt family
            // - "Fixed32.Rcp"
            // - "Fixed32" executes all the whole s16.16 ops
            string testFilter = "";

            // Run precision and performance tests.
            TestOperations(testFilter);

            /*Console.WriteLine("-ENTER-");
            Console.ReadLine();*/
        }
    }
}
