# FixPointCS 

A signed 32.32 fixed point numerics library for 64bit CPU architectures


## Description

The FixPointCS is numerics library that implements efficient signed 32.32bit fixed point numbers.
The library assumes that the target CPU architecture has native 64bit support, and all of
the operations are built on that assumption.
Intended use cases include deterministic game system simulations (for example in Unity games).
The library is easily portable across compilers and platforms.
The results of a certain sequence of numeric operations are the same on any platform, and any compiler optimizations.

Target languages: C#, C++ (generated)

Supported operations:
 - Conversions: from int/double, to double, to string
 - Basic: +, -, *, /, Mod (remainder)
 - Convenience: Abs, Min, Max, Sign
 - Sqrt, RSqrt (1/sqrt(x)), Rcp (1/x), Exp2, Exp, Log, Log2, Pow, Sin, Cos, Tan, Asin, Acos, Atan, Atan2

## Library design philosophy

The design of the operations can rely on the assumption that the target platform has efficient support for 64bit operations.
The raw library comes with multiple implementations of various operations.
These operations have different performance vs precision trade-offs.
Being a fixed point library, the library leans towards efficiency over full correctness.
There are no denors, infs or NaNs that are often supported by floating point numbers.
The core library is implemented as a single file (Fixed64.cs).

There are extra convenience wrapper classes for easier usage:

 - F64.cs: Struct wrapper for a single value with operator overloads.
 - F64Vec2.cs: 2D fixed point vector.

You're free to use the included convenience wrappers or build your own.

## Usage instructions

### (C#) Use as a library

When you compile FixPointCS, a .NET DLL is produced.
Reference that in your project and use the functionality directly.
It's advisable to compile the release version (debug version has assertions that slow the library down).

### (C#) Include source files

You can just copy the required files (Fixed64.cs at the minimum) into your own project and compile & use the functionality directly.

### (C++) Include source files

Copy the Fixed64.h file from the Cpp-folder into your project.
All the code is self contained within.

It is automatically generated from the Fixed64.cs file by the GenerateCpp -tool.

## Contributing

New contributions are welcome! Especially new operations, faster versions of existing ones or testing/verification code.

## Operations

Below is a table describing the supported operations, their relative performance and precision (as number of accurate bits).
The performance was measured with C# (Release build) on Windows 10 using a Core i7-4700MQ @ 2.40GHz using a single thread.

|          Operation |     Mops/s | Precision
|--------------------|-----------:|----------:
|           Identity |    1502.35 |     exact
|                a+b |    1066.67 |     exact
|                a-b |    1057.85 |     exact
|                a*b |     288.29 |     exact
|                a/b |      32.08 |     exact
|                a%b |      96.24 |     exact
|           Min(a,b) |     477.61 |     exact
|           Max(a,b) |     505.93 |     exact
|             Ceil() |     920.86 |     exact
|            Floor() |     977.10 |     exact
|            Round() |     507.94 |     exact
|            Fract() |    1163.64 |     exact
|              Abs() |     914.29 |     exact
|             Nabs() |     673.68 |     exact
|                1/x |      32.16 |     52.01
|             Rcp(x) |      79.50 |     23.71
|         RcpFast(x) |     101.59 |     16.13
|      RcpFastest(x) |     108.47 |     10.95
|     SqrtPrecise(x) |       6.92 |     exact
|            Sqrt(x) |      91.43 |     23.22
|        SqrtFast(x) |     104.07 |     16.23
|     SqrtFastest(x) |     104.07 |     13.32
|           RSqrt(x) |      89.51 |     24.25
|       RSqrtFast(x) |     101.59 |     15.71
|    RSqrtFastest(x) |     108.47 |     10.54
|             Exp(x) |     127.24 |     23.01
|         ExpFast(x) |     137.63 |     17.85
|      ExpFastest(x) |     150.94 |     12.97
|            Exp2(x) |     170.21 |     23.01
|        Exp2Fast(x) |     199.38 |     17.85
|     Exp2Fastest(x) |     238.81 |     12.97
|             Log(x) |      84.21 |     30.05
|         LogFast(x) |      96.24 |     15.10
|      LogFastest(x) |     120.75 |      9.27
|            Log2(x) |      89.51 |     exact
|        Log2Fast(x) |     104.92 |     17.98
|     Log2Fastest(x) |     121.90 |     10.69
|          Pow(a, b) |      41.29 |     22.89
|      PowFast(a, b) |      52.67 |     14.87
|   PowFastest(a, b) |      53.78 |     10.31
|             Sin(x) |     153.11 |     26.63
|         SinFast(x) |     177.29 |     19.57
|      SinFastest(x) |     212.62 |     12.57
|             Cos(x) |     125.74 |     26.64
|         CosFast(x) |     171.58 |     19.57
|      CosFastest(x) |     198.76 |     12.57
|             Tan(x) |      33.42 |     23.78
|         TanFast(x) |      38.67 |     16.24
|      TanFastest(x) |      48.48 |     11.22
|            Asin(x) |      23.62 |     23.46
|        AsinFast(x) |      27.23 |     16.60
|     AsinFastest(x) |      35.56 |     11.44
|            Acos(x) |      23.10 |     22.67
|        AcosFast(x) |      26.23 |     16.60
|     AcosFastest(x) |      34.78 |     11.44
|            Atan(x) |      39.02 |     24.90
|        AtanFast(x) |      47.58 |     17.53
|     AtanFastest(x) |      71.91 |     11.45
|        Atan2(a, b) |      39.02 |     22.03
|    Atan2Fast(a, b) |      47.94 |     17.52
| Atan2Fastest(a, b) |      72.73 |     11.13

## History

Initial version (Jere Sanisalo, Petri Kero)

