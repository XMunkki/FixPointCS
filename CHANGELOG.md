
# Changelog

## 0.3 (unrelesed)

### Changes

- Upgrade all C# projects to .NET 6.
- FixMath: Replace F32 and F64 constructors with explicit FromInt(), FromFloat(), FromDouble() to avoid accidental implicit casts.
- FixMath: F32 and F64 now implement IComparable to make them usable in unit test assertion macros.
- FixMath: Optimize Lerp() operations with a simpler formula.
- FixMath: Introduce Clamp01() method for F32 and F64 which clamps the input to [0, 1] range.
- FixMath: Introduce some extra inlining to speed up code using the library.
- FixMath: F32 family of Length*() and Normalize*() methods now use 64-bit intermediate results to avoid overflows.
- FixMath: F32Vec*.LengthSqr() now returns F64 to avoid overflows.
- Fixed32.Nlz(): Disabled the System.Numerics.BitOperations.LeadingZeroCount() because it's slower in benchmarks.

## 0.2 (2021-11-14)

### Changes

- Converted C# projects to .NET 5. The sources are still compatible with .NET Framework and Unity as well.
- Fix C++ transpiled code to use INT64_MIN and INT64_MAX.
- Generate only LF in transpiler.
- Removed extra logic from F32.GetHashCode(), still outputs the same value.
- Use System.Numerics.BitOperations.LeadingZeroCount() with .NET 5 or higher. Boosts performance by about 10% for methods relying on it.

### Breaking (these can cause math function outputs to change and thus break determinism)

- Changed F64.GetHashCode() to use System.Int64.GetHashCode().
- Fixed32 and Fixed64.Mod() checks for div-by-zero. Simplify the code by using the modulo operator (%).
- Fixed all Pow*(X, 0) to always return precisely 1.0, including Pow*(0, 0) which previously returned 0.

## 0.1 (2019-06-22)

- First released version (tagged after-the-fact)
