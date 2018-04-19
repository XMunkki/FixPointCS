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

## TODO: Benchmarks

Todo: performance profiles of functions (also against doubles, floats)

## History

Initial version (Jere Sanisalo, Petri Kero)

