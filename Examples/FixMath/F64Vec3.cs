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
    public struct F64Vec3
    {
        // Constants
        public static F64Vec3 Zero     { get { return new F64Vec3(F64.Zero, F64.Zero, F64.Zero); } }
        public static F64Vec3 One      { get { return new F64Vec3(F64.One, F64.One, F64.One); } }
        public static F64Vec3 Down     { get { return new F64Vec3(F64.Zero, F64.Neg1, F64.Zero); } }
        public static F64Vec3 Up       { get { return new F64Vec3(F64.Zero, F64.One, F64.Zero); } }
        public static F64Vec3 Left     { get { return new F64Vec3(F64.Neg1, F64.Zero, F64.Zero); } }
        public static F64Vec3 Right    { get { return new F64Vec3(F64.One, F64.Zero, F64.Zero); } }

        // Components
        public F64 x;
        public F64 y;
        public F64 z;

        public F64Vec3 (F64 x, F64 y, F64 z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static F64Vec3 FromInt(int x, int y, int z) { return new F64Vec3(F64.FromInt(x), F64.FromInt(y), F64.FromInt(z)); }
        public static F64Vec3 FromFloat(float x, float y, float z) { return new F64Vec3(F64.FromFloat(x), F64.FromFloat(y), F64.FromFloat(z)); }
        public static F64Vec3 FromDouble(double x, double y, double z) { return new F64Vec3(F64.FromDouble(x), F64.FromDouble(y), F64.FromDouble(z)); }

        // Operators
        public static F64Vec3 operator -(F64Vec3 a) { return new F64Vec3(-a.x, -a.y, -a.z); }

        public static F64Vec3 operator +(F64Vec3 a, F64Vec3 b) { return new F64Vec3(a.x + b.x, a.y + b.y, a.z + b.z); }
        public static F64Vec3 operator -(F64Vec3 a, F64Vec3 b) { return new F64Vec3(a.x - b.x, a.y - b.y, a.z - b.z); }
        public static F64Vec3 operator *(F64Vec3 a, F64Vec3 b) { return new F64Vec3(a.x * b.x, a.y * b.y, a.z * b.z); }
        public static F64Vec3 operator /(F64Vec3 a, F64Vec3 b) { return new F64Vec3(a.x / b.x, a.y / b.y, a.z / b.z); }
        public static F64Vec3 operator %(F64Vec3 a, F64Vec3 b) { return new F64Vec3(a.x % b.x, a.y % b.y, a.z % b.z); }

        public static F64Vec3 operator +(F64 a, F64Vec3 b) { return new F64Vec3(a + b.x, a + b.y, a + b.z); }
        public static F64Vec3 operator +(F64Vec3 a, F64 b) { return new F64Vec3(a.x + b, a.y + b, a.z + b); }
        public static F64Vec3 operator -(F64 a, F64Vec3 b) { return new F64Vec3(a - b.x, a - b.y, a - b.z); }
        public static F64Vec3 operator -(F64Vec3 a, F64 b) { return new F64Vec3(a.x - b, a.y - b, a.z - b); }
        public static F64Vec3 operator *(F64 a, F64Vec3 b) { return new F64Vec3(a * b.x, a * b.y, a * b.z); }
        public static F64Vec3 operator *(F64Vec3 a, F64 b) { return new F64Vec3(a.x * b, a.y * b, a.z * b); }
        public static F64Vec3 operator /(F64 a, F64Vec3 b) { return new F64Vec3(a / b.x, a / b.y, a / b.z); }
        public static F64Vec3 operator /(F64Vec3 a, F64 b) { return new F64Vec3(a.x / b, a.y / b, a.z / b); }
        public static F64Vec3 operator %(F64 a, F64Vec3 b) { return new F64Vec3(a % b.x, a % b.y, a % b.z); }
        public static F64Vec3 operator %(F64Vec3 a, F64 b) { return new F64Vec3(a.x % b, a.y % b, a.z % b); }

        public static bool operator ==(F64Vec3 a, F64Vec3 b) { return a.x == b.x && a.y == b.y && a.z == b.z; }
        public static bool operator !=(F64Vec3 a, F64Vec3 b) { return a.x != b.x || a.y != b.y || a.z != b.z; }

        public static F64Vec3 SqrtPrecise(F64Vec3 a) { return new F64Vec3(F64.SqrtPrecise(a.x), F64.SqrtPrecise(a.y), F64.SqrtPrecise(a.z)); }
        public static F64Vec3 Sqrt(F64Vec3 a) { return new F64Vec3(F64.Sqrt(a.x), F64.Sqrt(a.y), F64.Sqrt(a.z)); }
        public static F64Vec3 SqrtFast(F64Vec3 a) { return new F64Vec3(F64.SqrtFast(a.x), F64.SqrtFast(a.y), F64.SqrtFast(a.z)); }
        public static F64Vec3 SqrtFastest(F64Vec3 a) { return new F64Vec3(F64.SqrtFastest(a.x), F64.SqrtFastest(a.y), F64.SqrtFastest(a.z)); }
        public static F64Vec3 RSqrt(F64Vec3 a) { return new F64Vec3(F64.RSqrt(a.x), F64.RSqrt(a.y), F64.RSqrt(a.z)); }
        public static F64Vec3 RSqrtFast(F64Vec3 a) { return new F64Vec3(F64.RSqrtFast(a.x), F64.RSqrtFast(a.y), F64.RSqrtFast(a.z)); }
        public static F64Vec3 RSqrtFastest(F64Vec3 a) { return new F64Vec3(F64.RSqrtFastest(a.x), F64.RSqrtFastest(a.y), F64.RSqrtFastest(a.z)); }
        public static F64Vec3 Rcp(F64Vec3 a) { return new F64Vec3(F64.Rcp(a.x), F64.Rcp(a.y), F64.Rcp(a.z)); }
        public static F64Vec3 RcpFast(F64Vec3 a) { return new F64Vec3(F64.RcpFast(a.x), F64.RcpFast(a.y), F64.RcpFast(a.z)); }
        public static F64Vec3 RcpFastest(F64Vec3 a) { return new F64Vec3(F64.RcpFastest(a.x), F64.RcpFastest(a.y), F64.RcpFastest(a.z)); }
        public static F64Vec3 Exp(F64Vec3 a) { return new F64Vec3(F64.Exp(a.x), F64.Exp(a.y), F64.Exp(a.z)); }
        public static F64Vec3 ExpFast(F64Vec3 a) { return new F64Vec3(F64.ExpFast(a.x), F64.ExpFast(a.y), F64.ExpFast(a.z)); }
        public static F64Vec3 ExpFastest(F64Vec3 a) { return new F64Vec3(F64.ExpFastest(a.x), F64.ExpFastest(a.y), F64.ExpFastest(a.z)); }
        public static F64Vec3 Exp2(F64Vec3 a) { return new F64Vec3(F64.Exp2(a.x), F64.Exp2(a.y), F64.Exp2(a.z)); }
        public static F64Vec3 Exp2Fast(F64Vec3 a) { return new F64Vec3(F64.Exp2Fast(a.x), F64.Exp2Fast(a.y), F64.Exp2Fast(a.z)); }
        public static F64Vec3 Exp2Fastest(F64Vec3 a) { return new F64Vec3(F64.Exp2Fastest(a.x), F64.Exp2Fastest(a.y), F64.Exp2Fastest(a.z)); }
        public static F64Vec3 Log(F64Vec3 a) { return new F64Vec3(F64.Log(a.x), F64.Log(a.y), F64.Log(a.z)); }
        public static F64Vec3 LogFast(F64Vec3 a) { return new F64Vec3(F64.LogFast(a.x), F64.LogFast(a.y), F64.LogFast(a.z)); }
        public static F64Vec3 LogFastest(F64Vec3 a) { return new F64Vec3(F64.LogFastest(a.x), F64.LogFastest(a.y), F64.LogFastest(a.z)); }
        public static F64Vec3 Log2(F64Vec3 a) { return new F64Vec3(F64.Log2(a.x), F64.Log2(a.y), F64.Log2(a.z)); }
        public static F64Vec3 Log2Fast(F64Vec3 a) { return new F64Vec3(F64.Log2Fast(a.x), F64.Log2Fast(a.y), F64.Log2Fast(a.z)); }
        public static F64Vec3 Log2Fastest(F64Vec3 a) { return new F64Vec3(F64.Log2Fastest(a.x), F64.Log2Fastest(a.y), F64.Log2Fastest(a.z)); }
        public static F64Vec3 Sin(F64Vec3 a) { return new F64Vec3(F64.Sin(a.x), F64.Sin(a.y), F64.Sin(a.z)); }
        public static F64Vec3 SinFast(F64Vec3 a) { return new F64Vec3(F64.SinFast(a.x), F64.SinFast(a.y), F64.SinFast(a.z)); }
        public static F64Vec3 SinFastest(F64Vec3 a) { return new F64Vec3(F64.SinFastest(a.x), F64.SinFastest(a.y), F64.SinFastest(a.z)); }
        public static F64Vec3 Cos(F64Vec3 a) { return new F64Vec3(F64.Cos(a.x), F64.Cos(a.y), F64.Cos(a.z)); }
        public static F64Vec3 CosFast(F64Vec3 a) { return new F64Vec3(F64.CosFast(a.x), F64.CosFast(a.y), F64.CosFast(a.z)); }
        public static F64Vec3 CosFastest(F64Vec3 a) { return new F64Vec3(F64.CosFastest(a.x), F64.CosFastest(a.y), F64.CosFastest(a.z)); }

        public static F64Vec3 Pow(F64Vec3 a, F64 b) { return new F64Vec3(F64.Pow(a.x, b), F64.Pow(a.y, b), F64.Pow(a.z, b)); }
        public static F64Vec3 PowFast(F64Vec3 a, F64 b) { return new F64Vec3(F64.PowFast(a.x, b), F64.PowFast(a.y, b), F64.PowFast(a.z, b)); }
        public static F64Vec3 PowFastest(F64Vec3 a, F64 b) { return new F64Vec3(F64.PowFastest(a.x, b), F64.PowFastest(a.y, b), F64.PowFastest(a.z, b)); }
        public static F64Vec3 Pow(F64 a, F64Vec3 b) { return new F64Vec3(F64.Pow(a, b.x), F64.Pow(a, b.y), F64.Pow(a, b.z)); }
        public static F64Vec3 PowFast(F64 a, F64Vec3 b) { return new F64Vec3(F64.PowFast(a, b.x), F64.PowFast(a, b.y), F64.PowFast(a, b.z)); }
        public static F64Vec3 PowFastest(F64 a, F64Vec3 b) { return new F64Vec3(F64.PowFastest(a, b.x), F64.PowFastest(a, b.y), F64.PowFastest(a, b.z)); }
        public static F64Vec3 Pow(F64Vec3 a, F64Vec3 b) { return new F64Vec3(F64.Pow(a.x, b.x), F64.Pow(a.y, b.y), F64.Pow(a.z, b.z)); }
        public static F64Vec3 PowFast(F64Vec3 a, F64Vec3 b) { return new F64Vec3(F64.PowFast(a.x, b.x), F64.PowFast(a.y, b.y), F64.PowFast(a.z, b.z)); }
        public static F64Vec3 PowFastest(F64Vec3 a, F64Vec3 b) { return new F64Vec3(F64.PowFastest(a.x, b.x), F64.PowFastest(a.y, b.y), F64.PowFastest(a.z, b.z)); }

        public static F64 Length(F64Vec3 a) { return F64.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z); }
        public static F64 LengthFast(F64Vec3 a) { return F64.SqrtFast(a.x * a.x + a.y * a.y + a.z * a.z); }
        public static F64 LengthFastest(F64Vec3 a) { return F64.SqrtFastest(a.x * a.x + a.y * a.y + a.z * a.z); }
        public static F64 LengthSqr(F64Vec3 a) { return (a.x * a.x + a.y * a.y + a.z * a.z); }
        public static F64Vec3 Normalize(F64Vec3 a) { F64 ooLen = F64.RSqrt(LengthSqr(a)); return ooLen * a; }
        public static F64Vec3 NormalizeFast(F64Vec3 a) { F64 ooLen = F64.RSqrtFast(LengthSqr(a)); return ooLen * a; }
        public static F64Vec3 NormalizeFastest(F64Vec3 a) { F64 ooLen = F64.RSqrtFastest(LengthSqr(a)); return ooLen * a; }

        public static F64 Dot(F64Vec3 a, F64Vec3 b) { return (a.x * b.x + a.y * b.y + a.z * b.z); }
        public static F64 Distance(F64Vec3 a, F64Vec3 b) { return Length(a - b); }
        public static F64 DistanceFast(F64Vec3 a, F64Vec3 b) { return LengthFast(a - b); }
        public static F64 DistanceFastest(F64Vec3 a, F64Vec3 b) { return LengthFastest(a - b); }

        public static F64Vec3 Lerp(F64Vec3 a, F64Vec3 b, F64 t) { return a + t*(b - a); }

        public static F64Vec3 Cross(F64Vec3 a, F64Vec3 b)
        {
            return new F64Vec3(
                (a.y * b.z) - (a.z * b.y),
                (a.z * b.x) - (a.x * b.z),
                (a.x * b.y) - (a.y * b.x));
        }

        public static F64Vec3 Min(F64Vec3 a, F64Vec3 b) { return new F64Vec3(F64.Min(a.x, b.x), F64.Min(a.y, b.y), F64.Min(a.z, b.z)); }
        public static F64Vec3 Max(F64Vec3 a, F64Vec3 b) { return new F64Vec3(F64.Max(a.x, b.x), F64.Max(a.y, b.y), F64.Max(a.z, b.z)); }

        public override bool Equals(object obj)
        {
            if (!(obj is F64Vec3))
                return false;
            return ((F64Vec3)obj) == this;
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
