using System;
using System.Linq;
using System.Diagnostics;
using FixPointCS;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace BenchmarkDotNetCore
{
	using ValueGenerator = Func<Random, int, double[]>;
	
	#region Input generator
	
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
	
	#endregion // Input generator
	
	
	#region Ceil
	
	public class CeilFP64vsDouble
	{
		private const int BENCHMARK_CHUNK_SIZE = 128;
		private static readonly Random rnd = new Random(12345678);
	
		private readonly double[] doubleArrayInputs;
		private readonly F64[] f64ArrayInputs;
		
		private readonly double[] doubleArrayOutputs;
		private readonly F64[] f64ArrayOutputs;
		
		public CeilFP64vsDouble()
		{
			ValueGenerator inputGenerator = Input.Uniform(-1.0, 1.0);
			
			this.doubleArrayInputs = inputGenerator(rnd, BENCHMARK_CHUNK_SIZE);
			this.f64ArrayInputs = this.doubleArrayInputs.Select(d => F64.FromDouble(d)).ToArray();
			
			this.doubleArrayOutputs = new double[BENCHMARK_CHUNK_SIZE];
			this.f64ArrayOutputs = new F64[BENCHMARK_CHUNK_SIZE];
		}
		
		[Benchmark]
		public void FP64Run()
		{
			for (int i = 0; i < f64ArrayInputs.Length; i++)
			{
				this.f64ArrayOutputs[i] = this.f64ArrayInputs[i].Ceil();
			}
		}
		
		[Benchmark(Baseline = true)]
		public void DoubleRun()
		{
			for (int i = 0; i < doubleArrayInputs.Length; i++)
			{
				this.doubleArrayOutputs[i] = Math.Ceiling(this.doubleArrayInputs[i]);
			}
		}
	}
	
	#endregion // Ceil
	
	
	#region Floor
	
	public class FloorFP64vsDouble
	{
		private const int BENCHMARK_CHUNK_SIZE = 128;
		private static readonly Random rnd = new Random(12345678);
	
		private readonly double[] doubleArrayInputs;
		private readonly F64[] f64ArrayInputs;
		
		private readonly double[] doubleArrayOutputs;
		private readonly F64[] f64ArrayOutputs;
		
		public FloorFP64vsDouble()
		{
			ValueGenerator inputGenerator = Input.Uniform(-1.0, 1.0);
			
			this.doubleArrayInputs = inputGenerator(rnd, BENCHMARK_CHUNK_SIZE);
			this.f64ArrayInputs = this.doubleArrayInputs.Select(d => F64.FromDouble(d)).ToArray();
			
			this.doubleArrayOutputs = new double[BENCHMARK_CHUNK_SIZE];
			this.f64ArrayOutputs = new F64[BENCHMARK_CHUNK_SIZE];
		}
		
		[Benchmark]
		public void FP64Run()
		{
			for (int i = 0; i < f64ArrayInputs.Length; i++)
			{
				this.f64ArrayOutputs[i] = this.f64ArrayInputs[i].Floor();
			}
		}
		
		[Benchmark(Baseline = true)]
		public void DoubleRun()
		{
			for (int i = 0; i < doubleArrayInputs.Length; i++)
			{
				this.doubleArrayOutputs[i] = Math.Floor(this.doubleArrayInputs[i]);
			}
		}
	}
	
	#endregion // Floor
	
	
    public class Program
    {
        static void Main(string[] args)
        {
            var switcher = new BenchmarkSwitcher(new[] {
			typeof(CeilFP64vsDouble),
			typeof(FloorFP64vsDouble)
			// If you add another benchmark, please add reference to here
			});
			
			switcher.Run(args);
        }
    }
}
