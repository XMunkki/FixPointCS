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
    /// Signed 16.16 fixed point value struct.
    /// </summary>
    [Serializable]
    public struct F32 : IComparable<F32>
    {
        // Constants
        public static F32 Neg1 { get { return FromRaw(Fixed32.Neg1); } }
        public static F32 Zero { get { return FromRaw(Fixed32.Zero); } }
        public static F32 Half { get { return FromRaw(Fixed32.Half); } }
        public static F32 One { get { return FromRaw(Fixed32.One); } }
        public static F32 Two { get { return FromRaw(Fixed32.Two); } }
        public static F32 Pi { get { return FromRaw(Fixed32.Pi); } }
        public static F32 Pi2 { get { return FromRaw(Fixed32.Pi2); } }
        public static F32 PiHalf { get { return FromRaw(Fixed32.PiHalf); } }
        public static F32 E { get { return FromRaw(Fixed32.E); } }

        public static F32 MinValue { get { return FromRaw(Fixed32.MinValue); } }
        public static F32 MaxValue { get { return FromRaw(Fixed32.MaxValue); } }

        // Raw fixed point value
        public int raw;

        // Constructors
        public F32(int v) { raw = Fixed32.FromInt(v); }
        public F32(float v) { raw = Fixed32.FromFloat(v); }
        public F32(double v) { raw = Fixed32.FromDouble(v); }
        public F32(F64 v) { raw = (int)(v.raw >> 16); }

        // Conversions
        public static int FloorToInt(F32 a) { return Fixed32.FloorToInt(a.raw); }
        public static int CeilToInt(F32 a) { return Fixed32.CeilToInt(a.raw); }
        public static int RoundToInt(F32 a) { return Fixed32.RoundToInt(a.raw); }
        public float Float { get { return Fixed32.ToFloat(raw); } }
        public double Double { get { return Fixed32.ToDouble(raw); } }

        // Operators
        public static F32 operator -(F32 v1) { return FromRaw(-v1.raw); }

        public static F32 operator +(F32 v1, F32 v2) { return FromRaw(v1.raw + v2.raw); }
        public static F32 operator -(F32 v1, F32 v2) { return FromRaw(v1.raw - v2.raw); }
        public static F32 operator *(F32 v1, F32 v2) { return FromRaw(Fixed32.Mul(v1.raw, v2.raw)); }
        public static F32 operator /(F32 v1, F32 v2) { return FromRaw(Fixed32.DivPrecise(v1.raw, v2.raw)); }
        public static F32 operator %(F32 v1, F32 v2) { return FromRaw(Fixed32.Mod(v1.raw, v2.raw)); }

        public static F32 operator +(F32 v1, int v2) { return FromRaw(v1.raw + Fixed32.FromInt(v2)); }
        public static F32 operator +(int v1, F32 v2) { return FromRaw(Fixed32.FromInt(v1) + v2.raw); }
        public static F32 operator -(F32 v1, int v2) { return FromRaw(v1.raw - Fixed32.FromInt(v2)); }
        public static F32 operator -(int v1, F32 v2) { return FromRaw(Fixed32.FromInt(v1) - v2.raw); }
        public static F32 operator *(F32 v1, int v2) { return FromRaw(v1.raw * (int)v2); }
        public static F32 operator *(int v1, F32 v2) { return FromRaw((int)v1 * v2.raw); }
        public static F32 operator /(F32 v1, int v2) { return FromRaw(v1.raw / (int)v2); }
        public static F32 operator /(int v1, F32 v2) { return FromRaw(Fixed32.DivPrecise(Fixed32.FromInt(v1), v2.raw)); }
        public static F32 operator %(F32 v1, int v2) { return FromRaw(Fixed32.Mod(v1.raw, Fixed32.FromInt(v2))); }
        public static F32 operator %(int v1, F32 v2) { return FromRaw(Fixed32.Mod(Fixed32.FromInt(v1), v2.raw)); }

        public static F32 operator ++(F32 v1) { return FromRaw(v1.raw + Fixed32.One); }
        public static F32 operator --(F32 v1) { return FromRaw(v1.raw - Fixed32.One); }

        public static bool operator ==(F32 v1, F32 v2) { return v1.raw == v2.raw; }
        public static bool operator !=(F32 v1, F32 v2) { return v1.raw != v2.raw; }
        public static bool operator <(F32 v1, F32 v2) { return v1.raw < v2.raw; }
        public static bool operator <=(F32 v1, F32 v2) { return v1.raw <= v2.raw; }
        public static bool operator >(F32 v1, F32 v2) { return v1.raw > v2.raw; }
        public static bool operator >=(F32 v1, F32 v2) { return v1.raw >= v2.raw; }

        public static bool operator ==(int v1, F32 v2) { return Fixed32.FromInt(v1) == v2.raw; }
        public static bool operator ==(F32 v1, int v2) { return v1.raw == Fixed32.FromInt(v2); }
        public static bool operator !=(int v1, F32 v2) { return Fixed32.FromInt(v1) != v2.raw; }
        public static bool operator !=(F32 v1, int v2) { return v1.raw != Fixed32.FromInt(v2); }
        public static bool operator <(int v1, F32 v2) { return Fixed32.FromInt(v1) < v2.raw; }
        public static bool operator <(F32 v1, int v2) { return v1.raw < Fixed32.FromInt(v2); }
        public static bool operator <=(int v1, F32 v2) { return Fixed32.FromInt(v1) <= v2.raw; }
        public static bool operator <=(F32 v1, int v2) { return v1.raw <= Fixed32.FromInt(v2); }
        public static bool operator >(int v1, F32 v2) { return Fixed32.FromInt(v1) > v2.raw; }
        public static bool operator >(F32 v1, int v2) { return v1.raw > Fixed32.FromInt(v2); }
        public static bool operator >=(int v1, F32 v2) { return Fixed32.FromInt(v1) >= v2.raw; }
        public static bool operator >=(F32 v1, int v2) { return v1.raw >= Fixed32.FromInt(v2); }

        public static F32 Abs(F32 a) { return FromRaw(Fixed32.Abs(a.raw)); }
        public static F32 Nabs(F32 a) { return FromRaw(Fixed32.Nabs(a.raw)); }
        public static F32 Ceil(F32 a) { return FromRaw(Fixed32.Ceil(a.raw)); }
        public static F32 Floor(F32 a) { return FromRaw(Fixed32.Floor(a.raw)); }
        public static F32 Round(F32 a) { return FromRaw(Fixed32.Round(a.raw)); }
        public static F32 Fract(F32 a) { return FromRaw(Fixed32.Fract(a.raw)); }
        public static F32 Div(F32 a, F32 b) { return FromRaw(Fixed32.Div(a.raw, b.raw)); }
        public static F32 DivFast(F32 a, F32 b) { return FromRaw(Fixed32.DivFast(a.raw, b.raw)); }
        public static F32 DivFastest(F32 a, F32 b) { return FromRaw(Fixed32.DivFastest(a.raw, b.raw)); }
        // public static F32 SqrtPrecise(F32 a) { return FromRaw(Fixed32.SqrtPrecise(a.raw)); }
        public static F32 Sqrt(F32 a) { return FromRaw(Fixed32.Sqrt(a.raw)); }
        public static F32 SqrtFast(F32 a) { return FromRaw(Fixed32.SqrtFast(a.raw)); }
        public static F32 SqrtFastest(F32 a) { return FromRaw(Fixed32.SqrtFastest(a.raw)); }
        public static F32 RSqrt(F32 a) { return FromRaw(Fixed32.RSqrt(a.raw)); }
        public static F32 RSqrtFast(F32 a) { return FromRaw(Fixed32.RSqrtFast(a.raw)); }
        public static F32 RSqrtFastest(F32 a) { return FromRaw(Fixed32.RSqrtFastest(a.raw)); }
        public static F32 Rcp(F32 a) { return FromRaw(Fixed32.Rcp(a.raw)); }
        public static F32 RcpFast(F32 a) { return FromRaw(Fixed32.RcpFast(a.raw)); }
        public static F32 RcpFastest(F32 a) { return FromRaw(Fixed32.RcpFastest(a.raw)); }
        public static F32 Exp(F32 a) { return FromRaw(Fixed32.Exp(a.raw)); }
        public static F32 ExpFast(F32 a) { return FromRaw(Fixed32.ExpFast(a.raw)); }
        public static F32 ExpFastest(F32 a) { return FromRaw(Fixed32.ExpFastest(a.raw)); }
        public static F32 Exp2(F32 a) { return FromRaw(Fixed32.Exp2(a.raw)); }
        public static F32 Exp2Fast(F32 a) { return FromRaw(Fixed32.Exp2Fast(a.raw)); }
        public static F32 Exp2Fastest(F32 a) { return FromRaw(Fixed32.Exp2Fastest(a.raw)); }
        public static F32 Log(F32 a) { return FromRaw(Fixed32.Log(a.raw)); }
        public static F32 LogFast(F32 a) { return FromRaw(Fixed32.LogFast(a.raw)); }
        public static F32 LogFastest(F32 a) { return FromRaw(Fixed32.LogFastest(a.raw)); }
        public static F32 Log2(F32 a) { return FromRaw(Fixed32.Log2(a.raw)); }
        public static F32 Log2Fast(F32 a) { return FromRaw(Fixed32.Log2Fast(a.raw)); }
        public static F32 Log2Fastest(F32 a) { return FromRaw(Fixed32.Log2Fastest(a.raw)); }

        public static F32 Sin(F32 a) { return FromRaw(Fixed32.Sin(a.raw)); }
        public static F32 SinFast(F32 a) { return FromRaw(Fixed32.SinFast(a.raw)); }
        public static F32 SinFastest(F32 a) { return FromRaw(Fixed32.SinFastest(a.raw)); }
        public static F32 Cos(F32 a) { return FromRaw(Fixed32.Cos(a.raw)); }
        public static F32 CosFast(F32 a) { return FromRaw(Fixed32.CosFast(a.raw)); }
        public static F32 CosFastest(F32 a) { return FromRaw(Fixed32.CosFastest(a.raw)); }
        public static F32 Tan(F32 a) { return FromRaw(Fixed32.Tan(a.raw)); }
        public static F32 TanFast(F32 a) { return FromRaw(Fixed32.TanFast(a.raw)); }
        public static F32 TanFastest(F32 a) { return FromRaw(Fixed32.TanFastest(a.raw)); }
        public static F32 Asin(F32 a) { return FromRaw(Fixed32.Asin(a.raw)); }
        public static F32 AsinFast(F32 a) { return FromRaw(Fixed32.AsinFast(a.raw)); }
        public static F32 AsinFastest(F32 a) { return FromRaw(Fixed32.AsinFastest(a.raw)); }
        public static F32 Acos(F32 a) { return FromRaw(Fixed32.Acos(a.raw)); }
        public static F32 AcosFast(F32 a) { return FromRaw(Fixed32.AcosFast(a.raw)); }
        public static F32 AcosFastest(F32 a) { return FromRaw(Fixed32.AcosFastest(a.raw)); }
        public static F32 Atan(F32 a) { return FromRaw(Fixed32.Atan(a.raw)); }
        public static F32 AtanFast(F32 a) { return FromRaw(Fixed32.AtanFast(a.raw)); }
        public static F32 AtanFastest(F32 a) { return FromRaw(Fixed32.AtanFastest(a.raw)); }
        public static F32 Atan2(F32 y, F32 x) { return FromRaw(Fixed32.Atan2(y.raw, x.raw)); }
        public static F32 Atan2Fast(F32 y, F32 x) { return FromRaw(Fixed32.Atan2Fast(y.raw, x.raw)); }
        public static F32 Atan2Fastest(F32 y, F32 x) { return FromRaw(Fixed32.Atan2Fastest(y.raw, x.raw)); }
        public static F32 Pow(F32 a, F32 b) { return FromRaw(Fixed32.Pow(a.raw, b.raw)); }
        public static F32 PowFast(F32 a, F32 b) { return FromRaw(Fixed32.PowFast(a.raw, b.raw)); }
        public static F32 PowFastest(F32 a, F32 b) { return FromRaw(Fixed32.PowFastest(a.raw, b.raw)); }

        public static F32 Min(F32 a, F32 b) { return FromRaw(Fixed32.Min(a.raw, b.raw)); }
        public static F32 Max(F32 a, F32 b) { return FromRaw(Fixed32.Max(a.raw, b.raw)); }

        public static F32 FromRaw(int raw)
        {
            F32 r = new F32();
            r.raw = raw;
            return r;
        }

        public static F32 FromInt(int v) { return new F32(v); }
        public static F32 FromFloat(float v) { return new F32(v); }
        public static F32 FromDouble(double v) { return new F32(v); }
        public static F32 FromF64(F64 v) { return new F32(v); }

        public override bool Equals(object obj)
        {
            if (!(obj is F32))
                return false;
            return ((F32)obj).raw == raw;
        }

        public int CompareTo(F32 other)
        {
            if (raw < other.raw) return -1;
            if (raw > other.raw) return +1;
            return 0;
        }

        public override string ToString()
        {
            return Fixed32.ToString(raw);
        }

        public override int GetHashCode()
        {
            return (int)raw | (int)(raw >> 32);
        }
    }
}
