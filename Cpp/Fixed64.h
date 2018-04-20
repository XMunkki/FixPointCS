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
        return (FP_LONG)(v * (4294967296.0));
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

    static FP_INT qRcpNorm29(FP_INT n)
    {
        // Constants.
        static const FP_INT ONE = (1 << 29);
        static const FP_INT TWO = (2 << 29);

        FP_ASSERT(n >= ONE && n <= TWO);

        // Use polynomial approximation for initial guess of rcp().
        static const FP_INT n0 = (FP_INT)(ONE * 0.3388308335945457);
        static const FP_INT n1 = (FP_INT)(ONE * -1.5164925007836372);
        static const FP_INT n2 = (FP_INT)(ONE * 2.1776616671890916);
        FP_INT y = Qmul29(Qmul29(n0, n) + n1, n) + n2;

        // Use Newton iterations to increase accuracy: y' = y * (2 - n*y)
        y = Qmul29(y, TWO - Qmul29(n, y));
        y = Qmul29(y, TWO - Qmul29(n, y));
        //y = Qmul29(y, TWO - Qmul29(n, y));

        return y;
    }

    static FP_LONG SqrtFast(FP_LONG x)
    {
        // Performs basically RSqrt(), followed by reciprocal.
        // See: https://www.geometrictools.com/Documentation/ApproxInvSqrt.pdf

        // Return 0 for all non-positive values.
        if (x <= 0)
            return 0;

        // Constants (s3.29).
        static const FP_INT HALF = (1 << 28);
        static const FP_INT ONE = (1 << 29);
        static const FP_INT THREE = (3 << 29);
        static const FP_INT SQRT2 = 2 * 379625062; // sqrt(2.0)

        // Normalize input into [1.0, 2.0( range.
        // Convert normalized value to s3.29.
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 3);
        FP_ASSERT(n >= ONE && n < 2 * ONE);

        // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
        FP_INT adjust = ((offset & 1) != 0) ? SQRT2 : ONE;
        offset = offset >> 1;

        // Use polynomial approximation for initial guess of rsqrt().
        static const FP_INT n0 = (FP_INT)(ONE * -0.0854582920881071);
        static const FP_INT n1 = (FP_INT)(ONE * 0.534580261270677);
        static const FP_INT n2 = (FP_INT)(ONE * -1.298425958008734);
        static const FP_INT n3 = (FP_INT)(ONE * 1.8493039888261642);
        FP_INT y = Qmul29(Qmul29(Qmul29(n0, n) + n1, n) + n2, n) + n3;

        // Use Newton iterations to increase accuracy: y' = y/2 * (3 - x*y*y).
        y = Qmul29(y >> 1, THREE - Qmul29(n, Qmul29(y, y)));
        //y = Qmul29(y >> 1, THREE - Qmul29(n, Qmul29(y, y)));
        FP_ASSERT(y >= HALF && y <= ONE);

        // Reciprocal (pre- and post-multiply by 2 to get y into [1.0, 2.0] range).
        y = qRcpNorm29(y << 1) << 1;

        // Apply exponent, convert back to s32.32.
        FP_LONG yr = (FP_LONG)Qmul29(adjust, y);
        return ((offset >= 0) ? (yr << offset) : (yr >> -offset)) << 3;
    }

    /// <summary>
    /// Calculates the reciprocal square root.
    /// </summary>
    static FP_LONG RSqrt(FP_LONG x)
    {
        // Refinement using Newton's method: y' = y/2 * (3 - x*y^2)
        // see: https://www.geometrictools.com/Documentation/ApproxInvSqrt.pdf

        FP_ASSERT(x > 0);

        // Constants (s3.29).
        static const FP_INT ONE = (1 << 29);
        static const FP_INT THREE = (3 << 29);
        static const FP_INT HALF_SQRT2 = 379625062; // 0.5 * sqrt(2.0)

        // Normalize input into [1.0, 2.0( range.
        // Convert normalized value to s3.29.
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 3);
        FP_ASSERT(n >= ONE && n < 2*ONE);

        // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
        FP_INT adjust = ((offset & 1) != 0) ? HALF_SQRT2 : ONE;
        offset = offset >> 1;

        // Use polynomial approximation for initial guess of rsqrt(n).
        static const FP_INT n0 = (FP_INT)(ONE * -0.0854582920881071);
        static const FP_INT n1 = (FP_INT)(ONE * 0.534580261270677);
        static const FP_INT n2 = (FP_INT)(ONE * -1.298425958008734);
        static const FP_INT n3 = (FP_INT)(ONE * 1.8493039888261642);
        FP_INT y = Qmul29(Qmul29(Qmul29(n0, n) + n1, n) + n2, n) + n3;

        // Use Newton iterations to increase accuracy: y' = y/2 * (3 - x*y*y).
        y = Qmul29(y >> 1, THREE - Qmul29(n, Qmul29(y, y)));
        // y = Qmul29(y >> 1, THREE - Qmul29(n, Qmul29(y, y)));

        // Apply exponent, convert back to s32.32.
        FP_LONG yr = (FP_LONG)Qmul29(adjust, y);
        return ((offset >= 0) ? (yr >> offset) : (yr << -offset)) << 3;
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
        x = Abs(x);

        // Normalize input into [1.0, 2.0( range (convert to s3.29).
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 3);

        // Compute normalized reciprocal.
        FP_INT y = sign * qRcpNorm29(n);

        // Apply exponent, convert back to s32.32.
        return ((offset >= 0) ? ((FP_LONG)y >> offset) : ((FP_LONG)y << -offset)) << 3;
    }

    /// <summary>
    /// Calculates the base 2 exponent.
    /// </summary>
    static FP_LONG Exp2(FP_LONG x)
    {
        // Base 2 exponent: returns 2^x
        // See: https://github.com/asik/FixedMath.Net/blob/master/src/Fix64.cs

        // \todo [petri] early exit for zero and one?

        // Handle values that would under or overflow.
        if (x >= 32 * One) return MaxValue;
        if (x <= -32 * One) return 0;

        // Handle negative inputs with: exp(-x) == rcp(exp(x)).
        bool isNeg = x < 0;
        x = Abs(x);

        // Get fractional part as s2.30.
        FP_INT frac = (FP_INT)((x & FractionMask) >> 2);

        // Constants (in s2.30).
        static const FP_INT LN2 = 744261117; // log(e) ~= 0.6931471805599453

        // Accumulate fractional part iteratively.
        // \note adjust performance vs precision trade-off by adjusting the number of iterations
        FP_LONG result = 1 << 30;
	    FP_INT term = Qmul30(frac, LN2); result += term;
	    term = Qmul30(Qmul30(frac, term), LN2 / 2); result += term;
        term = Qmul30(Qmul30(frac, term), LN2 / 3); result += term;
        term = Qmul30(Qmul30(frac, term), LN2 / 4); result += term;
        term = Qmul30(Qmul30(frac, term), LN2 / 5); result += term;
        term = Qmul30(Qmul30(frac, term), LN2 / 6); result += term;
        //term = Qmul30(Qmul30(frac, term), LN2 / 7); result += term;

        // Combine integer and fractional result, and convert back to s32.32.
        FP_INT intPart = (FP_INT)(x >> 32);
        FP_LONG res = result << (intPart + 2);

        // Reciprocal if input was negative.
        // \todo [petri] is there a faster way?
        return isNeg ? RcpFast(res) : res;
    }

    static FP_LONG Exp(FP_LONG x)
    {
        // e^x == 2^(x / ln(2))
        return Exp2(Mul(x, RCP_LN2));
    }

    static FP_LONG Log(FP_LONG x)
    {
        // Natural logarithm (base e).
        // See: https://gist.github.com/Madsy/1088393#file-gistfile1-c-L127

        // Constants (in s3.29).
        static const FP_INT ONE = (1 << 29);
        static const FP_INT DENOM_0 = (1 << 29) / 1;
        static const FP_INT DENOM_1 = (1 << 29) / 3;
        static const FP_INT DENOM_2 = (1 << 29) / 5;
        static const FP_INT DENOM_3 = (1 << 29) / 7;
        static const FP_INT DENOM_4 = (1 << 29) / 9;
        static const FP_INT DENOM_5 = (1 << 29) / 11;
        //static const FP_INT DENOM_6 = (1 << 29) / 13;
        //static const FP_INT DENOM_7 = (1 << 29) / 15;
        //static const FP_INT DENOM_8 = (1 << 29) / 17;
        //static const FP_INT DENOM_9 = (1 << 29) / 19;
        static const FP_LONG RCP_LOG2_E = INT64_C(0xb17217f7);     // 1.0 / log2(e) ~= 0.6931471805599453

        FP_ASSERT(x > 0);

        // Normalize value to range [1.0, 2.0( as s3.29 and extract "exponent".
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT frac = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 3);
        FP_ASSERT(frac >= ONE && frac < 2*ONE);

        // Compute initial value: y = (frac - 1.0) / (frac + 1.0)
        // \note y is in range [0.0, 0.3333..], keep it as s3.29
        FP_INT y = Qmul29(frac - ONE, qRcpNorm29((frac + ONE) >> 1) >> 1);
        FP_ASSERT(y >= 0 && y < ONE/2);

        // Iterative refinement of fractional part: f' = (f + k) * y*y
        FP_INT ySq = Qmul29(y, y);
        FP_INT fracr = 0;
        fracr = Qmul29(fracr + DENOM_5, ySq);
        fracr = Qmul29(fracr + DENOM_4, ySq);
        fracr = Qmul29(fracr + DENOM_3, ySq);
        fracr = Qmul29(fracr + DENOM_2, ySq);
        fracr = Qmul29(fracr + DENOM_1, ySq);
        fracr = Qmul29(fracr + DENOM_0, y*2);

        // Combine integer and fractional parts (into s32.32).
        return (FP_LONG)offset * RCP_LOG2_E + ((FP_LONG)fracr << 3);
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
        // Formula for 5th order sin: z/2 * (pi - z*z*((2pi - 5) - z*z*(pi-3)))
        // See: http://www.coranac.com/2009/07/sines/

        // Map [0, 2pi] to [0, 4] (as s2.30).
        // This also wraps the values into one period.
        static const FP_INT RCP_HALF_PI = (FP_INT)(One / (4.0 * 0.5 * FP_PI));  // the 4.0 factor converts directly to s2.30
        FP_INT z = MulIntLongLow(RCP_HALF_PI, x);

        // Handle quadrants 1 and 2 by mirroring the [1, 3] range to [-1, 1] (by calculating 2 - z).
        // The if condition uses the fact that for the quadrants of interest are 0b01 and 0b10 (top two bits are different).
        if ((z ^ (z << 1)) < 0)
            z = (1 << 31) - z;

        // Now z is in range [-1, 1].
        static const FP_INT ONE = (1 << 30);
        FP_ASSERT((z >= -ONE) && (z <= ONE));

        // Calculate polynomial approximation.
        static const FP_INT n0 = (FP_INT)((1 << 30) * 0.07177026534258267);
        static const FP_INT n1 = (FP_INT)((1 << 30) * -0.64210704557858);
        static const FP_INT n2 = (FP_INT)((1 << 30) * 1.5703367802359975);
        FP_INT zz = Qmul30(z, z);
        FP_INT res = Qmul30(Qmul30(Qmul30(n0, zz) + n1, zz) + n2, z);

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

    static FP_LONG Atan2(FP_LONG y, FP_LONG x)
    {
        // Fast, but inaccurate 2nd order polynomial approximation.
        // See: https://www.dsprelated.com/showarticle/1052.php
        // \todo [petri] can divs-by-rcp be optimized, since result is known to be <= 1.0 ?
        // \todo [petri] more accurate variant? higher order poly?

        if (x == 0)
        {
            if (y > 0) return PiHalf;
            if (y < 0) return -PiHalf;
            return 0;
        }

        static const FP_INT n0 = (FP_INT)((1 << 30) * -0.2713689403818643);
        static const FP_INT n1 = (FP_INT)((1 << 30) * 1.0597123221174944);

        FP_LONG nx = Nabs(x);
        FP_LONG ny = Nabs(y);
        FP_LONG negMask = ((x ^ y) >> 63);   // \note this isn't strictly symmetrical

        if (nx <= ny)
        {
            FP_INT z = (FP_INT)(Mul(ny, RcpFast(nx)) >> 2);
            FP_LONG angle = negMask ^ (FP_LONG)Qmul30(Qmul30(n0, z) + n1, z) << 2;
            if (x > 0) return angle;
            if (y > 0) return angle + Pi;
            return angle - Pi;
        }
        else
        {
            FP_INT z = (FP_INT)(Mul(nx, RcpFast(ny)) >> 2);
            FP_LONG angle = negMask ^ (FP_LONG)Qmul30(Qmul30(n0, z) + n1, z) << 2;
            return (y > 0) ? (PiHalf - angle) : (-PiHalf - angle);
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

