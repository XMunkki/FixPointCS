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
    /// Vector2 struct with signed 32.32 fixed point components.
    /// </summary>
    [Serializable]
    public struct F64Vec2 : IEquatable<F64Vec2>
    {
        // Constants
        public static F64Vec2 Zero     { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec2(Fixed64.Zero, Fixed64.Zero); } }
        public static F64Vec2 One      { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec2(Fixed64.One, Fixed64.One); } }
        public static F64Vec2 Down     { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec2(Fixed64.Zero, Fixed64.Neg1); } }
        public static F64Vec2 Up       { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec2(Fixed64.Zero, Fixed64.One); } }
        public static F64Vec2 Left     { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec2(Fixed64.Neg1, Fixed64.Zero); } }
        public static F64Vec2 Right    { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec2(Fixed64.One, Fixed64.Zero); } }
        public static F64Vec2 AxisX    { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec2(Fixed64.One, Fixed64.Zero); } }
        public static F64Vec2 AxisY    { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec2(Fixed64.Zero, Fixed64.One); } }

        // Raw components
        public long RawX;
        public long RawY;

        // F64 accessors
        public F64 X { get { return F64.FromRaw(RawX); } set { RawX = value.Raw; } }
        public F64 Y { get { return F64.FromRaw(RawY); } set { RawY = value.Raw; } }

        public F64Vec2(F64 x, F64 y)
        {
            RawX = x.Raw;
            RawY = y.Raw;
        }

        // raw ctor only for internal usage
        private F64Vec2(long x, long y)
        {
            RawX = x;
            RawY = y;
        }

        public static F64Vec2 FromInt(int x, int y) { return new F64Vec2(Fixed64.FromInt(x), Fixed64.FromInt(y)); }
        public static F64Vec2 FromFloat(float x, float y) { return new F64Vec2(Fixed64.FromFloat(x), Fixed64.FromFloat(y)); }
        public static F64Vec2 FromDouble(double x, double y) { return new F64Vec2(Fixed64.FromDouble(x), Fixed64.FromDouble(y)); }

        public static F64Vec2 operator -(F64Vec2 a) { return new F64Vec2(-a.RawX, -a.RawY); }
        public static F64Vec2 operator +(F64Vec2 a, F64Vec2 b) { return new F64Vec2(a.RawX + b.RawX, a.RawY + b.RawY); }
        public static F64Vec2 operator -(F64Vec2 a, F64Vec2 b) { return new F64Vec2(a.RawX - b.RawX, a.RawY - b.RawY); }
        public static F64Vec2 operator *(F64Vec2 a, F64Vec2 b) { return new F64Vec2(Fixed64.Mul(a.RawX, b.RawX), Fixed64.Mul(a.RawY, b.RawY)); }
        public static F64Vec2 operator /(F64Vec2 a, F64Vec2 b) { return new F64Vec2(Fixed64.DivPrecise(a.RawX, b.RawX), Fixed64.DivPrecise(a.RawY, b.RawY)); }
        public static F64Vec2 operator %(F64Vec2 a, F64Vec2 b) { return new F64Vec2(a.RawX % b.RawX, a.RawY % b.RawY); }

        public static F64Vec2 operator +(F64 a, F64Vec2 b) { return new F64Vec2(a.Raw + b.RawX, a.Raw + b.RawY); }
        public static F64Vec2 operator +(F64Vec2 a, F64 b) { return new F64Vec2(a.RawX + b.Raw, a.RawY + b.Raw); }
        public static F64Vec2 operator -(F64 a, F64Vec2 b) { return new F64Vec2(a.Raw - b.RawX, a.Raw - b.RawY); }
        public static F64Vec2 operator -(F64Vec2 a, F64 b) { return new F64Vec2(a.RawX - b.Raw, a.RawY - b.Raw); }
        public static F64Vec2 operator *(F64 a, F64Vec2 b) { return new F64Vec2(Fixed64.Mul(a.Raw, b.RawX), Fixed64.Mul(a.Raw, b.RawY)); }
        public static F64Vec2 operator *(F64Vec2 a, F64 b) { return new F64Vec2(Fixed64.Mul(a.RawX, b.Raw), Fixed64.Mul(a.RawY, b.Raw)); }
        public static F64Vec2 operator /(F64 a, F64Vec2 b) { return new F64Vec2(Fixed64.DivPrecise(a.Raw, b.RawX), Fixed64.DivPrecise(a.Raw, b.RawY)); }
        public static F64Vec2 operator /(F64Vec2 a, F64 b) { return new F64Vec2(Fixed64.DivPrecise(a.RawX, b.Raw), Fixed64.DivPrecise(a.RawY, b.Raw)); }
        public static F64Vec2 operator %(F64 a, F64Vec2 b) { return new F64Vec2(a.Raw % b.RawX, a.Raw % b.RawY); }
        public static F64Vec2 operator %(F64Vec2 a, F64 b) { return new F64Vec2(a.RawX % b.Raw, a.RawY % b.Raw); }

        public static bool operator ==(F64Vec2 a, F64Vec2 b) { return a.RawX == b.RawX && a.RawY == b.RawY; }
        public static bool operator !=(F64Vec2 a, F64Vec2 b) { return a.RawX != b.RawX || a.RawY != b.RawY; }

        public static F64Vec2 Div(F64Vec2 a, F64 b) { long oob = Fixed64.Rcp(b.Raw); return new F64Vec2(Fixed64.Mul(a.RawX, oob), Fixed64.Mul(a.RawY, oob)); }
        public static F64Vec2 DivFast(F64Vec2 a, F64 b) { long oob = Fixed64.RcpFast(b.Raw); return new F64Vec2(Fixed64.Mul(a.RawX, oob), Fixed64.Mul(a.RawY, oob)); }
        public static F64Vec2 DivFastest(F64Vec2 a, F64 b) { long oob = Fixed64.RcpFastest(b.Raw); return new F64Vec2(Fixed64.Mul(a.RawX, oob), Fixed64.Mul(a.RawY, oob)); }
        public static F64Vec2 Div(F64Vec2 a, F64Vec2 b) { return new F64Vec2(Fixed64.Div(a.RawX, b.RawX), Fixed64.Div(a.RawY, b.RawY)); }
        public static F64Vec2 DivFast(F64Vec2 a, F64Vec2 b) { return new F64Vec2(Fixed64.DivFast(a.RawX, b.RawX), Fixed64.DivFast(a.RawY, b.RawY)); }
        public static F64Vec2 DivFastest(F64Vec2 a, F64Vec2 b) { return new F64Vec2(Fixed64.DivFastest(a.RawX, b.RawX), Fixed64.DivFastest(a.RawY, b.RawY)); }
        public static F64Vec2 SqrtPrecise(F64Vec2 a) { return new F64Vec2(Fixed64.SqrtPrecise(a.RawX), Fixed64.SqrtPrecise(a.RawY)); }
        public static F64Vec2 Sqrt(F64Vec2 a) { return new F64Vec2(Fixed64.Sqrt(a.RawX), Fixed64.Sqrt(a.RawY)); }
        public static F64Vec2 SqrtFast(F64Vec2 a) { return new F64Vec2(Fixed64.SqrtFast(a.RawX), Fixed64.SqrtFast(a.RawY)); }
        public static F64Vec2 SqrtFastest(F64Vec2 a) { return new F64Vec2(Fixed64.SqrtFastest(a.RawX), Fixed64.SqrtFastest(a.RawY)); }
        public static F64Vec2 RSqrt(F64Vec2 a) { return new F64Vec2(Fixed64.RSqrt(a.RawX), Fixed64.RSqrt(a.RawY)); }
        public static F64Vec2 RSqrtFast(F64Vec2 a) { return new F64Vec2(Fixed64.RSqrtFast(a.RawX), Fixed64.RSqrtFast(a.RawY)); }
        public static F64Vec2 RSqrtFastest(F64Vec2 a) { return new F64Vec2(Fixed64.RSqrtFastest(a.RawX), Fixed64.RSqrtFastest(a.RawY)); }
        public static F64Vec2 Rcp(F64Vec2 a) { return new F64Vec2(Fixed64.Rcp(a.RawX), Fixed64.Rcp(a.RawY)); }
        public static F64Vec2 RcpFast(F64Vec2 a) { return new F64Vec2(Fixed64.RcpFast(a.RawX), Fixed64.RcpFast(a.RawY)); }
        public static F64Vec2 RcpFastest(F64Vec2 a) { return new F64Vec2(Fixed64.RcpFastest(a.RawX), Fixed64.RcpFastest(a.RawY)); }
        public static F64Vec2 Exp(F64Vec2 a) { return new F64Vec2(Fixed64.Exp(a.RawX), Fixed64.Exp(a.RawY)); }
        public static F64Vec2 ExpFast(F64Vec2 a) { return new F64Vec2(Fixed64.ExpFast(a.RawX), Fixed64.ExpFast(a.RawY)); }
        public static F64Vec2 ExpFastest(F64Vec2 a) { return new F64Vec2(Fixed64.ExpFastest(a.RawX), Fixed64.ExpFastest(a.RawY)); }
        public static F64Vec2 Exp2(F64Vec2 a) { return new F64Vec2(Fixed64.Exp2(a.RawX), Fixed64.Exp2(a.RawY)); }
        public static F64Vec2 Exp2Fast(F64Vec2 a) { return new F64Vec2(Fixed64.Exp2Fast(a.RawX), Fixed64.Exp2Fast(a.RawY)); }
        public static F64Vec2 Exp2Fastest(F64Vec2 a) { return new F64Vec2(Fixed64.Exp2Fastest(a.RawX), Fixed64.Exp2Fastest(a.RawY)); }
        public static F64Vec2 Log(F64Vec2 a) { return new F64Vec2(Fixed64.Log(a.RawX), Fixed64.Log(a.RawY)); }
        public static F64Vec2 LogFast(F64Vec2 a) { return new F64Vec2(Fixed64.LogFast(a.RawX), Fixed64.LogFast(a.RawY)); }
        public static F64Vec2 LogFastest(F64Vec2 a) { return new F64Vec2(Fixed64.LogFastest(a.RawX), Fixed64.LogFastest(a.RawY)); }
        public static F64Vec2 Log2(F64Vec2 a) { return new F64Vec2(Fixed64.Log2(a.RawX), Fixed64.Log2(a.RawY)); }
        public static F64Vec2 Log2Fast(F64Vec2 a) { return new F64Vec2(Fixed64.Log2Fast(a.RawX), Fixed64.Log2Fast(a.RawY)); }
        public static F64Vec2 Log2Fastest(F64Vec2 a) { return new F64Vec2(Fixed64.Log2Fastest(a.RawX), Fixed64.Log2Fastest(a.RawY)); }
        public static F64Vec2 Sin(F64Vec2 a) { return new F64Vec2(Fixed64.Sin(a.RawX), Fixed64.Sin(a.RawY)); }
        public static F64Vec2 SinFast(F64Vec2 a) { return new F64Vec2(Fixed64.SinFast(a.RawX), Fixed64.SinFast(a.RawY)); }
        public static F64Vec2 SinFastest(F64Vec2 a) { return new F64Vec2(Fixed64.SinFastest(a.RawX), Fixed64.SinFastest(a.RawY)); }
        public static F64Vec2 Cos(F64Vec2 a) { return new F64Vec2(Fixed64.Cos(a.RawX), Fixed64.Cos(a.RawY)); }
        public static F64Vec2 CosFast(F64Vec2 a) { return new F64Vec2(Fixed64.CosFast(a.RawX), Fixed64.CosFast(a.RawY)); }
        public static F64Vec2 CosFastest(F64Vec2 a) { return new F64Vec2(Fixed64.CosFastest(a.RawX), Fixed64.CosFastest(a.RawY)); }

        public static F64Vec2 Pow(F64Vec2 a, F64 b) { return new F64Vec2(Fixed64.Pow(a.RawX, b.Raw), Fixed64.Pow(a.RawY, b.Raw)); }
        public static F64Vec2 PowFast(F64Vec2 a, F64 b) { return new F64Vec2(Fixed64.PowFast(a.RawX, b.Raw), Fixed64.PowFast(a.RawY, b.Raw)); }
        public static F64Vec2 PowFastest(F64Vec2 a, F64 b) { return new F64Vec2(Fixed64.PowFastest(a.RawX, b.Raw), Fixed64.PowFastest(a.RawY, b.Raw)); }
        public static F64Vec2 Pow(F64 a, F64Vec2 b) { return new F64Vec2(Fixed64.Pow(a.Raw, b.RawX), Fixed64.Pow(a.Raw, b.RawY)); }
        public static F64Vec2 PowFast(F64 a, F64Vec2 b) { return new F64Vec2(Fixed64.PowFast(a.Raw, b.RawX), Fixed64.PowFast(a.Raw, b.RawY)); }
        public static F64Vec2 PowFastest(F64 a, F64Vec2 b) { return new F64Vec2(Fixed64.PowFastest(a.Raw, b.RawX), Fixed64.PowFastest(a.Raw, b.RawY)); }
        public static F64Vec2 Pow(F64Vec2 a, F64Vec2 b) { return new F64Vec2(Fixed64.Pow(a.RawX, b.RawX), Fixed64.Pow(a.RawY, b.RawY)); }
        public static F64Vec2 PowFast(F64Vec2 a, F64Vec2 b) { return new F64Vec2(Fixed64.PowFast(a.RawX, b.RawX), Fixed64.PowFast(a.RawY, b.RawY)); }
        public static F64Vec2 PowFastest(F64Vec2 a, F64Vec2 b) { return new F64Vec2(Fixed64.PowFastest(a.RawX, b.RawX), Fixed64.PowFastest(a.RawY, b.RawY)); }

        public static F64 Length(F64Vec2 a) { return F64.FromRaw(Fixed64.Sqrt(Fixed64.Mul(a.RawX, a.RawX) + Fixed64.Mul(a.RawY, a.RawY))); }
        public static F64 LengthFast(F64Vec2 a) { return F64.FromRaw(Fixed64.SqrtFast(Fixed64.Mul(a.RawX, a.RawX) + Fixed64.Mul(a.RawY, a.RawY))); }
        public static F64 LengthFastest(F64Vec2 a) { return F64.FromRaw(Fixed64.SqrtFastest(Fixed64.Mul(a.RawX, a.RawX) + Fixed64.Mul(a.RawY, a.RawY))); }
        public static F64 LengthSqr(F64Vec2 a) { return F64.FromRaw(Fixed64.Mul(a.RawX, a.RawX) + Fixed64.Mul(a.RawY, a.RawY)); }
        public static F64Vec2 Normalize(F64Vec2 a) { F64 ooLen = F64.FromRaw(Fixed64.RSqrt(Fixed64.Mul(a.RawX, a.RawX) + Fixed64.Mul(a.RawY, a.RawY))); return ooLen * a; }
        public static F64Vec2 NormalizeFast(F64Vec2 a) { F64 ooLen = F64.FromRaw(Fixed64.RSqrtFast(Fixed64.Mul(a.RawX, a.RawX) + Fixed64.Mul(a.RawY, a.RawY))); return ooLen * a; }
        public static F64Vec2 NormalizeFastest(F64Vec2 a) { F64 ooLen = F64.FromRaw(Fixed64.RSqrtFastest(Fixed64.Mul(a.RawX, a.RawX) + Fixed64.Mul(a.RawY, a.RawY))); return ooLen * a; }

        public static F64 Dot(F64Vec2 a, F64Vec2 b) { return F64.FromRaw(Fixed64.Mul(a.RawX, b.RawX) + Fixed64.Mul(a.RawY, b.RawY)); }
        public static F64 Distance(F64Vec2 a, F64Vec2 b) { return Length(a - b); }
        public static F64 DistanceFast(F64Vec2 a, F64Vec2 b) { return LengthFast(a - b); }
        public static F64 DistanceFastest(F64Vec2 a, F64Vec2 b) { return LengthFastest(a - b); }

        public static F64Vec2 Min(F64Vec2 a, F64Vec2 b) { return new F64Vec2(Fixed64.Min(a.RawX, b.RawX), Fixed64.Min(a.RawY, b.RawY)); }
        public static F64Vec2 Max(F64Vec2 a, F64Vec2 b) { return new F64Vec2(Fixed64.Max(a.RawX, b.RawX), Fixed64.Max(a.RawY, b.RawY)); }

        public static F64Vec2 Clamp(F64Vec2 a, F64 min, F64 max)
        {
            return new F64Vec2(
                Fixed64.Clamp(a.RawX, min.Raw, max.Raw),
                Fixed64.Clamp(a.RawY, min.Raw, max.Raw));
        }

        public static F64Vec2 Clamp(F64Vec2 a, F64Vec2 min, F64Vec2 max)
        {
            return new F64Vec2(
                Fixed64.Clamp(a.RawX, min.RawX, max.RawX),
                Fixed64.Clamp(a.RawY, min.RawY, max.RawY));
        }

        public static F64Vec2 Lerp(F64Vec2 a, F64Vec2 b, F64 t)
        {
            long tr = t.Raw;
            return new F64Vec2(
                Fixed64.Lerp(a.RawX, b.RawX, tr),
                Fixed64.Lerp(a.RawY, b.RawY, tr));
        }

        public bool Equals(F64Vec2 other)
        {
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is F64Vec2))
                return false;
            return ((F64Vec2)obj) == this;
        }

        public override string ToString()
        {
            return "(" + Fixed64.ToString(RawX) + ", " + Fixed64.ToString(RawY) + ")";
        }

        public override int GetHashCode()
        {
            return RawX.GetHashCode() ^ RawY.GetHashCode() * 7919;
        }
    }
}
