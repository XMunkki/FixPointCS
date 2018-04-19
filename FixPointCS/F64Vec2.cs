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
    /// Vector2 struct with signed 32.32 fixed point components.
    /// </summary>
    public struct F64Vec2
    {
        // Constants
        public static F64Vec2 Zero     => new F64Vec2(F64.Zero, F64.Zero);
        public static F64Vec2 One      => new F64Vec2(F64.One, F64.One);
        public static F64Vec2 Down     => new F64Vec2(F64.Zero, F64.Neg1);
        public static F64Vec2 Up       => new F64Vec2(F64.Zero, F64.One);
        public static F64Vec2 Left     => new F64Vec2(F64.Neg1, F64.Zero);
        public static F64Vec2 Right    => new F64Vec2(F64.One, F64.Zero);

        // The components
        public F64 x;
        public F64 y;

        public F64Vec2 (F64 x, F64 y)
        {
            this.x = x;
            this.y = y;
        }

        // Conversions
//        public int Int { get { return FP.ToInt(raw); } }
//        public float Float { get { return FP.ToFloat(raw); } }
//        public double Double { get { return FP.ToDouble(raw); } }

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
//        public static bool operator <(F64Vec2 a, F64Vec2 b) { return a.raw < b.raw; }
//        public static bool operator <=(F64Vec2 a, F64Vec2 b) { return a.raw <= b.raw; }
//        public static bool operator >(F64Vec2 a, F64Vec2 b) { return a.raw > b.raw; }
//        public static bool operator >=(F64Vec2 a, F64Vec2 b) { return a.raw >= b.raw; }

        // \todo [petri] make static?
        public F64Vec2 Sqrt() { return new F64Vec2(x.Sqrt(), y.Sqrt()); }
        public F64Vec2 RSqrt() { return new F64Vec2(x.RSqrt(), y.RSqrt()); }
        public F64Vec2 Rcp() { return new F64Vec2(x.Rcp(), y.Rcp()); }
        public F64Vec2 Exp() { return new F64Vec2(x.Exp(), y.Exp()); }
        public F64Vec2 Exp2() { return new F64Vec2(x.Exp2(), y.Exp2()); }
        public F64Vec2 Log() { return new F64Vec2(x.Log(), y.Log()); }
        public F64Vec2 Log2() { return new F64Vec2(x.Log2(), y.Log2()); }
        public F64Vec2 Sin() { return new F64Vec2(x.Sin(), y.Sin()); }
        public F64Vec2 Cos() { return new F64Vec2(x.Cos(), y.Cos()); }

        public static F64Vec2 Pow(F64Vec2 a, F64 b) { return new F64Vec2(F64.Pow(a.x, b), F64.Pow(a.y, b)); }
        public static F64Vec2 Pow(F64 a, F64Vec2 b) { return new F64Vec2(F64.Pow(a, b.x), F64.Pow(a, b.y)); }
        public static F64Vec2 Pow(F64Vec2 a, F64Vec2 b) { return new F64Vec2(F64.Pow(a.x, b.x), F64.Pow(a.y, b.y)); }

        public static F64 Length(F64Vec2 a) { return (a.x * a.x + a.y * a.y).SqrtFast(); }
        public static F64 LengthSqr(F64Vec2 a) { return (a.x * a.x + a.y * a.y); }
        public static F64Vec2 Normalize(F64Vec2 a) { F64 ooLen = LengthSqr(a).RSqrt(); return ooLen * a; }

        public static F64 Dot(F64Vec2 a, F64Vec2 b) { return (a.x * b.x + a.y * b.y); }
        public static F64 Distance(F64Vec2 a, F64Vec2 b) { return Length(a - b); }

        public static F64Vec2 Lerp(F64Vec2 a, F64Vec2 b, F64 t) { return (F64.One - t) * a + t * b; }   // \todo [petri] is a + t*(b-a) better formula?

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
