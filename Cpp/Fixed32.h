//
// FixPointCS
//
// Copyright(c) Jere Sanisalo, Petri Kero
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
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY INT64_C(C)AIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER INT64_C(DEA)INGS IN THE
// SOFTWARE.
//

//
// GENERATED FILE!!!
//
// Generated from Fixed32.cs, part of the FixPointCS project (MIT license).
//
#pragma once
#ifndef __FIXED32_H
#define __FIXED32_H

// Include numeric types
#include <stdint.h>
#include "FixedUtil.h"
#include "Fixed64.h"


// If FP_ASSERT is not custom-defined, then use the standard one
#ifndef FP_ASSERT
#   include <assert.h>
#   define FP_ASSERT(x) assert(x)
#endif

// If FP_CUSTOM_INVALID_ARGS is defined, then the used is expected to implement the following functions in
// the FixedUtil namespace:
//  void InvalidArgument(const char* funcName, const char* argName, FP_INT argValue);
//  void InvalidArgument(const char* funcName, const char* argName, FP_INT argValue1, FP_INT argValue2);
//	void InvalidArgument(const char* funcName, const char* argName, FP_LONG argValue);
//	void InvalidArgument(const char* funcName, const char* argName, FP_LONG argValue1, FP_LONG argValue2);
// These functions should handle the cases for invalid arguments in any desired way (assert, exception, log, ignore etc).
//#define FP_CUSTOM_INVALID_ARGS

namespace Fixed32
{
    typedef int32_t FP_INT;
    typedef uint32_t FP_UINT;
    typedef int64_t FP_LONG;
    typedef uint64_t FP_ULONG;

    static_assert(sizeof(FP_INT) == 4, "Wrong bytesize for FP_INT");
    static_assert(sizeof(FP_UINT) == 4, "Wrong bytesize for FP_UINT");
    static_assert(sizeof(FP_LONG) == 8, "Wrong bytesize for FP_LONG");
    static_assert(sizeof(FP_ULONG) == 8, "Wrong bytesize for FP_ULONG");



    static const FP_INT Shift = 16;
    static const FP_INT FractionMask = (1 << Shift) - 1;
    static const FP_INT IntegerMask = ~FractionMask;

    // Constants
    static const FP_INT Zero = 0;
    static const FP_INT Neg1 = -1 << Shift;
    static const FP_INT One = 1 << Shift;
    static const FP_INT Two = 2 << Shift;
    static const FP_INT Three = 3 << Shift;
    static const FP_INT Four = 4 << Shift;
    static const FP_INT Half = One >> 1;
    static const FP_INT Pi = (FP_INT)(13493037705L >> 16); //(FP_INT)(Math.PI * 65536.0) << 16;
    static const FP_INT Pi2 = (FP_INT)(26986075409L >> 16);
    static const FP_INT PiHalf = (FP_INT)(6746518852L >> 16);
    static const FP_INT E = (FP_INT)(11674931555L >> 16);

    static const FP_INT MinValue = INT32_MIN;
    static const FP_INT MaxValue = INT32_MAX;

    // Private constants
    static const FP_INT RCP_LN2       = (FP_INT)(0x171547652L >> 16);    // 1.0 / log(2.0) ~= 1.4426950408889634
    static const FP_INT RCP_LOG2_E    = (FP_INT)(2977044471L >> 16);     // 1.0 / log2(e) ~= 0.6931471805599453
    static const FP_INT RCP_TWO_PI    = 683565276;                    // 1.0 / (4.0 * 0.5 * pi);  -- the 4.0 factor converts directly to s2.30

    /// <summary>
    /// Converts an integer to a fixed-point value.
    /// </summary>
    static FP_INT FromInt(FP_INT v)
    {
        return (FP_INT)v << Shift;
    }

    /// <summary>
    /// Converts a double to a fixed-point value.
    /// </summary>
    static FP_INT FromDouble(double v)
    {
        return (FP_INT)(v * 65536.0);
    }

    /// <summary>
    /// Converts a float to a fixed-point value.
    /// </summary>
    static FP_INT FromFloat(float v)
    {
        return (FP_INT)(v * 65536.0f);
    }

    /// <summary>
    /// Converts a fixed-point value into an integer by rounding it up to nearest integer.
    /// </summary>
    static FP_INT CeilToInt(FP_INT v)
    {
        return (FP_INT)((v + (One - 1)) >> Shift);
    }

    /// <summary>
    /// Converts a fixed-point value into an integer by rounding it down to nearest integer.
    /// </summary>
    static FP_INT FloorToInt(FP_INT v)
    {
        return (FP_INT)(v >> Shift);
    }

    /// <summary>
    /// Converts a fixed-point value into an integer by rounding it to nearest integer.
    /// </summary>
    static FP_INT RoundToInt(FP_INT v)
    {
        return (FP_INT)((v + Half) >> Shift);
    }

    /// <summary>
    /// Converts a fixed-point value into a double.
    /// </summary>
    static double ToDouble(FP_INT v)
    {
        return (double)v * (1.0 / 65536.0);
    }

    /// <summary>
    /// Converts a FP value into a float.
    /// </summary>
    static float ToFloat(FP_INT v)
    {
        return (float)v * (1.0f / 65536.0f);
    }

    /// <summary>
    /// Converts the value to a human readable string.
    /// </summary>

    /// <summary>
    /// Returns the absolute (positive) value of x.
    /// </summary>
    static FP_INT Abs(FP_INT x)
    {
        // \note fails with MinValue
        FP_INT mask = x >> 31;
        return (x + mask) ^ mask;
    }

    /// <summary>
    /// Negative absolute value (returns -abs(x)).
    /// </summary>
    static FP_INT Nabs(FP_INT x)
    {
        return -Abs(x);
    }

    /// <summary>
    /// Round up to nearest integer.
    /// </summary>
    static FP_INT Ceil(FP_INT x)
    {
        return (x + FractionMask) & IntegerMask;
    }

    /// <summary>
    /// Round down to nearest integer.
    /// </summary>
    static FP_INT Floor(FP_INT x)
    {
        return x & IntegerMask;
    }

    /// <summary>
    /// Round to nearest integer.
    /// </summary>
    static FP_INT Round(FP_INT x)
    {
        return (x + Half) & IntegerMask;
    }

    /// <summary>
    /// Returns the fractional part of x. Equal to 'x - floor(x)'.
    /// </summary>
    static FP_INT Fract(FP_INT x)
    {
        return x & FractionMask;
    }

    /// <summary>
    /// Returns the minimum of the two values.
    /// </summary>
    static FP_INT Min(FP_INT a, FP_INT b)
    {
        return (a < b) ? a : b;
    }

    /// <summary>
    /// Returns the maximum of the two values.
    /// </summary>
    static FP_INT Max(FP_INT a, FP_INT b)
    {
        return (a > b) ? a : b;
    }

    /// <summary>
    /// Returns the value clamped between min and max.
    /// </summary>
    static FP_INT Clamp(FP_INT a, FP_INT min, FP_INT max)
    {
        return (a > max) ? max : (a < min) ? min : a;
    }

    /// <summary>
    /// Returns the sign of the value (-1 if negative, 0 if zero, 1 if positive).
    /// </summary>
    static FP_INT Sign(FP_INT x)
    {
        // https://stackoverflow.com/questions/14579920/fast-sign-of-integer-in-c/14612418#14612418
        return ((x >> 31) | (FP_INT)(((FP_UINT)-x) >> 31));
    }

    /// <summary>
    /// Adds the two FP numbers together.
    /// </summary>
    static FP_INT Add(FP_INT a, FP_INT b)
    {
        return a + b;
    }

    /// <summary>
    /// Subtracts the two FP numbers from each other.
    /// </summary>
    static FP_INT Sub(FP_INT a, FP_INT b)
    {
        return a - b;
    }

    /// <summary>
    /// Multiplies two FP values together.
    /// </summary>
    static FP_INT Mul(FP_INT a, FP_INT b)
    {
        return (FP_INT)(((FP_LONG)a * (FP_LONG)b) >> Shift);
    }

    /// <summary>
    /// Linearly interpolate from a to b by t.
    /// </summary>
    static FP_INT Lerp(FP_INT a, FP_INT b, FP_INT t)
    {
        FP_LONG ta = (FP_LONG)a * (One - (FP_LONG)t);
        FP_LONG tb = (FP_LONG)b * (FP_LONG)t;
        return (FP_INT)((ta + tb) >> Shift);
    }

    static FP_INT Nlz(FP_UINT x)
    {
    #if NET5_0_OR_GREATER
        return System.Numerics.BitOperations.LeadingZeroCount(x);
    #else
        FP_INT n = 0;
        if (x <= 0x0000FFFF) { n = n + 16; x = x << 16; }
        if (x <= 0x00FFFFFF) { n = n + 8; x = x << 8; }
        if (x <= 0x0FFFFFFF) { n = n + 4; x = x << 4; }
        if (x <= 0x3FFFFFFF) { n = n + 2; x = x << 2; }
        if (x <= 0x7FFFFFFF) { n = n + 1; }
        if (x == 0) return 32;
        return n;
    #endif
    }

    /// <summary>
    /// Divides two FP values.
    /// </summary>
    static FP_INT DivPrecise(FP_INT a, FP_INT b)
    {
        if (b == MinValue || b == 0)
            return 0;

        FP_INT res = (FP_INT)(((FP_LONG)a << Shift) / (FP_LONG)b);
        return res;
    }

    /// <summary>
    /// Calculates division approximation.
    /// </summary>
    static FP_INT Div(FP_INT a, FP_INT b)
    {
        if (b == MinValue || b == 0)
        {
            FixedUtil::InvalidArgument("Fixed32.Div", "b", b);
            return 0;
        }

        return (FP_INT)(((FP_LONG)a << 16) / b);
    }

    /// <summary>
    /// Calculates division approximation.
    /// </summary>
    static FP_INT DivFast(FP_INT a, FP_INT b)
    {
        if (b == MinValue || b == 0)
        {
            FixedUtil::InvalidArgument("Fixed32.DivFast", "b", b);
            return 0;
        }

        // Handle negative values.
        FP_INT sign = (b < 0) ? -1 : 1;
        b *= sign;

        // Normalize input into [1.0, 2.0( range (convert to s2.30).
        FP_INT offset = 29 - Nlz((FP_UINT)b);
        FP_INT n = FixedUtil::ShiftRight(b, offset - 28);
        static const FP_INT ONE = (1 << 30);
        FP_ASSERT(n >= ONE);

        // Polynomial approximation.
        FP_INT res = FixedUtil::RcpPoly6(n - ONE);

        // Multiply by reciprocal, apply exponent, convert back to s16.16.
        FP_INT y = FixedUtil::Qmul30(res, a);
        return FixedUtil::ShiftRight(sign * y, offset - 14);
    }

    /// <summary>
    /// Calculates division approximation.
    /// </summary>
    static FP_INT DivFastest(FP_INT a, FP_INT b)
    {
        if (b == MinValue || b == 0)
        {
            FixedUtil::InvalidArgument("Fixed32.DivFastest", "b", b);
            return 0;
        }

        // Handle negative values.
        FP_INT sign = (b < 0) ? -1 : 1;
        b *= sign;

        // Normalize input into [1.0, 2.0( range (convert to s2.30).
        FP_INT offset = 29 - Nlz((FP_UINT)b);
        FP_INT n = FixedUtil::ShiftRight(b, offset - 28);
        static const FP_INT ONE = (1 << 30);
        FP_ASSERT(n >= ONE);

        // Polynomial approximation.
        FP_INT res = FixedUtil::RcpPoly4(n - ONE);

        // Multiply by reciprocal, apply exponent, convert back to s16.16.
        FP_INT y = FixedUtil::Qmul30(res, a);
        return FixedUtil::ShiftRight(sign * y, offset - 14);
    }

    /// <summary>
    /// Divides two FP values and returns the modulus.
    /// </summary>
    static FP_INT Mod(FP_INT a, FP_INT b)
    {
        FP_INT di = a / b;
        FP_INT ret = a - (di * b);
        return ret;
    }

    /// <summary>
    /// Calculates the square root of the given number.
    /// </summary>
    static FP_INT SqrtPrecise(FP_INT a)
    {
        // Adapted from https://github.com/chmike/fpsqrt
        if (a <= 0)
        {
            if (a < 0)
                FixedUtil::InvalidArgument("Fixed32.SqrtPrecise", "a", a);
            return 0;
        }

        FP_UINT r = (FP_UINT)a;
        FP_UINT b = 0x40000000;
        FP_UINT q = 0;
        while (b > 0x40)
        {
            FP_UINT t = q + b;
            if (r >= t)
            {
                r -= t;
                q = t + b;
            }
            r <<= 1;
            b >>= 1;
        }
        q >>= 8;
        return (FP_INT)q;
    }

    static FP_INT Sqrt(FP_INT x)
    {
        // Return 0 for all non-positive values.
        if (x <= 0)
        {
            if (x < 0)
                FixedUtil::InvalidArgument("Fixed32.Sqrt", "x", x);
            return 0;
        }

        // Constants (s2.30).
        static const FP_INT ONE = (1 << 30);
        static const FP_INT SQRT2 = 1518500249; // sqrt(2.0)

        // Normalize input into [1.0, 2.0( range (as s2.30).
        FP_INT offset = 15 - Nlz((FP_UINT)x);
        FP_INT n = FixedUtil::ShiftRight(x, offset - 14);
        FP_ASSERT(n >= ONE);
        FP_INT y = FixedUtil::SqrtPoly3Lut8(n - ONE);

        // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
        FP_INT adjust = ((offset & 1) != 0) ? SQRT2 : ONE;
        offset = offset >> 1;

        // Apply exponent, convert back to s16.16.
        FP_INT yr = FixedUtil::Qmul30(adjust, y);
        return FixedUtil::ShiftRight(yr, 14 - offset);
    }

    static FP_INT SqrtFast(FP_INT x)
    {
        // Return 0 for all non-positive values.
        if (x <= 0)
        {
            if (x < 0)
                FixedUtil::InvalidArgument("Fixed32.SqrtFast", "x", x);
            return 0;
        }

        // Constants (s2.30).
        static const FP_INT ONE = (1 << 30);
        static const FP_INT SQRT2 = 1518500249; // sqrt(2.0)

        // Normalize input into [1.0, 2.0( range (as s2.30).
        FP_INT offset = 15 - Nlz((FP_UINT)x);
        FP_INT n = FixedUtil::ShiftRight(x, offset - 14);
        FP_ASSERT(n >= ONE);
        FP_INT y = FixedUtil::SqrtPoly4(n - ONE);

        // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
        FP_INT adjust = ((offset & 1) != 0) ? SQRT2 : ONE;
        offset = offset >> 1;

        // Apply exponent, convert back to s16.16.
        FP_INT yr = FixedUtil::Qmul30(adjust, y);
        return FixedUtil::ShiftRight(yr, 14 - offset);
    }

    static FP_INT SqrtFastest(FP_INT x)
    {
        // Return 0 for all non-positive values.
        if (x <= 0)
        {
            if (x < 0)
                FixedUtil::InvalidArgument("Fixed32.SqrtFastest", "x", x);
            return 0;
        }

        // Constants (s2.30).
        static const FP_INT ONE = (1 << 30);
        static const FP_INT SQRT2 = 1518500249; // sqrt(2.0)

        // Normalize input into [1.0, 2.0( range (as s2.30).
        FP_INT offset = 15 - Nlz((FP_UINT)x);
        FP_INT n = FixedUtil::ShiftRight(x, offset - 14);
        FP_ASSERT(n >= ONE);
        FP_INT y = FixedUtil::SqrtPoly3(n - ONE);

        // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
        FP_INT adjust = ((offset & 1) != 0) ? SQRT2 : ONE;
        offset = offset >> 1;

        // Apply exponent, convert back to s16.16.
        FP_INT yr = FixedUtil::Qmul30(adjust, y);
        return FixedUtil::ShiftRight(yr, 14 - offset);
    }

    /// <summary>
    /// Calculates the reciprocal square root.
    /// </summary>
    static FP_INT RSqrt(FP_INT x)
    {
        // Return 0 for invalid values
        if (x <= 0)
        {
            FixedUtil::InvalidArgument("Fixed32.RSqrt", "x", x);
            return 0;
        }

        // Constants (s2.30).
        static const FP_INT ONE = (1 << 30);
        static const FP_INT HALF_SQRT2 = 759250125; // 0.5 * sqrt(2.0)

        // Normalize input into [1.0, 2.0( range (as s2.30).
        FP_INT offset = 1 - Nlz((FP_UINT)x);
        FP_INT n = FixedUtil::ShiftRight(x, offset);
        FP_ASSERT(n >= ONE);
        FP_INT y = FixedUtil::RSqrtPoly3Lut16(n - ONE);

        // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
        FP_INT adjust = ((offset & 1) != 0) ? HALF_SQRT2 : ONE;
        offset = offset >> 1;

        // Apply exponent, convert back to s16.16.
        FP_INT yr = FixedUtil::Qmul30(adjust, y);
        return FixedUtil::ShiftRight(yr, offset + 21);
    }

    /// <summary>
    /// Calculates the reciprocal square root.
    /// </summary>
    static FP_INT RSqrtFast(FP_INT x)
    {
        // Return 0 for invalid values
        if (x <= 0)
        {
            FixedUtil::InvalidArgument("Fixed32.RSqrtFast", "x", x);
            return 0;
        }

        // Constants (s2.30).
        static const FP_INT ONE = (1 << 30);
        static const FP_INT HALF_SQRT2 = 759250125; // 0.5 * sqrt(2.0)

        // Normalize input into [1.0, 2.0( range (as s2.30).
        FP_INT offset = 1 - Nlz((FP_UINT)x);
        FP_INT n = FixedUtil::ShiftRight(x, offset);
        FP_ASSERT(n >= ONE);
        FP_INT y = FixedUtil::RSqrtPoly5(n - ONE);

        // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
        FP_INT adjust = ((offset & 1) != 0) ? HALF_SQRT2 : ONE;
        offset = offset >> 1;

        // Apply exponent, convert back to s16.16.
        FP_INT yr = FixedUtil::Qmul30(adjust, y);
        return FixedUtil::ShiftRight(yr, offset + 21);
    }

    /// <summary>
    /// Calculates the reciprocal square root.
    /// </summary>
    static FP_INT RSqrtFastest(FP_INT x)
    {
        // Return 0 for invalid values
        if (x <= 0)
        {
            FixedUtil::InvalidArgument("Fixed32.RSqrtFastest", "x", x);
            return 0;
        }

        // Constants (s2.30).
        static const FP_INT ONE = (1 << 30);
        static const FP_INT HALF_SQRT2 = 759250125; // 0.5 * sqrt(2.0)

        // Normalize input into [1.0, 2.0( range (as s2.30).
        FP_INT offset = 1 - Nlz((FP_UINT)x);
        FP_INT n = FixedUtil::ShiftRight(x, offset);
        FP_ASSERT(n >= ONE);
        FP_INT y = FixedUtil::RSqrtPoly3(n - ONE);

        // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
        FP_INT adjust = ((offset & 1) != 0) ? HALF_SQRT2 : ONE;
        offset = offset >> 1;

        // Apply exponent, convert back to s16.16.
        FP_INT yr = FixedUtil::Qmul30(adjust, y);
        return FixedUtil::ShiftRight(yr, offset + 21);
    }

    /// <summary>
    /// Calculates reciprocal approximation.
    /// </summary>
    static FP_INT Rcp(FP_INT x)
    {
        if (x == MinValue || x == 0)
        {
            FixedUtil::InvalidArgument("Fixed32.Rcp", "x", x);
            return 0;
        }

        // Handle negative values.
        FP_INT sign = (x < 0) ? -1 : 1;
        x *= sign;

        // Normalize input into [1.0, 2.0( range (convert to s2.30).
        FP_INT offset = 29 - Nlz((FP_UINT)x);
        FP_INT n = FixedUtil::ShiftRight(x, offset - 28);
        static const FP_INT ONE = (1 << 30);
        FP_ASSERT(n >= ONE);

        // Polynomial approximation.
        FP_INT res = FixedUtil::RcpPoly4Lut8(n - ONE);

        // Apply exponent, convert back to s16.16.
        return FixedUtil::ShiftRight(sign * res, offset);
    }

    /// <summary>
    /// Calculates reciprocal approximation.
    /// </summary>
    static FP_INT RcpFast(FP_INT x)
    {
        if (x == MinValue || x == 0)
        {
            FixedUtil::InvalidArgument("Fixed32.RcpFast", "x", x);
            return 0;
        }

        // Handle negative values.
        FP_INT sign = (x < 0) ? -1 : 1;
        x *= sign;

        // Normalize input into [1.0, 2.0( range (convert to s2.30).
        FP_INT offset = 29 - Nlz((FP_UINT)x);
        FP_INT n = FixedUtil::ShiftRight(x, offset - 28);
        static const FP_INT ONE = (1 << 30);
        FP_ASSERT(n >= ONE);

        // Polynomial approximation.
        FP_INT res = FixedUtil::RcpPoly6(n - ONE);
        //FP_INT res = Util.RcpPoly3Lut8(n - ONE);

        // Apply exponent, convert back to s16.16.
        return FixedUtil::ShiftRight(sign * res, offset);
    }

    /// <summary>
    /// Calculates reciprocal approximation.
    /// </summary>
    static FP_INT RcpFastest(FP_INT x)
    {
        if (x == MinValue || x == 0)
        {
            FixedUtil::InvalidArgument("Fixed32.RcpFastest", "x", x);
            return 0;
        }

        // Handle negative values.
        FP_INT sign = (x < 0) ? -1 : 1;
        x *= sign;

        // Normalize input into [1.0, 2.0( range (convert to s2.30).
        FP_INT offset = 29 - Nlz((FP_UINT)x);
        FP_INT n = FixedUtil::ShiftRight(x, offset - 28);
        static const FP_INT ONE = (1 << 30);
        FP_ASSERT(n >= ONE);

        // Polynomial approximation.
        FP_INT res = FixedUtil::RcpPoly4(n - ONE);
        //FP_INT res = Util.RcpPoly3Lut4(n - ONE);

        // Apply exponent, convert back to s16.16.
        return FixedUtil::ShiftRight(sign * res, offset);
    }

    /// <summary>
    /// Calculates the base 2 exponent.
    /// </summary>
    static FP_INT Exp2(FP_INT x)
    {
        // Handle values that would under or overflow.
        if (x >= 15 * One) return MaxValue;
        if (x <= -16 * One) return 0;

        // Compute exp2 for fractional part.
        FP_INT k = (x & FractionMask) << 14;
        FP_INT y = FixedUtil::Exp2Poly5(k);

        // Combine integer and fractional result, and convert back to s16.16.
        FP_INT intPart = x >> Shift;
        return FixedUtil::ShiftRight(y, 14 - intPart);
    }

    /// <summary>
    /// Calculates the base 2 exponent.
    /// </summary>
    static FP_INT Exp2Fast(FP_INT x)
    {
        // Handle values that would under or overflow.
        if (x >= 15 * One) return MaxValue;
        if (x <= -16 * One) return 0;

        // Compute exp2 for fractional part.
        FP_INT k = (x & FractionMask) << 14;
        FP_INT y = FixedUtil::Exp2Poly4(k);

        // Combine integer and fractional result, and convert back to s16.16.
        FP_INT intPart = x >> Shift;
        return FixedUtil::ShiftRight(y, 14 - intPart);
    }

    /// <summary>
    /// Calculates the base 2 exponent.
    /// </summary>
    static FP_INT Exp2Fastest(FP_INT x)
    {
        // Handle values that would under or overflow.
        if (x >= 15 * One) return MaxValue;
        if (x <= -16 * One) return 0;

        // Compute exp2 for fractional part.
        FP_INT k = (x & FractionMask) << 14;
        FP_INT y = FixedUtil::Exp2Poly3(k);

        // Combine integer and fractional result, and convert back to s16.16.
        FP_INT intPart = x >> Shift;
        return FixedUtil::ShiftRight(y, 14 - intPart);
    }

    static FP_INT Exp(FP_INT x)
    {
        // e^x == 2^(x / ln(2))
        return Exp2(Mul(x, RCP_LN2));
    }

    static FP_INT ExpFast(FP_INT x)
    {
        // e^x == 2^(x / ln(2))
        return Exp2Fast(Mul(x, RCP_LN2));
    }

    static FP_INT ExpFastest(FP_INT x)
    {
        // e^x == 2^(x / ln(2))
        return Exp2Fastest(Mul(x, RCP_LN2));
    }

    static FP_INT Log(FP_INT x)
    {
        // Return 0 for invalid values
        if (x <= 0)
        {
            FixedUtil::InvalidArgument("Fixed32.Log", "x", x);
            return 0;
        }

        // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
        FP_INT offset = 15 - Nlz((FP_UINT)x);
        FP_INT n = FixedUtil::ShiftRight(x, offset - 14);

        // Polynomial approximation.
        static const FP_INT ONE = (1 << 30);
        FP_ASSERT(n >= ONE);
        FP_INT y = FixedUtil::LogPoly5Lut8(n - ONE);

        // Combine integer and fractional parts (into s16.16).
        return offset * RCP_LOG2_E + (y >> 14);
    }

    static FP_INT LogFast(FP_INT x)
    {
        // Return 0 for invalid values
        if (x <= 0)
        {
            FixedUtil::InvalidArgument("Fixed32.LogFast", "x", x);
            return 0;
        }

        // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
        FP_INT offset = 15 - Nlz((FP_UINT)x);
        FP_INT n = FixedUtil::ShiftRight(x, offset - 14);

        // Polynomial approximation.
        static const FP_INT ONE = (1 << 30);
        FP_ASSERT(n >= ONE);
        FP_INT y = FixedUtil::LogPoly3Lut8(n - ONE);

        // Combine integer and fractional parts (into s16.16).
        return offset * RCP_LOG2_E + (y >> 14);
    }

    static FP_INT LogFastest(FP_INT x)
    {
        // Return 0 for invalid values
        if (x <= 0)
        {
            FixedUtil::InvalidArgument("Fixed32.LogFastest", "x", x);
            return 0;
        }

        // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
        FP_INT offset = 15 - Nlz((FP_UINT)x);
        FP_INT n = FixedUtil::ShiftRight(x, offset - 14);

        // Polynomial approximation.
        static const FP_INT ONE = (1 << 30);
        FP_ASSERT(n >= ONE);
        FP_INT y = FixedUtil::LogPoly5(n - ONE);

        // Combine integer and fractional parts (into s16.16).
        return offset * RCP_LOG2_E + (y >> 14);
    }

    static FP_INT Log2(FP_INT x)
    {
        // Return 0 for invalid values
        if (x <= 0)
        {
            FixedUtil::InvalidArgument("Fixed32.Log2", "x", x);
            return 0;
        }

        // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
        FP_INT offset = 15 - Nlz((FP_UINT)x);
        FP_INT n = FixedUtil::ShiftRight(x, offset - 14);

        // Polynomial approximation of mantissa.
        static const FP_INT ONE = (1 << 30);
        FP_ASSERT(n >= ONE);
        FP_INT y = FixedUtil::Log2Poly4Lut16(n - ONE);

        // Combine integer and fractional parts (into s16.16).
        return (offset << Shift) + (y >> 14);
    }

    static FP_INT Log2Fast(FP_INT x)
    {
        // Return 0 for invalid values
        if (x <= 0)
        {
            FixedUtil::InvalidArgument("Fixed32.Log2Fast", "x", x);
            return 0;
        }

        // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
        FP_INT offset = 15 - Nlz((FP_UINT)x);
        FP_INT n = FixedUtil::ShiftRight(x, offset - 14);

        // Polynomial approximation of mantissa.
        static const FP_INT ONE = (1 << 30);
        FP_ASSERT(n >= ONE);
        FP_INT y = FixedUtil::Log2Poly3Lut16(n - ONE);

        // Combine integer and fractional parts (into s16.16).
        return (offset << Shift) + (y >> 14);
    }

    static FP_INT Log2Fastest(FP_INT x)
    {
        // Return 0 for invalid values
        if (x <= 0)
        {
            FixedUtil::InvalidArgument("Fixed32.Log2Fastest", "x", x);
            return 0;
        }

        // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
        FP_INT offset = 15 - Nlz((FP_UINT)x);
        FP_INT n = FixedUtil::ShiftRight(x, offset - 14);

        // Polynomial approximation of mantissa.
        static const FP_INT ONE = (1 << 30);
        FP_ASSERT(n >= ONE);
        FP_INT y = FixedUtil::Log2Poly5(n - ONE);

        // Combine integer and fractional parts (into s16.16).
        return (offset << Shift) + (y >> 14);
    }

    /// <summary>
    /// Calculates x to the power of the exponent.
    /// </summary>
    static FP_INT Pow(FP_INT x, FP_INT exponent)
    {
        // Return 0 for invalid values
        if (x <= 0)
        {
            if (x < 0)
                FixedUtil::InvalidArgument("Fixed32.Pow", "x", x);
            return 0;
        }

        return Exp(Mul(exponent, Log(x)));
    }

    /// <summary>
    /// Calculates x to the power of the exponent.
    /// </summary>
    static FP_INT PowFast(FP_INT x, FP_INT exponent)
    {
        // Return 0 for invalid values
        if (x <= 0)
        {
            if (x < 0)
                FixedUtil::InvalidArgument("Fixed32.PowFast", "x", x);
            return 0;
        }

        return ExpFast(Mul(exponent, LogFast(x)));
    }

    /// <summary>
    /// Calculates x to the power of the exponent.
    /// </summary>
    static FP_INT PowFastest(FP_INT x, FP_INT exponent)
    {
        // Return 0 for invalid values
        if (x <= 0)
        {
            if (x < 0)
                FixedUtil::InvalidArgument("Fixed32.PowFastest", "x", x);
            return 0;
        }

        return ExpFastest(Mul(exponent, LogFastest(x)));
    }

    static FP_INT UnitSin(FP_INT z)
    {
        // See: http://www.coranac.com/2009/07/sines/

        // Handle quadrants 1 and 2 by mirroring the [1, 3] range to [-1, 1] (by calculating 2 - z).
        // The if condition uses the fact that for the quadrants of interest are 0b01 and 0b10 (top two bits are different).
        if ((z ^ (z << 1)) < 0)
            z = (1 << 31) - z;

        // Now z is in range [-1, 1].
        static const FP_INT ONE = (1 << 30);
        FP_ASSERT((z >= -ONE) && (z <= ONE));

        // Polynomial approximation.
        FP_INT zz = FixedUtil::Qmul30(z, z);
        FP_INT res = FixedUtil::Qmul30(FixedUtil::SinPoly4(zz), z);

        // Return as s2.30.
        return res;
    }

    static FP_INT UnitSinFast(FP_INT z)
    {
        // See: http://www.coranac.com/2009/07/sines/

        // Handle quadrants 1 and 2 by mirroring the [1, 3] range to [-1, 1] (by calculating 2 - z).
        // The if condition uses the fact that for the quadrants of interest are 0b01 and 0b10 (top two bits are different).
        if ((z ^ (z << 1)) < 0)
            z = (1 << 31) - z;

        // Now z is in range [-1, 1].
        static const FP_INT ONE = (1 << 30);
        FP_ASSERT((z >= -ONE) && (z <= ONE));

        // Polynomial approximation.
        FP_INT zz = FixedUtil::Qmul30(z, z);
        FP_INT res = FixedUtil::Qmul30(FixedUtil::SinPoly3(zz), z);

        // Return as s2.30.
        return res;
    }

    static FP_INT UnitSinFastest(FP_INT z)
    {
        // See: http://www.coranac.com/2009/07/sines/

        // Handle quadrants 1 and 2 by mirroring the [1, 3] range to [-1, 1] (by calculating 2 - z).
        // The if condition uses the fact that for the quadrants of interest are 0b01 and 0b10 (top two bits are different).
        if ((z ^ (z << 1)) < 0)
            z = (1 << 31) - z;

        // Now z is in range [-1, 1].
        static const FP_INT ONE = (1 << 30);
        FP_ASSERT((z >= -ONE) && (z <= ONE));

        // Polynomial approximation.
        FP_INT zz = FixedUtil::Qmul30(z, z);
        FP_INT res = FixedUtil::Qmul30(FixedUtil::SinPoly2(zz), z);

        // Return as s2.30.
        return res;
    }

    static FP_INT Sin(FP_INT x)
    {
        // Map [0, 2pi] to [0, 4] (as s2.30).
        // This also wraps the values into one period.
        FP_INT z = Mul(RCP_TWO_PI, x);

        // Compute sin from s2.30 and convert back to s16.16.
        return UnitSin(z) >> 14;
    }

    static FP_INT SinFast(FP_INT x)
    {
        // Map [0, 2pi] to [0, 4] (as s2.30).
        // This also wraps the values into one period.
        FP_INT z = Mul(RCP_TWO_PI, x);

        // Compute sin from s2.30 and convert back to s16.16.
        return UnitSinFast(z) >> 14;
    }

    static FP_INT SinFastest(FP_INT x)
    {
        // Map [0, 2pi] to [0, 4] (as s2.30).
        // This also wraps the values into one period.
        FP_INT z = Mul(RCP_TWO_PI, x);

        // Compute sin from s2.30 and convert back to s16.16.
        return UnitSinFastest(z) >> 14;
    }

    static FP_INT Cos(FP_INT x)
    {
        return Sin(x + PiHalf);
    }

    static FP_INT CosFast(FP_INT x)
    {
        return SinFast(x + PiHalf);
    }

    static FP_INT CosFastest(FP_INT x)
    {
        return SinFastest(x + PiHalf);
    }

    static FP_INT Tan(FP_INT x)
    {
        FP_INT z = Mul(RCP_TWO_PI, x);
        FP_INT sinX = UnitSin(z);
        FP_INT cosX = UnitSin(z + (1 << 30));
        return Div(sinX, cosX);
    }

    static FP_INT TanFast(FP_INT x)
    {
        FP_INT z = Mul(RCP_TWO_PI, x);
        FP_INT sinX = UnitSinFast(z);
        FP_INT cosX = UnitSinFast(z + (1 << 30));
        return DivFast(sinX, cosX);
    }

    static FP_INT TanFastest(FP_INT x)
    {
        FP_INT z = Mul(RCP_TWO_PI, x);
        FP_INT sinX = UnitSinFastest(z);
        FP_INT cosX = UnitSinFastest(z + (1 << 30));
        return DivFastest(sinX, cosX);
    }

    static FP_INT Atan2Div(FP_INT y, FP_INT x)
    {
        FP_ASSERT(y >= 0 && x > 0 && x >= y);

        // Normalize input into [1.0, 2.0( range (convert to s2.30).
        static const FP_INT ONE = (1 << 30);
        static const FP_INT HALF = (1 << 29);
        FP_INT offset = 1 - Nlz((FP_UINT)x);
        FP_INT n = FixedUtil::ShiftRight(x, offset);
        FP_ASSERT(n >= ONE);

        // Polynomial approximation of reciprocal.
        FP_INT oox = FixedUtil::RcpPoly4Lut8(n - ONE);
        FP_ASSERT(oox >= HALF && oox <= ONE);

        // Apply exponent and multiply.
        FP_INT yr = FixedUtil::ShiftRight(y, offset);
        return FixedUtil::Qmul30(yr, oox);
    }

    static FP_INT Atan2(FP_INT y, FP_INT x)
    {
        // See: https://www.dsprelated.com/showarticle/1052.php

        if (x == 0)
        {
            if (y > 0) return PiHalf;
            if (y < 0) return -PiHalf;

            FixedUtil::InvalidArgument("Fixed32.Atan2", "y, x", y, x);
            return 0;
        }

        FP_INT nx = Abs(x);
        FP_INT ny = Abs(y);
        FP_INT negMask = ((x ^ y) >> 31);

        if (nx >= ny)
        {
            FP_INT k = Atan2Div(ny, nx);
            FP_INT z = FixedUtil::AtanPoly5Lut8(k);
            FP_INT angle = (negMask ^ (z >> 14)) - negMask;
            if (x > 0) return angle;
            if (y >= 0) return angle + Pi;
            return angle - Pi;
        }
        else
        {
            FP_INT k = Atan2Div(nx, ny);
            FP_INT z = FixedUtil::AtanPoly5Lut8(k);
            FP_INT angle = negMask ^  (z >> 14);
            return ((y > 0) ? PiHalf : -PiHalf) - angle;
        }
    }

    static FP_INT Atan2DivFast(FP_INT y, FP_INT x)
    {
        FP_ASSERT(y >= 0 && x > 0 && x >= y);

        // Normalize input into [1.0, 2.0( range (convert to s2.30).
        static const FP_INT ONE = (1 << 30);
        static const FP_INT HALF = (1 << 29);
        FP_INT offset = 1 - Nlz((FP_UINT)x);
        FP_INT n = FixedUtil::ShiftRight(x, offset);

        // Polynomial approximation.
        FP_INT oox = FixedUtil::RcpPoly6(n - ONE);
        FP_ASSERT(oox >= HALF && oox <= ONE);

        // Apply exponent and multiply.
        FP_INT yr = FixedUtil::ShiftRight(y, offset);
        return FixedUtil::Qmul30(yr, oox);
    }

    static FP_INT Atan2Fast(FP_INT y, FP_INT x)
    {
        // See: https://www.dsprelated.com/showarticle/1052.php

        if (x == 0)
        {
            if (y > 0) return PiHalf;
            if (y < 0) return -PiHalf;

            FixedUtil::InvalidArgument("Fixed32.Atan2Fast", "y, x", y, x);
            return 0;
        }

        FP_INT nx = Abs(x);
        FP_INT ny = Abs(y);
        FP_INT negMask = ((x ^ y) >> 31);

        if (nx >= ny)
        {
            FP_INT k = Atan2DivFast(ny, nx);
            FP_INT z = FixedUtil::AtanPoly3Lut8(k);
            FP_INT angle = negMask ^ (z >> 14);
            if (x > 0) return angle;
            if (y >= 0) return angle + Pi;
            return angle - Pi;
        }
        else
        {
            FP_INT k = Atan2DivFast(nx, ny);
            FP_INT z = FixedUtil::AtanPoly3Lut8(k);
            FP_INT angle = negMask ^ (z >> 14);
            return ((y > 0) ? PiHalf : -PiHalf) - angle;
        }
    }

    static FP_INT Atan2DivFastest(FP_INT y, FP_INT x)
    {
        FP_ASSERT(y >= 0 && x > 0 && x >= y);

        // Normalize input into [1.0, 2.0( range (convert to s2.30).
        static const FP_INT ONE = (1 << 30);
        static const FP_INT HALF = (1 << 29);
        FP_INT offset = 1 - Nlz((FP_UINT)x);
        FP_INT n = FixedUtil::ShiftRight(x, offset);

        // Polynomial approximation.
        FP_INT oox = FixedUtil::RcpPoly4(n - ONE);
        FP_ASSERT(oox >= HALF && oox <= ONE);

        // Apply exponent and multiply.
        FP_INT yr = FixedUtil::ShiftRight(y, offset);
        return FixedUtil::Qmul30(yr, oox);
    }

    static FP_INT Atan2Fastest(FP_INT y, FP_INT x)
    {
        // See: https://www.dsprelated.com/showarticle/1052.php

        if (x == 0)
        {
            if (y > 0) return PiHalf;
            if (y < 0) return -PiHalf;

            FixedUtil::InvalidArgument("Fixed32.Atan2Fastest", "y, x", y, x);
            return 0;
        }

        FP_INT nx = Abs(x);
        FP_INT ny = Abs(y);
        FP_INT negMask = ((x ^ y) >> 31);

        if (nx >= ny)
        {
            FP_INT k = Atan2DivFastest(ny, nx);
            FP_INT z = FixedUtil::AtanPoly4(k);
            FP_INT angle = negMask ^ (z >> 14);
            if (x > 0) return angle;
            if (y >= 0) return angle + Pi;
            return angle - Pi;
        }
        else
        {
            FP_INT k = Atan2DivFastest(nx, ny);
            FP_INT z = FixedUtil::AtanPoly4(k);
            FP_INT angle = negMask ^ (z >> 14);
            return ((y > 0) ? PiHalf : -PiHalf) - angle;
        }
    }

    static FP_INT Asin(FP_INT x)
    {
        // Return 0 for invalid values
        if (x < -One || x > One)
        {
            FixedUtil::InvalidArgument("Fixed32.Asin", "x", x);
            return 0;
        }

        // Compute Atan2(x, Sqrt((1+x) * (1-x))), using s32.32.
        FP_LONG xx = (FP_LONG)(One + x) * (FP_LONG)(One - x);
        FP_LONG y = Fixed64::Sqrt(xx);
        return (FP_INT)(Fixed64::Atan2((FP_LONG)x << 16, y) >> 16);
    }

    static FP_INT AsinFast(FP_INT x)
    {
        // Return 0 for invalid values
        if (x < -One || x > One)
        {
            FixedUtil::InvalidArgument("Fixed32.AsinFast", "x", x);
            return 0;
        }

        // Compute Atan2(x, Sqrt((1+x) * (1-x))), using s32.32.
        FP_LONG xx = (FP_LONG)(One + x) * (FP_LONG)(One - x);
        FP_LONG y = Fixed64::SqrtFast(xx);
        return (FP_INT)(Fixed64::Atan2Fast((FP_LONG)x << 16, y) >> 16);
    }

    static FP_INT AsinFastest(FP_INT x)
    {
        // Return 0 for invalid values
        if (x < -One || x > One)
        {
            FixedUtil::InvalidArgument("Fixed32.AsinFastest", "x", x);
            return 0;
        }

        // Compute Atan2(x, Sqrt((1+x) * (1-x))), using s32.32.
        FP_LONG xx = (FP_LONG)(One + x) * (FP_LONG)(One - x);
        FP_LONG y = Fixed64::SqrtFastest(xx);
        return (FP_INT)(Fixed64::Atan2Fastest((FP_LONG)x << 16, y) >> 16);
    }

    static FP_INT Acos(FP_INT x)
    {
        // Return 0 for invalid values
        if (x < -One || x > One)
        {
            FixedUtil::InvalidArgument("Fixed32.Acos", "x", x);
            return 0;
        }

        // Compute Atan2(Sqrt((1+x) * (1-x)), x), using s32.32.
        FP_LONG xx = (FP_LONG)(One + x) * (FP_LONG)(One - x);
        FP_LONG y = Fixed64::Sqrt(xx);
        return (FP_INT)(Fixed64::Atan2(y, (FP_LONG)x << 16) >> 16);
    }

    static FP_INT AcosFast(FP_INT x)
    {
        // Return 0 for invalid values
        if (x < -One || x > One)
        {
            FixedUtil::InvalidArgument("Fixed32.AcosFast", "x", x);
            return 0;
        }

        // Compute Atan2(Sqrt((1+x) * (1-x)), x), using s32.32.
        FP_LONG xx = (FP_LONG)(One + x) * (FP_LONG)(One - x);
        FP_LONG y = Fixed64::SqrtFast(xx);
        return (FP_INT)(Fixed64::Atan2Fast(y, (FP_LONG)x << 16) >> 16);
    }

    static FP_INT AcosFastest(FP_INT x)
    {
        // Return 0 for invalid values
        if (x < -One || x > One)
        {
            FixedUtil::InvalidArgument("Fixed32.AcosFastest", "x", x);
            return 0;
        }

        // Compute Atan2(Sqrt((1+x) * (1-x)), x), using s32.32.
        FP_LONG xx = (FP_LONG)(One + x) * (FP_LONG)(One - x);
        FP_LONG y = Fixed64::SqrtFastest(xx);
        return (FP_INT)(Fixed64::Atan2Fastest(y, (FP_LONG)x << 16) >> 16);
    }

    static FP_INT Atan(FP_INT x)
    {
        return Atan2(x, One);
    }

    static FP_INT AtanFast(FP_INT x)
    {
        return Atan2Fast(x, One);
    }

    static FP_INT AtanFastest(FP_INT x)
    {
        return Atan2Fastest(x, One);
    }




    #undef FP_ASSERT
};
#endif // __FIXED32_H

