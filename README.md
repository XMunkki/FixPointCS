# FixPointCS

#### A fast, multi-language, multi-precision fixed-point library!

---

## Key Features

- **Deterministic**: Operations produce bit-identical results across languages and compilers
- **Fast**: All operations use efficient algorithms and are highly optimized
- **Multi-Language**: Supports C#, Java, and C++
- **Multiple Types**: Supports signed 32.32 and 16.16 fixed-point numbers
- **Multiple Precisions**: Operations provide variants with 24, 16, and 10 bits of precision
- **Extensive**: All standard math functions supported, except hyperbolic trigonometry
- **Well Tested**: The library comes with a comprehensive performance and precision testing suite

## Overview

FixPointCS aims to provide as robust and efficient core math operations as possible, while keeping
the API simple to stay compatible with multiple languages. It is intended to be a building block
for higher-level math libraries, not to be used directly from application code.

For convenience, FixPointCS does include a higher-level math library as well (only for C#). It is
provided as an example on how to utilize the core primitives. It also be used as-is, or customized
to a project's needs. The convenience library is located in *Examples/FixMath*.

## Getting Started

### Core Math Library (all languages)

The simplest and recommended way to use FixPointCS is to copy the relevant source files directly into
your project. The core math operations reside in the following files:
- For **C#**: FixPointCS/Fixed32.cs, FixPointCS/Fixed64.cs and FixPointCS/FixedUtil.cs
- For **Java**: Java/Fixed32.java, Java/Fixed64.java and Java/FixedUtil.java
- For **C++**: Cpp/Fixed64.h, Cpp/Fixed32.h and Cpp/FixedUtil.h

### FixMath Convenience Library (C# only)

For C#, you can also use the provided higher-level math library, located under *Examples/FixMath*. In
order to get started writing applications as easily as possible, using the example library is recommended.

The FixMath library provides basic scalar types (*F32* and *F64*, signed 16.16 and 32.32 fixed-point
values, respectively), as well as vector types (*F32VecN* and *F64VecN*).

For examples on how to use the FixMath library, see *Examples/FixedTracer/FixedTracer.cs*, which
implements a simple fixed-point raytracer.

## Supported Functions

Supported operations include:
- Arithmetic: Add(), Sub(), Mul(), Div(), Rcp() (reciprocal), Mod() (modulo)
- Trigonometry: Sin(), Cos(), Tan(), Asin(), Acos(), Atan(), Atan2()
- Exponential: Exp(), Exp2(), Log(), Log2(), Pow()
- Square root: Sqrt(), RSqrt() (reciprocal square root)
- Utility: Abs(), Nabs(), Sign(), Ceil(), Floor(), Round(), Fract(), Min(), Max()
- Conversions: CeilToInt(), FloorToInt(), RoundToInt(), FromDouble(), FromFloat(), ToDouble(), ToFloat()

Note that the conversions to/from floating point values are not guaranteed to be deterministic.

For full list of supported operations, see [functions.md](functions.md).

## Precision Guide

The library supports both signed 32.32 fixed-point type (Fixed64), and signed 16.16 fixed-point (Fixed32).
For each operation, for both types, each approximate function comes with three precision variants with
different speed/precision trade off. For example, *Sin()* has 24 bits of precision, *SinFast()* has 16
bits of precision and *SinFastest()* has 10 bits of precision.

The precision is relative to the output value, so for example 10 bits of precision means that the answer
is accurate to about 1 part in 1000 (or has error margin of 0.1%). The Fast variant is typically about
10-15% faster than the highest precision, and the Fastest variant about 20-30% faster. See
[functions.md](functions.md) for details.

Div and Sqrt also come with a Precise variant (*DivPrecise()*, *SqrtPrecise()*), which produce a result
that is exactly correct within representable fixed-point numbers.

## Known Limitations

- Few operations are much slower without a 64-bit CPU, most notably s32.32 multiply and division
- The library opts for performance and determinism over full correctness in edge cases like overflows

## License

Available under MIT license, see LICENSE.txt for details.
