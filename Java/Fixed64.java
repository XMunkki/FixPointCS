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

//
// GENERATED FILE!!!
//
// Generated from Fixed64.cs, part of the FixPointCS project (MIT license).
//

package fixpointcs;

import java.lang.Long;
import java.lang.Double;


/// <summary>
/// Direct fixed point (signed 32.32) functions.
/// </summary>
public class Fixed64
{
    public static final int Shift = 32;
    public static final long FractionMask = ( 1L << Shift ) - 1; // Space before 1L needed because of hacky C++ code generator
    public static final long IntegerMask = ~FractionMask;

    // Constants
    public static final long Zero = 0L;
    public static final long Neg1 = -1L << Shift;
    public static final long One = 1L << Shift;
    public static final long Two = 2L << Shift;
    public static final long Three = 3L << Shift;
    public static final long Four = 4L << Shift;
    public static final long Half = One >> 1;
    public static final long Pi = 13493037705L; //(long)(Math.PI * 65536.0) << 16;
    public static final long Pi2 = 26986075409L;
    public static final long PiHalf = 6746518852L;
    public static final long E = 11674931555L;

    public static final long MinValue = -9223372036854775808L;
    public static final long MaxValue = 9223372036854775807L;

    // Private constants
    private static final long RCP_LN2      = 0x171547652L; // 1.0 / log(2.0) ~= 1.4426950408889634
    private static final long RCP_LOG2_E   = 2977044471L;  // 1.0 / log2(e) ~= 0.6931471805599453
    private static final int  RCP_HALF_PI  = 683565276; // 1.0 / (4.0 * 0.5 * Math.PI);  // the 4.0 factor converts directly to s2.30

    /// <summary>
    /// Converts an integer to a fixed-point value.
    /// </summary>
    public static long FromInt(int v)
    {
        return (long)v << Shift;
    }

    /// <summary>
    /// Converts a double to a fixed-point value.
    /// </summary>
    public static long FromDouble(double v)
    {
        return (long)(v * 4294967296.0);
    }

    /// <summary>
    /// Converts a float to a fixed-point value.
    /// </summary>
    public static long FromFloat(float v)
    {
        return (long)(v * 4294967296.0f);
    }

    /// <summary>
    /// Converts a fixed-point value into an integer by rounding it up to nearest integer.
    /// </summary>
    public static int CeilToInt(long v)
    {
        return (int)((v + (One - 1)) >> Shift);
    }

    /// <summary>
    /// Converts a fixed-point value into an integer by rounding it down to nearest integer.
    /// </summary>
    public static int FloorToInt(long v)
    {
        return (int)(v >> Shift);
    }

    /// <summary>
    /// Converts a fixed-point value into an integer by rounding it to nearest integer.
    /// </summary>
    public static int RoundToInt(long v)
    {
        return (int)((v + Half) >> Shift);
    }

    /// <summary>
    /// Converts a fixed-point value into a double.
    /// </summary>
    public static double ToDouble(long v)
    {
        return (double)v * (1.0 / 4294967296.0);
    }

    /// <summary>
    /// Converts a FP value into a float.
    /// </summary>
    public static float ToFloat(long v)
    {
        return (float)v * (1.0f / 4294967296.0f);
    }

    /// <summary>
    /// Converts the value to a human readable string.
    /// </summary>
    public static String ToString(long v)
    {
        return Double.toString(ToDouble(v));
    }

    /// <summary>
    /// Returns the absolute (positive) value of x.
    /// </summary>
    public static long Abs(long x)
    {
        // \note fails with LONG_MIN
        long mask = x >> 63;
        return (x + mask) ^ mask;
    }

    /// <summary>
    /// Negative absolute value (returns -abs(x)).
    /// </summary>
    public static long Nabs(long x)
    {
        return -Abs(x);
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
        return (x + Half) & IntegerMask;
    }

    /// <summary>
    /// Returns the fractional part of x. Equal to 'x - floor(x)'.
    /// </summary>
    public static long Fract(long x)
    {
        return x & FractionMask;
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
    /// Returns the value clamped between min and max.
    /// </summary>
    public static long Clamp(long a, long min, long max)
    {
        return (a > max) ? max : (a < min) ? min : a;
    }

    /// <summary>
    /// Returns the sign of the value (-1 if negative, 0 if zero, 1 if positive).
    /// </summary>
    public static int Sign(long x)
    {
        // https://stackoverflow.com/questions/14579920/fast-sign-of-integer-in-c/14612418#14612418
        return (int)((x >> 63) | (long)(((ulong)-x) >> 63));
    }

    /// <summary>
    /// Adds the two FP numbers together.
    /// </summary>
    public static long Add(long a, long b)
    {
        return a + b;
    }

    /// <summary>
    /// Subtracts the two FP numbers from each other.
    /// </summary>
    public static long Sub(long a, long b)
    {
        return a - b;
    }

    /// <summary>
    /// Multiplies two FP values together.
    /// </summary>
    public static long Mul(long a, long b)
    {
        long ai = a >> Shift;
        long af = (a & FractionMask);
        long bi = b >> Shift;
        long bf = (b & FractionMask);
        return FixedUtil.LogicalShiftRight(af * bf, Shift) + ai * b + af * bi;
    }

    private static int MulIntLongLow(int a, long b)
    {
        assert(a >= 0);
        int bi = (int)(b >> Shift);
        long bf = b & FractionMask;
        return (int)FixedUtil.LogicalShiftRight(a * bf, Shift) + a * bi;
    }

    private static long MulIntLongLong(int a, long b)
    {
        assert(a >= 0);
        long bi = b >> Shift;
        long bf = b & FractionMask;
        return FixedUtil.LogicalShiftRight(a * bf, Shift) + a * bi;
    }

    /// <summary>
    /// Linearly interpolate from a to b by t.
    /// </summary>
    public static long Lerp(long a, long b, long t)
    {
        return Mul(a, t) + Mul(b, One - t);
    }

    private static int Nlz(long x)
    {
        return Long.numberOfLeadingZeros(x);
    }

    /// <summary>
    /// Divides two FP values.
    /// </summary>
    public static long DivPrecise(long arg_a, long arg_b)
    {
        // From http://www.hackersdelight.org/hdcodetxt/divlu.c.txt

        long sign_dif = arg_a ^ arg_b;

        final long b = 0x100000000L; // Number base (32 bits)
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
        int offset = 31 - Nlz(b);
        int n = (int)FixedUtil.ShiftRight(b, offset + 2);
        final int ONE = (1 << 30);
        assert(n >= ONE);

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
        int offset = 31 - Nlz(b);
        int n = (int)FixedUtil.ShiftRight(b, offset + 2);
        final int ONE = (1 << 30);
        assert(n >= ONE);

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
        int offset = 31 - Nlz(b);
        int n = (int)FixedUtil.ShiftRight(b, offset + 2);
        final int ONE = (1 << 30);
        assert(n >= ONE);

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
            return 0;

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
    }

    public static long Sqrt(long x)
    {
        // Return 0 for all non-positive values.
        if (x <= 0)
            return 0;

        // Constants (s2.30).
        final int ONE = (1 << 30);
        final int SQRT2 = 1518500249; // sqrt(2.0)

        // Normalize input into [1.0, 2.0( range (as s2.30).
        int offset = 31 - Nlz(x);
        int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        assert(n >= ONE);
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
        final int ONE = (1 << 30);
        final int SQRT2 = 1518500249; // sqrt(2.0)

        // Normalize input into [1.0, 2.0( range (as s2.30).
        int offset = 31 - Nlz(x);
        int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        assert(n >= ONE);
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
        final int ONE = (1 << 30);
        final int SQRT2 = 1518500249; // sqrt(2.0)

        // Normalize input into [1.0, 2.0( range (as s2.30).
        int offset = 31 - Nlz(x);
        int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        assert(n >= ONE);
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
        final int ONE = (1 << 30);
        final int HALF_SQRT2 = 759250125; // 0.5 * sqrt(2.0)

        // Normalize input into [1.0, 2.0( range (as s2.30).
        int offset = 31 - Nlz(x);
        int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        assert(n >= ONE);
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
        final int ONE = (1 << 30);
        final int HALF_SQRT2 = 759250125; // 0.5 * sqrt(2.0)

        // Normalize input into [1.0, 2.0( range (as s2.30).
        int offset = 31 - Nlz(x);
        int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        assert(n >= ONE);
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
        final int ONE = (1 << 30);
        final int HALF_SQRT2 = 759250125; // 0.5 * sqrt(2.0)

        // Normalize input into [1.0, 2.0( range (as s2.30).
        int offset = 31 - Nlz(x);
        int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        assert(n >= ONE);
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
        int offset = 31 - Nlz(x);
        int n = (int)FixedUtil.ShiftRight(x, offset + 2);
        final int ONE = (1 << 30);
        assert(n >= ONE);

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
        int offset = 31 - Nlz(x);
        int n = (int)FixedUtil.ShiftRight(x, offset + 2);
        final int ONE = (1 << 30);
        assert(n >= ONE);

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
        final int ONE = (1 << 30);
        int offset = 31 - Nlz(x);
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
        final int ONE = (1 << 30);
        int offset = 31 - Nlz(x);
        int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        assert(n >= ONE);
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
        final int ONE = (1 << 30);
        int offset = 31 - Nlz(x);
        int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        assert(n >= ONE);
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
        final int ONE = (1 << 30);
        int offset = 31 - Nlz(x);
        int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        assert(n >= ONE);
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
        int offset = 31 - Nlz(x);
        int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);

        // Polynomial approximation of mantissa.
        final int ONE = (1 << 30);
        assert(n >= ONE);
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
        int offset = 31 - Nlz(x);
        int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);

        // Polynomial approximation of mantissa.
        final int ONE = (1 << 30);
        assert(n >= ONE);
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
        int offset = 31 - Nlz(x);
        int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);

        // Polynomial approximation of mantissa.
        final int ONE = (1 << 30);
        assert(n >= ONE);
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

    private static int UnitSin(int z)
    {
        // See: http://www.coranac.com/2009/07/sines/

        // Handle quadrants 1 and 2 by mirroring the [1, 3] range to [-1, 1] (by calculating 2 - z).
        // The if condition uses the fact that for the quadrants of interest are 0b01 and 0b10 (top two bits are different).
        if ((z ^ (z << 1)) < 0)
            z = (1 << 31) - z;

        // Now z is in range [-1, 1].
        final int ONE = (1 << 30);
        assert((z >= -ONE) && (z <= ONE));

        // Polynomial approximation.
        int zz = FixedUtil.Qmul30(z, z);
        int res = FixedUtil.Qmul30(FixedUtil.SinPoly4(zz), z);

        // Return s2.30 value.
        return res;
    }

    private static int UnitSinFast(int z)
    {
        // See: http://www.coranac.com/2009/07/sines/

        // Handle quadrants 1 and 2 by mirroring the [1, 3] range to [-1, 1] (by calculating 2 - z).
        // The if condition uses the fact that for the quadrants of interest are 0b01 and 0b10 (top two bits are different).
        if ((z ^ (z << 1)) < 0)
            z = (1 << 31) - z;

        // Now z is in range [-1, 1].
        final int ONE = (1 << 30);
        assert((z >= -ONE) && (z <= ONE));

        // Polynomial approximation.
        int zz = FixedUtil.Qmul30(z, z);
        int res = FixedUtil.Qmul30(FixedUtil.SinPoly3(zz), z);

        // Return s2.30 value.
        return res;
    }

    private static int UnitSinFastest(int z)
    {
        // See: http://www.coranac.com/2009/07/sines/

        // Handle quadrants 1 and 2 by mirroring the [1, 3] range to [-1, 1] (by calculating 2 - z).
        // The if condition uses the fact that for the quadrants of interest are 0b01 and 0b10 (top two bits are different).
        if ((z ^ (z << 1)) < 0)
            z = (1 << 31) - z;

        // Now z is in range [-1, 1].
        final int ONE = (1 << 30);
        assert((z >= -ONE) && (z <= ONE));

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

    public static long Cos(long x)
    {
        return Sin(x + PiHalf);
    }

    public static long CosFast(long x)
    {
        return SinFast(x + PiHalf);
    }

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
        assert(y >= 0 && x > 0 && x >= y);

        // Normalize input into [1.0, 2.0( range (convert to s2.30).
        final int ONE = (1 << 30);
        final int HALF = (1 << 29);
        int offset = 31 - Nlz(x);
        int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        int k = n - ONE;

        // Polynomial approximation of reciprocal.
        int oox = FixedUtil.RcpPoly4Lut8(k);
        assert(oox >= HALF && oox <= ONE);

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
        assert(y >= 0 && x > 0 && x >= y);

        // Normalize input into [1.0, 2.0( range (convert to s2.30).
        final int ONE = (1 << 30);
        final int HALF = (1 << 29);
        int offset = 31 - Nlz(x);
        int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        int k = n - ONE;

        // Polynomial approximation.
        int oox = FixedUtil.RcpPoly6(k);
        assert(oox >= HALF && oox <= ONE);

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
        assert(y >= 0 && x > 0 && x >= y);

        // Normalize input into [1.0, 2.0( range (convert to s2.30).
        final int ONE = (1 << 30);
        final int HALF = (1 << 29);
        int offset = 31 - Nlz(x);
        int n = (int)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        int k = n - ONE;

        // Polynomial approximation.
        int oox = FixedUtil.RcpPoly4(k);
        assert(oox >= HALF && oox <= ONE);

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

    public static long Atan(long x)
    {
        return Atan2(x, One);
    }

    public static long AtanFast(long x)
    {
        return Atan2Fast(x, One);
    }

    public static long AtanFastest(long x)
    {
        return Atan2Fastest(x, One);
    }
} // Fixed64


// END GENERATED FILE
