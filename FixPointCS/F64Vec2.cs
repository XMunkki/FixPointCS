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
    /// Vector2 struct with signed 32.32 fixed point components.
    /// </summary>
    [Serializable]
    public struct F64Vec2
    {
        // Constants
        public static F64Vec2 Zero     { get { return new F64Vec2(F64.Zero, F64.Zero); } }
        public static F64Vec2 One      { get { return new F64Vec2(F64.One, F64.One); } }
        public static F64Vec2 Down     { get { return new F64Vec2(F64.Zero, F64.Neg1); } }
        public static F64Vec2 Up       { get { return new F64Vec2(F64.Zero, F64.One); } }
        public static F64Vec2 Left     { get { return new F64Vec2(F64.Neg1, F64.Zero); } }
        public static F64Vec2 Right    { get { return new F64Vec2(F64.One, F64.Zero); } }

        // Components
        public F64 x;
        public F64 y;

        public F64Vec2 (F64 x, F64 y)
        {
            this.x = x;
            this.y = y;
        }

        public static F64Vec2 FromInt(int x, int y) { return new F64Vec2(F64.FromInt(x), F64.FromInt(y)); }
        public static F64Vec2 FromFloat(float x, float y) { return new F64Vec2(F64.FromFloat(x), F64.FromFloat(y)); }
        public static F64Vec2 FromDouble(double x, double y) { return new F64Vec2(F64.FromDouble(x), F64.FromDouble(y)); }

        // Operators
        public static F64Vec2 operator -(F64Vec2 a) { return new F64Vec2(-a.x, -a.y); }

        public static F64Vec2 operator +(F64Vec2 a, F64Vec2 b) { return new F64Vec2(a.x + b.x, a.y + b.y); }
        public static F64Vec2 operator -(F64Vec2 a, F64Vec2 b) { return new F64Vec2(a.x - b.x, a.y - b.y); }
        public static F64Vec2 operator *(F64Vec2 a, F64Vec2 b) { return new F64Vec2(a.x * b.x, a.y * b.y); }
        public static F64Vec2 operator /(F64Vec2 a, F64Vec2 b) { return new F64Vec2(a.x / b.x, a.y / b.y); }
        public static F64Vec2 operator %(F64Vec2 a, F64Vec2 b) { return new F64Vec2(a.x % b.x, a.y % b.y); }

        public static F64Vec2 operator +(F64 a, F64Vec2 b) { return new F64Vec2(a + b.x, a + b.y); }
        public static F64Vec2 operator +(F64Vec2 a, F64 b) { return new F64Vec2(a.x + b, a.y + b); }
        public static F64Vec2 operator -(F64 a, F64Vec2 b) { return new F64Vec2(a - b.x, a - b.y); }
        public static F64Vec2 operator -(F64Vec2 a, F64 b) { return new F64Vec2(a.x - b, a.y - b); }
        public static F64Vec2 operator *(F64 a, F64Vec2 b) { return new F64Vec2(a * b.x, a * b.y); }
        public static F64Vec2 operator *(F64Vec2 a, F64 b) { return new F64Vec2(a.x * b, a.y * b); }
        public static F64Vec2 operator /(F64 a, F64Vec2 b) { return new F64Vec2(a / b.x, a / b.y); }
        public static F64Vec2 operator /(F64Vec2 a, F64 b) { return new F64Vec2(a.x / b, a.y / b); }
        public static F64Vec2 operator %(F64 a, F64Vec2 b) { return new F64Vec2(a % b.x, a % b.y); }
        public static F64Vec2 operator %(F64Vec2 a, F64 b) { return new F64Vec2(a.x % b, a.y % b); }

        public static bool operator ==(F64Vec2 a, F64Vec2 b) { return a.x == b.x && a.y == b.y; }
        public static bool operator !=(F64Vec2 a, F64Vec2 b) { return a.x != b.x || a.y != b.y; }

        public static F64Vec2 SqrtPrecise(F64Vec2 a) { return new F64Vec2(F64.SqrtPrecise(a.x), F64.SqrtPrecise(a.y)); }
        public static F64Vec2 Sqrt(F64Vec2 a) { return new F64Vec2(F64.Sqrt(a.x), F64.Sqrt(a.y)); }
        public static F64Vec2 SqrtFast(F64Vec2 a) { return new F64Vec2(F64.SqrtFast(a.x), F64.SqrtFast(a.y)); }
        public static F64Vec2 SqrtFastest(F64Vec2 a) { return new F64Vec2(F64.SqrtFastest(a.x), F64.SqrtFastest(a.y)); }
        public static F64Vec2 RSqrt(F64Vec2 a) { return new F64Vec2(F64.RSqrt(a.x), F64.RSqrt(a.y)); }
        public static F64Vec2 RSqrtFast(F64Vec2 a) { return new F64Vec2(F64.RSqrtFast(a.x), F64.RSqrtFast(a.y)); }
        public static F64Vec2 RSqrtFastest(F64Vec2 a) { return new F64Vec2(F64.RSqrtFastest(a.x), F64.RSqrtFastest(a.y)); }
        public static F64Vec2 Rcp(F64Vec2 a) { return new F64Vec2(F64.Rcp(a.x), F64.Rcp(a.y)); }
        public static F64Vec2 RcpFast(F64Vec2 a) { return new F64Vec2(F64.RcpFast(a.x), F64.RcpFast(a.y)); }
        public static F64Vec2 RcpFastest(F64Vec2 a) { return new F64Vec2(F64.RcpFastest(a.x), F64.RcpFastest(a.y)); }
        public static F64Vec2 Exp(F64Vec2 a) { return new F64Vec2(F64.Exp(a.x), F64.Exp(a.y)); }
        public static F64Vec2 ExpFast(F64Vec2 a) { return new F64Vec2(F64.ExpFast(a.x), F64.ExpFast(a.y)); }
        public static F64Vec2 ExpFastest(F64Vec2 a) { return new F64Vec2(F64.ExpFastest(a.x), F64.ExpFastest(a.y)); }
        public static F64Vec2 Exp2(F64Vec2 a) { return new F64Vec2(F64.Exp2(a.x), F64.Exp2(a.y)); }
        public static F64Vec2 Exp2Fast(F64Vec2 a) { return new F64Vec2(F64.Exp2Fast(a.x), F64.Exp2Fast(a.y)); }
        public static F64Vec2 Exp2Fastest(F64Vec2 a) { return new F64Vec2(F64.Exp2Fastest(a.x), F64.Exp2Fastest(a.y)); }
        public static F64Vec2 Log(F64Vec2 a) { return new F64Vec2(F64.Log(a.x), F64.Log(a.y)); }
        public static F64Vec2 LogFast(F64Vec2 a) { return new F64Vec2(F64.LogFast(a.x), F64.LogFast(a.y)); }
        public static F64Vec2 LogFastest(F64Vec2 a) { return new F64Vec2(F64.LogFastest(a.x), F64.LogFastest(a.y)); }
        public static F64Vec2 Log2(F64Vec2 a) { return new F64Vec2(F64.Log2(a.x), F64.Log2(a.y)); }
        public static F64Vec2 Log2Fast(F64Vec2 a) { return new F64Vec2(F64.Log2Fast(a.x), F64.Log2Fast(a.y)); }
        public static F64Vec2 Log2Fastest(F64Vec2 a) { return new F64Vec2(F64.Log2Fastest(a.x), F64.Log2Fastest(a.y)); }
        public static F64Vec2 Sin(F64Vec2 a) { return new F64Vec2(F64.Sin(a.x), F64.Sin(a.y)); }
        public static F64Vec2 SinFast(F64Vec2 a) { return new F64Vec2(F64.SinFast(a.x), F64.SinFast(a.y)); }
        public static F64Vec2 SinFastest(F64Vec2 a) { return new F64Vec2(F64.SinFastest(a.x), F64.SinFastest(a.y)); }
        public static F64Vec2 Cos(F64Vec2 a) { return new F64Vec2(F64.Cos(a.x), F64.Cos(a.y)); }
        public static F64Vec2 CosFast(F64Vec2 a) { return new F64Vec2(F64.CosFast(a.x), F64.CosFast(a.y)); }
        public static F64Vec2 CosFastest(F64Vec2 a) { return new F64Vec2(F64.CosFastest(a.x), F64.CosFastest(a.y)); }

        public static F64Vec2 Pow(F64Vec2 a, F64 b) { return new F64Vec2(F64.Pow(a.x, b), F64.Pow(a.y, b)); }
        public static F64Vec2 PowFast(F64Vec2 a, F64 b) { return new F64Vec2(F64.PowFast(a.x, b), F64.PowFast(a.y, b)); }
        public static F64Vec2 PowFastest(F64Vec2 a, F64 b) { return new F64Vec2(F64.PowFastest(a.x, b), F64.PowFastest(a.y, b)); }
        public static F64Vec2 Pow(F64 a, F64Vec2 b) { return new F64Vec2(F64.Pow(a, b.x), F64.Pow(a, b.y)); }
        public static F64Vec2 PowFast(F64 a, F64Vec2 b) { return new F64Vec2(F64.PowFast(a, b.x), F64.PowFast(a, b.y)); }
        public static F64Vec2 PowFastest(F64 a, F64Vec2 b) { return new F64Vec2(F64.PowFastest(a, b.x), F64.PowFastest(a, b.y)); }
        public static F64Vec2 Pow(F64Vec2 a, F64Vec2 b) { return new F64Vec2(F64.Pow(a.x, b.x), F64.Pow(a.y, b.y)); }
        public static F64Vec2 PowFast(F64Vec2 a, F64Vec2 b) { return new F64Vec2(F64.PowFast(a.x, b.x), F64.PowFast(a.y, b.y)); }
        public static F64Vec2 PowFastest(F64Vec2 a, F64Vec2 b) { return new F64Vec2(F64.PowFastest(a.x, b.x), F64.PowFastest(a.y, b.y)); }

        public static F64 Length(F64Vec2 a) { return F64.Sqrt(a.x * a.x + a.y * a.y); }
        public static F64 LengthFast(F64Vec2 a) { return F64.SqrtFast(a.x * a.x + a.y * a.y); }
        public static F64 LengthFastest(F64Vec2 a) { return F64.SqrtFastest(a.x * a.x + a.y * a.y); }
        public static F64 LengthSqr(F64Vec2 a) { return (a.x * a.x + a.y * a.y); }
        public static F64Vec2 Normalize(F64Vec2 a) { F64 ooLen = F64.RSqrt(LengthSqr(a)); return ooLen * a; }
        public static F64Vec2 NormalizeFast(F64Vec2 a) { F64 ooLen = F64.RSqrtFast(LengthSqr(a)); return ooLen * a; }
        public static F64Vec2 NormalizeFastest(F64Vec2 a) { F64 ooLen = F64.RSqrtFastest(LengthSqr(a)); return ooLen * a; }

        public static F64 Dot(F64Vec2 a, F64Vec2 b) { return (a.x * b.x + a.y * b.y); }
        public static F64 Distance(F64Vec2 a, F64Vec2 b) { return Length(a - b); }
        public static F64 DistanceFast(F64Vec2 a, F64Vec2 b) { return LengthFast(a - b); }
        public static F64 DistanceFastest(F64Vec2 a, F64Vec2 b) { return LengthFastest(a - b); }

        public static F64Vec2 Lerp(F64Vec2 a, F64Vec2 b, F64 t) { return a + t*(b - a); }

        public static F64Vec2 Min(F64Vec2 a, F64Vec2 b) { return new F64Vec2(F64.Min(a.x, b.x), F64.Min(a.y, b.y)); }
        public static F64Vec2 Max(F64Vec2 a, F64Vec2 b) { return new F64Vec2(F64.Max(a.x, b.x), F64.Max(a.y, b.y)); }

        public override bool Equals(object obj)
        {
            if (!(obj is F64Vec2))
                return false;
            return ((F64Vec2)obj) == this;
        }

        public override string ToString()
        {
            return "(" + x.ToString() + ", " + y.ToString() + ")";
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode()*7919;
        }
    }
}
