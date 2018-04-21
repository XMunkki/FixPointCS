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
The performance was measured with C# (Release build) on a Core i7-4700MQ @ 2.40GHz using a single thread.

|        Operation |     Mops/s | Precision
|------------------|-----------:|----------:
|         Identity |  1523.8095 |     exact
|           Ceil() |   870.7483 |     exact
|          Floor() |  1015.8730 |     exact
|          Round() |   441.3793 |     exact
|            Abs() |   416.9381 |     exact
|           Nabs() |   528.9256 |     exact
|              1/x |    32.8205 |     16.00
|           Rcp(x) |   101.5873 |     13.22
|          Sqrt(x) |     6.9189 |     17.23
|    Rcp(RSqrt(x)) |    46.8864 |     13.52
|      SqrtFast(x) |    96.9697 |     17.00
|         RSqrt(x) |   110.1549 |     15.66
|        1/Sqrt(x) |     5.8986 |     17.00
|     Rcp(Sqrt(x)) |     6.6321 |     13.39
|           Exp(x) |   120.7547 |     17.59
|          Exp2(x) |   156.4792 |     21.69
|           Log(x) |   111.3043 |     12.45
|          Log2(x) |    96.2406 |     12.20
|           Sin(x) |   188.7906 |     10.16
|           Cos(x) |   192.1922 |     10.15
|           Tan(x) |    44.5993 |     11.25
|          Asin(x) |    31.2195 |     11.78
|          Acos(x) |    31.2195 |     11.78
|          Atan(x) |    67.3684 |     11.87
|              a+b |  1049.1803 |     exact
|              a-b |  1066.6667 |     exact
|              a*b |   432.4324 |     26.45
|              a/b |    33.1606 |     41.97
|              a%b |    96.9697 |     exact
|         Min(a,b) |   463.7681 |     exact
|         Max(a,b) |   526.7490 |     exact
|        Pow(a, b) |    46.0432 |     14.94
|      Atan2(a, b) |    65.3061 |     11.66

## History

Initial version (Jere Sanisalo, Petri Kero)

