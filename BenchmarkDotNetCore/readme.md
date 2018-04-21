# Benchmark FixPointCS with BenchmarkDotNet
This project can be used to benchmark FixPointCS C# version with [BenchmarkDotNet](http://benchmarkdotnet.org/). 

## Requirements
* [.NET Core](https://www.microsoft.com/net/download/windows)
* BenchmarkDotNet
* FixPointCS

## Benchmark guide
1. Make sure you have .NET Core installed (*dotnet --version*)
2. Download the [FixPointCS](https://github.com/XMunkki/FixPointCS) to your computer
3. Move to **BenchmarkDotNet** folder
4. Do dotnet restore (*dotnet restore*) and wait few seconds while additional files are downloaded
5. Run the benchmark you want (Either via *dotnet run -c Release* and select benchmark or provide benchmark as parameter, like *dotnet run -c Release 0*)
6. Wait while benchmark runs and check out results afterwards from console/terminal or from **BenchmarkDotNet.Artifacts\results** folder

## Development guide

### Add additional benchmark
You can copy and paste existing code from **Program.cs** file (e.g. *public class CeilFP64vsDouble*) to create new benchmark. You have to also add new entry to the **switcher** variable located in *static void Main(string[] args)* in case you want to run your benchmark via switcher

### Add something fancy to benchmarks
BenchmarkDotNet does provide many nice features and you can find more help related to BenchmarkDotNet from [the official documentation](http://benchmarkdotnet.org/Guides/GettingStarted.htm)