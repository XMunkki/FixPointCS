//
// FixPointCS
//
// Copyright(c) 2018-2019 Jere Sanisalo, Petri Kero
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

// PREFIX
#if CPP
#elif JAVA
package fixpointcs;

import java.lang.Long;
import java.lang.Double;
#else // C#
/* Coding style:
 * 
 * In order to keep the transpiled C++/Java code working, here are some generic
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
 * There is a very limited preprocessor, which accepts "#if <LANG>",
 * "#elif <LANG>", "#else", as well as "#if !TRANSPILE" directives. No nested
 * directives are allowed.
 * 
 * Use up-to C# 3 features to keep the library compatible with older versions
 * of Unity.
 */
using System;
using System.Runtime.CompilerServices;
using System.Diagnostics;
#endif

#if !TRANSPILE
namespace FixPointCS
{
#endif

#if CPP
#else
    /// <summary>
    /// Direct fixed point (signed 32.32) functions.
    /// </summary>
    public static class Fixed64
    {
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

        public const long MinValue = -9223372036854775808L;
        public const long MaxValue = 9223372036854775807L;

        // Private constants
        private const long RCP_LN2      = 0x171547652L; // 1.0 / log(2.0) ~= 1.4426950408889634
        private const long RCP_LOG2_E   = 2977044471L;  // 1.0 / log2(e) ~= 0.6931471805599453
        private const int  RCP_HALF_PI  = 683565276; // 1.0 / (4.0 * 0.5 * Math.PI);  // the 4.0 factor converts directly to s2.30

        /// <summary>
        /// Converts an integer to a fixed-point value.
        /// </summary>
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static long FromInt(int v)
        {
            return (long)v << Shift;
        }

        /// <summary>
        /// Converts a double to a fixed-point value.
        /// </summary>
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static long FromDouble(double v)
        {
            return (long)(v * 4294967296.0);
        }

        /// <summary>
        /// Converts a float to a fixed-point value.
        /// </summary>
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static long FromFloat(float v)
        {
            return (long)(v * 4294967296.0f);
        }

        /// <summary>
        /// Converts a fixed-point value into an integer by rounding it up to nearest integer.
        /// </summary>
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static int CeilToInt(long v)
        {
            return (int)((v + (One - 1)) >> Shift);
        }

        /// <summary>
        /// Converts a fixed-point value into an integer by rounding it down to nearest integer.
        /// </summary>
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static int FloorToInt(long v)
        {
            return (int)(v >> Shift);
        }

        /// <summary>
        /// Converts a fixed-point value into an integer by rounding it to nearest integer.
        /// </summary>
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static int RoundToInt(long v)
        {
            return (int)((v + Half) >> Shift);
        }

        /// <summary>
        /// Converts a fixed-point value into a double.
        /// </summary>
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static double ToDouble(long v)
        {
            return (double)v * (1.0 / 4294967296.0);
        }

        /// <summary>
        /// Converts a FP value into a float.
        /// </summary>
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static float ToFloat(long v)
        {
            return (float)v * (1.0f / 4294967296.0f);
        }

        /// <summary>
        /// Converts the value to a human readable string.
        /// </summary>
#if CPP
#elif JAVA
        public static String ToString(long v)
        {
            return Double.toString(ToDouble(v));
        }
#else
        public static string ToString(long v)
        {
            return ToDouble(v).ToString();
        }
#endif

        /// <summary>
        /// Returns the absolute (positive) value of x.
        /// </summary>
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static long Abs(long x)
        {
            // \note fails with LONG_MIN
            // \note for some reason this is twice as fast as (x > 0) ? x : -x
            return (x < 0) ? -x : x;
        }

        /// <summary>
        /// Negative absolute value (returns -abs(x)).
        /// </summary>
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static long Nabs(long x)
        {
            return (x > 0) ? -x : x;
        }

        /// <summary>
        /// Round up to nearest integer.
        /// </summary>
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static long Ceil(long x)
        {
            return (x + FractionMask) & IntegerMask;
        }

        /// <summary>
        /// Round down to nearest integer.
        /// </summary>
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static long Floor(long x)
        {
            return x & IntegerMask;
        }

        /// <summary>
        /// Round to nearest integer.
        /// </summary>
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static long Round(long x)
        {
            return (x + Half) & IntegerMask;
        }

        /// <summary>
        /// Returns the fractional part of x. Equal to 'x - floor(x)'.
        /// </summary>
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static long Fract(long x)
        {
            return x & FractionMask;
        }

        /// <summary>
        /// Returns the minimum of the two values.
        /// </summary>
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static long Min(long a, long b)
        {
            return (a < b) ? a : b;
        }

        /// <summary>
        /// Returns the maximum of the two values.
        /// </summary>
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static long Max(long a, long b)
        {
            return (a > b) ? a : b;
        }

        /// <summary>
        /// Returns the value clamped between min and max.
        /// </summary>
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static long Clamp(long a, long min, long max)
        {
            return (a > max) ? max : (a < min) ? min : a;
        }

        /// <summary>
        /// Linearly interpolate from a to b by t.
        /// </summary>
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static long Lerp(long a, long b, long t)
        {
            return Mul(a, t) + Mul(b, One - t);
        }

        /// <summary>
        /// Returns the sign of the value (-1 if negative, 0 if zero, 1 if positive).
        /// </summary>
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static int Sign(long x)
        {
            if (x == 0) return 0;
            return (x < 0) ? -1 : 1;
        }

        /// <summary>
        /// Adds the two FP numbers together.
        /// </summary>
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static long Add(long a, long b)
        {
            return a + b;
        }

        /// <summary>
        /// Subtracts the two FP numbers from each other.
        /// </summary>
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static long Sub(long a, long b)
        {
            return a - b;
        }

        /// <summary>
        /// Multiplies two FP values together.
        /// </summary>
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static long Mul(long a, long b)
        {
            long ai = a >> Shift;
            long af = (a & FractionMask);
            long bi = b >> Shift;
            long bf = (b & FractionMask);
            return FixedUtil.LogicalShiftRight(af * bf, Shift) + ai * b + af * bi;
        }

        [MethodImpl(FixedUtil.AggressiveInlining)]
        private static int MulIntLongLow(int a, long b)
        {
            Debug.Assert(a >= 0);
            int bi = (int)(b >> Shift);
            long bf = b & FractionMask;
            return (int)FixedUtil.LogicalShiftRight(a * bf, Shift) + a * bi;
        }

        [MethodImpl(FixedUtil.AggressiveInlining)]
        private static long MulIntLongLong(int a, long b)
        {
            Debug.Assert(a >= 0);
            long bi = b >> Shift;
            long bf = b & FractionMask;
            return FixedUtil.LogicalShiftRight(a * bf, Shift) + a * bi;
        }

#if JAVA
        private static int Nlz(long x)
        {
            return Long.numberOfLeadingZeros(x);
        }
#else
        [MethodImpl(FixedUtil.AggressiveInlining)]
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
#endif

        /// <summary>
        /// Divides two FP values.
        /// </summary>
        public static long DivPrecise(long arg_a, long arg_b)
        {
            // From http://www.hackersdelight.org/hdcodetxt/divlu.c.txt

#if JAVA
            long sign_dif = arg_a ^ arg_b;

            const long b = 0x100000000L; // Number base (32 bits)
            long abs_arg_a = (arg_a < 0) ? -arg_a : arg_a;
            long u1 = abs_arg_a >>> 32;
            long u0 = abs_arg_a << 32;
            long v = (arg_b < 0) ? -arg_b : arg_b;

            // Overflow?
            if (Long.compareUnsigned(u1, v) >= 0) // u1 >= v
            {
                //rem = 0;
                return 0x7fffffffffffffffL;
            }

            // Shift amount for norm
            int s = Nlz(v); // 0 <= s <= 63
            v = v << s; // Normalize the divisor
            long vn1 = v >>> 32; // Break the divisor into two 32-bit digits
            long vn0 = v & 0xffffffffL;

            long un32 = (u1 << s) | (u0 >>> (64 - s)) & (-(long)s >> 63);
            long un10 = u0 << s; // Shift dividend left

            long un1 = un10 >>> 32; // Break the right half of dividend into two digits
            long un0 = un10 & 0xffffffffL;

            // Compute the first quotient digit, q1
            long q1 = Long.divideUnsigned(un32, vn1);
            long rhat = un32 - q1 * vn1;
            do
            {
                if ((Long.compareUnsigned(q1, b) >= 0) || (Long.compareUnsigned(q1 * vn0, b * rhat + un1) > 0))
                {
                    q1 = q1 - 1;
                    rhat = rhat + vn1;
                }
                else break;
            } while (Long.compareUnsigned(rhat, b) < 0);

            long un21 = un32 * b + un1 - q1 * v; // Multiply and subtract

            // Compute the second quotient digit, q0
            long q0 = Long.divideUnsigned(un21, vn1);
            rhat = un21 - q0 * vn1;
            do
            {
                if ((Long.compareUnsigned(q0, b) >= 0) || (Long.compareUnsigned(q0 * vn0, b * rhat + un0) > 0))
                {
                    q0 = q0 - 1;
                    rhat = rhat + vn1;
                }
                else break;
            } while (Long.compareUnsigned(rhat, b) < 0);

            // Calculate the remainder
            // ulong r = (un21 * b + un0 - q0 * v) >>> s;
            // rem = (long)r;

            long ret = q1 * b + q0;
            return (sign_dif < 0) ? -ret : ret;
#else
            long sign_dif = arg_a ^ arg_b;

            const ulong b = 0x100000000L; // Number base (32 bits)
            ulong abs_arg_a = (ulong)((arg_a < 0) ? -arg_a : arg_a);
            ulong u1 = abs_arg_a >> 32;
            ulong u0 = abs_arg_a << 32;
            ulong v = (ulong)((arg_b < 0) ? -arg_b : arg_b);

            // Overflow?
            if (u1 >= v)
            {
                //rem = 0;
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
                }
                else break;
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
                }
                else break;
            } while (rhat < b);

            // Calculate the remainder
            // ulong r = (un21 * b + un0 - q0 * v) >> s;
            // rem = (long)r;

            ulong ret = q1 * b + q0;
            return (sign_dif < 0) ? -(long)ret : (long)ret;
#endif
        }

        /// <summary>
        /// Calculates division approximation.
        /// </summary>
        public static long Div(long a, long b)
        {
            if (b == MinValue)
                return 0;

            // Handle negative values.
            int sign = (b < 0) ? -1 : 1;
            b *= sign;

            // Normalize input into [1.0, 2.0( range (convert to s2.30).
            int offset = 31 - Nlz((ulong)b);
            int n = (int)FixedUtil.ShiftRight(b, offset + 2);
            const int ONE = (1 << 30);
            Debug.Assert(n >= ONE);

            // Polynomial approximation.
            int res = FixedUtil.RcpPoly4Lut8(n - ONE);

            // Apply exponent, convert back to s32.32.
            long y = MulIntLongLong(res, a) << 2;
            return FixedUtil.ShiftRight(sign * y, offset);
        }

        /// <summary>
        /// Calculates division approximation.
        /// </summary>
        public static long DivFast(long a, long b)
        {
            if (b == MinValue)
                return 0;

            // Handle negative values.
            int sign = (b < 0) ? -1 : 1;
            b *= sign;

            // Normalize input into [1.0, 2.0( range (convert to s2.30).
            int offset = 31 - Nlz((ulong)b);
            int n = (int)FixedUtil.ShiftRight(b, offset + 2);
            const int ONE = (1 << 30);
            Debug.Assert(n >= ONE);

            // Polynomial approximation.
            int res = FixedUtil.RcpPoly6(n - ONE);

            // Apply exponent, convert back to s32.32.
            long y = MulIntLongLong(res, a) << 2;
            return FixedUtil.ShiftRight(sign * y, offset);
        }

        /// <summary>
        /// Calculates division approximation.
        /// </summary>
        public static long DivFastest(long a, long b)
        {
            if (b == MinValue)
                return 0;

            // Handle negative values.
            int sign = (b < 0) ? -1 : 1;
            b *= sign;

            // Normalize input into [1.0, 2.0( range (convert to s2.30).
            int offset = 31 - Nlz((ulong)b);
            int n = (int)FixedUtil.ShiftRight(b, offset + 2);
            const int ONE = (1 << 30);
            Debug.Assert(n >= ONE);

            // Polynomial approximation.
            int res = FixedUtil.RcpPoly4(n - ONE);

            // Apply exponent, convert back to s32.32.
            long y = MulIntLongLong(res, a) << 2;
            return FixedUtil.ShiftRight(sign * y, offset);
        }

        /// <summary>
        /// Divides two FP values and returns the modulus.
        /// </summary>
        public static long Mod(long a, long b)
        {
            long di = a / b;
            long ret = a - (di * b);
            return ret;
        }

        /// <summary>
        /// Calculates the square root of the given number.
        /// </summary>
        public static long SqrtPrecise(long a)
        {
            // Adapted from https://github.com/chmike/fpsqrt
            if (a < 0)
                return -1;

#if JAVA
            long r = a;
            long b = 0x4000000000000000L;
            long q = 0;
            while (b > 0x40)
            {
                long t = q + b;
                if (Long.compareUnsigned(r, t) >= 0)
                {
                    r -= t;
                    q = t + b;
                }
                r <<= 1;
                b >>= 1;
            }
            q >>>= 16;
            return q;
#else
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
#endif
        }

        public static long Sqrt(long x)
        {
            // Return 0 for all non-positive values.
            if (x <= 0)
                return 0;

            // Constants (s2.30).
            const int ONE = (1 << 30);
            const int SQRT2 = 1518500249; // sqrt(2.0)

            // Normalize input into [1.0, 2.0( range (as s2.30).
            int offset = 31 - Nlz((ulong)x);
            int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
            Debug.Assert(n >= ONE);
            int y = FixedUtil.SqrtPoly3Lut8(n - ONE);

            // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
            int adjust = ((offset & 1) != 0) ? SQRT2 : ONE;
            offset = offset >> 1;

            // Apply exponent, convert back to s32.32.
            long yr = (long)FixedUtil.Qmul30(adjust, y) << 2;
            return (offset >= 0) ? (yr << offset) : (yr >> -offset);
        }

        public static long SqrtFast(long x)
        {
            // Return 0 for all non-positive values.
            if (x <= 0)
                return 0;

            // Constants (s2.30).
            const int ONE = (1 << 30);
            const int SQRT2 = 1518500249; // sqrt(2.0)

            // Normalize input into [1.0, 2.0( range (as s2.30).
            int offset = 31 - Nlz((ulong)x);
            int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
            Debug.Assert(n >= ONE);
            int y = FixedUtil.SqrtPoly4(n - ONE);

            // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
            int adjust = ((offset & 1) != 0) ? SQRT2 : ONE;
            offset = offset >> 1;

            // Apply exponent, convert back to s32.32.
            long yr = (long)FixedUtil.Qmul30(adjust, y) << 2;
            return (offset >= 0) ? (yr << offset) : (yr >> -offset);
        }

        public static long SqrtFastest(long x)
        {
            // Return 0 for all non-positive values.
            if (x <= 0)
                return 0;

            // Constants (s2.30).
            const int ONE = (1 << 30);
            const int SQRT2 = 1518500249; // sqrt(2.0)

            // Normalize input into [1.0, 2.0( range (as s2.30).
            int offset = 31 - Nlz((ulong)x);
            int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
            Debug.Assert(n >= ONE);
            int y = FixedUtil.SqrtPoly3(n - ONE);

            // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
            int adjust = ((offset & 1) != 0) ? SQRT2 : ONE;
            offset = offset >> 1;

            // Apply exponent, convert back to s32.32.
            long yr = (long)FixedUtil.Qmul30(adjust, y) << 2;
            return (offset >= 0) ? (yr << offset) : (yr >> -offset);
        }

        /// <summary>
        /// Calculates the reciprocal square root.
        /// </summary>
        public static long RSqrt(long x)
        {
            // Return 0 for invalid values
            if (x <= 0)
                return 0;

            // Constants (s2.30).
            const int ONE = (1 << 30);
            const int HALF_SQRT2 = 759250125; // 0.5 * sqrt(2.0)

            // Normalize input into [1.0, 2.0( range (as s2.30).
            int offset = 31 - Nlz((ulong)x);
            int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
            Debug.Assert(n >= ONE);
            int y = FixedUtil.RSqrtPoly3Lut16(n - ONE);

            // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
            int adjust = ((offset & 1) != 0) ? HALF_SQRT2 : ONE;
            offset = offset >> 1;

            // Apply exponent, convert back to s32.32.
            long yr = (long)FixedUtil.Qmul30(adjust, y) << 2;
            return (offset >= 0) ? (yr >> offset) : (yr << -offset);
        }

        /// <summary>
        /// Calculates the reciprocal square root.
        /// </summary>
        public static long RSqrtFast(long x)
        {
            // Return 0 for invalid values
            if (x <= 0)
                return 0;

            // Constants (s2.30).
            const int ONE = (1 << 30);
            const int HALF_SQRT2 = 759250125; // 0.5 * sqrt(2.0)

            // Normalize input into [1.0, 2.0( range (as s2.30).
            int offset = 31 - Nlz((ulong)x);
            int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
            Debug.Assert(n >= ONE);
            int y = FixedUtil.RSqrtPoly5(n - ONE);

            // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
            int adjust = ((offset & 1) != 0) ? HALF_SQRT2 : ONE;
            offset = offset >> 1;

            // Apply exponent, convert back to s32.32.
            long yr = (long)FixedUtil.Qmul30(adjust, y) << 2;
            return (offset >= 0) ? (yr >> offset) : (yr << -offset);
        }

        /// <summary>
        /// Calculates the reciprocal square root.
        /// </summary>
        public static long RSqrtFastest(long x)
        {
            // Return 0 for invalid values
            if (x <= 0)
                return 0;

            // Constants (s2.30).
            const int ONE = (1 << 30);
            const int HALF_SQRT2 = 759250125; // 0.5 * sqrt(2.0)

            // Normalize input into [1.0, 2.0( range (as s2.30).
            int offset = 31 - Nlz((ulong)x);
            int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
            Debug.Assert(n >= ONE);
            int y = FixedUtil.RSqrtPoly3(n - ONE);

            // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
            int adjust = ((offset & 1) != 0) ? HALF_SQRT2 : ONE;
            offset = offset >> 1;

            // Apply exponent, convert back to s32.32.
            long yr = (long)FixedUtil.Qmul30(adjust, y) << 2;
            return (offset >= 0) ? (yr >> offset) : (yr << -offset);
        }

        /// <summary>
        /// Calculates reciprocal approximation.
        /// </summary>
        public static long Rcp(long x)
        {
            if (x == MinValue || x == 0)
                return 0;

            // Handle negative values.
            int sign = (x < 0) ? -1 : 1;
            x *= sign;

            // Normalize input into [1.0, 2.0( range (convert to s2.30).
            int offset = 31 - Nlz((ulong)x);
            int n = (int)FixedUtil.ShiftRight(x, offset + 2);
            const int ONE = (1 << 30);
            Debug.Assert(n >= ONE);

            // Polynomial approximation.
            int res = FixedUtil.RcpPoly4Lut8(n - ONE);
            long y = (long)(sign * res) << 2;

            // Apply exponent, convert back to s32.32.
            return FixedUtil.ShiftRight(y, offset);
        }

        /// <summary>
        /// Calculates reciprocal approximation.
        /// </summary>
        public static long RcpFast(long x)
        {
            if (x == MinValue || x == 0)
                return 0;

            // Handle negative values.
            int sign = (x < 0) ? -1 : 1;
            x *= sign;

            // Normalize input into [1.0, 2.0( range (convert to s2.30).
            int offset = 31 - Nlz((ulong)x);
            int n = (int)FixedUtil.ShiftRight(x, offset + 2);
            const int ONE = (1 << 30);
            Debug.Assert(n >= ONE);

            // Polynomial approximation.
            int res = FixedUtil.RcpPoly6(n - ONE);
            long y = (long)(sign * res) << 2;

            // Apply exponent, convert back to s32.32.
            return FixedUtil.ShiftRight(y, offset);
        }

        /// <summary>
        /// Calculates reciprocal approximation.
        /// </summary>
        public static long RcpFastest(long x)
        {
            if (x == MinValue || x == 0)
                return 0;

            // Handle negative values.
            int sign = (x < 0) ? -1 : 1;
            x *= sign;

            // Normalize input into [1.0, 2.0( range (convert to s2.30).
            const int ONE = (1 << 30);
            int offset = 31 - Nlz((ulong)x);
            int n = (int)FixedUtil.ShiftRight(x, offset + 2);
            //int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);

            // Polynomial approximation.
            int res = FixedUtil.RcpPoly4(n - ONE);
            long y = (long)(sign * res) << 2;

            // Apply exponent, convert back to s32.32.
            return FixedUtil.ShiftRight(y, offset);
        }

        /// <summary>
        /// Calculates the base 2 exponent.
        /// </summary>
        public static long Exp2(long x)
        {
            // Handle values that would under or overflow.
            if (x >= 32 * One) return MaxValue;
            if (x <= -32 * One) return 0;

            // Compute exp2 for fractional part.
            int k = (int)((x & FractionMask) >> 2);
            long y = (long)FixedUtil.Exp2Poly5(k) << 2;

            // Combine integer and fractional result, and convert back to s32.32.
            int intPart = (int)(x >> Shift);
            return (intPart >= 0) ? (y << intPart) : (y >> -intPart);
        }

        /// <summary>
        /// Calculates the base 2 exponent.
        /// </summary>
        public static long Exp2Fast(long x)
        {
            // Handle values that would under or overflow.
            if (x >= 32 * One) return MaxValue;
            if (x <= -32 * One) return 0;

            // Compute exp2 for fractional part.
            int k = (int)((x & FractionMask) >> 2);
            long y = (long)FixedUtil.Exp2Poly4(k) << 2;

            // Combine integer and fractional result, and convert back to s32.32.
            int intPart = (int)(x >> Shift);
            return (intPart >= 0) ? (y << intPart) : (y >> -intPart);
        }

        /// <summary>
        /// Calculates the base 2 exponent.
        /// </summary>
        public static long Exp2Fastest(long x)
        {
            // Handle values that would under or overflow.
            if (x >= 32 * One) return MaxValue;
            if (x <= -32 * One) return 0;

            // Compute exp2 for fractional part.
            int k = (int)((x & FractionMask) >> 2);
            long y = (long)FixedUtil.Exp2Poly3(k) << 2;

            // Combine integer and fractional result, and convert back to s32.32.
            int intPart = (int)(x >> Shift);
            return (intPart >= 0) ? (y << intPart) : (y >> -intPart);
        }

        public static long Exp(long x)
        {
            // e^x == 2^(x / ln(2))
            return Exp2(Mul(x, RCP_LN2));
        }

        public static long ExpFast(long x)
        {
            // e^x == 2^(x / ln(2))
            return Exp2Fast(Mul(x, RCP_LN2));
        }

        public static long ExpFastest(long x)
        {
            // e^x == 2^(x / ln(2))
            return Exp2Fastest(Mul(x, RCP_LN2));
        }

        // Natural logarithm (base e).
        public static long Log(long x)
        {
            // Return 0 for invalid values
            if (x <= 0)
                return 0;

            // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
            const int ONE = (1 << 30);
            int offset = 31 - Nlz((ulong)x);
            int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
            Debug.Assert(n >= ONE);
            long y = (long)FixedUtil.LogPoly5Lut8(n - ONE) << 2;

            // Combine integer and fractional parts (into s32.32).
            return (long)offset * RCP_LOG2_E + y;
        }

        public static long LogFast(long x)
        {
            // Return 0 for invalid values
            if (x <= 0)
                return 0;

            // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
            const int ONE = (1 << 30);
            int offset = 31 - Nlz((ulong)x);
            int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
            Debug.Assert(n >= ONE);
            long y = (long)FixedUtil.LogPoly3Lut8(n - ONE) << 2;

            // Combine integer and fractional parts (into s32.32).
            return (long)offset * RCP_LOG2_E + y;
        }

        public static long LogFastest(long x)
        {
            // Return 0 for invalid values
            if (x <= 0)
                return 0;

            // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
            const int ONE = (1 << 30);
            int offset = 31 - Nlz((ulong)x);
            int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
            Debug.Assert(n >= ONE);
            long y = (long)FixedUtil.LogPoly5(n - ONE) << 2;

            // Combine integer and fractional parts (into s32.32).
            return (long)offset * RCP_LOG2_E + y;
        }

        public static long Log2(long x)
        {
            // Return 0 for invalid values
            if (x <= 0)
                return 0;

            // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
            int offset = 31 - Nlz((ulong)x);
            int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);

            // Polynomial approximation of mantissa.
            const int ONE = (1 << 30);
            Debug.Assert(n >= ONE);
            long y = (long)FixedUtil.Log2Poly4Lut16(n - ONE) << 2;

            // Combine integer and fractional parts (into s32.32).
            return ((long)offset << Shift) + y;
        }

        public static long Log2Fast(long x)
        {
            // Return 0 for invalid values
            if (x <= 0)
                return 0;

            // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
            int offset = 31 - Nlz((ulong)x);
            int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);

            // Polynomial approximation of mantissa.
            const int ONE = (1 << 30);
            Debug.Assert(n >= ONE);
            long y = (long)FixedUtil.Log2Poly3Lut16(n - ONE) << 2;

            // Combine integer and fractional parts (into s32.32).
            return ((long)offset << Shift) + y;
        }

        public static long Log2Fastest(long x)
        {
            // Return 0 for invalid values
            if (x <= 0)
                return 0;

            // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
            int offset = 31 - Nlz((ulong)x);
            int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);

            // Polynomial approximation of mantissa.
            const int ONE = (1 << 30);
            Debug.Assert(n >= ONE);
            long y = (long)FixedUtil.Log2Poly5(n - ONE) << 2;

            // Combine integer and fractional parts (into s32.32).
            return ((long)offset << Shift) + y;
        }

        /// <summary>
        /// Calculates x to the power of the exponent.
        /// </summary>
        public static long Pow(long x, long exponent)
        {
            // Return 0 for invalid values
            if (x <= 0)
                return 0;

            return Exp(Mul(exponent, Log(x)));
        }

        /// <summary>
        /// Calculates x to the power of the exponent.
        /// </summary>
        public static long PowFast(long x, long exponent)
        {
            // Return 0 for invalid values
            if (x <= 0)
                return 0;

            return ExpFast(Mul(exponent, LogFast(x)));
        }

        /// <summary>
        /// Calculates x to the power of the exponent.
        /// </summary>
        public static long PowFastest(long x, long exponent)
        {
            // Return 0 for invalid values
            if (x <= 0)
                return 0;

            return ExpFastest(Mul(exponent, LogFastest(x)));
        }

        [MethodImpl(FixedUtil.AggressiveInlining)]
        private static int UnitSin(int z)
        {
            // See: http://www.coranac.com/2009/07/sines/

            // Handle quadrants 1 and 2 by mirroring the [1, 3] range to [-1, 1] (by calculating 2 - z).
            // The if condition uses the fact that for the quadrants of interest are 0b01 and 0b10 (top two bits are different).
            if ((z ^ (z << 1)) < 0)
                z = (1 << 31) - z;

            // Now z is in range [-1, 1].
            const int ONE = (1 << 30);
            Debug.Assert((z >= -ONE) && (z <= ONE));

            // Polynomial approximation.
            int zz = FixedUtil.Qmul30(z, z);
            int res = FixedUtil.Qmul30(FixedUtil.SinPoly4(zz), z);

            // Return s2.30 value.
            return res;
        }

        [MethodImpl(FixedUtil.AggressiveInlining)]
        private static int UnitSinFast(int z)
        {
            // See: http://www.coranac.com/2009/07/sines/

            // Handle quadrants 1 and 2 by mirroring the [1, 3] range to [-1, 1] (by calculating 2 - z).
            // The if condition uses the fact that for the quadrants of interest are 0b01 and 0b10 (top two bits are different).
            if ((z ^ (z << 1)) < 0)
                z = (1 << 31) - z;

            // Now z is in range [-1, 1].
            const int ONE = (1 << 30);
            Debug.Assert((z >= -ONE) && (z <= ONE));

            // Polynomial approximation.
            int zz = FixedUtil.Qmul30(z, z);
            int res = FixedUtil.Qmul30(FixedUtil.SinPoly3(zz), z);

            // Return s2.30 value.
            return res;
        }

        [MethodImpl(FixedUtil.AggressiveInlining)]
        private static int UnitSinFastest(int z)
        {
            // See: http://www.coranac.com/2009/07/sines/

            // Handle quadrants 1 and 2 by mirroring the [1, 3] range to [-1, 1] (by calculating 2 - z).
            // The if condition uses the fact that for the quadrants of interest are 0b01 and 0b10 (top two bits are different).
            if ((z ^ (z << 1)) < 0)
                z = (1 << 31) - z;

            // Now z is in range [-1, 1].
            const int ONE = (1 << 30);
            Debug.Assert((z >= -ONE) && (z <= ONE));

            // Polynomial approximation.
            int zz = FixedUtil.Qmul30(z, z);
            int res = FixedUtil.Qmul30(FixedUtil.SinPoly2(zz), z);

            // Return s2.30 value.
            return res;
        }

        public static long Sin(long x)
        {
            // Map [0, 2pi] to [0, 4] (as s2.30).
            // This also wraps the values into one period.
            int z = MulIntLongLow(RCP_HALF_PI, x);

            // Compute sine and convert to s32.32.
            return (long)UnitSin(z) << 2;
        }

        public static long SinFast(long x)
        {
            // Map [0, 2pi] to [0, 4] (as s2.30).
            // This also wraps the values into one period.
            int z = MulIntLongLow(RCP_HALF_PI, x);

            // Compute sine and convert to s32.32.
            return (long)UnitSinFast(z) << 2;
        }

        public static long SinFastest(long x)
        {
            // Map [0, 2pi] to [0, 4] (as s2.30).
            // This also wraps the values into one period.
            int z = MulIntLongLow(RCP_HALF_PI, x);

            // Compute sine and convert to s32.32.
            return (long)UnitSinFastest(z) << 2;
        }

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static long Cos(long x)
        {
            return Sin(x + PiHalf);
        }

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static long CosFast(long x)
        {
            return SinFast(x + PiHalf);
        }

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static long CosFastest(long x)
        {
            return SinFastest(x + PiHalf);
        }

        public static long Tan(long x)
        {
            int z = MulIntLongLow(RCP_HALF_PI, x);
            long sinX = (long)UnitSin(z) << 32;
            long cosX = (long)UnitSin(z + (1 << 30)) << 32;
            return Div(sinX, cosX);
        }

        public static long TanFast(long x)
        {
            int z = MulIntLongLow(RCP_HALF_PI, x);
            long sinX = (long)UnitSinFast(z) << 32;
            long cosX = (long)UnitSinFast(z + (1 << 30)) << 32;
            return DivFast(sinX, cosX);
        }

        public static long TanFastest(long x)
        {
            int z = MulIntLongLow(RCP_HALF_PI, x);
            long sinX = (long)UnitSinFastest(z) << 32;
            long cosX = (long)UnitSinFastest(z + (1 << 30)) << 32;
            return DivFastest(sinX, cosX);
        }

        private static int Atan2Div(long y, long x)
        {
            Debug.Assert(y >= 0 && x > 0 && x >= y);

            // Normalize input into [1.0, 2.0( range (convert to s2.30).
            const int ONE = (1 << 30);
            const int HALF = (1 << 29);
            int offset = 31 - Nlz((ulong)x);
            int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
            int k = n - ONE;

            // Polynomial approximation of reciprocal.
            int oox = FixedUtil.RcpPoly4Lut8(k);
            Debug.Assert(oox >= HALF && oox <= ONE);

            // Apply exponent and multiply.
            long yr = (offset >= 0) ? (y >> offset) : (y << -offset);
            return FixedUtil.Qmul30((int)(yr >> 2), oox);
        }

        public static long Atan2(long y, long x)
        {
            // See: https://www.dsprelated.com/showarticle/1052.php

            if (x == 0)
            {
                if (y > 0) return PiHalf;
                if (y < 0) return -PiHalf;
                return 0;
            }

            // \note these round negative numbers slightly
            long nx = x ^ (x >> 63);
            long ny = y ^ (y >> 63);
            long negMask = ((x ^ y) >> 63);

            if (nx >= ny)
            {
                int k = Atan2Div(ny, nx);
                int z = FixedUtil.AtanPoly5Lut8(k);
                long angle = negMask ^ ((long)z << 2);
                if (x > 0) return angle;
                if (y >= 0) return angle + Pi;
                return angle - Pi;
            }
            else
            {
                int k = Atan2Div(nx, ny);
                int z = FixedUtil.AtanPoly5Lut8(k);
                long angle = negMask ^ ((long)z << 2);
                return ((y > 0) ? PiHalf : -PiHalf) - angle;
            }
        }

        private static int Atan2DivFast(long y, long x)
        {
            Debug.Assert(y >= 0 && x > 0 && x >= y);

            // Normalize input into [1.0, 2.0( range (convert to s2.30).
            const int ONE = (1 << 30);
            const int HALF = (1 << 29);
            int offset = 31 - Nlz((ulong)x);
            int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
            int k = n - ONE;

            // Polynomial approximation.
            int oox = FixedUtil.RcpPoly6(k);
            Debug.Assert(oox >= HALF && oox <= ONE);

            // Apply exponent and multiply.
            long yr = (offset >= 0) ? (y >> offset) : (y << -offset);
            return FixedUtil.Qmul30((int)(yr >> 2), oox);
        }

        public static long Atan2Fast(long y, long x)
        {
            // See: https://www.dsprelated.com/showarticle/1052.php

            if (x == 0)
            {
                if (y > 0) return PiHalf;
                if (y < 0) return -PiHalf;
                return 0;
            }

            // \note these round negative numbers slightly
            long nx = x ^ (x >> 63);
            long ny = y ^ (y >> 63);
            long negMask = ((x ^ y) >> 63);

            if (nx >= ny)
            {
                int k = Atan2DivFast(ny, nx);
                int z = FixedUtil.AtanPoly3Lut8(k);
                long angle = negMask ^ ((long)z << 2);
                if (x > 0) return angle;
                if (y >= 0) return angle + Pi;
                return angle - Pi;
            }
            else
            {
                int k = Atan2DivFast(nx, ny);
                int z = FixedUtil.AtanPoly3Lut8(k);
                long angle = negMask ^ ((long)z << 2);
                return ((y > 0) ? PiHalf : -PiHalf) - angle;
            }
        }

        private static int Atan2DivFastest(long y, long x)
        {
            Debug.Assert(y >= 0 && x > 0 && x >= y);

            // Normalize input into [1.0, 2.0( range (convert to s2.30).
            const int ONE = (1 << 30);
            const int HALF = (1 << 29);
            int offset = 31 - Nlz((ulong)x);
            int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
            int k = n - ONE;

            // Polynomial approximation.
            int oox = FixedUtil.RcpPoly4(k);
            Debug.Assert(oox >= HALF && oox <= ONE);

            // Apply exponent and multiply.
            long yr = (offset >= 0) ? (y >> offset) : (y << -offset);
            return FixedUtil.Qmul30((int)(yr >> 2), oox);
        }

        public static long Atan2Fastest(long y, long x)
        {
            // See: https://www.dsprelated.com/showarticle/1052.php

            if (x == 0)
            {
                if (y > 0) return PiHalf;
                if (y < 0) return -PiHalf;
                return 0;
            }

            // \note these round negative numbers slightly
            long nx = x ^ (x >> 63);
            long ny = y ^ (y >> 63);
            long negMask = ((x ^ y) >> 63);

            if (nx >= ny)
            {
                int z = Atan2DivFastest(ny, nx);
                int res = FixedUtil.AtanPoly4(z);
                long angle = negMask ^ ((long)res << 2);
                if (x > 0) return angle;
                if (y >= 0) return angle + Pi;
                return angle - Pi;
            }
            else
            {
                int z = Atan2DivFastest(nx, ny);
                int res = FixedUtil.AtanPoly4(z);
                long angle = negMask ^ ((long)res << 2);
                return ((y > 0) ? PiHalf : -PiHalf) - angle;
            }
        }

        public static long Asin(long x)
        {
            // Return 0 for invalid values
            if (x < -One || x > One)
                return 0;

            return Atan2(x, Sqrt(Mul(One + x, One - x)));
        }

        public static long AsinFast(long x)
        {
            // Return 0 for invalid values
            if (x < -One || x > One)
                return 0;

            return Atan2Fast(x, SqrtFast(Mul(One + x, One - x)));
        }

        public static long AsinFastest(long x)
        {
            // Return 0 for invalid values
            if (x < -One || x > One)
                return 0;

            return Atan2Fastest(x, SqrtFastest(Mul(One + x, One - x)));
        }

        public static long Acos(long x)
        {
            // Return 0 for invalid values
            if (x < -One || x > One)
                return 0;

            return Atan2(Sqrt(Mul(One + x, One - x)), x);
        }

        public static long AcosFast(long x)
        {
            // Return 0 for invalid values
            if (x < -One || x > One)
                return 0;

            return Atan2Fast(SqrtFast(Mul(One + x, One - x)), x);
        }

        public static long AcosFastest(long x)
        {
            // Return 0 for invalid values
            if (x < -One || x > One)
                return 0;

            return Atan2Fastest(SqrtFastest(Mul(One + x, One - x)), x);
        }

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static long Atan(long x)
        {
            return Atan2(x, One);
        }

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static long AtanFast(long x)
        {
            return Atan2Fast(x, One);
        }

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static long AtanFastest(long x)
        {
            return Atan2Fastest(x, One);
        }
#if CPP
#else
    } // Fixed64
#endif

#if !TRANSPILE
} // namespace
#endif

// SUFFIX
