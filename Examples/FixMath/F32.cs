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
    /// Signed 16.16 fixed point value struct.
    /// </summary>
    [Serializable]
    public struct F32 : IComparable<F32>, IEquatable<F32>
    {
        // Constants
        public static F32 Neg1      { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed32.Neg1); } }
        public static F32 Zero      { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed32.Zero); } }
        public static F32 Half      { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed32.Half); } }
        public static F32 One       { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed32.One); } }
        public static F32 Two       { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed32.Two); } }
        public static F32 Pi        { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed32.Pi); } }
        public static F32 Pi2       { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed32.Pi2); } }
        public static F32 PiHalf    { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed32.PiHalf); } }
        public static F32 E         { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed32.E); } }

        public static F32 MinValue  { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed32.MinValue); } }
        public static F32 MaxValue  { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed32.MaxValue); } }

        // Raw fixed point value
        public int Raw;

        // Constructors
        public F32(int v) { Raw = Fixed32.FromInt(v); }
        public F32(float v) { Raw = Fixed32.FromFloat(v); }
        public F32(double v) { Raw = Fixed32.FromDouble(v); }
        public F32(F64 v) { Raw = (int)(v.Raw >> 16); }

        // Conversions
        public static int FloorToInt(F32 a) { return Fixed32.FloorToInt(a.Raw); }
        public static int CeilToInt(F32 a) { return Fixed32.CeilToInt(a.Raw); }
        public static int RoundToInt(F32 a) { return Fixed32.RoundToInt(a.Raw); }
        public float Float { get { return Fixed32.ToFloat(Raw); } }
        public double Double { get { return Fixed32.ToDouble(Raw); } }

        // Creates the fixed point number that's a divided by b.
        public static F32 Ratio(int a, int b) { return F32.FromRaw((int)(((long)a << 16) / b)); }
        // Creates the fixed point number that's a divided by 10.
        public static F32 Ratio10(int a) { return F32.FromRaw((int)(((long)a << 16) / 10)); }
        // Creates the fixed point number that's a divided by 100.
        public static F32 Ratio100(int a) { return F32.FromRaw((int)(((long)a << 16) / 100)); }
        // Creates the fixed point number that's a divided by 1000.
        public static F32 Ratio1000(int a) { return F32.FromRaw((int)(((long)a << 16) / 1000)); }

        // Operators
        public static F32 operator -(F32 v1) { return FromRaw(-v1.Raw); }

        //public static F32 operator +(F32 v1, F32 v2) { F32 r; r.raw = v1.raw + v2.raw; return r; }
        public static F32 operator +(F32 v1, F32 v2) { return FromRaw(v1.Raw + v2.Raw); }
        public static F32 operator -(F32 v1, F32 v2) { return FromRaw(v1.Raw - v2.Raw); }
        public static F32 operator *(F32 v1, F32 v2) { return FromRaw(Fixed32.Mul(v1.Raw, v2.Raw)); }
        public static F32 operator /(F32 v1, F32 v2) { return FromRaw(Fixed32.DivPrecise(v1.Raw, v2.Raw)); }
        public static F32 operator %(F32 v1, F32 v2) { return FromRaw(Fixed32.Mod(v1.Raw, v2.Raw)); }

        public static F32 operator +(F32 v1, int v2) { return FromRaw(v1.Raw + Fixed32.FromInt(v2)); }
        public static F32 operator +(int v1, F32 v2) { return FromRaw(Fixed32.FromInt(v1) + v2.Raw); }
        public static F32 operator -(F32 v1, int v2) { return FromRaw(v1.Raw - Fixed32.FromInt(v2)); }
        public static F32 operator -(int v1, F32 v2) { return FromRaw(Fixed32.FromInt(v1) - v2.Raw); }
        public static F32 operator *(F32 v1, int v2) { return FromRaw(v1.Raw * (int)v2); }
        public static F32 operator *(int v1, F32 v2) { return FromRaw((int)v1 * v2.Raw); }
        public static F32 operator /(F32 v1, int v2) { return FromRaw(v1.Raw / (int)v2); }
        public static F32 operator /(int v1, F32 v2) { return FromRaw(Fixed32.DivPrecise(Fixed32.FromInt(v1), v2.Raw)); }
        public static F32 operator %(F32 v1, int v2) { return FromRaw(Fixed32.Mod(v1.Raw, Fixed32.FromInt(v2))); }
        public static F32 operator %(int v1, F32 v2) { return FromRaw(Fixed32.Mod(Fixed32.FromInt(v1), v2.Raw)); }

        public static F32 operator ++(F32 v1) { return FromRaw(v1.Raw + Fixed32.One); }
        public static F32 operator --(F32 v1) { return FromRaw(v1.Raw - Fixed32.One); }

        public static bool operator ==(F32 v1, F32 v2) { return v1.Raw == v2.Raw; }
        public static bool operator !=(F32 v1, F32 v2) { return v1.Raw != v2.Raw; }
        public static bool operator <(F32 v1, F32 v2) { return v1.Raw < v2.Raw; }
        public static bool operator <=(F32 v1, F32 v2) { return v1.Raw <= v2.Raw; }
        public static bool operator >(F32 v1, F32 v2) { return v1.Raw > v2.Raw; }
        public static bool operator >=(F32 v1, F32 v2) { return v1.Raw >= v2.Raw; }

        public static bool operator ==(int v1, F32 v2) { return Fixed32.FromInt(v1) == v2.Raw; }
        public static bool operator ==(F32 v1, int v2) { return v1.Raw == Fixed32.FromInt(v2); }
        public static bool operator !=(int v1, F32 v2) { return Fixed32.FromInt(v1) != v2.Raw; }
        public static bool operator !=(F32 v1, int v2) { return v1.Raw != Fixed32.FromInt(v2); }
        public static bool operator <(int v1, F32 v2) { return Fixed32.FromInt(v1) < v2.Raw; }
        public static bool operator <(F32 v1, int v2) { return v1.Raw < Fixed32.FromInt(v2); }
        public static bool operator <=(int v1, F32 v2) { return Fixed32.FromInt(v1) <= v2.Raw; }
        public static bool operator <=(F32 v1, int v2) { return v1.Raw <= Fixed32.FromInt(v2); }
        public static bool operator >(int v1, F32 v2) { return Fixed32.FromInt(v1) > v2.Raw; }
        public static bool operator >(F32 v1, int v2) { return v1.Raw > Fixed32.FromInt(v2); }
        public static bool operator >=(int v1, F32 v2) { return Fixed32.FromInt(v1) >= v2.Raw; }
        public static bool operator >=(F32 v1, int v2) { return v1.Raw >= Fixed32.FromInt(v2); }

        public static F32 RadToDeg(F32 a) { return FromRaw(Fixed32.Mul(a.Raw, 3754943)); }  // 180 / F32.Pi
        public static F32 DegToRad(F32 a) { return FromRaw(Fixed32.Mul(a.Raw, 1143)); }     // F32.Pi / 180

        public static F32 Div2(F32 a) { return FromRaw(a.Raw >> 1); }
        public static F32 Abs(F32 a) { return FromRaw(Fixed32.Abs(a.Raw)); }
        public static F32 Nabs(F32 a) { return FromRaw(Fixed32.Nabs(a.Raw)); }
        public static F32 Ceil(F32 a) { return FromRaw(Fixed32.Ceil(a.Raw)); }
        public static F32 Floor(F32 a) { return FromRaw(Fixed32.Floor(a.Raw)); }
        public static F32 Round(F32 a) { return FromRaw(Fixed32.Round(a.Raw)); }
        public static F32 Fract(F32 a) { return FromRaw(Fixed32.Fract(a.Raw)); }
        public static F32 Div(F32 a, F32 b) { return FromRaw(Fixed32.Div(a.Raw, b.Raw)); }
        public static F32 DivFast(F32 a, F32 b) { return FromRaw(Fixed32.DivFast(a.Raw, b.Raw)); }
        public static F32 DivFastest(F32 a, F32 b) { return FromRaw(Fixed32.DivFastest(a.Raw, b.Raw)); }
        public static F32 SqrtPrecise(F32 a) { return FromRaw(Fixed32.SqrtPrecise(a.Raw)); }
        public static F32 Sqrt(F32 a) { return FromRaw(Fixed32.Sqrt(a.Raw)); }
        public static F32 SqrtFast(F32 a) { return FromRaw(Fixed32.SqrtFast(a.Raw)); }
        public static F32 SqrtFastest(F32 a) { return FromRaw(Fixed32.SqrtFastest(a.Raw)); }
        public static F32 RSqrt(F32 a) { return FromRaw(Fixed32.RSqrt(a.Raw)); }
        public static F32 RSqrtFast(F32 a) { return FromRaw(Fixed32.RSqrtFast(a.Raw)); }
        public static F32 RSqrtFastest(F32 a) { return FromRaw(Fixed32.RSqrtFastest(a.Raw)); }
        public static F32 Rcp(F32 a) { return FromRaw(Fixed32.Rcp(a.Raw)); }
        public static F32 RcpFast(F32 a) { return FromRaw(Fixed32.RcpFast(a.Raw)); }
        public static F32 RcpFastest(F32 a) { return FromRaw(Fixed32.RcpFastest(a.Raw)); }
        public static F32 Exp(F32 a) { return FromRaw(Fixed32.Exp(a.Raw)); }
        public static F32 ExpFast(F32 a) { return FromRaw(Fixed32.ExpFast(a.Raw)); }
        public static F32 ExpFastest(F32 a) { return FromRaw(Fixed32.ExpFastest(a.Raw)); }
        public static F32 Exp2(F32 a) { return FromRaw(Fixed32.Exp2(a.Raw)); }
        public static F32 Exp2Fast(F32 a) { return FromRaw(Fixed32.Exp2Fast(a.Raw)); }
        public static F32 Exp2Fastest(F32 a) { return FromRaw(Fixed32.Exp2Fastest(a.Raw)); }
        public static F32 Log(F32 a) { return FromRaw(Fixed32.Log(a.Raw)); }
        public static F32 LogFast(F32 a) { return FromRaw(Fixed32.LogFast(a.Raw)); }
        public static F32 LogFastest(F32 a) { return FromRaw(Fixed32.LogFastest(a.Raw)); }
        public static F32 Log2(F32 a) { return FromRaw(Fixed32.Log2(a.Raw)); }
        public static F32 Log2Fast(F32 a) { return FromRaw(Fixed32.Log2Fast(a.Raw)); }
        public static F32 Log2Fastest(F32 a) { return FromRaw(Fixed32.Log2Fastest(a.Raw)); }

        public static F32 Sin(F32 a) { return FromRaw(Fixed32.Sin(a.Raw)); }
        public static F32 SinFast(F32 a) { return FromRaw(Fixed32.SinFast(a.Raw)); }
        public static F32 SinFastest(F32 a) { return FromRaw(Fixed32.SinFastest(a.Raw)); }
        public static F32 Cos(F32 a) { return FromRaw(Fixed32.Cos(a.Raw)); }
        public static F32 CosFast(F32 a) { return FromRaw(Fixed32.CosFast(a.Raw)); }
        public static F32 CosFastest(F32 a) { return FromRaw(Fixed32.CosFastest(a.Raw)); }
        public static F32 Tan(F32 a) { return FromRaw(Fixed32.Tan(a.Raw)); }
        public static F32 TanFast(F32 a) { return FromRaw(Fixed32.TanFast(a.Raw)); }
        public static F32 TanFastest(F32 a) { return FromRaw(Fixed32.TanFastest(a.Raw)); }
        public static F32 Asin(F32 a) { return FromRaw(Fixed32.Asin(a.Raw)); }
        public static F32 AsinFast(F32 a) { return FromRaw(Fixed32.AsinFast(a.Raw)); }
        public static F32 AsinFastest(F32 a) { return FromRaw(Fixed32.AsinFastest(a.Raw)); }
        public static F32 Acos(F32 a) { return FromRaw(Fixed32.Acos(a.Raw)); }
        public static F32 AcosFast(F32 a) { return FromRaw(Fixed32.AcosFast(a.Raw)); }
        public static F32 AcosFastest(F32 a) { return FromRaw(Fixed32.AcosFastest(a.Raw)); }
        public static F32 Atan(F32 a) { return FromRaw(Fixed32.Atan(a.Raw)); }
        public static F32 AtanFast(F32 a) { return FromRaw(Fixed32.AtanFast(a.Raw)); }
        public static F32 AtanFastest(F32 a) { return FromRaw(Fixed32.AtanFastest(a.Raw)); }
        public static F32 Atan2(F32 y, F32 x) { return FromRaw(Fixed32.Atan2(y.Raw, x.Raw)); }
        public static F32 Atan2Fast(F32 y, F32 x) { return FromRaw(Fixed32.Atan2Fast(y.Raw, x.Raw)); }
        public static F32 Atan2Fastest(F32 y, F32 x) { return FromRaw(Fixed32.Atan2Fastest(y.Raw, x.Raw)); }
        public static F32 Pow(F32 a, F32 b) { return FromRaw(Fixed32.Pow(a.Raw, b.Raw)); }
        public static F32 PowFast(F32 a, F32 b) { return FromRaw(Fixed32.PowFast(a.Raw, b.Raw)); }
        public static F32 PowFastest(F32 a, F32 b) { return FromRaw(Fixed32.PowFastest(a.Raw, b.Raw)); }

        public static F32 Min(F32 a, F32 b) { return FromRaw(Fixed32.Min(a.Raw, b.Raw)); }
        public static F32 Max(F32 a, F32 b) { return FromRaw(Fixed32.Max(a.Raw, b.Raw)); }
        public static F32 Clamp(F32 a, F32 min, F32 max) { return FromRaw(Fixed32.Clamp(a.Raw, min.Raw, max.Raw)); }

        public static F32 Lerp(F32 a, F32 b, F32 t)
        {
            int tb = t.Raw;
            int ta = Fixed32.One - tb;
            return FromRaw(Fixed32.Mul(a.Raw, ta) + Fixed32.Mul(b.Raw, tb));
        }

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static F32 FromRaw(int raw)
        {
            F32 r;
            r.Raw = raw;
            return r;
        }

        public static F32 FromInt(int v) { return new F32(v); }
        public static F32 FromFloat(float v) { return new F32(v); }
        public static F32 FromDouble(double v) { return new F32(v); }
        public static F32 FromF64(F64 v) { return new F32(v); }

        public bool Equals(F32 other)
        {
            return (Raw == other.Raw);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is F32))
                return false;
            return ((F32)obj).Raw == Raw;
        }

        public int CompareTo(F32 other)
        {
            if (Raw < other.Raw) return -1;
            if (Raw > other.Raw) return +1;
            return 0;
        }

        public override string ToString()
        {
            return Fixed32.ToString(Raw);
        }

        public override int GetHashCode()
        {
            return (int)Raw | (int)(Raw >> 32);
        }
    }
}
