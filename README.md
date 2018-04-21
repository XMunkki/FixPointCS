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
The performance was measured on a Core i7-4700MQ @ 2.40GHz using a single thread.

|        Operation |     Mops/s | Precision
|------------------|-----------:|----------:
|           Ceil() |   870.7483 |     exact
|          Floor() |  1015.8730 |     exact
|          Round() |   439.8625 |     exact
|            Abs() |   402.5157 |     exact
|           Nabs() |   505.9289 |     exact
|              1/x |    33.2468 |     16.00
|           Rcp(x) |    89.5105 |     13.00
|          Sqrt(x) |     7.3988 |     17.23
|    Rcp(RSqrt(x)) |    42.5249 |     14.01
|      SqrtFast(x) |    56.1404 |     14.00
|         RSqrt(x) |   100.6289 |     14.01
|        1/Sqrt(x) |     6.0952 |     17.00
|     Rcp(Sqrt(x)) |     6.9189 |     14.01
|           Exp(x) |    45.7143 |     14.58
|          Exp2(x) |    52.2449 |     16.87
|           Log(x) |    55.1724 |      7.08
|          Log2(x) |    46.5455 |      7.60
|           Sin(x) |   199.3769 |     10.16
|           Cos(x) |   194.5289 |     10.15
|           Tan(x) |    43.0976 |     11.25
|          Asin(x) |    21.6216 |      7.98
|          Acos(x) |    21.8430 |      7.98
|          Atan(x) |    48.4848 |      7.98
|              a+b |  1066.6667 |     exact
|              a-b |  1075.6303 |     exact
|              a*b |   438.3562 |     26.45
|              a/b |    33.1606 |     41.97
|         Min(a,b) |   445.9930 |     exact
|         Max(a,b) |   503.9370 |     exact
|        Pow(a, b) |    17.5824 |      7.72
|      Atan2(a, b) |    54.0084 |      7.89

## History

Initial version (Jere Sanisalo, Petri Kero)

