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

using FixPointCS;

namespace FixMath
{
    /// <summary>
    /// Vector2 struct with signed 32.32 fixed point components.
    /// </summary>
    [Serializable]
    public struct F32Vec3 : IEquatable<F32Vec3>
    {
        // Constants
        public static F32Vec3 Zero     { get { return new F32Vec3(F32.Zero, F32.Zero, F32.Zero); } }
        public static F32Vec3 One      { get { return new F32Vec3(F32.One, F32.One, F32.One); } }
        public static F32Vec3 Down     { get { return new F32Vec3(F32.Zero, F32.Neg1, F32.Zero); } }
        public static F32Vec3 Up       { get { return new F32Vec3(F32.Zero, F32.One, F32.Zero); } }
        public static F32Vec3 Left     { get { return new F32Vec3(F32.Neg1, F32.Zero, F32.Zero); } }
        public static F32Vec3 Right    { get { return new F32Vec3(F32.One, F32.Zero, F32.Zero); } }

        // Components
        public F32 x;
        public F32 y;
        public F32 z;

        public F32Vec3 (F32 x, F32 y, F32 z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static F32Vec3 FromInt(int x, int y, int z) { return new F32Vec3(F32.FromInt(x), F32.FromInt(y), F32.FromInt(z)); }
        public static F32Vec3 FromFloat(float x, float y, float z) { return new F32Vec3(F32.FromFloat(x), F32.FromFloat(y), F32.FromFloat(z)); }
        public static F32Vec3 FromDouble(double x, double y, double z) { return new F32Vec3(F32.FromDouble(x), F32.FromDouble(y), F32.FromDouble(z)); }

        // Operators
        public static F32Vec3 operator -(F32Vec3 a) { return new F32Vec3(-a.x, -a.y, -a.z); }

        public static F32Vec3 operator +(F32Vec3 a, F32Vec3 b) { return new F32Vec3(a.x + b.x, a.y + b.y, a.z + b.z); }
        public static F32Vec3 operator -(F32Vec3 a, F32Vec3 b) { return new F32Vec3(a.x - b.x, a.y - b.y, a.z - b.z); }
        public static F32Vec3 operator *(F32Vec3 a, F32Vec3 b) { return new F32Vec3(a.x * b.x, a.y * b.y, a.z * b.z); }
        public static F32Vec3 operator /(F32Vec3 a, F32Vec3 b) { return new F32Vec3(a.x / b.x, a.y / b.y, a.z / b.z); }
        public static F32Vec3 operator %(F32Vec3 a, F32Vec3 b) { return new F32Vec3(a.x % b.x, a.y % b.y, a.z % b.z); }

        public static F32Vec3 operator +(F32 a, F32Vec3 b) { return new F32Vec3(a + b.x, a + b.y, a + b.z); }
        public static F32Vec3 operator +(F32Vec3 a, F32 b) { return new F32Vec3(a.x + b, a.y + b, a.z + b); }
        public static F32Vec3 operator -(F32 a, F32Vec3 b) { return new F32Vec3(a - b.x, a - b.y, a - b.z); }
        public static F32Vec3 operator -(F32Vec3 a, F32 b) { return new F32Vec3(a.x - b, a.y - b, a.z - b); }
        public static F32Vec3 operator *(F32 a, F32Vec3 b) { return new F32Vec3(a * b.x, a * b.y, a * b.z); }
        public static F32Vec3 operator *(F32Vec3 a, F32 b) { return new F32Vec3(a.x * b, a.y * b, a.z * b); }
        public static F32Vec3 operator /(F32 a, F32Vec3 b) { return new F32Vec3(a / b.x, a / b.y, a / b.z); }
        public static F32Vec3 operator /(F32Vec3 a, F32 b) { return new F32Vec3(a.x / b, a.y / b, a.z / b); }
        public static F32Vec3 operator %(F32 a, F32Vec3 b) { return new F32Vec3(a % b.x, a % b.y, a % b.z); }
        public static F32Vec3 operator %(F32Vec3 a, F32 b) { return new F32Vec3(a.x % b, a.y % b, a.z % b); }

        public static bool operator ==(F32Vec3 a, F32Vec3 b) { return a.x == b.x && a.y == b.y && a.z == b.z; }
        public static bool operator !=(F32Vec3 a, F32Vec3 b) { return a.x != b.x || a.y != b.y || a.z != b.z; }

        public static F32Vec3 Div(F32Vec3 a, F32 b) { F32 oob = F32.Rcp(b); return new F32Vec3(a.x * oob, a. y * oob, a.z * oob); }
        public static F32Vec3 DivFast(F32Vec3 a, F32 b) { F32 oob = F32.RcpFast(b); return new F32Vec3(a.x * oob, a.y * oob, a.z * oob); }
        public static F32Vec3 DivFastest(F32Vec3 a, F32 b) { F32 oob = F32.RcpFastest(b); return new F32Vec3(a.x * oob, a.y * oob, a.z * oob); }
        public static F32Vec3 Div(F32Vec3 a, F32Vec3 b) { return new F32Vec3(F32.Div(a.x, b.x), F32.Div(a.y, b.y), F32.Div(a.z, b.z)); }
        public static F32Vec3 DivFast(F32Vec3 a, F32Vec3 b) { return new F32Vec3(F32.DivFast(a.x, b.x), F32.DivFast(a.y, b.y), F32.DivFast(a.z, b.z)); }
        public static F32Vec3 DivFastest(F32Vec3 a, F32Vec3 b) { return new F32Vec3(F32.DivFastest(a.x, b.x), F32.DivFastest(a.y, b.y), F32.DivFastest(a.z, b.z)); }
        public static F32Vec3 SqrtPrecise(F32Vec3 a) { return new F32Vec3(F32.SqrtPrecise(a.x), F32.SqrtPrecise(a.y), F32.SqrtPrecise(a.z)); }
        public static F32Vec3 Sqrt(F32Vec3 a) { return new F32Vec3(F32.Sqrt(a.x), F32.Sqrt(a.y), F32.Sqrt(a.z)); }
        public static F32Vec3 SqrtFast(F32Vec3 a) { return new F32Vec3(F32.SqrtFast(a.x), F32.SqrtFast(a.y), F32.SqrtFast(a.z)); }
        public static F32Vec3 SqrtFastest(F32Vec3 a) { return new F32Vec3(F32.SqrtFastest(a.x), F32.SqrtFastest(a.y), F32.SqrtFastest(a.z)); }
        public static F32Vec3 RSqrt(F32Vec3 a) { return new F32Vec3(F32.RSqrt(a.x), F32.RSqrt(a.y), F32.RSqrt(a.z)); }
        public static F32Vec3 RSqrtFast(F32Vec3 a) { return new F32Vec3(F32.RSqrtFast(a.x), F32.RSqrtFast(a.y), F32.RSqrtFast(a.z)); }
        public static F32Vec3 RSqrtFastest(F32Vec3 a) { return new F32Vec3(F32.RSqrtFastest(a.x), F32.RSqrtFastest(a.y), F32.RSqrtFastest(a.z)); }
        public static F32Vec3 Rcp(F32Vec3 a) { return new F32Vec3(F32.Rcp(a.x), F32.Rcp(a.y), F32.Rcp(a.z)); }
        public static F32Vec3 RcpFast(F32Vec3 a) { return new F32Vec3(F32.RcpFast(a.x), F32.RcpFast(a.y), F32.RcpFast(a.z)); }
        public static F32Vec3 RcpFastest(F32Vec3 a) { return new F32Vec3(F32.RcpFastest(a.x), F32.RcpFastest(a.y), F32.RcpFastest(a.z)); }
        public static F32Vec3 Exp(F32Vec3 a) { return new F32Vec3(F32.Exp(a.x), F32.Exp(a.y), F32.Exp(a.z)); }
        public static F32Vec3 ExpFast(F32Vec3 a) { return new F32Vec3(F32.ExpFast(a.x), F32.ExpFast(a.y), F32.ExpFast(a.z)); }
        public static F32Vec3 ExpFastest(F32Vec3 a) { return new F32Vec3(F32.ExpFastest(a.x), F32.ExpFastest(a.y), F32.ExpFastest(a.z)); }
        public static F32Vec3 Exp2(F32Vec3 a) { return new F32Vec3(F32.Exp2(a.x), F32.Exp2(a.y), F32.Exp2(a.z)); }
        public static F32Vec3 Exp2Fast(F32Vec3 a) { return new F32Vec3(F32.Exp2Fast(a.x), F32.Exp2Fast(a.y), F32.Exp2Fast(a.z)); }
        public static F32Vec3 Exp2Fastest(F32Vec3 a) { return new F32Vec3(F32.Exp2Fastest(a.x), F32.Exp2Fastest(a.y), F32.Exp2Fastest(a.z)); }
        public static F32Vec3 Log(F32Vec3 a) { return new F32Vec3(F32.Log(a.x), F32.Log(a.y), F32.Log(a.z)); }
        public static F32Vec3 LogFast(F32Vec3 a) { return new F32Vec3(F32.LogFast(a.x), F32.LogFast(a.y), F32.LogFast(a.z)); }
        public static F32Vec3 LogFastest(F32Vec3 a) { return new F32Vec3(F32.LogFastest(a.x), F32.LogFastest(a.y), F32.LogFastest(a.z)); }
        public static F32Vec3 Log2(F32Vec3 a) { return new F32Vec3(F32.Log2(a.x), F32.Log2(a.y), F32.Log2(a.z)); }
        public static F32Vec3 Log2Fast(F32Vec3 a) { return new F32Vec3(F32.Log2Fast(a.x), F32.Log2Fast(a.y), F32.Log2Fast(a.z)); }
        public static F32Vec3 Log2Fastest(F32Vec3 a) { return new F32Vec3(F32.Log2Fastest(a.x), F32.Log2Fastest(a.y), F32.Log2Fastest(a.z)); }
        public static F32Vec3 Sin(F32Vec3 a) { return new F32Vec3(F32.Sin(a.x), F32.Sin(a.y), F32.Sin(a.z)); }
        public static F32Vec3 SinFast(F32Vec3 a) { return new F32Vec3(F32.SinFast(a.x), F32.SinFast(a.y), F32.SinFast(a.z)); }
        public static F32Vec3 SinFastest(F32Vec3 a) { return new F32Vec3(F32.SinFastest(a.x), F32.SinFastest(a.y), F32.SinFastest(a.z)); }
        public static F32Vec3 Cos(F32Vec3 a) { return new F32Vec3(F32.Cos(a.x), F32.Cos(a.y), F32.Cos(a.z)); }
        public static F32Vec3 CosFast(F32Vec3 a) { return new F32Vec3(F32.CosFast(a.x), F32.CosFast(a.y), F32.CosFast(a.z)); }
        public static F32Vec3 CosFastest(F32Vec3 a) { return new F32Vec3(F32.CosFastest(a.x), F32.CosFastest(a.y), F32.CosFastest(a.z)); }

        public static F32Vec3 Pow(F32Vec3 a, F32 b) { return new F32Vec3(F32.Pow(a.x, b), F32.Pow(a.y, b), F32.Pow(a.z, b)); }
        public static F32Vec3 PowFast(F32Vec3 a, F32 b) { return new F32Vec3(F32.PowFast(a.x, b), F32.PowFast(a.y, b), F32.PowFast(a.z, b)); }
        public static F32Vec3 PowFastest(F32Vec3 a, F32 b) { return new F32Vec3(F32.PowFastest(a.x, b), F32.PowFastest(a.y, b), F32.PowFastest(a.z, b)); }
        public static F32Vec3 Pow(F32 a, F32Vec3 b) { return new F32Vec3(F32.Pow(a, b.x), F32.Pow(a, b.y), F32.Pow(a, b.z)); }
        public static F32Vec3 PowFast(F32 a, F32Vec3 b) { return new F32Vec3(F32.PowFast(a, b.x), F32.PowFast(a, b.y), F32.PowFast(a, b.z)); }
        public static F32Vec3 PowFastest(F32 a, F32Vec3 b) { return new F32Vec3(F32.PowFastest(a, b.x), F32.PowFastest(a, b.y), F32.PowFastest(a, b.z)); }
        public static F32Vec3 Pow(F32Vec3 a, F32Vec3 b) { return new F32Vec3(F32.Pow(a.x, b.x), F32.Pow(a.y, b.y), F32.Pow(a.z, b.z)); }
        public static F32Vec3 PowFast(F32Vec3 a, F32Vec3 b) { return new F32Vec3(F32.PowFast(a.x, b.x), F32.PowFast(a.y, b.y), F32.PowFast(a.z, b.z)); }
        public static F32Vec3 PowFastest(F32Vec3 a, F32Vec3 b) { return new F32Vec3(F32.PowFastest(a.x, b.x), F32.PowFastest(a.y, b.y), F32.PowFastest(a.z, b.z)); }

        public static F32 Length(F32Vec3 a) { return F32.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z); }
        public static F32 LengthFast(F32Vec3 a) { return F32.SqrtFast(a.x * a.x + a.y * a.y + a.z * a.z); }
        public static F32 LengthFastest(F32Vec3 a) { return F32.SqrtFastest(a.x * a.x + a.y * a.y + a.z * a.z); }
        public static F32 LengthSqr(F32Vec3 a) { return (a.x * a.x + a.y * a.y + a.z * a.z); }
        public static F32Vec3 Normalize(F32Vec3 a) { F32 ooLen = F32.RSqrt(LengthSqr(a)); return ooLen * a; }
        public static F32Vec3 NormalizeFast(F32Vec3 a) { F32 ooLen = F32.RSqrtFast(LengthSqr(a)); return ooLen * a; }
        public static F32Vec3 NormalizeFastest(F32Vec3 a) { F32 ooLen = F32.RSqrtFastest(LengthSqr(a)); return ooLen * a; }

        public static F32 Dot(F32Vec3 a, F32Vec3 b) { return (a.x * b.x + a.y * b.y + a.z * b.z); }
        public static F32 Distance(F32Vec3 a, F32Vec3 b) { return Length(a - b); }
        public static F32 DistanceFast(F32Vec3 a, F32Vec3 b) { return LengthFast(a - b); }
        public static F32 DistanceFastest(F32Vec3 a, F32Vec3 b) { return LengthFastest(a - b); }

        public static F32Vec3 Lerp(F32Vec3 a, F32Vec3 b, F32 t) { return a + t*(b - a); }

        public static F32Vec3 Cross(F32Vec3 a, F32Vec3 b)
        {
            return new F32Vec3(
                (a.y * b.z) - (a.z * b.y),
                (a.z * b.x) - (a.x * b.z),
                (a.x * b.y) - (a.y * b.x));
        }

        public static F32Vec3 Min(F32Vec3 a, F32Vec3 b) { return new F32Vec3(F32.Min(a.x, b.x), F32.Min(a.y, b.y), F32.Min(a.z, b.z)); }
        public static F32Vec3 Max(F32Vec3 a, F32Vec3 b) { return new F32Vec3(F32.Max(a.x, b.x), F32.Max(a.y, b.y), F32.Max(a.z, b.z)); }

        public bool Equals(F32Vec3 other)
        {
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is F32Vec3))
                return false;
            return ((F32Vec3)obj) == this;
        }

        public override string ToString()
        {
            return "(" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ")";
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode()*7919 ^ z.GetHashCode()*4513;
        }
    }
}
