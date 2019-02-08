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
    /// Vector2 struct with signed 16.16 fixed point components.
    /// </summary>
    [Serializable]
    public struct F32Vec2 : IEquatable<F32Vec2>
    {
        // Constants
        public static F32Vec2 Zero     { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec2(Fixed32.Zero, Fixed32.Zero); } }
        public static F32Vec2 One      { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec2(Fixed32.One, Fixed32.One); } }
        public static F32Vec2 Down     { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec2(Fixed32.Zero, Fixed32.Neg1); } }
        public static F32Vec2 Up       { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec2(Fixed32.Zero, Fixed32.One); } }
        public static F32Vec2 Left     { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec2(Fixed32.Neg1, Fixed32.Zero); } }
        public static F32Vec2 Right    { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec2(Fixed32.One, Fixed32.Zero); } }
        public static F32Vec2 AxisX    { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec2(Fixed32.One, Fixed32.Zero); } }
        public static F32Vec2 AxisY    { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec2(Fixed32.Zero, Fixed32.One); } }

        // Raw components
        public int RawX;
        public int RawY;

        // F32 accessors
        public F32 X { get { return F32.FromRaw(RawX); } set { RawX = value.Raw; } }
        public F32 Y { get { return F32.FromRaw(RawY); } set { RawY = value.Raw; } }

        public F32Vec2(F32 x, F32 y)
        {
            RawX = x.Raw;
            RawY = y.Raw;
        }

        // raw ctor only for internal usage
        private F32Vec2(int x, int y)
        {
            RawX = x;
            RawY = y;
        }

        public static F32Vec2 FromInt(int x, int y) { return new F32Vec2(Fixed32.FromInt(x), Fixed32.FromInt(y)); }
        public static F32Vec2 FromFloat(float x, float y) { return new F32Vec2(Fixed32.FromFloat(x), Fixed32.FromFloat(y)); }
        public static F32Vec2 FromDouble(double x, double y) { return new F32Vec2(Fixed32.FromDouble(x), Fixed32.FromDouble(y)); }

        public static F32Vec2 operator -(F32Vec2 a) { return new F32Vec2(-a.RawX, -a.RawY); }
        public static F32Vec2 operator +(F32Vec2 a, F32Vec2 b) { return new F32Vec2(a.RawX + b.RawX, a.RawY + b.RawY); }
        public static F32Vec2 operator -(F32Vec2 a, F32Vec2 b) { return new F32Vec2(a.RawX - b.RawX, a.RawY - b.RawY); }
        public static F32Vec2 operator *(F32Vec2 a, F32Vec2 b) { return new F32Vec2(Fixed32.Mul(a.RawX, b.RawX), Fixed32.Mul(a.RawY, b.RawY)); }
        public static F32Vec2 operator /(F32Vec2 a, F32Vec2 b) { return new F32Vec2(Fixed32.DivPrecise(a.RawX, b.RawX), Fixed32.DivPrecise(a.RawY, b.RawY)); }
        public static F32Vec2 operator %(F32Vec2 a, F32Vec2 b) { return new F32Vec2(a.RawX % b.RawX, a.RawY % b.RawY); }

        public static F32Vec2 operator +(F32 a, F32Vec2 b) { return new F32Vec2(a.Raw + b.RawX, a.Raw + b.RawY); }
        public static F32Vec2 operator +(F32Vec2 a, F32 b) { return new F32Vec2(a.RawX + b.Raw, a.RawY + b.Raw); }
        public static F32Vec2 operator -(F32 a, F32Vec2 b) { return new F32Vec2(a.Raw - b.RawX, a.Raw - b.RawY); }
        public static F32Vec2 operator -(F32Vec2 a, F32 b) { return new F32Vec2(a.RawX - b.Raw, a.RawY - b.Raw); }
        public static F32Vec2 operator *(F32 a, F32Vec2 b) { return new F32Vec2(Fixed32.Mul(a.Raw, b.RawX), Fixed32.Mul(a.Raw, b.RawY)); }
        public static F32Vec2 operator *(F32Vec2 a, F32 b) { return new F32Vec2(Fixed32.Mul(a.RawX, b.Raw), Fixed32.Mul(a.RawY, b.Raw)); }
        public static F32Vec2 operator /(F32 a, F32Vec2 b) { return new F32Vec2(Fixed32.DivPrecise(a.Raw, b.RawX), Fixed32.DivPrecise(a.Raw, b.RawY)); }
        public static F32Vec2 operator /(F32Vec2 a, F32 b) { return new F32Vec2(Fixed32.DivPrecise(a.RawX, b.Raw), Fixed32.DivPrecise(a.RawY, b.Raw)); }
        public static F32Vec2 operator %(F32 a, F32Vec2 b) { return new F32Vec2(a.Raw % b.RawX, a.Raw % b.RawY); }
        public static F32Vec2 operator %(F32Vec2 a, F32 b) { return new F32Vec2(a.RawX % b.Raw, a.RawY % b.Raw); }

        public static bool operator ==(F32Vec2 a, F32Vec2 b) { return a.RawX == b.RawX && a.RawY == b.RawY; }
        public static bool operator !=(F32Vec2 a, F32Vec2 b) { return a.RawX != b.RawX || a.RawY != b.RawY; }

        public static F32Vec2 Div(F32Vec2 a, F32 b) { int oob = Fixed32.Rcp(b.Raw); return new F32Vec2(Fixed32.Mul(a.RawX, oob), Fixed32.Mul(a.RawY, oob)); }
        public static F32Vec2 DivFast(F32Vec2 a, F32 b) { int oob = Fixed32.RcpFast(b.Raw); return new F32Vec2(Fixed32.Mul(a.RawX, oob), Fixed32.Mul(a.RawY, oob)); }
        public static F32Vec2 DivFastest(F32Vec2 a, F32 b) { int oob = Fixed32.RcpFastest(b.Raw); return new F32Vec2(Fixed32.Mul(a.RawX, oob), Fixed32.Mul(a.RawY, oob)); }
        public static F32Vec2 Div(F32Vec2 a, F32Vec2 b) { return new F32Vec2(Fixed32.Div(a.RawX, b.RawX), Fixed32.Div(a.RawY, b.RawY)); }
        public static F32Vec2 DivFast(F32Vec2 a, F32Vec2 b) { return new F32Vec2(Fixed32.DivFast(a.RawX, b.RawX), Fixed32.DivFast(a.RawY, b.RawY)); }
        public static F32Vec2 DivFastest(F32Vec2 a, F32Vec2 b) { return new F32Vec2(Fixed32.DivFastest(a.RawX, b.RawX), Fixed32.DivFastest(a.RawY, b.RawY)); }
        public static F32Vec2 SqrtPrecise(F32Vec2 a) { return new F32Vec2(Fixed32.SqrtPrecise(a.RawX), Fixed32.SqrtPrecise(a.RawY)); }
        public static F32Vec2 Sqrt(F32Vec2 a) { return new F32Vec2(Fixed32.Sqrt(a.RawX), Fixed32.Sqrt(a.RawY)); }
        public static F32Vec2 SqrtFast(F32Vec2 a) { return new F32Vec2(Fixed32.SqrtFast(a.RawX), Fixed32.SqrtFast(a.RawY)); }
        public static F32Vec2 SqrtFastest(F32Vec2 a) { return new F32Vec2(Fixed32.SqrtFastest(a.RawX), Fixed32.SqrtFastest(a.RawY)); }
        public static F32Vec2 RSqrt(F32Vec2 a) { return new F32Vec2(Fixed32.RSqrt(a.RawX), Fixed32.RSqrt(a.RawY)); }
        public static F32Vec2 RSqrtFast(F32Vec2 a) { return new F32Vec2(Fixed32.RSqrtFast(a.RawX), Fixed32.RSqrtFast(a.RawY)); }
        public static F32Vec2 RSqrtFastest(F32Vec2 a) { return new F32Vec2(Fixed32.RSqrtFastest(a.RawX), Fixed32.RSqrtFastest(a.RawY)); }
        public static F32Vec2 Rcp(F32Vec2 a) { return new F32Vec2(Fixed32.Rcp(a.RawX), Fixed32.Rcp(a.RawY)); }
        public static F32Vec2 RcpFast(F32Vec2 a) { return new F32Vec2(Fixed32.RcpFast(a.RawX), Fixed32.RcpFast(a.RawY)); }
        public static F32Vec2 RcpFastest(F32Vec2 a) { return new F32Vec2(Fixed32.RcpFastest(a.RawX), Fixed32.RcpFastest(a.RawY)); }
        public static F32Vec2 Exp(F32Vec2 a) { return new F32Vec2(Fixed32.Exp(a.RawX), Fixed32.Exp(a.RawY)); }
        public static F32Vec2 ExpFast(F32Vec2 a) { return new F32Vec2(Fixed32.ExpFast(a.RawX), Fixed32.ExpFast(a.RawY)); }
        public static F32Vec2 ExpFastest(F32Vec2 a) { return new F32Vec2(Fixed32.ExpFastest(a.RawX), Fixed32.ExpFastest(a.RawY)); }
        public static F32Vec2 Exp2(F32Vec2 a) { return new F32Vec2(Fixed32.Exp2(a.RawX), Fixed32.Exp2(a.RawY)); }
        public static F32Vec2 Exp2Fast(F32Vec2 a) { return new F32Vec2(Fixed32.Exp2Fast(a.RawX), Fixed32.Exp2Fast(a.RawY)); }
        public static F32Vec2 Exp2Fastest(F32Vec2 a) { return new F32Vec2(Fixed32.Exp2Fastest(a.RawX), Fixed32.Exp2Fastest(a.RawY)); }
        public static F32Vec2 Log(F32Vec2 a) { return new F32Vec2(Fixed32.Log(a.RawX), Fixed32.Log(a.RawY)); }
        public static F32Vec2 LogFast(F32Vec2 a) { return new F32Vec2(Fixed32.LogFast(a.RawX), Fixed32.LogFast(a.RawY)); }
        public static F32Vec2 LogFastest(F32Vec2 a) { return new F32Vec2(Fixed32.LogFastest(a.RawX), Fixed32.LogFastest(a.RawY)); }
        public static F32Vec2 Log2(F32Vec2 a) { return new F32Vec2(Fixed32.Log2(a.RawX), Fixed32.Log2(a.RawY)); }
        public static F32Vec2 Log2Fast(F32Vec2 a) { return new F32Vec2(Fixed32.Log2Fast(a.RawX), Fixed32.Log2Fast(a.RawY)); }
        public static F32Vec2 Log2Fastest(F32Vec2 a) { return new F32Vec2(Fixed32.Log2Fastest(a.RawX), Fixed32.Log2Fastest(a.RawY)); }
        public static F32Vec2 Sin(F32Vec2 a) { return new F32Vec2(Fixed32.Sin(a.RawX), Fixed32.Sin(a.RawY)); }
        public static F32Vec2 SinFast(F32Vec2 a) { return new F32Vec2(Fixed32.SinFast(a.RawX), Fixed32.SinFast(a.RawY)); }
        public static F32Vec2 SinFastest(F32Vec2 a) { return new F32Vec2(Fixed32.SinFastest(a.RawX), Fixed32.SinFastest(a.RawY)); }
        public static F32Vec2 Cos(F32Vec2 a) { return new F32Vec2(Fixed32.Cos(a.RawX), Fixed32.Cos(a.RawY)); }
        public static F32Vec2 CosFast(F32Vec2 a) { return new F32Vec2(Fixed32.CosFast(a.RawX), Fixed32.CosFast(a.RawY)); }
        public static F32Vec2 CosFastest(F32Vec2 a) { return new F32Vec2(Fixed32.CosFastest(a.RawX), Fixed32.CosFastest(a.RawY)); }

        public static F32Vec2 Pow(F32Vec2 a, F32 b) { return new F32Vec2(Fixed32.Pow(a.RawX, b.Raw), Fixed32.Pow(a.RawY, b.Raw)); }
        public static F32Vec2 PowFast(F32Vec2 a, F32 b) { return new F32Vec2(Fixed32.PowFast(a.RawX, b.Raw), Fixed32.PowFast(a.RawY, b.Raw)); }
        public static F32Vec2 PowFastest(F32Vec2 a, F32 b) { return new F32Vec2(Fixed32.PowFastest(a.RawX, b.Raw), Fixed32.PowFastest(a.RawY, b.Raw)); }
        public static F32Vec2 Pow(F32 a, F32Vec2 b) { return new F32Vec2(Fixed32.Pow(a.Raw, b.RawX), Fixed32.Pow(a.Raw, b.RawY)); }
        public static F32Vec2 PowFast(F32 a, F32Vec2 b) { return new F32Vec2(Fixed32.PowFast(a.Raw, b.RawX), Fixed32.PowFast(a.Raw, b.RawY)); }
        public static F32Vec2 PowFastest(F32 a, F32Vec2 b) { return new F32Vec2(Fixed32.PowFastest(a.Raw, b.RawX), Fixed32.PowFastest(a.Raw, b.RawY)); }
        public static F32Vec2 Pow(F32Vec2 a, F32Vec2 b) { return new F32Vec2(Fixed32.Pow(a.RawX, b.RawX), Fixed32.Pow(a.RawY, b.RawY)); }
        public static F32Vec2 PowFast(F32Vec2 a, F32Vec2 b) { return new F32Vec2(Fixed32.PowFast(a.RawX, b.RawX), Fixed32.PowFast(a.RawY, b.RawY)); }
        public static F32Vec2 PowFastest(F32Vec2 a, F32Vec2 b) { return new F32Vec2(Fixed32.PowFastest(a.RawX, b.RawX), Fixed32.PowFastest(a.RawY, b.RawY)); }

        public static F32 Length(F32Vec2 a) { return F32.FromRaw(Fixed32.Sqrt(Fixed32.Mul(a.RawX, a.RawX) + Fixed32.Mul(a.RawY, a.RawY))); }
        public static F32 LengthFast(F32Vec2 a) { return F32.FromRaw(Fixed32.SqrtFast(Fixed32.Mul(a.RawX, a.RawX) + Fixed32.Mul(a.RawY, a.RawY))); }
        public static F32 LengthFastest(F32Vec2 a) { return F32.FromRaw(Fixed32.SqrtFastest(Fixed32.Mul(a.RawX, a.RawX) + Fixed32.Mul(a.RawY, a.RawY))); }
        public static F32 LengthSqr(F32Vec2 a) { return F32.FromRaw(Fixed32.Mul(a.RawX, a.RawX) + Fixed32.Mul(a.RawY, a.RawY)); }
        public static F32Vec2 Normalize(F32Vec2 a) { F32 ooLen = F32.FromRaw(Fixed32.RSqrt(Fixed32.Mul(a.RawX, a.RawX) + Fixed32.Mul(a.RawY, a.RawY))); return ooLen * a; }
        public static F32Vec2 NormalizeFast(F32Vec2 a) { F32 ooLen = F32.FromRaw(Fixed32.RSqrtFast(Fixed32.Mul(a.RawX, a.RawX) + Fixed32.Mul(a.RawY, a.RawY))); return ooLen * a; }
        public static F32Vec2 NormalizeFastest(F32Vec2 a) { F32 ooLen = F32.FromRaw(Fixed32.RSqrtFastest(Fixed32.Mul(a.RawX, a.RawX) + Fixed32.Mul(a.RawY, a.RawY))); return ooLen * a; }

        public static F32 Dot(F32Vec2 a, F32Vec2 b) { return F32.FromRaw(Fixed32.Mul(a.RawX, b.RawX) + Fixed32.Mul(a.RawY, b.RawY)); }
        public static F32 Distance(F32Vec2 a, F32Vec2 b) { return Length(a - b); }
        public static F32 DistanceFast(F32Vec2 a, F32Vec2 b) { return LengthFast(a - b); }
        public static F32 DistanceFastest(F32Vec2 a, F32Vec2 b) { return LengthFastest(a - b); }

        public static F32Vec2 Min(F32Vec2 a, F32Vec2 b) { return new F32Vec2(Fixed32.Min(a.RawX, b.RawX), Fixed32.Min(a.RawY, b.RawY)); }
        public static F32Vec2 Max(F32Vec2 a, F32Vec2 b) { return new F32Vec2(Fixed32.Max(a.RawX, b.RawX), Fixed32.Max(a.RawY, b.RawY)); }

        public static F32Vec2 Clamp(F32Vec2 a, F32 min, F32 max)
        {
            return new F32Vec2(
                Fixed32.Clamp(a.RawX, min.Raw, max.Raw),
                Fixed32.Clamp(a.RawY, min.Raw, max.Raw));
        }

        public static F32Vec2 Clamp(F32Vec2 a, F32Vec2 min, F32Vec2 max)
        {
            return new F32Vec2(
                Fixed32.Clamp(a.RawX, min.RawX, max.RawX),
                Fixed32.Clamp(a.RawY, min.RawY, max.RawY));
        }

        public static F32Vec2 Lerp(F32Vec2 a, F32Vec2 b, F32 t)
        {
            int tr = t.Raw;
            return new F32Vec2(
                Fixed32.Lerp(a.RawX, b.RawX, tr),
                Fixed32.Lerp(a.RawY, b.RawY, tr));
        }

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
            return "(" + Fixed32.ToString(RawX) + ", " + Fixed32.ToString(RawY) + ")";
        }

        public override int GetHashCode()
        {
            return RawX.GetHashCode() ^ RawY.GetHashCode() * 7919;
        }
    }
}
