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
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
#if !CPP
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Diagnostics;

/* Coding style:
 * 
 * In order to keep the generated C++ code working, here are some generic
 * coding guidelines.
 * 
 * All 64bit constants should be of the form " -1234L" or " 0x1234L" (so start
 * with a whitespace and end with a capital L).
 * 
 * All definitions should be in dependency order. That is, define functions
 * that are used later first. This is because C++ processes things in order,
 * where as in C# the definition order doesn't matter.
 * 
 * Minimize the use of system libraries.
 * 
 * You can use "#if CPP" else "#if !CPP" to block out code that's C# or C++
 * only. Currently the generator preprocessor is really ad-hoc and gets mixed
 * up by any other #if:s, #else:s or #endif:s.
 */

namespace FixPointCS
{
    /// <summary>
    /// Direct fixed point (signed 32.32) functions.
    /// </summary>
    public static class Fixed64
    {
        // Backwards compatible way to use MethodImplOptions.AggressiveInlining
        public const MethodImplOptions AggressiveInlining = (MethodImplOptions)256;
#endif
        public const int Shift = 32;
        public const long FractionMask = ( 1L << Shift ) - 1; // Space before 1L needed because of hacky C++ code generator
        public const long IntegerMask = ~FractionMask;

        // Constants
        public const long Zero = 0L;
        public const long Neg1 = -1L << Shift;
        public const long One = 1L << Shift;
        public const long Two = 2L << Shift;
        public const long Three = 3L << Shift;
        public const long Four = 4L << Shift;
        public const long Half = One >> 1;
        public const long Pi = 13493037705L; //(long)(Math.PI * 65536.0) << 16;
        public const long Pi2 = 26986075409L;
        public const long PiHalf = 6746518852L;
        public const long E = 11674931555L;

        public const long MinValue = -9223372036854775808L; // 0x8000000000000000L
        public const long MaxValue = 0x7FFFFFFFFFFFFFFFL;

        // Private constants
        private const long RCP_LN2 = 0x171547652L; // 1.0 / Math.Log(2.0) ~= 1.4426950408889634

        /// <summary>
        /// Converts an integer into a FP value.
        /// </summary>
        public static long FromInt(int v)
        {
            return (long)v << Shift;
        }

        /// <summary>
        /// Converts a double into a FP value.
        /// </summary>
        public static long FromDouble(double v)
        {
            return (long)(v * (4294967296.0));
        }

        /// <summary>
        /// Converts a float into a FP value.
        /// </summary>
        public static long FromFloat(float v)
        {
            return FromDouble((double)v);
        }

        /// <summary>
        /// Converts a FP value into an integer by rounding it up to nearest integer.
        /// </summary>
        public static int CeilToInt(long v)
        {
            return (int)((v + (One - 1)) >> Shift);
        }

        /// <summary>
        /// Converts a FP value into an integer by rounding it down to nearest integer.
        /// </summary>
        public static int FloorToInt(long v)
        {
            return (int)(v >> Shift);
        }

        /// <summary>
        /// Converts a FP value into an integer by rounding it to nearest integer.
        /// </summary>
        public static int RoundToInt(long v)
        {
            return (int)((v + Half) >> Shift);
        }

        /// <summary>
        /// Converts a FP value into a double.
        /// </summary>
        public static double ToDouble(long v)
        {
            return (double)v * (1.0 / (4294967296.0));
        }

        /// <summary>
        /// Converts a FP value into a float.
        /// </summary>
        public static float ToFloat(long v)
        {
            return (float)ToDouble(v);
        }

        /// <summary>
        /// Converts the value to a human readable string.
        /// </summary>
#if !CPP
        public static string ToString(long v)
        {
            return ToDouble(v).ToString();
        }
#endif

        /// <summary>
        /// Returns the absolute (positive) value of x.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static long Abs(long x)
        {
            // \note fails with LONG_MIN
            // \note for some reason this is twice as fast as (x > 0) ? x : -x
            return (x < 0) ? -x : x;
        }

        /// <summary>
        /// Negative absolute value (returns -abs(x)).
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static long Nabs(long x)
        {
            return (x > 0) ? -x : x;
        }

        /// <summary>
        /// Round up to nearest integer.
        /// </summary>
        public static long Ceil(long x)
        {
            return (x + FractionMask) & IntegerMask;
        }

        /// <summary>
        /// Round down to nearest integer.
        /// </summary>
        public static long Floor(long x)
        {
            return x & IntegerMask;
        }

        /// <summary>
        /// Round to nearest integer.
        /// </summary>
        public static long Round(long x)
        {
            // \todo [petri] Math.Round() round to nearest even number
            return (x + Half) & IntegerMask;
        }

        /// <summary>
        /// Returns the minimum of the two values.
        /// </summary>
        public static long Min(long a, long b)
        {
            return (a < b) ? a : b;
        }

        /// <summary>
        /// Returns the maximum of the two values.
        /// </summary>
        public static long Max(long a, long b)
        {
            return (a > b) ? a : b;
        }

        /// <summary>
        /// Returns the sign of the value (-1 if negative, 0 if zero, 1 if positive).
        /// </summary>
        public static int Sign(long x)
        {
            if (x == 0) return 0;
            return (x < 0) ? -1 : 1;
        }

        /// <summary>
        /// Adds the two FP numbers together.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static long Add(long a, long b)
        {
            return a + b;
        }

        /// <summary>
        /// Subtracts the two FP numbers from each other.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static long Sub(long a, long b)
        {
            return a - b;
        }

        /// <summary>
        /// Multiplies two FP values together.
        /// </summary>
        public static long Mul(long a, long b)
        {
            /*ulong alow = (ulong)(a & FP_Mask_Fract);
            long ahigh = a >> FP_Shift;
            ulong blow = (ulong)(b & FP_Mask_Fract);
            long bhigh = b >> FP_Shift;

            ulong lowlow = alow * blow;
            long lowhigh = (long)(uint)alow * bhigh;
            long highlow = ahigh * (long)(uint)blow;
            long highhigh = ahigh * bhigh;

            long lo_res = (long)(lowlow >> 32);
            long mid_res1 = lowhigh;
            long mid_res2 = highlow;
            long hi_res = highhigh << 32;

            return lo_res + mid_res1 + mid_res2 + hi_res;*/

            long ai = a >> Shift;
            ulong af = (ulong)(a & FractionMask);
            long bi = b >> Shift;
            ulong bf = (ulong)(b & FractionMask);

            return
                (long)((af * bf) >> Shift) +
                ai * b +
                (long)af * bi;
        }

        [MethodImpl(AggressiveInlining)]
        private static int MulIntLongLow(int a, long b)
        {
            Debug.Assert(a >= 0);
            ulong af = (ulong)a;
            long bi = b >> Shift;
            ulong bf = (ulong)(b & FractionMask);

            return (int)((long)((af * bf) >> Shift) + (long)af * bi);
        }

        [MethodImpl(AggressiveInlining)]
        private static int Nlz(ulong x)
        {
            int n = 0;
            if (x <= 0x00000000FFFFFFFFL) { n = n + 32; x = x << 32; }
            if (x <= 0x0000FFFFFFFFFFFFL) { n = n + 16; x = x << 16; }
            if (x <= 0x00FFFFFFFFFFFFFFL) { n = n + 8; x = x << 8; }
            if (x <= 0x0FFFFFFFFFFFFFFFL) { n = n + 4; x = x << 4; }
            if (x <= 0x3FFFFFFFFFFFFFFFL) { n = n + 2; x = x << 2; }
            if (x <= 0x7FFFFFFFFFFFFFFFL) { n = n + 1; }
            if (x == 0) return 64;
            return n;
        }

#if !CPP
        private static long DivRem(long arg_a, long arg_b, out long rem)
#else
        private static long DivRem(long arg_a, long arg_b, long &rem)
#endif
        {
            // From http://www.hackersdelight.org/hdcodetxt/divlu.c.txt

            long sign_dif = arg_a ^ arg_b;

            const ulong b = 0x100000000L; // Number base (32 bits)
            ulong unsigned_arg_a = (ulong)((arg_a < 0) ? -arg_a : arg_a);
            ulong u1 = unsigned_arg_a >> 32;
            ulong u0 = unsigned_arg_a << 32;
            ulong v = (ulong)((arg_b < 0) ? -arg_b : arg_b);

            // Overflow?
            if (u1 >= v)
            {
                rem = 0;
                return 0x7fffffffffffffffL;
            }

            // Shift amount for norm
            int s = Nlz(v); // 0 <= s <= 63
            v = v << s; // Normalize the divisor
            ulong vn1 = v >> 32; // Break the divisor into two 32-bit digits
            ulong vn0 = v & 0xffffffffL;

            ulong un32 = (u1 << s) | (u0 >> (64 - s)) & (ulong)((long)-s >> 63);
            ulong un10 = u0 << s; // Shift dividend left

            ulong un1 = un10 >> 32; // Break the right half of dividend into two digits
            ulong un0 = un10 & 0xffffffffL;

            // Compute the first quotient digit, q1
            ulong q1 = un32 / vn1;
            ulong rhat = un32 - q1 * vn1;
            do
            {
                if ((q1 >= b) || ((q1 * vn0) > (b * rhat + un1)))
                {
                    q1 = q1 - 1;
                    rhat = rhat + vn1;
                } else break;
            } while (rhat < b);

            ulong un21 = un32 * b + un1 - q1 * v; // Multiply and subtract

            // Compute the second quotient digit, q0
            ulong q0 = un21 / vn1;
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
            ulong r = (un21 * b + un0 - q0 * v) >> s;
            rem = (long)r;

            ulong ret = q1 * b + q0;
            return (sign_dif < 0) ? -(long)ret : (long)ret;
        }

        /// <summary>
        /// Divides two FP values.
        /// </summary>
        public static long Div(long arg_a, long arg_b)
        {
            long rem;
#if !CPP
            return DivRem(arg_a, arg_b, out rem);
#else
            return DivRem(arg_a, arg_b, rem);
#endif
        }

        /// <summary>
        /// Divides two FP values and returns the modulus.
        /// </summary>
        public static long Mod(long a, long b)
        {
            /*long d = Div(a, b);
            int di = ToInt(d);
            long ret = a - (di * b);
             * 
            // Sign difference?
            if ((a ^ b) < 0)
                return ret - b;
            return ret;*/

            //long di = Div(a, b) >> FP_Shift;
            long di = a / b;
            long ret = a - (di * b);
            return ret;
        }

        /// <summary>
        /// Calculates the square root of the given number.
        /// </summary>
        public static long Sqrt(long a)
        {
            // Adapted from https://github.com/chmike/fpsqrt
            if (a < 0)
                return -1;

            ulong r = (ulong)a;
            ulong b = 0x4000000000000000L;
            ulong q = 0L;
            while (b > 0x40L)
            {
                ulong t = q + b;
                if (r >= t)
                {
                    r -= t;
                    q = t + b;
                }
                r <<= 1;
                b >>= 1;
            }
            q >>= 16;
            return (long)q;
        }

        [MethodImpl(AggressiveInlining)]
        private static int Qmul29(int a, int b)
        {
            return (int)((long)a * (long)b >> 29);
        }

        [MethodImpl(AggressiveInlining)]
        private static int Qmul30(int a, int b)
        {
            return (int)((long)a * (long)b >> 30);
        }

        [MethodImpl(AggressiveInlining)]
        private static int qRcpNorm29(int n)
        {
            // Constants.
            const int ONE = (1 << 29);
            const int TWO = (2 << 29);

            Debug.Assert(n >= ONE && n <= TWO);

            // Use polynomial approximation for initial guess of rcp().
            const int n0 = (int)(ONE * 0.3388308335945457);
            const int n1 = (int)(ONE * -1.5164925007836372);
            const int n2 = (int)(ONE * 2.1776616671890916);
            int y = Qmul29(Qmul29(n0, n) + n1, n) + n2;

            // Use Newton iterations to increase accuracy: y' = y * (2 - n*y)
            y = Qmul29(y, TWO - Qmul29(n, y));
            y = Qmul29(y, TWO - Qmul29(n, y));
            //y = Qmul29(y, TWO - Qmul29(n, y));

            return y;
        }

        public static long SqrtFast(long x)
        {
            // Performs basically RSqrt(), followed by reciprocal.
            // See: https://www.geometrictools.com/Documentation/ApproxInvSqrt.pdf

            // Return 0 for all non-positive values.
            if (x <= 0)
                return 0;

            // Constants (s3.29).
            const int HALF = (1 << 28);
            const int ONE = (1 << 29);
            const int THREE = (3 << 29);
            const int SQRT2 = 2 * 379625062; // sqrt(2.0)

            // Normalize input into [1.0, 2.0( range.
            // Convert normalized value to s3.29.
            int offset = 31 - Nlz((ulong)x);
            int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 3);
            Debug.Assert(n >= ONE && n < 2 * ONE);

            // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
            int adjust = ((offset & 1) != 0) ? SQRT2 : ONE;
            offset = offset >> 1;

            // Use polynomial approximation for initial guess of rsqrt().
            const int n0 = (int)(ONE * -0.0854582920881071);
            const int n1 = (int)(ONE * 0.534580261270677);
            const int n2 = (int)(ONE * -1.298425958008734);
            const int n3 = (int)(ONE * 1.8493039888261642);
            int y = Qmul29(Qmul29(Qmul29(n0, n) + n1, n) + n2, n) + n3;

            // Use Newton iterations to increase accuracy: y' = y/2 * (3 - x*y*y).
            y = Qmul29(y >> 1, THREE - Qmul29(n, Qmul29(y, y)));
            //y = Qmul29(y >> 1, THREE - Qmul29(n, Qmul29(y, y)));
            Debug.Assert(y >= HALF && y <= ONE);

            // Reciprocal (pre- and post-multiply by 2 to get y into [1.0, 2.0] range).
            y = qRcpNorm29(y << 1) << 1;

            // Apply exponent, convert back to s32.32.
            long yr = (long)Qmul29(adjust, y);
            return ((offset >= 0) ? (yr << offset) : (yr >> -offset)) << 3;
        }

        /// <summary>
        /// Calculates the reciprocal square root.
        /// </summary>
        public static long RSqrt(long x)
        {
            // Refinement using Newton's method: y' = y/2 * (3 - x*y^2)
            // see: https://www.geometrictools.com/Documentation/ApproxInvSqrt.pdf

            Debug.Assert(x > 0);

            // Constants (s3.29).
            const int ONE = (1 << 29);
            const int THREE = (3 << 29);
            const int HALF_SQRT2 = 379625062; // 0.5 * sqrt(2.0)

            // Normalize input into [1.0, 2.0( range.
            // Convert normalized value to s3.29.
            int offset = 31 - Nlz((ulong)x);
            int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 3);
            Debug.Assert(n >= ONE && n < 2*ONE);

            // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
            int adjust = ((offset & 1) != 0) ? HALF_SQRT2 : ONE;
            offset = offset >> 1;

            // Use polynomial approximation for initial guess of rsqrt(n).
            const int n0 = (int)(ONE * -0.0854582920881071);
            const int n1 = (int)(ONE * 0.534580261270677);
            const int n2 = (int)(ONE * -1.298425958008734);
            const int n3 = (int)(ONE * 1.8493039888261642);
            int y = Qmul29(Qmul29(Qmul29(n0, n) + n1, n) + n2, n) + n3;

            // Use Newton iterations to increase accuracy: y' = y/2 * (3 - x*y*y).
            y = Qmul29(y >> 1, THREE - Qmul29(n, Qmul29(y, y)));
            // y = Qmul29(y >> 1, THREE - Qmul29(n, Qmul29(y, y)));

            // Apply exponent, convert back to s32.32.
            long yr = (long)Qmul29(adjust, y);
            return ((offset >= 0) ? (yr >> offset) : (yr << -offset)) << 3;
        }

        /// <summary>
        /// Calculates the reciprocal using precise division.
        /// </summary>
        public static long RcpDiv(long a)
        {
            return Div(One, a);
        }

        /// <summary>
        /// Calculates reciprocal approximation.
        /// </summary>
        public static long RcpFast(long x)
        {
            // Refinement using Newton's method: y' = y * (2 - x*y)
            // see: https://www.geometrictools.com/Documentation/ApproxInvSqrt.pdf

            // \todo [petri] optimize
            if (x == MinValue)
                return 0;

            // Handle negative values.
            int sign = (x < 0) ? -1 : 1;
            x = Abs(x);

            // Normalize input into [1.0, 2.0( range (convert to s3.29).
            int offset = 31 - Nlz((ulong)x);
            int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 3);

            // Compute normalized reciprocal.
            int y = sign * qRcpNorm29(n);

            // Apply exponent, convert back to s32.32.
            return ((offset >= 0) ? ((long)y >> offset) : ((long)y << -offset)) << 3;
        }

        /// <summary>
        /// Calculates the base 2 exponent.
        /// </summary>
        public static long Exp2(long x)
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
            int frac = (int)((x & FractionMask) >> 2);

            // Constants (in s2.30).
            const int LN2 = 744261117; // log(e) ~= 0.6931471805599453

            // Accumulate fractional part iteratively.
            // \note adjust performance vs precision trade-off by adjusting the number of iterations
            long result = 1 << 30;
		    int term = Qmul30(frac, LN2); result += term;
		    term = Qmul30(Qmul30(frac, term), LN2 / 2); result += term;
            term = Qmul30(Qmul30(frac, term), LN2 / 3); result += term;
            term = Qmul30(Qmul30(frac, term), LN2 / 4); result += term;
            term = Qmul30(Qmul30(frac, term), LN2 / 5); result += term;
            term = Qmul30(Qmul30(frac, term), LN2 / 6); result += term;
            //term = Qmul30(Qmul30(frac, term), LN2 / 7); result += term;

            // Combine integer and fractional result, and convert back to s32.32.
            int intPart = (int)(x >> 32);
            long res = result << (intPart + 2);

            // Reciprocal if input was negative.
            // \todo [petri] is there a faster way?
            return isNeg ? RcpFast(res) : res;
        }

        public static long Exp(long x)
        {
            // e^x == 2^(x / ln(2))
            return Exp2(Mul(x, RCP_LN2));
        }

        public static long Log(long x)
        {
            // Natural logarithm (base e).
            // See: https://gist.github.com/Madsy/1088393#file-gistfile1-c-L127

            // Constants (in s3.29).
            const int ONE = (1 << 29);
            const int DENOM_0 = (1 << 29) / 1;
            const int DENOM_1 = (1 << 29) / 3;
            const int DENOM_2 = (1 << 29) / 5;
            const int DENOM_3 = (1 << 29) / 7;
            const int DENOM_4 = (1 << 29) / 9;
            const int DENOM_5 = (1 << 29) / 11;
            //const int DENOM_6 = (1 << 29) / 13;
            //const int DENOM_7 = (1 << 29) / 15;
            //const int DENOM_8 = (1 << 29) / 17;
            //const int DENOM_9 = (1 << 29) / 19;
            const long RCP_LOG2_E = 0xb17217f7L;     // 1.0 / log2(e) ~= 0.6931471805599453

            Debug.Assert(x > 0);

            // Normalize value to range [1.0, 2.0( as s3.29 and extract "exponent".
            int offset = 31 - Nlz((ulong)x);
            int frac = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 3);
            Debug.Assert(frac >= ONE && frac < 2*ONE);

            // Compute initial value: y = (frac - 1.0) / (frac + 1.0)
            // \note y is in range [0.0, 0.3333..], keep it as s3.29
            int y = Qmul29(frac - ONE, qRcpNorm29((frac + ONE) >> 1) >> 1);
            Debug.Assert(y >= 0 && y < ONE/2);

            // Iterative refinement of fractional part: f' = (f + k) * y*y
            int ySq = Qmul29(y, y);
            int fracr = 0;
            fracr = Qmul29(fracr + DENOM_5, ySq);
            fracr = Qmul29(fracr + DENOM_4, ySq);
            fracr = Qmul29(fracr + DENOM_3, ySq);
            fracr = Qmul29(fracr + DENOM_2, ySq);
            fracr = Qmul29(fracr + DENOM_1, ySq);
            fracr = Qmul29(fracr + DENOM_0, y*2);

            // Combine integer and fractional parts (into s32.32).
            return (long)offset * RCP_LOG2_E + ((long)fracr << 3);
        }

        public static long Log2(long x)
        {
            return Mul(Log(x), RCP_LN2);
        }

        /// <summary>
        /// Calculates x to the power of the exponent. Assumes that x is positive.
        /// </summary>
        public static long Pow(long x, long exponent)
        {
            Debug.Assert(x > 0);
            return Exp(Mul(exponent, Log(x)));
        }

        public static long SinPoly5(long x)
        {
            // Formula for 5th order sin: z/2 * (pi - z*z*((2pi - 5) - z*z*(pi-3)))
            // See: http://www.coranac.com/2009/07/sines/

            // Map [0, 2pi] to [0, 4] (as s2.30).
            // This also wraps the values into one period.
            const int RCP_HALF_PI = (int)(One / (4.0 * 0.5 * Math.PI));  // the 4.0 factor converts directly to s2.30
            int z = MulIntLongLow(RCP_HALF_PI, x);

            // Handle quadrants 1 and 2 by mirroring the [1, 3] range to [-1, 1] (by calculating 2 - z).
            // The if condition uses the fact that for the quadrants of interest are 0b01 and 0b10 (top two bits are different).
            if ((z ^ (z << 1)) < 0)
                z = (1 << 31) - z;

            // Now z is in range [-1, 1].
            const int ONE = (1 << 30);
            Debug.Assert((z >= -ONE) && (z <= ONE));

            // Calculate polynomial approximation.
            const int n0 = (int)((1 << 30) * 0.07177026534258267);
            const int n1 = (int)((1 << 30) * -0.64210704557858);
            const int n2 = (int)((1 << 30) * 1.5703367802359975);
            int zz = Qmul30(z, z);
            int res = Qmul30(Qmul30(Qmul30(n0, zz) + n1, zz) + n2, z);

            // Convert back to s32.32.
            return (long)res << 2;
        }

        public static long CosPoly5(long x)
        {
            return SinPoly5(x + PiHalf);
        }

        public static long TanPoly5(long x)
        {
            // \todo [petri] naive implementation
            return Mul(SinPoly5(x), RcpFast(CosPoly5(x)));
        }

        public static long Atan2(long y, long x)
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

            const int n0 = (int)((1 << 30) * -0.2713689403818643);
            const int n1 = (int)((1 << 30) * 1.0597123221174944);

            long nx = Nabs(x);
            long ny = Nabs(y);
            long negMask = ((x ^ y) >> 63);   // \note this isn't strictly symmetrical

            if (nx <= ny)
            {
                int z = (int)(Mul(ny, RcpFast(nx)) >> 2);
                long angle = negMask ^ (long)Qmul30(Qmul30(n0, z) + n1, z) << 2;
                if (x > 0) return angle;
                if (y > 0) return angle + Pi;
                return angle - Pi;
            }
            else
            {
                int z = (int)(Mul(nx, RcpFast(ny)) >> 2);
                long angle = negMask ^ (long)Qmul30(Qmul30(n0, z) + n1, z) << 2;
                return (y > 0) ? (PiHalf - angle) : (-PiHalf - angle);
            }
        }

        public static long Asin(long x)
        {
            Debug.Assert(x >= -One && x <= One);
            return Atan2(x, SqrtFast(Mul(One + x, One - x)));
        }

        public static long Acos(long x)
        {
            Debug.Assert(x >= -One && x <= One);
            return Atan2(SqrtFast(Mul(One + x, One - x)), x);
        }

        public static long Atan(long x)
        {
            // \todo [petri] better implementation?
            return Atan2(x, One);
        }
#if !CPP
    }
}
#endif
