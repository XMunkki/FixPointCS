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
    /// Vector3 struct with signed 32.32 fixed point components.
    /// </summary>
    [Serializable]
    public struct F64Vec3 : IEquatable<F64Vec3>
    {
        public static F64Vec3 Zero      { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec3(Fixed64.Zero, Fixed64.Zero, Fixed64.Zero); } }
        public static F64Vec3 One       { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec3(Fixed64.One, Fixed64.One, Fixed64.One); } }
        public static F64Vec3 Down      { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec3(Fixed64.Zero, Fixed64.Neg1, Fixed64.Zero); } }
        public static F64Vec3 Up        { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec3(Fixed64.Zero, Fixed64.One, Fixed64.Zero); } }
        public static F64Vec3 Left      { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec3(Fixed64.Neg1, Fixed64.Zero, Fixed64.Zero); } }
        public static F64Vec3 Right     { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec3(Fixed64.One, Fixed64.Zero, Fixed64.Zero); } }
        public static F64Vec3 Forward   { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec3(Fixed64.Zero, Fixed64.Zero, Fixed64.One); } }
        public static F64Vec3 Back      { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec3(Fixed64.Zero, Fixed64.Zero, Fixed64.Neg1); } }
        public static F64Vec3 AxisX     { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec3(Fixed64.One, Fixed64.Zero, Fixed64.Zero); } }
        public static F64Vec3 AxisY     { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec3(Fixed64.Zero, Fixed64.One, Fixed64.Zero); } }
        public static F64Vec3 AxisZ     { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec3(Fixed64.Zero, Fixed64.Zero, Fixed64.One); } }

        public long RawX;
        public long RawY;
        public long RawZ;

        public F64 X { get { return F64.FromRaw(RawX); } set { RawX = value.Raw; } }
        public F64 Y { get { return F64.FromRaw(RawY); } set { RawY = value.Raw; } }
        public F64 Z { get { return F64.FromRaw(RawZ); } set { RawZ = value.Raw; } }

        public F64Vec3(F64 x, F64 y, F64 z)
        {
            RawX = x.Raw;
            RawY = y.Raw;
            RawZ = z.Raw;
        }

        public F64Vec3(F32Vec3 src)
        {
            RawX = src.RawX << 16;
            RawY = src.RawY << 16;
            RawZ = src.RawZ << 16;
        }

        // raw ctor for internal use only
        private F64Vec3(long x, long y, long z)
        {
            RawX = x;
            RawY = y;
            RawZ = z;
        }

        public static F64Vec3 FromInt(int x, int y, int z) { return new F64Vec3(Fixed64.FromInt(x), Fixed64.FromInt(y), Fixed64.FromInt(z)); }
        public static F64Vec3 FromFloat(float x, float y, float z) { return new F64Vec3(Fixed64.FromFloat(x), Fixed64.FromFloat(y), Fixed64.FromFloat(z)); }
        public static F64Vec3 FromDouble(double x, double y, double z) { return new F64Vec3(Fixed64.FromDouble(x), Fixed64.FromDouble(y), Fixed64.FromDouble(z)); }

        public static F64Vec3 operator -(F64Vec3 a) { return new F64Vec3(-a.RawX, -a.RawY, -a.RawZ); }
        public static F64Vec3 operator +(F64Vec3 a, F64Vec3 b) { return new F64Vec3(a.RawX + b.RawX, a.RawY + b.RawY, a.RawZ + b.RawZ); }
        public static F64Vec3 operator -(F64Vec3 a, F64Vec3 b) { return new F64Vec3(a.RawX - b.RawX, a.RawY - b.RawY, a.RawZ - b.RawZ); }
        public static F64Vec3 operator *(F64Vec3 a, F64Vec3 b) { return new F64Vec3(Fixed64.Mul(a.RawX, b.RawX), Fixed64.Mul(a.RawY, b.RawY), Fixed64.Mul(a.RawZ, b.RawZ)); }
        public static F64Vec3 operator /(F64Vec3 a, F64Vec3 b) { return new F64Vec3(Fixed64.DivPrecise(a.RawX, b.RawX), Fixed64.DivPrecise(a.RawY, b.RawY), Fixed64.DivPrecise(a.RawZ, b.RawZ)); }
        public static F64Vec3 operator %(F64Vec3 a, F64Vec3 b) { return new F64Vec3(a.RawX % b.RawX, a.RawY % b.RawY, a.RawZ % b.RawZ); }

        public static F64Vec3 operator +(F64 a, F64Vec3 b) { return new F64Vec3(a.Raw + b.RawX, a.Raw + b.RawY, a.Raw + b.RawZ); }
        public static F64Vec3 operator +(F64Vec3 a, F64 b) { return new F64Vec3(a.RawX + b.Raw, a.RawY + b.Raw, a.RawZ + b.Raw); }
        public static F64Vec3 operator -(F64 a, F64Vec3 b) { return new F64Vec3(a.Raw - b.RawX, a.Raw - b.RawY, a.Raw - b.RawZ); }
        public static F64Vec3 operator -(F64Vec3 a, F64 b) { return new F64Vec3(a.RawX - b.Raw, a.RawY - b.Raw, a.RawZ - b.Raw); }
        public static F64Vec3 operator *(F64 a, F64Vec3 b) { return new F64Vec3(Fixed64.Mul(a.Raw, b.RawX), Fixed64.Mul(a.Raw, b.RawY), Fixed64.Mul(a.Raw, b.RawZ)); }
        public static F64Vec3 operator *(F64Vec3 a, F64 b) { return new F64Vec3(Fixed64.Mul(a.RawX, b.Raw), Fixed64.Mul(a.RawY, b.Raw), Fixed64.Mul(a.RawZ, b.Raw)); }
        public static F64Vec3 operator /(F64 a, F64Vec3 b) { return new F64Vec3(Fixed64.DivPrecise(a.Raw, b.RawX), Fixed64.DivPrecise(a.Raw, b.RawY), Fixed64.DivPrecise(a.Raw, b.RawZ)); }
        public static F64Vec3 operator /(F64Vec3 a, F64 b) { return new F64Vec3(Fixed64.DivPrecise(a.RawX, b.Raw), Fixed64.DivPrecise(a.RawY, b.Raw), Fixed64.DivPrecise(a.RawZ, b.Raw)); }
        public static F64Vec3 operator %(F64 a, F64Vec3 b) { return new F64Vec3(a.Raw % b.RawX, a.Raw % b.RawY, a.Raw % b.RawZ); }
        public static F64Vec3 operator %(F64Vec3 a, F64 b) { return new F64Vec3(a.RawX % b.Raw, a.RawY % b.Raw, a.RawZ % b.Raw); }

        public static bool operator ==(F64Vec3 a, F64Vec3 b) { return a.RawX == b.RawX && a.RawY == b.RawY && a.RawZ == b.RawZ; }
        public static bool operator !=(F64Vec3 a, F64Vec3 b) { return a.RawX != b.RawX || a.RawY != b.RawY || a.RawZ != b.RawZ; }

        public static F64Vec3 Div(F64Vec3 a, F64 b) { long oob = Fixed64.Rcp(b.Raw); return new F64Vec3(Fixed64.Mul(a.RawX, oob), Fixed64.Mul(a.RawY, oob), Fixed64.Mul(a.RawZ, oob)); }
        public static F64Vec3 DivFast(F64Vec3 a, F64 b) { long oob = Fixed64.RcpFast(b.Raw); return new F64Vec3(Fixed64.Mul(a.RawX, oob), Fixed64.Mul(a.RawY, oob), Fixed64.Mul(a.RawZ, oob)); }
        public static F64Vec3 DivFastest(F64Vec3 a, F64 b) { long oob = Fixed64.RcpFastest(b.Raw); return new F64Vec3(Fixed64.Mul(a.RawX, oob), Fixed64.Mul(a.RawY, oob), Fixed64.Mul(a.RawZ, oob)); }
        public static F64Vec3 Div(F64Vec3 a, F64Vec3 b) { return new F64Vec3(Fixed64.Div(a.RawX, b.RawX), Fixed64.Div(a.RawY, b.RawY), Fixed64.Div(a.RawZ, b.RawZ)); }
        public static F64Vec3 DivFast(F64Vec3 a, F64Vec3 b) { return new F64Vec3(Fixed64.DivFast(a.RawX, b.RawX), Fixed64.DivFast(a.RawY, b.RawY), Fixed64.DivFast(a.RawZ, b.RawZ)); }
        public static F64Vec3 DivFastest(F64Vec3 a, F64Vec3 b) { return new F64Vec3(Fixed64.DivFastest(a.RawX, b.RawX), Fixed64.DivFastest(a.RawY, b.RawY), Fixed64.DivFastest(a.RawZ, b.RawZ)); }
        public static F64Vec3 SqrtPrecise(F64Vec3 a) { return new F64Vec3(Fixed64.SqrtPrecise(a.RawX), Fixed64.SqrtPrecise(a.RawY), Fixed64.SqrtPrecise(a.RawZ)); }
        public static F64Vec3 Sqrt(F64Vec3 a) { return new F64Vec3(Fixed64.Sqrt(a.RawX), Fixed64.Sqrt(a.RawY), Fixed64.Sqrt(a.RawZ)); }
        public static F64Vec3 SqrtFast(F64Vec3 a) { return new F64Vec3(Fixed64.SqrtFast(a.RawX), Fixed64.SqrtFast(a.RawY), Fixed64.SqrtFast(a.RawZ)); }
        public static F64Vec3 SqrtFastest(F64Vec3 a) { return new F64Vec3(Fixed64.SqrtFastest(a.RawX), Fixed64.SqrtFastest(a.RawY), Fixed64.SqrtFastest(a.RawZ)); }
        public static F64Vec3 RSqrt(F64Vec3 a) { return new F64Vec3(Fixed64.RSqrt(a.RawX), Fixed64.RSqrt(a.RawY), Fixed64.RSqrt(a.RawZ)); }
        public static F64Vec3 RSqrtFast(F64Vec3 a) { return new F64Vec3(Fixed64.RSqrtFast(a.RawX), Fixed64.RSqrtFast(a.RawY), Fixed64.RSqrtFast(a.RawZ)); }
        public static F64Vec3 RSqrtFastest(F64Vec3 a) { return new F64Vec3(Fixed64.RSqrtFastest(a.RawX), Fixed64.RSqrtFastest(a.RawY), Fixed64.RSqrtFastest(a.RawZ)); }
        public static F64Vec3 Rcp(F64Vec3 a) { return new F64Vec3(Fixed64.Rcp(a.RawX), Fixed64.Rcp(a.RawY), Fixed64.Rcp(a.RawZ)); }
        public static F64Vec3 RcpFast(F64Vec3 a) { return new F64Vec3(Fixed64.RcpFast(a.RawX), Fixed64.RcpFast(a.RawY), Fixed64.RcpFast(a.RawZ)); }
        public static F64Vec3 RcpFastest(F64Vec3 a) { return new F64Vec3(Fixed64.RcpFastest(a.RawX), Fixed64.RcpFastest(a.RawY), Fixed64.RcpFastest(a.RawZ)); }
        public static F64Vec3 Exp(F64Vec3 a) { return new F64Vec3(Fixed64.Exp(a.RawX), Fixed64.Exp(a.RawY), Fixed64.Exp(a.RawZ)); }
        public static F64Vec3 ExpFast(F64Vec3 a) { return new F64Vec3(Fixed64.ExpFast(a.RawX), Fixed64.ExpFast(a.RawY), Fixed64.ExpFast(a.RawZ)); }
        public static F64Vec3 ExpFastest(F64Vec3 a) { return new F64Vec3(Fixed64.ExpFastest(a.RawX), Fixed64.ExpFastest(a.RawY), Fixed64.ExpFastest(a.RawZ)); }
        public static F64Vec3 Exp2(F64Vec3 a) { return new F64Vec3(Fixed64.Exp2(a.RawX), Fixed64.Exp2(a.RawY), Fixed64.Exp2(a.RawZ)); }
        public static F64Vec3 Exp2Fast(F64Vec3 a) { return new F64Vec3(Fixed64.Exp2Fast(a.RawX), Fixed64.Exp2Fast(a.RawY), Fixed64.Exp2Fast(a.RawZ)); }
        public static F64Vec3 Exp2Fastest(F64Vec3 a) { return new F64Vec3(Fixed64.Exp2Fastest(a.RawX), Fixed64.Exp2Fastest(a.RawY), Fixed64.Exp2Fastest(a.RawZ)); }
        public static F64Vec3 Log(F64Vec3 a) { return new F64Vec3(Fixed64.Log(a.RawX), Fixed64.Log(a.RawY), Fixed64.Log(a.RawZ)); }
        public static F64Vec3 LogFast(F64Vec3 a) { return new F64Vec3(Fixed64.LogFast(a.RawX), Fixed64.LogFast(a.RawY), Fixed64.LogFast(a.RawZ)); }
        public static F64Vec3 LogFastest(F64Vec3 a) { return new F64Vec3(Fixed64.LogFastest(a.RawX), Fixed64.LogFastest(a.RawY), Fixed64.LogFastest(a.RawZ)); }
        public static F64Vec3 Log2(F64Vec3 a) { return new F64Vec3(Fixed64.Log2(a.RawX), Fixed64.Log2(a.RawY), Fixed64.Log2(a.RawZ)); }
        public static F64Vec3 Log2Fast(F64Vec3 a) { return new F64Vec3(Fixed64.Log2Fast(a.RawX), Fixed64.Log2Fast(a.RawY), Fixed64.Log2Fast(a.RawZ)); }
        public static F64Vec3 Log2Fastest(F64Vec3 a) { return new F64Vec3(Fixed64.Log2Fastest(a.RawX), Fixed64.Log2Fastest(a.RawY), Fixed64.Log2Fastest(a.RawZ)); }
        public static F64Vec3 Sin(F64Vec3 a) { return new F64Vec3(Fixed64.Sin(a.RawX), Fixed64.Sin(a.RawY), Fixed64.Sin(a.RawZ)); }
        public static F64Vec3 SinFast(F64Vec3 a) { return new F64Vec3(Fixed64.SinFast(a.RawX), Fixed64.SinFast(a.RawY), Fixed64.SinFast(a.RawZ)); }
        public static F64Vec3 SinFastest(F64Vec3 a) { return new F64Vec3(Fixed64.SinFastest(a.RawX), Fixed64.SinFastest(a.RawY), Fixed64.SinFastest(a.RawZ)); }
        public static F64Vec3 Cos(F64Vec3 a) { return new F64Vec3(Fixed64.Cos(a.RawX), Fixed64.Cos(a.RawY), Fixed64.Cos(a.RawZ)); }
        public static F64Vec3 CosFast(F64Vec3 a) { return new F64Vec3(Fixed64.CosFast(a.RawX), Fixed64.CosFast(a.RawY), Fixed64.CosFast(a.RawZ)); }
        public static F64Vec3 CosFastest(F64Vec3 a) { return new F64Vec3(Fixed64.CosFastest(a.RawX), Fixed64.CosFastest(a.RawY), Fixed64.CosFastest(a.RawZ)); }

        public static F64Vec3 Pow(F64Vec3 a, F64 b) { return new F64Vec3(Fixed64.Pow(a.RawX, b.Raw), Fixed64.Pow(a.RawY, b.Raw), Fixed64.Pow(a.RawZ, b.Raw)); }
        public static F64Vec3 PowFast(F64Vec3 a, F64 b) { return new F64Vec3(Fixed64.PowFast(a.RawX, b.Raw), Fixed64.PowFast(a.RawY, b.Raw), Fixed64.PowFast(a.RawZ, b.Raw)); }
        public static F64Vec3 PowFastest(F64Vec3 a, F64 b) { return new F64Vec3(Fixed64.PowFastest(a.RawX, b.Raw), Fixed64.PowFastest(a.RawY, b.Raw), Fixed64.PowFastest(a.RawZ, b.Raw)); }
        public static F64Vec3 Pow(F64 a, F64Vec3 b) { return new F64Vec3(Fixed64.Pow(a.Raw, b.RawX), Fixed64.Pow(a.Raw, b.RawY), Fixed64.Pow(a.Raw, b.RawZ)); }
        public static F64Vec3 PowFast(F64 a, F64Vec3 b) { return new F64Vec3(Fixed64.PowFast(a.Raw, b.RawX), Fixed64.PowFast(a.Raw, b.RawY), Fixed64.PowFast(a.Raw, b.RawZ)); }
        public static F64Vec3 PowFastest(F64 a, F64Vec3 b) { return new F64Vec3(Fixed64.PowFastest(a.Raw, b.RawX), Fixed64.PowFastest(a.Raw, b.RawY), Fixed64.PowFastest(a.Raw, b.RawZ)); }
        public static F64Vec3 Pow(F64Vec3 a, F64Vec3 b) { return new F64Vec3(Fixed64.Pow(a.RawX, b.RawX), Fixed64.Pow(a.RawY, b.RawY), Fixed64.Pow(a.RawZ, b.RawZ)); }
        public static F64Vec3 PowFast(F64Vec3 a, F64Vec3 b) { return new F64Vec3(Fixed64.PowFast(a.RawX, b.RawX), Fixed64.PowFast(a.RawY, b.RawY), Fixed64.PowFast(a.RawZ, b.RawZ)); }
        public static F64Vec3 PowFastest(F64Vec3 a, F64Vec3 b) { return new F64Vec3(Fixed64.PowFastest(a.RawX, b.RawX), Fixed64.PowFastest(a.RawY, b.RawY), Fixed64.PowFastest(a.RawZ, b.RawZ)); }

        public static F64 Length(F64Vec3 a) { return F64.FromRaw(Fixed64.Sqrt(Fixed64.Mul(a.RawX, a.RawX) + Fixed64.Mul(a.RawY, a.RawY) + Fixed64.Mul(a.RawZ, a.RawZ))); }
        public static F64 LengthFast(F64Vec3 a) { return F64.FromRaw(Fixed64.SqrtFast(Fixed64.Mul(a.RawX, a.RawX) + Fixed64.Mul(a.RawY, a.RawY) + Fixed64.Mul(a.RawZ, a.RawZ))); }
        public static F64 LengthFastest(F64Vec3 a) { return F64.FromRaw(Fixed64.SqrtFastest(Fixed64.Mul(a.RawX, a.RawX) + Fixed64.Mul(a.RawY, a.RawY) + Fixed64.Mul(a.RawZ, a.RawZ))); }
        public static F64 LengthSqr(F64Vec3 a) { return F64.FromRaw(Fixed64.Mul(a.RawX, a.RawX) + Fixed64.Mul(a.RawY, a.RawY) + Fixed64.Mul(a.RawZ, a.RawZ)); }
        public static F64Vec3 Normalize(F64Vec3 a) { F64 ooLen = F64.FromRaw(Fixed64.RSqrt(Fixed64.Mul(a.RawX, a.RawX) + Fixed64.Mul(a.RawY, a.RawY) + Fixed64.Mul(a.RawZ, a.RawZ))); return ooLen * a; }
        public static F64Vec3 NormalizeFast(F64Vec3 a) { F64 ooLen = F64.FromRaw(Fixed64.RSqrtFast(Fixed64.Mul(a.RawX, a.RawX) + Fixed64.Mul(a.RawY, a.RawY) + Fixed64.Mul(a.RawZ, a.RawZ))); return ooLen * a; }
        public static F64Vec3 NormalizeFastest(F64Vec3 a) { F64 ooLen = F64.FromRaw(Fixed64.RSqrtFastest(Fixed64.Mul(a.RawX, a.RawX) + Fixed64.Mul(a.RawY, a.RawY) + Fixed64.Mul(a.RawZ, a.RawZ))); return ooLen * a; }

        public static F64 Dot(F64Vec3 a, F64Vec3 b) { return F64.FromRaw(Fixed64.Mul(a.RawX, b.RawX) + Fixed64.Mul(a.RawY, b.RawY) + Fixed64.Mul(a.RawZ, b.RawZ)); }
        public static F64 Distance(F64Vec3 a, F64Vec3 b) { return Length(a - b); }
        public static F64 DistanceFast(F64Vec3 a, F64Vec3 b) { return LengthFast(a - b); }
        public static F64 DistanceFastest(F64Vec3 a, F64Vec3 b) { return LengthFastest(a - b); }

        public static F64Vec3 Min(F64Vec3 a, F64Vec3 b) { return new F64Vec3(Fixed64.Min(a.RawX, b.RawX), Fixed64.Min(a.RawY, b.RawY), Fixed64.Min(a.RawZ, b.RawZ)); }
        public static F64Vec3 Max(F64Vec3 a, F64Vec3 b) { return new F64Vec3(Fixed64.Max(a.RawX, b.RawX), Fixed64.Max(a.RawY, b.RawY), Fixed64.Max(a.RawZ, b.RawZ)); }

        public static F64Vec3 Clamp(F64Vec3 a, F64 min, F64 max)
        {
            return new F64Vec3(
                Fixed64.Clamp(a.RawX, min.Raw, max.Raw),
                Fixed64.Clamp(a.RawY, min.Raw, max.Raw),
                Fixed64.Clamp(a.RawZ, min.Raw, max.Raw));
        }

        public static F64Vec3 Clamp(F64Vec3 a, F64Vec3 min, F64Vec3 max)
        {
            return new F64Vec3(
                Fixed64.Clamp(a.RawX, min.RawX, max.RawX),
                Fixed64.Clamp(a.RawY, min.RawY, max.RawY),
                Fixed64.Clamp(a.RawZ, min.RawZ, max.RawZ));
        }

        public static F64Vec3 Lerp(F64Vec3 a, F64Vec3 b, F64 t)
        {
            long tr = t.Raw;
            return new F64Vec3(
                Fixed64.Lerp(a.RawX, b.RawX, tr),
                Fixed64.Lerp(a.RawY, b.RawY, tr),
                Fixed64.Lerp(a.RawZ, b.RawZ, tr));
        }

        public static F64Vec3 Cross(F64Vec3 a, F64Vec3 b)
        {
            return new F64Vec3(
                Fixed64.Mul(a.RawY, b.RawZ) - Fixed64.Mul(a.RawZ, b.RawY),
                Fixed64.Mul(a.RawZ, b.RawX) - Fixed64.Mul(a.RawX, b.RawZ),
                Fixed64.Mul(a.RawX, b.RawY) - Fixed64.Mul(a.RawY, b.RawX));
        }

        public bool Equals(F64Vec3 other)
        {
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is F64Vec3))
                return false;
            return ((F64Vec3)obj) == this;
        }

        public override string ToString()
        {
            return "(" + Fixed64.ToString(RawX) + ", " + Fixed64.ToString(RawY) + ", " + Fixed64.ToString(RawZ) + ")";
        }

        public override int GetHashCode()
        {
            return RawX.GetHashCode() ^ RawY.GetHashCode() * 7919 ^ RawZ.GetHashCode() * 4513;
        }
    }
}
