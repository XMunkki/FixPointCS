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
using System;

namespace FixPointCS
{
    /// <summary>
    /// Signed 32.32 fixed point value struct.
    /// </summary>
    [Serializable]
    public struct F64 : IComparable<F64>
    {
        // Constants
        public static F64 Neg1 { get { return FromRaw(Fixed64.Neg1); } }
        public static F64 Zero { get { return FromRaw(Fixed64.Zero); } }
        public static F64 Half { get { return FromRaw(Fixed64.Half); } }
        public static F64 One { get { return FromRaw(Fixed64.One); } }
        public static F64 Two { get { return FromRaw(Fixed64.Two); } }
        public static F64 Pi { get { return FromRaw(Fixed64.Pi); } }
        public static F64 Pi2 { get { return FromRaw(Fixed64.Pi2); } }
        public static F64 PiHalf { get { return FromRaw(Fixed64.PiHalf); } }
        public static F64 E { get { return FromRaw(Fixed64.E); } }

        public static F64 MinValue { get { return FromRaw(Fixed64.MinValue); } }
        public static F64 MaxValue { get { return FromRaw(Fixed64.MaxValue); } }

        // Raw fixed point value
        public long raw;

        // Constructors
        public F64(int v) { raw = Fixed64.FromInt(v); }
        public F64(float v) { raw = Fixed64.FromFloat(v); }
        public F64(double v) { raw = Fixed64.FromDouble(v); }
        public F64(F32 v) { raw = (long)v.raw << 16; }

        // Conversions
        public static int FloorToInt(F64 a) { return Fixed64.FloorToInt(a.raw); }
        public static int CeilToInt(F64 a) { return Fixed64.CeilToInt(a.raw); }
        public static int RoundToInt(F64 a) { return Fixed64.RoundToInt(a.raw); }
        public float Float { get { return Fixed64.ToFloat(raw); } }
        public double Double { get { return Fixed64.ToDouble(raw); } }

        // Operators
        public static F64 operator -(F64 v1) { return FromRaw(-v1.raw); }

        public static F64 operator +(F64 v1, F64 v2) { return FromRaw(v1.raw + v2.raw); }
        public static F64 operator -(F64 v1, F64 v2) { return FromRaw(v1.raw - v2.raw); }
        public static F64 operator *(F64 v1, F64 v2) { return FromRaw(Fixed64.Mul(v1.raw, v2.raw)); }
        public static F64 operator /(F64 v1, F64 v2) { return FromRaw(Fixed64.DivPrecise(v1.raw, v2.raw)); }
        public static F64 operator %(F64 v1, F64 v2) { return FromRaw(Fixed64.Mod(v1.raw, v2.raw)); }

        public static F64 operator +(F64 v1, int v2) { return FromRaw(v1.raw + Fixed64.FromInt(v2)); }
        public static F64 operator +(int v1, F64 v2) { return FromRaw(Fixed64.FromInt(v1) + v2.raw); }
        public static F64 operator -(F64 v1, int v2) { return FromRaw(v1.raw - Fixed64.FromInt(v2)); }
        public static F64 operator -(int v1, F64 v2) { return FromRaw(Fixed64.FromInt(v1) - v2.raw); }
        public static F64 operator *(F64 v1, int v2) { return FromRaw(v1.raw * (long)v2); }
        public static F64 operator *(int v1, F64 v2) { return FromRaw((long)v1 * v2.raw); }
        public static F64 operator /(F64 v1, int v2) { return FromRaw(v1.raw / (long)v2); }
        public static F64 operator /(int v1, F64 v2) { return FromRaw(Fixed64.DivPrecise(Fixed64.FromInt(v1), v2.raw)); }
        public static F64 operator %(F64 v1, int v2) { return FromRaw(Fixed64.Mod(v1.raw, Fixed64.FromInt(v2))); }
        public static F64 operator %(int v1, F64 v2) { return FromRaw(Fixed64.Mod(Fixed64.FromInt(v1), v2.raw)); }

        public static F64 operator ++(F64 v1) { return FromRaw(v1.raw + Fixed64.One); }
        public static F64 operator --(F64 v1) { return FromRaw(v1.raw - Fixed64.One); }

        public static bool operator ==(F64 v1, F64 v2) { return v1.raw == v2.raw; }
        public static bool operator !=(F64 v1, F64 v2) { return v1.raw != v2.raw; }
        public static bool operator <(F64 v1, F64 v2) { return v1.raw < v2.raw; }
        public static bool operator <=(F64 v1, F64 v2) { return v1.raw <= v2.raw; }
        public static bool operator >(F64 v1, F64 v2) { return v1.raw > v2.raw; }
        public static bool operator >=(F64 v1, F64 v2) { return v1.raw >= v2.raw; }

        public static bool operator ==(int v1, F64 v2) { return Fixed64.FromInt(v1) == v2.raw; }
        public static bool operator ==(F64 v1, int v2) { return v1.raw == Fixed64.FromInt(v2); }
        public static bool operator !=(int v1, F64 v2) { return Fixed64.FromInt(v1) != v2.raw; }
        public static bool operator !=(F64 v1, int v2) { return v1.raw != Fixed64.FromInt(v2); }
        public static bool operator <(int v1, F64 v2) { return Fixed64.FromInt(v1) < v2.raw; }
        public static bool operator <(F64 v1, int v2) { return v1.raw < Fixed64.FromInt(v2); }
        public static bool operator <=(int v1, F64 v2) { return Fixed64.FromInt(v1) <= v2.raw; }
        public static bool operator <=(F64 v1, int v2) { return v1.raw <= Fixed64.FromInt(v2); }
        public static bool operator >(int v1, F64 v2) { return Fixed64.FromInt(v1) > v2.raw; }
        public static bool operator >(F64 v1, int v2) { return v1.raw > Fixed64.FromInt(v2); }
        public static bool operator >=(int v1, F64 v2) { return Fixed64.FromInt(v1) >= v2.raw; }
        public static bool operator >=(F64 v1, int v2) { return v1.raw >= Fixed64.FromInt(v2); }

        public static F64 Abs(F64 a) { return FromRaw(Fixed64.Abs(a.raw)); }
        public static F64 Nabs(F64 a) { return FromRaw(Fixed64.Nabs(a.raw)); }
        public static F64 Ceil(F64 a) { return FromRaw(Fixed64.Ceil(a.raw)); }
        public static F64 Floor(F64 a) { return FromRaw(Fixed64.Floor(a.raw)); }
        public static F64 Round(F64 a) { return FromRaw(Fixed64.Round(a.raw)); }
        public static F64 Fract(F64 a) { return FromRaw(Fixed64.Fract(a.raw)); }
        public static F64 Div(F64 a, F64 b) { return FromRaw(Fixed64.Div(a.raw, b.raw)); }
        public static F64 DivFast(F64 a, F64 b) { return FromRaw(Fixed64.DivFast(a.raw, b.raw)); }
        public static F64 DivFastest(F64 a, F64 b) { return FromRaw(Fixed64.DivFastest(a.raw, b.raw)); }
        public static F64 SqrtPrecise(F64 a) { return FromRaw(Fixed64.SqrtPrecise(a.raw)); }
        public static F64 Sqrt(F64 a) { return FromRaw(Fixed64.Sqrt(a.raw)); }
        public static F64 SqrtFast(F64 a) { return FromRaw(Fixed64.SqrtFast(a.raw)); }
        public static F64 SqrtFastest(F64 a) { return FromRaw(Fixed64.SqrtFastest(a.raw)); }
        public static F64 RSqrt(F64 a) { return FromRaw(Fixed64.RSqrt(a.raw)); }
        public static F64 RSqrtFast(F64 a) { return FromRaw(Fixed64.RSqrtFast(a.raw)); }
        public static F64 RSqrtFastest(F64 a) { return FromRaw(Fixed64.RSqrtFastest(a.raw)); }
        public static F64 Rcp(F64 a) { return FromRaw(Fixed64.Rcp(a.raw)); }
        public static F64 RcpFast(F64 a) { return FromRaw(Fixed64.RcpFast(a.raw)); }
        public static F64 RcpFastest(F64 a) { return FromRaw(Fixed64.RcpFastest(a.raw)); }
        public static F64 Exp(F64 a) { return FromRaw(Fixed64.Exp(a.raw)); }
        public static F64 ExpFast(F64 a) { return FromRaw(Fixed64.ExpFast(a.raw)); }
        public static F64 ExpFastest(F64 a) { return FromRaw(Fixed64.ExpFastest(a.raw)); }
        public static F64 Exp2(F64 a) { return FromRaw(Fixed64.Exp2(a.raw)); }
        public static F64 Exp2Fast(F64 a) { return FromRaw(Fixed64.Exp2Fast(a.raw)); }
        public static F64 Exp2Fastest(F64 a) { return FromRaw(Fixed64.Exp2Fastest(a.raw)); }
        public static F64 Log(F64 a) { return FromRaw(Fixed64.Log(a.raw)); }
        public static F64 LogFast(F64 a) { return FromRaw(Fixed64.LogFast(a.raw)); }
        public static F64 LogFastest(F64 a) { return FromRaw(Fixed64.LogFastest(a.raw)); }
        public static F64 Log2(F64 a) { return FromRaw(Fixed64.Log2(a.raw)); }
        public static F64 Log2Fast(F64 a) { return FromRaw(Fixed64.Log2Fast(a.raw)); }
        public static F64 Log2Fastest(F64 a) { return FromRaw(Fixed64.Log2Fastest(a.raw)); }

        public static F64 Sin(F64 a) { return FromRaw(Fixed64.Sin(a.raw)); }
        public static F64 SinFast(F64 a) { return FromRaw(Fixed64.SinFast(a.raw)); }
        public static F64 SinFastest(F64 a) { return FromRaw(Fixed64.SinFastest(a.raw)); }
        public static F64 Cos(F64 a) { return FromRaw(Fixed64.Cos(a.raw)); }
        public static F64 CosFast(F64 a) { return FromRaw(Fixed64.CosFast(a.raw)); }
        public static F64 CosFastest(F64 a) { return FromRaw(Fixed64.CosFastest(a.raw)); }
        public static F64 Tan(F64 a) { return FromRaw(Fixed64.Tan(a.raw)); }
        public static F64 TanFast(F64 a) { return FromRaw(Fixed64.TanFast(a.raw)); }
        public static F64 TanFastest(F64 a) { return FromRaw(Fixed64.TanFastest(a.raw)); }
        public static F64 Asin(F64 a) { return FromRaw(Fixed64.Asin(a.raw)); }
        public static F64 AsinFast(F64 a) { return FromRaw(Fixed64.AsinFast(a.raw)); }
        public static F64 AsinFastest(F64 a) { return FromRaw(Fixed64.AsinFastest(a.raw)); }
        public static F64 Acos(F64 a) { return FromRaw(Fixed64.Acos(a.raw)); }
        public static F64 AcosFast(F64 a) { return FromRaw(Fixed64.AcosFast(a.raw)); }
        public static F64 AcosFastest(F64 a) { return FromRaw(Fixed64.AcosFastest(a.raw)); }
        public static F64 Atan(F64 a) { return FromRaw(Fixed64.Atan(a.raw)); }
        public static F64 AtanFast(F64 a) { return FromRaw(Fixed64.AtanFast(a.raw)); }
        public static F64 AtanFastest(F64 a) { return FromRaw(Fixed64.AtanFastest(a.raw)); }
        public static F64 Atan2(F64 y, F64 x) { return FromRaw(Fixed64.Atan2(y.raw, x.raw)); }
        public static F64 Atan2Fast(F64 y, F64 x) { return FromRaw(Fixed64.Atan2Fast(y.raw, x.raw)); }
        public static F64 Atan2Fastest(F64 y, F64 x) { return FromRaw(Fixed64.Atan2Fastest(y.raw, x.raw)); }
        public static F64 Pow(F64 a, F64 b) { return FromRaw(Fixed64.Pow(a.raw, b.raw)); }
        public static F64 PowFast(F64 a, F64 b) { return FromRaw(Fixed64.PowFast(a.raw, b.raw)); }
        public static F64 PowFastest(F64 a, F64 b) { return FromRaw(Fixed64.PowFastest(a.raw, b.raw)); }

        public static F64 Min(F64 a, F64 b) { return FromRaw(Fixed64.Min(a.raw, b.raw)); }
        public static F64 Max(F64 a, F64 b) { return FromRaw(Fixed64.Max(a.raw, b.raw)); }

        public static F64 FromRaw(long raw)
        {
            F64 r = new F64();
            r.raw = raw;
            return r;
        }

        public static F64 FromInt(int v) { return new F64(v); }
        public static F64 FromFloat(float v) { return new F64(v); }
        public static F64 FromDouble(double v) { return new F64(v); }
        public static F64 FromF32(F32 v) { return new F64(v); }

        public override bool Equals(object obj)
        {
            if (!(obj is F64))
                return false;
            return ((F64)obj).raw == raw;
        }

        public int CompareTo(F64 other)
        {
            if (raw < other.raw) return -1;
            if (raw > other.raw) return +1;
            return 0;
        }

        public override string ToString()
        {
            return Fixed64.ToString(raw);
        }

        public override int GetHashCode()
        {
            return (int)raw | (int)(raw >> 32);
        }
    }
}
