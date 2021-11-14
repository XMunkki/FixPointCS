
# Changelog

## Unreleased

### Changes

- Converted C# projects to .NET 5. The sources are still compatible with .NET Framework and Unity as well.
- Fix C++ transpiled code to use INT64_MIN and INT64_MAX.
- Generate only LF in transpiler.
- Removed extra logic from F32.GetHashCode(), still outputs the same value.
- Use System.Numerics.BitOperations.LeadingZeroCount() with .NET 5 or higher. Boosts performance by about 10% for methods relying on it.

### Breaking (these can cause math function outputs to change and thus break determinism)

- Changed F64.GetHashCode() to use System.Int64.GetHashCode().
- Fixed64.Mod() checks for div-by-zero. Simplify the code by using the modulo operator (%).
- Fixed all Pow*(X, 0) to always return precisely 1.0, including Pow*(0, 0) which previously returned 0.

## 0.1 (2019-06-22)

- First released version (tagged after-the-fact)
