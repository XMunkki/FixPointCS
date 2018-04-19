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
using System.Collections.Generic;
using System.Text;

namespace FixPointCS
{
    /// <summary>
    /// Fixed point (31.32 signed) value struct.
    /// </summary>
    public struct F64
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

        // The raw FP value
        public long raw;

        // Conversions
        public int FloorToInt { get { return Fixed64.FloorToInt(raw); } }
        public int CeilToInt { get { return Fixed64.CeilToInt(raw); } }
        public int RoundToInt { get { return Fixed64.RoundToInt(raw); } }
        public float Float { get { return Fixed64.ToFloat(raw); } }
        public double Double { get { return Fixed64.ToDouble(raw); } }

        // Operators
        public static F64 operator -(F64 v1) { return FromRaw(-v1.raw); }

        public static F64 operator +(F64 v1, F64 v2) { return FromRaw(v1.raw + v2.raw); }
        public static F64 operator -(F64 v1, F64 v2) { return FromRaw(v1.raw - v2.raw); }
        public static F64 operator *(F64 v1, F64 v2) { return FromRaw(Fixed64.Mul(v1.raw, v2.raw)); }
        public static F64 operator /(F64 v1, F64 v2) { return FromRaw(Fixed64.Div(v1.raw, v2.raw)); }
        public static F64 operator %(F64 v1, F64 v2) { return FromRaw(Fixed64.Mod(v1.raw, v2.raw)); }

        public static F64 operator +(F64 v1, int v2) { return FromRaw(v1.raw + Fixed64.FromInt(v2)); }
        public static F64 operator +(int v1, F64 v2) { return FromRaw(Fixed64.FromInt(v1) + v2.raw); }
        public static F64 operator -(F64 v1, int v2) { return FromRaw(v1.raw - Fixed64.FromInt(v2)); }
        public static F64 operator -(int v1, F64 v2) { return FromRaw(Fixed64.FromInt(v1) - v2.raw); }
        public static F64 operator *(F64 v1, int v2) { return FromRaw(v1.raw * (long)v2); }
        public static F64 operator *(int v1, F64 v2) { return FromRaw(Fixed64.Mul(Fixed64.FromInt(v1), v2.raw)); }
        public static F64 operator /(F64 v1, int v2) { return FromRaw(v1.raw / (long)v2); }
        public static F64 operator /(int v1, F64 v2) { return FromRaw(Fixed64.Div(Fixed64.FromInt(v1), v2.raw)); }
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

        public F64(int v) { raw = Fixed64.FromInt(v); }
        public F64(float v) { raw = Fixed64.FromFloat(v); }
        public F64(double v) { raw = Fixed64.FromDouble(v); }

        // \todo [petri] make static?
        public F64 Abs() { return FromRaw(Fixed64.Abs(raw)); }
        public F64 Nabs() { return FromRaw(Fixed64.Nabs(raw)); }
        public F64 Ceil() { return FromRaw(Fixed64.Ceil(raw)); }
        public F64 Floor() { return FromRaw(Fixed64.Floor(raw)); }
        public F64 Round() { return FromRaw(Fixed64.Round(raw)); }
        public F64 Sqrt() { return FromRaw(Fixed64.Sqrt(raw)); }
        public F64 SqrtFast() { return FromRaw(Fixed64.SqrtFast(raw)); }
        public F64 RSqrt() { return FromRaw(Fixed64.RSqrt(raw)); }
        public F64 Rcp() { return FromRaw(Fixed64.RcpFast(raw)); }
        public F64 Exp() { return FromRaw(Fixed64.Exp(raw)); }
        public F64 Exp2() { return FromRaw(Fixed64.Exp2(raw)); }
        public F64 Log() { return FromRaw(Fixed64.Log(raw)); }
        public F64 Log2() { return FromRaw(Fixed64.Log2(raw)); }

        public F64 Sin() { return FromRaw(Fixed64.SinPoly5(raw)); }
        public F64 Cos() { return FromRaw(Fixed64.CosPoly5(raw)); }
        public F64 Tan() { return FromRaw(Fixed64.TanPoly5(raw)); }
        public F64 Asin() { return FromRaw(Fixed64.Asin(raw)); }
        public F64 Acos() { return FromRaw(Fixed64.Acos(raw)); }
        public F64 Atan() { return FromRaw(Fixed64.Atan(raw)); }
        public static F64 Atan2(F64 y, F64 x) { return FromRaw(Fixed64.Atan2(y.raw, x.raw)); }

        public static F64 Min(F64 a, F64 b) { return FromRaw(Fixed64.Min(a.raw, b.raw)); }
        public static F64 Max(F64 a, F64 b) { return FromRaw(Fixed64.Max(a.raw, b.raw)); }
        public static F64 Pow(F64 a, F64 b) { return FromRaw(Fixed64.Pow(a.raw, b.raw)); }

        public static F64 FromRaw(long raw)
        {
            F64 r = new F64();
            r.raw = raw;
            return r;
        }

        public static F64 FromInt(int v)
        {
            return new F64(v);
        }

        public static F64 FromFloat(float v)
        {
            return new F64(v);
        }

        public static F64 FromDouble(double v)
        {
            return new F64(v);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is F64))
                return false;
            return ((F64)obj).raw == raw;
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
