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
    public struct F32Vec2 : IEquatable<F32Vec2>
    {
        // Constants
        public static F32Vec2 Zero     { get { return new F32Vec2(F32.Zero, F32.Zero); } }
        public static F32Vec2 One      { get { return new F32Vec2(F32.One, F32.One); } }
        public static F32Vec2 Down     { get { return new F32Vec2(F32.Zero, F32.Neg1); } }
        public static F32Vec2 Up       { get { return new F32Vec2(F32.Zero, F32.One); } }
        public static F32Vec2 Left     { get { return new F32Vec2(F32.Neg1, F32.Zero); } }
        public static F32Vec2 Right    { get { return new F32Vec2(F32.One, F32.Zero); } }

        // Components
        public F32 x;
        public F32 y;

        public F32Vec2 (F32 x, F32 y)
        {
            this.x = x;
            this.y = y;
        }

        public static F32Vec2 FromInt(int x, int y) { return new F32Vec2(F32.FromInt(x), F32.FromInt(y)); }
        public static F32Vec2 FromFloat(float x, float y) { return new F32Vec2(F32.FromFloat(x), F32.FromFloat(y)); }
        public static F32Vec2 FromDouble(double x, double y) { return new F32Vec2(F32.FromDouble(x), F32.FromDouble(y)); }

        // Operators
        public static F32Vec2 operator -(F32Vec2 a) { return new F32Vec2(-a.x, -a.y); }

        public static F32Vec2 operator +(F32Vec2 a, F32Vec2 b) { return new F32Vec2(a.x + b.x, a.y + b.y); }
        public static F32Vec2 operator -(F32Vec2 a, F32Vec2 b) { return new F32Vec2(a.x - b.x, a.y - b.y); }
        public static F32Vec2 operator *(F32Vec2 a, F32Vec2 b) { return new F32Vec2(a.x * b.x, a.y * b.y); }
        public static F32Vec2 operator /(F32Vec2 a, F32Vec2 b) { return new F32Vec2(a.x / b.x, a.y / b.y); }
        public static F32Vec2 operator %(F32Vec2 a, F32Vec2 b) { return new F32Vec2(a.x % b.x, a.y % b.y); }

        public static F32Vec2 operator +(F32 a, F32Vec2 b) { return new F32Vec2(a + b.x, a + b.y); }
        public static F32Vec2 operator +(F32Vec2 a, F32 b) { return new F32Vec2(a.x + b, a.y + b); }
        public static F32Vec2 operator -(F32 a, F32Vec2 b) { return new F32Vec2(a - b.x, a - b.y); }
        public static F32Vec2 operator -(F32Vec2 a, F32 b) { return new F32Vec2(a.x - b, a.y - b); }
        public static F32Vec2 operator *(F32 a, F32Vec2 b) { return new F32Vec2(a * b.x, a * b.y); }
        public static F32Vec2 operator *(F32Vec2 a, F32 b) { return new F32Vec2(a.x * b, a.y * b); }
        public static F32Vec2 operator /(F32 a, F32Vec2 b) { return new F32Vec2(a / b.x, a / b.y); }
        public static F32Vec2 operator /(F32Vec2 a, F32 b) { return new F32Vec2(a.x / b, a.y / b); }
        public static F32Vec2 operator %(F32 a, F32Vec2 b) { return new F32Vec2(a % b.x, a % b.y); }
        public static F32Vec2 operator %(F32Vec2 a, F32 b) { return new F32Vec2(a.x % b, a.y % b); }

        public static bool operator ==(F32Vec2 a, F32Vec2 b) { return a.x == b.x && a.y == b.y; }
        public static bool operator !=(F32Vec2 a, F32Vec2 b) { return a.x != b.x || a.y != b.y; }

        public static F32Vec2 Div(F32Vec2 a, F32 b) { F32 oob = F32.Rcp(b); return new F32Vec2(a.x * oob, a.y * oob); }
        public static F32Vec2 DivFast(F32Vec2 a, F32 b) { F32 oob = F32.RcpFast(b); return new F32Vec2(a.x * oob, a.y * oob); }
        public static F32Vec2 DivFastest(F32Vec2 a, F32 b) { F32 oob = F32.RcpFastest(b); return new F32Vec2(a.x * oob, a.y * oob); }
        public static F32Vec2 Div(F32Vec2 a, F32Vec2 b) { return new F32Vec2(F32.Div(a.x, b.x), F32.Div(a.y, b.y)); }
        public static F32Vec2 DivFast(F32Vec2 a, F32Vec2 b) { return new F32Vec2(F32.DivFast(a.x, b.x), F32.DivFast(a.y, b.y)); }
        public static F32Vec2 DivFastest(F32Vec2 a, F32Vec2 b) { return new F32Vec2(F32.DivFastest(a.x, b.x), F32.DivFastest(a.y, b.y)); }
        public static F32Vec2 SqrtPrecise(F32Vec2 a) { return new F32Vec2(F32.SqrtPrecise(a.x), F32.SqrtPrecise(a.y)); }
        public static F32Vec2 Sqrt(F32Vec2 a) { return new F32Vec2(F32.Sqrt(a.x), F32.Sqrt(a.y)); }
        public static F32Vec2 SqrtFast(F32Vec2 a) { return new F32Vec2(F32.SqrtFast(a.x), F32.SqrtFast(a.y)); }
        public static F32Vec2 SqrtFastest(F32Vec2 a) { return new F32Vec2(F32.SqrtFastest(a.x), F32.SqrtFastest(a.y)); }
        public static F32Vec2 RSqrt(F32Vec2 a) { return new F32Vec2(F32.RSqrt(a.x), F32.RSqrt(a.y)); }
        public static F32Vec2 RSqrtFast(F32Vec2 a) { return new F32Vec2(F32.RSqrtFast(a.x), F32.RSqrtFast(a.y)); }
        public static F32Vec2 RSqrtFastest(F32Vec2 a) { return new F32Vec2(F32.RSqrtFastest(a.x), F32.RSqrtFastest(a.y)); }
        public static F32Vec2 Rcp(F32Vec2 a) { return new F32Vec2(F32.Rcp(a.x), F32.Rcp(a.y)); }
        public static F32Vec2 RcpFast(F32Vec2 a) { return new F32Vec2(F32.RcpFast(a.x), F32.RcpFast(a.y)); }
        public static F32Vec2 RcpFastest(F32Vec2 a) { return new F32Vec2(F32.RcpFastest(a.x), F32.RcpFastest(a.y)); }
        public static F32Vec2 Exp(F32Vec2 a) { return new F32Vec2(F32.Exp(a.x), F32.Exp(a.y)); }
        public static F32Vec2 ExpFast(F32Vec2 a) { return new F32Vec2(F32.ExpFast(a.x), F32.ExpFast(a.y)); }
        public static F32Vec2 ExpFastest(F32Vec2 a) { return new F32Vec2(F32.ExpFastest(a.x), F32.ExpFastest(a.y)); }
        public static F32Vec2 Exp2(F32Vec2 a) { return new F32Vec2(F32.Exp2(a.x), F32.Exp2(a.y)); }
        public static F32Vec2 Exp2Fast(F32Vec2 a) { return new F32Vec2(F32.Exp2Fast(a.x), F32.Exp2Fast(a.y)); }
        public static F32Vec2 Exp2Fastest(F32Vec2 a) { return new F32Vec2(F32.Exp2Fastest(a.x), F32.Exp2Fastest(a.y)); }
        public static F32Vec2 Log(F32Vec2 a) { return new F32Vec2(F32.Log(a.x), F32.Log(a.y)); }
        public static F32Vec2 LogFast(F32Vec2 a) { return new F32Vec2(F32.LogFast(a.x), F32.LogFast(a.y)); }
        public static F32Vec2 LogFastest(F32Vec2 a) { return new F32Vec2(F32.LogFastest(a.x), F32.LogFastest(a.y)); }
        public static F32Vec2 Log2(F32Vec2 a) { return new F32Vec2(F32.Log2(a.x), F32.Log2(a.y)); }
        public static F32Vec2 Log2Fast(F32Vec2 a) { return new F32Vec2(F32.Log2Fast(a.x), F32.Log2Fast(a.y)); }
        public static F32Vec2 Log2Fastest(F32Vec2 a) { return new F32Vec2(F32.Log2Fastest(a.x), F32.Log2Fastest(a.y)); }
        public static F32Vec2 Sin(F32Vec2 a) { return new F32Vec2(F32.Sin(a.x), F32.Sin(a.y)); }
        public static F32Vec2 SinFast(F32Vec2 a) { return new F32Vec2(F32.SinFast(a.x), F32.SinFast(a.y)); }
        public static F32Vec2 SinFastest(F32Vec2 a) { return new F32Vec2(F32.SinFastest(a.x), F32.SinFastest(a.y)); }
        public static F32Vec2 Cos(F32Vec2 a) { return new F32Vec2(F32.Cos(a.x), F32.Cos(a.y)); }
        public static F32Vec2 CosFast(F32Vec2 a) { return new F32Vec2(F32.CosFast(a.x), F32.CosFast(a.y)); }
        public static F32Vec2 CosFastest(F32Vec2 a) { return new F32Vec2(F32.CosFastest(a.x), F32.CosFastest(a.y)); }

        public static F32Vec2 Pow(F32Vec2 a, F32 b) { return new F32Vec2(F32.Pow(a.x, b), F32.Pow(a.y, b)); }
        public static F32Vec2 PowFast(F32Vec2 a, F32 b) { return new F32Vec2(F32.PowFast(a.x, b), F32.PowFast(a.y, b)); }
        public static F32Vec2 PowFastest(F32Vec2 a, F32 b) { return new F32Vec2(F32.PowFastest(a.x, b), F32.PowFastest(a.y, b)); }
        public static F32Vec2 Pow(F32 a, F32Vec2 b) { return new F32Vec2(F32.Pow(a, b.x), F32.Pow(a, b.y)); }
        public static F32Vec2 PowFast(F32 a, F32Vec2 b) { return new F32Vec2(F32.PowFast(a, b.x), F32.PowFast(a, b.y)); }
        public static F32Vec2 PowFastest(F32 a, F32Vec2 b) { return new F32Vec2(F32.PowFastest(a, b.x), F32.PowFastest(a, b.y)); }
        public static F32Vec2 Pow(F32Vec2 a, F32Vec2 b) { return new F32Vec2(F32.Pow(a.x, b.x), F32.Pow(a.y, b.y)); }
        public static F32Vec2 PowFast(F32Vec2 a, F32Vec2 b) { return new F32Vec2(F32.PowFast(a.x, b.x), F32.PowFast(a.y, b.y)); }
        public static F32Vec2 PowFastest(F32Vec2 a, F32Vec2 b) { return new F32Vec2(F32.PowFastest(a.x, b.x), F32.PowFastest(a.y, b.y)); }

        public static F32 Length(F32Vec2 a) { return F32.Sqrt(a.x * a.x + a.y * a.y); }
        public static F32 LengthFast(F32Vec2 a) { return F32.SqrtFast(a.x * a.x + a.y * a.y); }
        public static F32 LengthFastest(F32Vec2 a) { return F32.SqrtFastest(a.x * a.x + a.y * a.y); }
        public static F32 LengthSqr(F32Vec2 a) { return (a.x * a.x + a.y * a.y); }
        public static F32Vec2 Normalize(F32Vec2 a) { F32 ooLen = F32.RSqrt(LengthSqr(a)); return ooLen * a; }
        public static F32Vec2 NormalizeFast(F32Vec2 a) { F32 ooLen = F32.RSqrtFast(LengthSqr(a)); return ooLen * a; }
        public static F32Vec2 NormalizeFastest(F32Vec2 a) { F32 ooLen = F32.RSqrtFastest(LengthSqr(a)); return ooLen * a; }

        public static F32 Dot(F32Vec2 a, F32Vec2 b) { return (a.x * b.x + a.y * b.y); }
        public static F32 Distance(F32Vec2 a, F32Vec2 b) { return Length(a - b); }
        public static F32 DistanceFast(F32Vec2 a, F32Vec2 b) { return LengthFast(a - b); }
        public static F32 DistanceFastest(F32Vec2 a, F32Vec2 b) { return LengthFastest(a - b); }

        public static F32Vec2 Lerp(F32Vec2 a, F32Vec2 b, F32 t) { return a + t*(b - a); }

        public static F32Vec2 Min(F32Vec2 a, F32Vec2 b) { return new F32Vec2(F32.Min(a.x, b.x), F32.Min(a.y, b.y)); }
        public static F32Vec2 Max(F32Vec2 a, F32Vec2 b) { return new F32Vec2(F32.Max(a.x, b.x), F32.Max(a.y, b.y)); }

        public bool Equals(F32Vec2 other)
        {
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is F32Vec2))
                return false;
            return ((F32Vec2)obj) == this;
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
