//
// GENERATED FILE!!!
//
// Generated from Fixed64.cs, part of the FixPointCS project (MIT license).
//
#pragma once
#ifndef __FIXED64_H
#define __FIXED64_H

// Include 64bit numeric types
#include <stdint.h>

// If FP_ASSERT is not custom-defined, then use the standard one
#ifndef FP_ASSERT
#include <assert.h>
#endif

namespace Fixed64
{
    typedef int FP_INT;
    typedef unsigned int FP_UINT;
    typedef int64_t FP_LONG;
    typedef uint64_t FP_ULONG;

    static_assert(sizeof(FP_INT) == 4, "Wrong bytesize for FP_INT");
    static_assert(sizeof(FP_UINT) == 4, "Wrong bytesize for FP_UINT");
    static_assert(sizeof(FP_LONG) == 8, "Wrong bytesize for FP_LONG");
    static_assert(sizeof(FP_ULONG) == 8, "Wrong bytesize for FP_ULONG");

    #ifndef FP_ASSERT
    #define FP_ASSERT(x) assert(x)
    #endif

    static const double FP_PI = 3.14159265359;

//
// FixPointCS
//
// Copyright(c) 2018 Jere Sanisalo, Petri Kero
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
    static const FP_INT Shift = 32;
    static const FP_LONG FractionMask = ( INT64_C(1) << Shift ) - 1; // Space before INT64_C(1) needed because of hacky C++ code generator
    static const FP_LONG IntegerMask = ~FractionMask;

    // Constants
    static const FP_LONG Zero = INT64_C(0);
    static const FP_LONG Neg1 = INT64_C(-1) << Shift;
    static const FP_LONG One = INT64_C(1) << Shift;
    static const FP_LONG Two = INT64_C(2) << Shift;
    static const FP_LONG Three = INT64_C(3) << Shift;
    static const FP_LONG Four = INT64_C(4) << Shift;
    static const FP_LONG Half = One >> 1;
    static const FP_LONG Pi = INT64_C(13493037705); //(FP_LONG)(FP_PI * 65536.0) << 16;
    static const FP_LONG Pi2 = INT64_C(26986075409);
    static const FP_LONG PiHalf = INT64_C(6746518852);
    static const FP_LONG E = INT64_C(11674931555);

    static const FP_LONG MinValue = INT64_C(-9223372036854775808); // INT64_C(0x8000000000000000)
    static const FP_LONG MaxValue = INT64_C(0x7FFFFFFFFFFFFFFF);

    // Private constants
    static const FP_LONG RCP_LN2 = INT64_C(0x171547652); // 1.0 / Math.Log(2.0) ~= 1.4426950408889634

    /// <summary>
    /// Converts an integer into a FP value.
    /// </summary>
    static FP_LONG FromInt(FP_INT v)
    {
        return (FP_LONG)v << Shift;
    }

    /// <summary>
    /// Converts a double into a FP value.
    /// </summary>
    static FP_LONG FromDouble(double v)
    {
        return (FP_LONG)(v * 4294967296.0);
    }

    /// <summary>
    /// Converts a float into a FP value.
    /// </summary>
    static FP_LONG FromFloat(float v)
    {
        return FromDouble((double)v);
    }

    /// <summary>
    /// Converts a FP value into an integer by rounding it up to nearest integer.
    /// </summary>
    static FP_INT CeilToInt(FP_LONG v)
    {
        return (FP_INT)((v + (One - 1)) >> Shift);
    }

    /// <summary>
    /// Converts a FP value into an integer by rounding it down to nearest integer.
    /// </summary>
    static FP_INT FloorToInt(FP_LONG v)
    {
        return (FP_INT)(v >> Shift);
    }

    /// <summary>
    /// Converts a FP value into an integer by rounding it to nearest integer.
    /// </summary>
    static FP_INT RoundToInt(FP_LONG v)
    {
        return (FP_INT)((v + Half) >> Shift);
    }

    /// <summary>
    /// Converts a FP value into a double.
    /// </summary>
    static double ToDouble(FP_LONG v)
    {
        return (double)v * (1.0 / (4294967296.0));
    }

    /// <summary>
    /// Converts a FP value into a float.
    /// </summary>
    static float ToFloat(FP_LONG v)
    {
        return (float)ToDouble(v);
    }

    /// <summary>
    /// Converts the value to a human readable string.
    /// </summary>

    /// <summary>
    /// Returns the absolute (positive) value of x.
    /// </summary>
    static FP_LONG Abs(FP_LONG x)
    {
        // \note fails with LONG_MIN
        // \note for some reason this is twice as fast as (x > 0) ? x : -x
        return (x < 0) ? -x : x;
    }

    /// <summary>
    /// Negative absolute value (returns -abs(x)).
    /// </summary>
    static FP_LONG Nabs(FP_LONG x)
    {
        return (x > 0) ? -x : x;
    }

    /// <summary>
    /// Round up to nearest integer.
    /// </summary>
    static FP_LONG Ceil(FP_LONG x)
    {
        return (x + FractionMask) & IntegerMask;
    }

    /// <summary>
    /// Round down to nearest integer.
    /// </summary>
    static FP_LONG Floor(FP_LONG x)
    {
        return x & IntegerMask;
    }

    /// <summary>
    /// Round to nearest integer.
    /// </summary>
    static FP_LONG Round(FP_LONG x)
    {
        // \todo [petri] Math.Round() round to nearest even number
        return (x + Half) & IntegerMask;
    }

    /// <summary>
    /// Returns the minimum of the two values.
    /// </summary>
    static FP_LONG Min(FP_LONG a, FP_LONG b)
    {
        return (a < b) ? a : b;
    }

    /// <summary>
    /// Returns the maximum of the two values.
    /// </summary>
    static FP_LONG Max(FP_LONG a, FP_LONG b)
    {
        return (a > b) ? a : b;
    }

    /// <summary>
    /// Returns the sign of the value (-1 if negative, 0 if zero, 1 if positive).
    /// </summary>
    static FP_INT Sign(FP_LONG x)
    {
        if (x == 0) return 0;
        return (x < 0) ? -1 : 1;
    }

    /// <summary>
    /// Adds the two FP numbers together.
    /// </summary>
    static FP_LONG Add(FP_LONG a, FP_LONG b)
    {
        return a + b;
    }

    /// <summary>
    /// Subtracts the two FP numbers from each other.
    /// </summary>
    static FP_LONG Sub(FP_LONG a, FP_LONG b)
    {
        return a - b;
    }

    /// <summary>
    /// Multiplies two FP values together.
    /// </summary>
    static FP_LONG Mul(FP_LONG a, FP_LONG b)
    {
        /*FP_ULONG alow = (FP_ULONG)(a & FP_Mask_Fract);
        FP_LONG ahigh = a >> FP_Shift;
        FP_ULONG blow = (FP_ULONG)(b & FP_Mask_Fract);
        FP_LONG bhigh = b >> FP_Shift;

        FP_ULONG lowlow = alow * blow;
        FP_LONG lowhigh = (FP_LONG)(uint)alow * bhigh;
        FP_LONG highlow = ahigh * (FP_LONG)(uint)blow;
        FP_LONG highhigh = ahigh * bhigh;

        FP_LONG lo_res = (FP_LONG)(lowlow >> 32);
        FP_LONG mid_res1 = lowhigh;
        FP_LONG mid_res2 = highlow;
        FP_LONG hi_res = highhigh << 32;

        return lo_res + mid_res1 + mid_res2 + hi_res;*/

        FP_LONG ai = a >> Shift;
        FP_ULONG af = (FP_ULONG)(a & FractionMask);
        FP_LONG bi = b >> Shift;
        FP_ULONG bf = (FP_ULONG)(b & FractionMask);

        return
            (FP_LONG)((af * bf) >> Shift) +
            ai * b +
            (FP_LONG)af * bi;
    }

    static FP_INT MulIntLongLow(FP_INT a, FP_LONG b)
    {
        FP_ASSERT(a >= 0);
        FP_ULONG af = (FP_ULONG)a;
        FP_LONG bi = b >> Shift;
        FP_ULONG bf = (FP_ULONG)(b & FractionMask);

        return (FP_INT)((FP_LONG)((af * bf) >> Shift) + (FP_LONG)af * bi);
    }

    static FP_INT Nlz(FP_ULONG x)
    {
        FP_INT n = 0;
        if (x <= INT64_C(0x00000000FFFFFFFF)) { n = n + 32; x = x << 32; }
        if (x <= INT64_C(0x0000FFFFFFFFFFFF)) { n = n + 16; x = x << 16; }
        if (x <= INT64_C(0x00FFFFFFFFFFFFFF)) { n = n + 8; x = x << 8; }
        if (x <= INT64_C(0x0FFFFFFFFFFFFFFF)) { n = n + 4; x = x << 4; }
        if (x <= INT64_C(0x3FFFFFFFFFFFFFFF)) { n = n + 2; x = x << 2; }
        if (x <= INT64_C(0x7FFFFFFFFFFFFFFF)) { n = n + 1; }
        if (x == 0) return 64;
        return n;
    }

    static FP_LONG DivRem(FP_LONG arg_a, FP_LONG arg_b, FP_LONG &rem)
    {
        // From http://www.hackersdelight.org/hdcodetxt/divlu.c.txt

        FP_LONG sign_dif = arg_a ^ arg_b;

        static const FP_ULONG b = INT64_C(0x100000000); // Number base (32 bits)
        FP_ULONG unsigned_arg_a = (FP_ULONG)((arg_a < 0) ? -arg_a : arg_a);
        FP_ULONG u1 = unsigned_arg_a >> 32;
        FP_ULONG u0 = unsigned_arg_a << 32;
        FP_ULONG v = (FP_ULONG)((arg_b < 0) ? -arg_b : arg_b);

        // Overflow?
        if (u1 >= v)
        {
            rem = 0;
            return INT64_C(0x7fffffffffffffff);
        }

        // Shift amount for norm
        FP_INT s = Nlz(v); // 0 <= s <= 63
        v = v << s; // Normalize the divisor
        FP_ULONG vn1 = v >> 32; // Break the divisor into two 32-bit digits
        FP_ULONG vn0 = v & INT64_C(0xffffffff);

        FP_ULONG un32 = (u1 << s) | (u0 >> (64 - s)) & (FP_ULONG)((FP_LONG)-s >> 63);
        FP_ULONG un10 = u0 << s; // Shift dividend left

        FP_ULONG un1 = un10 >> 32; // Break the right half of dividend into two digits
        FP_ULONG un0 = un10 & INT64_C(0xffffffff);

        // Compute the first quotient digit, q1
        FP_ULONG q1 = un32 / vn1;
        FP_ULONG rhat = un32 - q1 * vn1;
        do
        {
            if ((q1 >= b) || ((q1 * vn0) > (b * rhat + un1)))
            {
                q1 = q1 - 1;
                rhat = rhat + vn1;
            } else break;
        } while (rhat < b);

        FP_ULONG un21 = un32 * b + un1 - q1 * v; // Multiply and subtract

        // Compute the second quotient digit, q0
        FP_ULONG q0 = un21 / vn1;
        rhat = un21 - q0 * vn1;
        do
        {
            if ((q0 >= b) || ((q0 * vn0) > (b * rhat + un0)))
            {
                q0 = q0 - 1;
                rhat = rhat + vn1;
            } else break;
        } while (rhat < b);

        // Calculate the remainder
        FP_ULONG r = (un21 * b + un0 - q0 * v) >> s;
        rem = (FP_LONG)r;

        FP_ULONG ret = q1 * b + q0;
        return (sign_dif < 0) ? -(FP_LONG)ret : (FP_LONG)ret;
    }

    /// <summary>
    /// Divides two FP values.
    /// </summary>
    static FP_LONG Div(FP_LONG arg_a, FP_LONG arg_b)
    {
        FP_LONG rem;
        return DivRem(arg_a, arg_b, rem);
    }

    /// <summary>
    /// Divides two FP values and returns the modulus.
    /// </summary>
    static FP_LONG Mod(FP_LONG a, FP_LONG b)
    {
        /*FP_LONG d = Div(a, b);
        FP_INT di = ToInt(d);
        FP_LONG ret = a - (di * b);
         * 
        // Sign difference?
        if ((a ^ b) < 0)
            return ret - b;
        return ret;*/

        //FP_LONG di = Div(a, b) >> FP_Shift;
        FP_LONG di = a / b;
        FP_LONG ret = a - (di * b);
        return ret;
    }

    /// <summary>
    /// Calculates the square root of the given number.
    /// </summary>
    static FP_LONG Sqrt(FP_LONG a)
    {
        // Adapted from https://github.com/chmike/fpsqrt
        if (a < 0)
            return -1;

        FP_ULONG r = (FP_ULONG)a;
        FP_ULONG b = INT64_C(0x4000000000000000);
        FP_ULONG q = INT64_C(0);
        while (b > INT64_C(0x40))
        {
            FP_ULONG t = q + b;
            if (r >= t)
            {
                r -= t;
                q = t + b;
            }
            r <<= 1;
            b >>= 1;
        }
        q >>= 16;
        return (FP_LONG)q;
    }

    static FP_INT Qmul29(FP_INT a, FP_INT b)
    {
        return (FP_INT)((FP_LONG)a * (FP_LONG)b >> 29);
    }

    static FP_INT Qmul30(FP_INT a, FP_INT b)
    {
        return (FP_INT)((FP_LONG)a * (FP_LONG)b >> 30);
    }

    static FP_LONG SqrtFast(FP_LONG x)
    {
        // Return 0 for all non-positive values.
        if (x <= 0)
            return 0;

        // Constants (s2.30).
        static const FP_INT ONE = (1 << 30);
        static const FP_INT SQRT2 = 1518500250; // sqrt(2.0)

        // Normalize input into [1.0, 2.0( range (as s2.30).
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        FP_ASSERT(n >= ONE);

        // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
        FP_INT adjust = ((offset & 1) != 0) ? SQRT2 : ONE;
        offset = offset >> 1;

        // Polynomial approximation.
        static const FP_INT C0 = 314419284; // 0.29282577753675165
        static const FP_INT C1 = 1106846240; // 1.0308308904662433
        static const FP_INT C2 = -513029237; // -0.4777957102057744
        static const FP_INT C3 = 211384540; // 0.1968671947173073
        static const FP_INT C4 = -51222328; // -0.04770451046583708
        static const FP_INT C5 = 5343323; // 0.004976357951309034
        FP_INT y = Qmul30(Qmul30(Qmul30(Qmul30(Qmul30(C5, n) + C4, n) + C3, n) + C2, n) + C1, n) + C0;

        // Apply exponent, convert back to s32.32.
        FP_LONG yr = (FP_LONG)Qmul30(adjust, y) << 2;
        return (offset >= 0) ? (yr << offset) : (yr >> -offset);
    }

    /// <summary>
    /// Calculates the reciprocal square root.
    /// </summary>
    static FP_LONG RSqrt(FP_LONG x)
    {
        FP_ASSERT(x > 0);

        // Constants (s2.30).
        static const FP_INT ONE = (1 << 30);
        static const FP_INT HALF_SQRT2 = 759250125; // 0.5 * sqrt(2.0)

        // Normalize input into [1.0, 2.0( range (as s2.30).
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        FP_ASSERT(n >= ONE);
        FP_INT k = n - ONE;

        // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
        FP_INT adjust = ((offset & 1) != 0) ? HALF_SQRT2 : ONE;
        offset = offset >> 1;

        // Polynomial approximation.
        static const FP_INT C0 = 1073741824; // 1.0
        static const FP_INT C1 = -536046292; // -0.49923201361564506
        static const FP_INT C2 = 390824581; // 0.36398375576777275
        static const FP_INT C3 = -274993667; // -0.2561078101369758
        static const FP_INT C4 = 139580279; // 0.12999426553507287
        static const FP_INT C5 = -33856600; // -0.031531416363677275
        FP_INT y = Qmul30(Qmul30(Qmul30(Qmul30(Qmul30(C5, k) + C4, k) + C3, k) + C2, k) + C1, k) + C0;

        // Apply exponent, convert back to s32.32.
        FP_LONG yr = (FP_LONG)Qmul30(adjust, y) << 2;
        return (offset >= 0) ? (yr >> offset) : (yr << -offset);
    }

    /// <summary>
    /// Calculates the reciprocal using precise division.
    /// </summary>
    static FP_LONG RcpDiv(FP_LONG a)
    {
        return Div(One, a);
    }

    /// <summary>
    /// Calculates reciprocal approximation.
    /// </summary>
    static FP_LONG RcpFast(FP_LONG x)
    {
        // Refinement using Newton's method: y' = y * (2 - x*y)
        // see: https://www.geometrictools.com/Documentation/ApproxInvSqrt.pdf

        // \todo [petri] optimize
        if (x == MinValue)
            return 0;

        // Handle negative values.
        FP_INT sign = (x < 0) ? -1 : 1;
        x *= sign;

        // Normalize input into [1.0, 2.0( range (convert to s2.30).
        static const FP_INT ONE = (1 << 30);
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        FP_INT k = n - ONE;

        // Fifth order polynomial approximation.
        static const FP_INT C0 = 1073741823; // 0.9999999999999999
        static const FP_INT C1 = -1070600273; // -0.9970742035352514
        static const FP_INT C2 = 1028280545; // 0.9576608849486308
        static const FP_INT C3 = -837745462; // -0.7802112611061196
        static const FP_INT C4 = 459071950; // 0.42754407090099306
        static const FP_INT C5 = -115877671; // -0.10791949120825255
        FP_LONG y = (FP_LONG)(sign * (Qmul30(Qmul30(Qmul30(Qmul30(Qmul30(C5, k) + C4, k) + C3, k) + C2, k) + C1, k) + C0)) << 2;

        // Apply exponent, convert back to s32.32.
        return (offset >= 0) ? (y >> offset) : (y << -offset);
    }

    /// <summary>
    /// Calculates the base 2 exponent.
    /// </summary>
    static FP_LONG Exp2(FP_LONG x)
    {
        // Base 2 exponent: returns 2^x

        // Handle values that would under or overflow.
        if (x >= 32 * One) return MaxValue;
        if (x <= -32 * One) return 0;

        // Get fractional part as s2.30.
        FP_INT k = (FP_INT)((x & FractionMask) >> 2);

        // Fifth order polynomial approximation.
        static const FP_INT C0 = 1073741823; // 0.9999999999999998
        static const FP_INT C1 = 744267999; // 0.6931535893913809
        static const FP_INT C2 = 257852859; // 0.24014418895798456
        static const FP_INT C3 = 59977680; // 0.05585856767689564
        static const FP_INT C4 = 9608316; // 0.008948442467833984
        static const FP_INT C5 = 2034967; // 0.001895211505904865
        FP_LONG y = (FP_LONG)(Qmul30(Qmul30(Qmul30(Qmul30(Qmul30(C5, k) + C4, k) + C3, k) + C2, k) + C1, k) + C0) << 2;

        // Combine integer and fractional result, and convert back to s32.32.
        FP_INT intPart = (FP_INT)(x >> 32);
        return (intPart >= 0) ? (y << intPart) : (y >> -intPart);
    }

    static FP_LONG Exp(FP_LONG x)
    {
        // e^x == 2^(x / ln(2))
        return Exp2(Mul(x, RCP_LN2));
    }

    static FP_LONG Log(FP_LONG x)
    {
        // Natural logarithm (base e).

        FP_ASSERT(x > 0);

        // Constants (in s2.30).
        static const FP_INT ONE = (1 << 30);

        // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        FP_ASSERT(n >= ONE);
        FP_INT k = n - ONE;

        // Sixth order polynomial approximation.
        static const FP_INT C1 = 1073597915; // 0.9998659751270219
        static const FP_INT C2 = -534132486; // -0.4974496428625768
        static const FP_INT C3 = 339191744; // 0.3158969285196944
        static const FP_INT C4 = -204547008; // -0.19049924682078279
        static const FP_INT C5 = 88850940; // 0.08274888660136276
        static const FP_INT C6 = -18699986; // -0.017415720004774433
        FP_INT y = Qmul30(Qmul30(Qmul30(Qmul30(Qmul30(Qmul30(C6, k) + C5, k) + C4, k) + C3, k) + C2, k) + C1, k);

        // Combine integer and fractional parts (into s32.32).
        static const FP_LONG RCP_LOG2_E = INT64_C(0xb17217f7);     // 1.0 / log2(e) ~= 0.6931471805599453
        return (FP_LONG)offset * RCP_LOG2_E + ((FP_LONG)y << 2);
    }

    static FP_LONG Log2(FP_LONG x)
    {
        return Mul(Log(x), RCP_LN2);
    }

    /// <summary>
    /// Calculates x to the power of the exponent. Assumes that x is positive.
    /// </summary>
    static FP_LONG Pow(FP_LONG x, FP_LONG exponent)
    {
        FP_ASSERT(x > 0);
        return Exp(Mul(exponent, Log(x)));
    }

    static FP_LONG SinPoly5(FP_LONG x)
    {
        // See: http://www.coranac.com/2009/07/sines/

        // Map [0, 2pi] to [0, 4] (as s2.30).
        // This also wraps the values into one period.
        static const FP_INT RCP_HALF_PI = 683565276; // 1.0 / (4.0 * 0.5 * FP_PI);  // the 4.0 factor converts directly to s2.30
        FP_INT z = MulIntLongLow(RCP_HALF_PI, x);

        // Handle quadrants 1 and 2 by mirroring the [1, 3] range to [-1, 1] (by calculating 2 - z).
        // The if condition uses the fact that for the quadrants of interest are 0b01 and 0b10 (top two bits are different).
        if ((z ^ (z << 1)) < 0)
            z = (1 << 31) - z;

        // Now z is in range [-1, 1].
        static const FP_INT ONE = (1 << 30);
        FP_ASSERT((z >= -ONE) && (z <= ONE));

        // Polynomial approximation.
        static const FP_INT C1 = 1686136278; // 1.5703367802360138
        static const FP_INT C3 = -689457190; // -0.6421070455786427
        static const FP_INT C5 = 77062735; // 0.0717702653426288
        FP_INT zz = Qmul30(z, z);
        FP_INT res = Qmul30(Qmul30(Qmul30(C5, zz) + C3, zz) + C1, z);

        // Convert back to s32.32.
        return (FP_LONG)res << 2;
    }

    static FP_LONG CosPoly5(FP_LONG x)
    {
        return SinPoly5(x + PiHalf);
    }

    static FP_LONG TanPoly5(FP_LONG x)
    {
        // \todo [petri] naive implementation
        return Mul(SinPoly5(x), RcpFast(CosPoly5(x)));
    }

    static FP_INT Atan2Div(FP_LONG y, FP_LONG x)
    {
        FP_ASSERT(x > 0);
        FP_ASSERT(y > 0);
        FP_ASSERT(x >= y);

        static const FP_INT HALF = (1 << 29);
        static const FP_INT ONE = (1 << 30);

        // Normalize input into [1.0, 2.0( range (convert to s2.30).
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        FP_INT k = n - ONE;

        // Fifth order polynomial approximation.
        static const FP_INT C0 = 1073741823; // 0.9999999999999999
        static const FP_INT C1 = -1070600273; // -0.9970742035352514
        static const FP_INT C2 = 1028280545; // 0.9576608849486308
        static const FP_INT C3 = -837745462; // -0.7802112611061196
        static const FP_INT C4 = 459071950; // 0.42754407090099306
        static const FP_INT C5 = -115877671; // -0.10791949120825255
        FP_INT oox = Qmul30(Qmul30(Qmul30(Qmul30(Qmul30(C5, k) + C4, k) + C3, k) + C2, k) + C1, k) + C0;
        FP_ASSERT(oox >= HALF && oox <= ONE);

        // Apply exponent and multiply.
        FP_LONG yr = (offset >= 0) ? (y >> offset) : (y << -offset);
        return Qmul30((FP_INT)(yr >> 2), oox);
    }

    static FP_LONG Atan2(FP_LONG y, FP_LONG x)
    {
        // See: https://www.dsprelated.com/showarticle/1052.php
        // \todo [petri] can divs-by-rcp be optimized, since result is known to be <= 1.0 ?

        if (x == 0)
        {
            if (y > 0) return PiHalf;
            if (y < 0) return -PiHalf;
            return 0;
        }

        FP_LONG nx = x ^ (x >> 63); // approx abs
        FP_LONG ny = y ^ (y >> 63);
        FP_LONG negMask = ((x ^ y) >> 63);   // \note this isn't strictly symmetrical

        static const FP_INT C1 = 1075846406; // 1.0019600447288488
        static const FP_INT C2 = -10418146; // -0.009702654828140988
        static const FP_INT C3 = -377890075; // -0.3519375581283367
        static const FP_INT C4 = 156064417; // 0.14534631494325442

        if (nx >= ny)
        {
            FP_INT z = Atan2Div(ny, nx);
            FP_LONG angle = negMask ^ ((FP_LONG)Qmul30(Qmul30(Qmul30(Qmul30(C4, z) + C3, z) + C2, z) + C1, z) << 2);
            if (x > 0) return angle;
            if (y > 0) return angle + Pi;
            return angle - Pi;
        }
        else
        {
            FP_INT z = Atan2Div(nx, ny);
            FP_LONG angle = negMask ^ ((FP_LONG)Qmul30(Qmul30(Qmul30(Qmul30(C4, z) + C3, z) + C2, z) + C1, z) << 2);
            return ((y > 0) ? PiHalf : -PiHalf) - angle;
        }
    }

    static FP_LONG Asin(FP_LONG x)
    {
        FP_ASSERT(x >= -One && x <= One);
        return Atan2(x, SqrtFast(Mul(One + x, One - x)));
    }

    static FP_LONG Acos(FP_LONG x)
    {
        FP_ASSERT(x >= -One && x <= One);
        return Atan2(SqrtFast(Mul(One + x, One - x)), x);
    }

    static FP_LONG Atan(FP_LONG x)
    {
        // \todo [petri] better implementation?
        return Atan2(x, One);
    }


    #undef FP_ASSERT
};
#endif

