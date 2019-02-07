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
using System;
using System.Runtime.CompilerServices;
using FixPointCS;

namespace FixMath
{
    /// <summary>
    /// Signed 32.32 fixed point value struct.
    /// </summary>
    [Serializable]
    public struct F64 : IComparable<F64>, IEquatable<F64>
    {
        // Constants
        public static F64 Neg1      { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed64.Neg1); } }
        public static F64 Zero      { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed64.Zero); } }
        public static F64 Half      { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed64.Half); } }
        public static F64 One       { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed64.One); } }
        public static F64 Two       { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed64.Two); } }
        public static F64 Pi        { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed64.Pi); } }
        public static F64 Pi2       { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed64.Pi2); } }
        public static F64 PiHalf    { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed64.PiHalf); } }
        public static F64 E         { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed64.E); } }

        public static F64 MinValue  { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed64.MinValue); } }
        public static F64 MaxValue  { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed64.MaxValue); } }

        // Raw fixed point value
        public long Raw;

        // Constructors
        public F64(int v) { Raw = Fixed64.FromInt(v); }
        public F64(float v) { Raw = Fixed64.FromFloat(v); }
        public F64(double v) { Raw = Fixed64.FromDouble(v); }
        public F64(F32 v) { Raw = (long)v.Raw << 16; }

        // Conversions
        public static int FloorToInt(F64 a) { return Fixed64.FloorToInt(a.Raw); }
        public static int CeilToInt(F64 a) { return Fixed64.CeilToInt(a.Raw); }
        public static int RoundToInt(F64 a) { return Fixed64.RoundToInt(a.Raw); }
        public float Float { get { return Fixed64.ToFloat(Raw); } }
        public double Double { get { return Fixed64.ToDouble(Raw); } }
        public F32 F32 { get { return F32.FromRaw((int)(Raw >> 16)); } }

        // Creates the fixed point number that's a divided by b.
        public static F64 Ratio(int a, int b) { return F64.FromRaw(((long)a << 32) / b); }
        // Creates the fixed point number that's a divided by 10.
        public static F64 Ratio10(int a) { return F64.FromRaw(((long)a << 32) / 10); }
        // Creates the fixed point number that's a divided by 100.
        public static F64 Ratio100(int a) { return F64.FromRaw(((long)a << 32) / 100); }
        // Creates the fixed point number that's a divided by 1000.
        public static F64 Ratio1000(int a) { return F64.FromRaw(((long)a << 32) / 1000); }

        public void test()
        {
            int x = 123;
            test2(ref x, out x);
        }
        public void test2(ref int a, out int b)
        {
            b = a;
        }

        // Operators
        public static F64 operator -(F64 v1) { return FromRaw(-v1.Raw); }

        public static F64 operator +(F64 v1, F64 v2) { return FromRaw(v1.Raw + v2.Raw); }
        public static F64 operator -(F64 v1, F64 v2) { return FromRaw(v1.Raw - v2.Raw); }
        public static F64 operator *(F64 v1, F64 v2) { return FromRaw(Fixed64.Mul(v1.Raw, v2.Raw)); }
        public static F64 operator /(F64 v1, F64 v2) { return FromRaw(Fixed64.DivPrecise(v1.Raw, v2.Raw)); }
        public static F64 operator %(F64 v1, F64 v2) { return FromRaw(Fixed64.Mod(v1.Raw, v2.Raw)); }

        public static F64 operator +(F64 v1, int v2) { return FromRaw(v1.Raw + Fixed64.FromInt(v2)); }
        public static F64 operator +(int v1, F64 v2) { return FromRaw(Fixed64.FromInt(v1) + v2.Raw); }
        public static F64 operator -(F64 v1, int v2) { return FromRaw(v1.Raw - Fixed64.FromInt(v2)); }
        public static F64 operator -(int v1, F64 v2) { return FromRaw(Fixed64.FromInt(v1) - v2.Raw); }
        public static F64 operator *(F64 v1, int v2) { return FromRaw(v1.Raw * (long)v2); }
        public static F64 operator *(int v1, F64 v2) { return FromRaw((long)v1 * v2.Raw); }
        public static F64 operator /(F64 v1, int v2) { return FromRaw(v1.Raw / (long)v2); }
        public static F64 operator /(int v1, F64 v2) { return FromRaw(Fixed64.DivPrecise(Fixed64.FromInt(v1), v2.Raw)); }
        public static F64 operator %(F64 v1, int v2) { return FromRaw(Fixed64.Mod(v1.Raw, Fixed64.FromInt(v2))); }
        public static F64 operator %(int v1, F64 v2) { return FromRaw(Fixed64.Mod(Fixed64.FromInt(v1), v2.Raw)); }

        public static F64 operator ++(F64 v1) { return FromRaw(v1.Raw + Fixed64.One); }
        public static F64 operator --(F64 v1) { return FromRaw(v1.Raw - Fixed64.One); }

        public static bool operator ==(F64 v1, F64 v2) { return v1.Raw == v2.Raw; }
        public static bool operator !=(F64 v1, F64 v2) { return v1.Raw != v2.Raw; }
        public static bool operator <(F64 v1, F64 v2) { return v1.Raw < v2.Raw; }
        public static bool operator <=(F64 v1, F64 v2) { return v1.Raw <= v2.Raw; }
        public static bool operator >(F64 v1, F64 v2) { return v1.Raw > v2.Raw; }
        public static bool operator >=(F64 v1, F64 v2) { return v1.Raw >= v2.Raw; }

        public static bool operator ==(int v1, F64 v2) { return Fixed64.FromInt(v1) == v2.Raw; }
        public static bool operator ==(F64 v1, int v2) { return v1.Raw == Fixed64.FromInt(v2); }
        public static bool operator !=(int v1, F64 v2) { return Fixed64.FromInt(v1) != v2.Raw; }
        public static bool operator !=(F64 v1, int v2) { return v1.Raw != Fixed64.FromInt(v2); }
        public static bool operator <(int v1, F64 v2) { return Fixed64.FromInt(v1) < v2.Raw; }
        public static bool operator <(F64 v1, int v2) { return v1.Raw < Fixed64.FromInt(v2); }
        public static bool operator <=(int v1, F64 v2) { return Fixed64.FromInt(v1) <= v2.Raw; }
        public static bool operator <=(F64 v1, int v2) { return v1.Raw <= Fixed64.FromInt(v2); }
        public static bool operator >(int v1, F64 v2) { return Fixed64.FromInt(v1) > v2.Raw; }
        public static bool operator >(F64 v1, int v2) { return v1.Raw > Fixed64.FromInt(v2); }
        public static bool operator >=(int v1, F64 v2) { return Fixed64.FromInt(v1) >= v2.Raw; }
        public static bool operator >=(F64 v1, int v2) { return v1.Raw >= Fixed64.FromInt(v2); }

        public static bool operator ==(F32 a, F64 b) { return F64.FromF32(a) == b; }
        public static bool operator ==(F64 a, F32 b) { return a == F64.FromF32(b); }
        public static bool operator !=(F32 a, F64 b) { return F64.FromF32(a) != b; }
        public static bool operator !=(F64 a, F32 b) { return a != F64.FromF32(b); }
        public static bool operator <(F32 a, F64 b) { return F64.FromF32(a) < b; }
        public static bool operator <(F64 a, F32 b) { return a < F64.FromF32(b); }
        public static bool operator <=(F32 a, F64 b) { return F64.FromF32(a) <= b; }
        public static bool operator <=(F64 a, F32 b) { return a <= F64.FromF32(b); }
        public static bool operator >(F32 a, F64 b) { return F64.FromF32(a) > b; }
        public static bool operator >(F64 a, F32 b) { return a > F64.FromF32(b); }
        public static bool operator >=(F32 a, F64 b) { return F64.FromF32(a) >= b; }
        public static bool operator >=(F64 a, F32 b) { return a >= F64.FromF32(b); }

        public static F64 RadToDeg(F64 a) { return FromRaw(Fixed64.Mul(a.Raw, 246083499198)); } // 180 / F64.Pi
        public static F64 DegToRad(F64 a) { return FromRaw(Fixed64.Mul(a.Raw, 74961320)); }     // F64.Pi / 180

        public static F64 Div2(F64 a) { return FromRaw(a.Raw >> 1); }
        public static F64 Abs(F64 a) { return FromRaw(Fixed64.Abs(a.Raw)); }
        public static F64 Nabs(F64 a) { return FromRaw(Fixed64.Nabs(a.Raw)); }
        public static F64 Ceil(F64 a) { return FromRaw(Fixed64.Ceil(a.Raw)); }
        public static F64 Floor(F64 a) { return FromRaw(Fixed64.Floor(a.Raw)); }
        public static F64 Round(F64 a) { return FromRaw(Fixed64.Round(a.Raw)); }
        public static F64 Fract(F64 a) { return FromRaw(Fixed64.Fract(a.Raw)); }
        public static F64 Div(F64 a, F64 b) { return FromRaw(Fixed64.Div(a.Raw, b.Raw)); }
        public static F64 DivFast(F64 a, F64 b) { return FromRaw(Fixed64.DivFast(a.Raw, b.Raw)); }
        public static F64 DivFastest(F64 a, F64 b) { return FromRaw(Fixed64.DivFastest(a.Raw, b.Raw)); }
        public static F64 SqrtPrecise(F64 a) { return FromRaw(Fixed64.SqrtPrecise(a.Raw)); }
        public static F64 Sqrt(F64 a) { return FromRaw(Fixed64.Sqrt(a.Raw)); }
        public static F64 SqrtFast(F64 a) { return FromRaw(Fixed64.SqrtFast(a.Raw)); }
        public static F64 SqrtFastest(F64 a) { return FromRaw(Fixed64.SqrtFastest(a.Raw)); }
        public static F64 RSqrt(F64 a) { return FromRaw(Fixed64.RSqrt(a.Raw)); }
        public static F64 RSqrtFast(F64 a) { return FromRaw(Fixed64.RSqrtFast(a.Raw)); }
        public static F64 RSqrtFastest(F64 a) { return FromRaw(Fixed64.RSqrtFastest(a.Raw)); }
        public static F64 Rcp(F64 a) { return FromRaw(Fixed64.Rcp(a.Raw)); }
        public static F64 RcpFast(F64 a) { return FromRaw(Fixed64.RcpFast(a.Raw)); }
        public static F64 RcpFastest(F64 a) { return FromRaw(Fixed64.RcpFastest(a.Raw)); }
        public static F64 Exp(F64 a) { return FromRaw(Fixed64.Exp(a.Raw)); }
        public static F64 ExpFast(F64 a) { return FromRaw(Fixed64.ExpFast(a.Raw)); }
        public static F64 ExpFastest(F64 a) { return FromRaw(Fixed64.ExpFastest(a.Raw)); }
        public static F64 Exp2(F64 a) { return FromRaw(Fixed64.Exp2(a.Raw)); }
        public static F64 Exp2Fast(F64 a) { return FromRaw(Fixed64.Exp2Fast(a.Raw)); }
        public static F64 Exp2Fastest(F64 a) { return FromRaw(Fixed64.Exp2Fastest(a.Raw)); }
        public static F64 Log(F64 a) { return FromRaw(Fixed64.Log(a.Raw)); }
        public static F64 LogFast(F64 a) { return FromRaw(Fixed64.LogFast(a.Raw)); }
        public static F64 LogFastest(F64 a) { return FromRaw(Fixed64.LogFastest(a.Raw)); }
        public static F64 Log2(F64 a) { return FromRaw(Fixed64.Log2(a.Raw)); }
        public static F64 Log2Fast(F64 a) { return FromRaw(Fixed64.Log2Fast(a.Raw)); }
        public static F64 Log2Fastest(F64 a) { return FromRaw(Fixed64.Log2Fastest(a.Raw)); }

        public static F64 Sin(F64 a) { return FromRaw(Fixed64.Sin(a.Raw)); }
        public static F64 SinFast(F64 a) { return FromRaw(Fixed64.SinFast(a.Raw)); }
        public static F64 SinFastest(F64 a) { return FromRaw(Fixed64.SinFastest(a.Raw)); }
        public static F64 Cos(F64 a) { return FromRaw(Fixed64.Cos(a.Raw)); }
        public static F64 CosFast(F64 a) { return FromRaw(Fixed64.CosFast(a.Raw)); }
        public static F64 CosFastest(F64 a) { return FromRaw(Fixed64.CosFastest(a.Raw)); }
        public static F64 Tan(F64 a) { return FromRaw(Fixed64.Tan(a.Raw)); }
        public static F64 TanFast(F64 a) { return FromRaw(Fixed64.TanFast(a.Raw)); }
        public static F64 TanFastest(F64 a) { return FromRaw(Fixed64.TanFastest(a.Raw)); }
        public static F64 Asin(F64 a) { return FromRaw(Fixed64.Asin(a.Raw)); }
        public static F64 AsinFast(F64 a) { return FromRaw(Fixed64.AsinFast(a.Raw)); }
        public static F64 AsinFastest(F64 a) { return FromRaw(Fixed64.AsinFastest(a.Raw)); }
        public static F64 Acos(F64 a) { return FromRaw(Fixed64.Acos(a.Raw)); }
        public static F64 AcosFast(F64 a) { return FromRaw(Fixed64.AcosFast(a.Raw)); }
        public static F64 AcosFastest(F64 a) { return FromRaw(Fixed64.AcosFastest(a.Raw)); }
        public static F64 Atan(F64 a) { return FromRaw(Fixed64.Atan(a.Raw)); }
        public static F64 AtanFast(F64 a) { return FromRaw(Fixed64.AtanFast(a.Raw)); }
        public static F64 AtanFastest(F64 a) { return FromRaw(Fixed64.AtanFastest(a.Raw)); }
        public static F64 Atan2(F64 y, F64 x) { return FromRaw(Fixed64.Atan2(y.Raw, x.Raw)); }
        public static F64 Atan2Fast(F64 y, F64 x) { return FromRaw(Fixed64.Atan2Fast(y.Raw, x.Raw)); }
        public static F64 Atan2Fastest(F64 y, F64 x) { return FromRaw(Fixed64.Atan2Fastest(y.Raw, x.Raw)); }
        public static F64 Pow(F64 a, F64 b) { return FromRaw(Fixed64.Pow(a.Raw, b.Raw)); }
        public static F64 PowFast(F64 a, F64 b) { return FromRaw(Fixed64.PowFast(a.Raw, b.Raw)); }
        public static F64 PowFastest(F64 a, F64 b) { return FromRaw(Fixed64.PowFastest(a.Raw, b.Raw)); }

        public static F64 Min(F64 a, F64 b) { return FromRaw(Fixed64.Min(a.Raw, b.Raw)); }
        public static F64 Max(F64 a, F64 b) { return FromRaw(Fixed64.Max(a.Raw, b.Raw)); }
        public static F64 Clamp(F64 a, F64 min, F64 max) { return FromRaw(Fixed64.Clamp(a.Raw, min.Raw, max.Raw)); }

        public static F64 Lerp(F64 a, F64 b, F64 t)
        {
            long tb = t.Raw;
            long ta = Fixed64.One - tb;
            return FromRaw(Fixed64.Mul(a.Raw, ta) + Fixed64.Mul(b.Raw, tb));
        }

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static F64 FromRaw(long raw)
        {
            F64 r;
            r.Raw = raw;
            return r;
        }

        public static F64 FromInt(int v) { return new F64(v); }
        public static F64 FromFloat(float v) { return new F64(v); }
        public static F64 FromDouble(double v) { return new F64(v); }
        public static F64 FromF32(F32 v) { return new F64(v); }

        public bool Equals(F64 other)
        {
            return (Raw == other.Raw);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is F64))
                return false;
            return ((F64)obj).Raw == Raw;
        }

        public int CompareTo(F64 other)
        {
            if (Raw < other.Raw) return -1;
            if (Raw > other.Raw) return +1;
            return 0;
        }

        public override string ToString()
        {
            return Fixed64.ToString(Raw);
        }

        public override int GetHashCode()
        {
            return (int)Raw | (int)(Raw >> 32);
        }
    }
}
