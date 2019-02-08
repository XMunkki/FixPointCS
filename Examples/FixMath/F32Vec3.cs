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
    /// Vector3 struct with signed 16.16 fixed point components.
    /// </summary>
    [Serializable]
    public struct F32Vec3 : IEquatable<F32Vec3>
    {
        public static F32Vec3 Zero      { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec3(Fixed32.Zero, Fixed32.Zero, Fixed32.Zero); } }
        public static F32Vec3 One       { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec3(Fixed32.One, Fixed32.One, Fixed32.One); } }
        public static F32Vec3 Down      { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec3(Fixed32.Zero, Fixed32.Neg1, Fixed32.Zero); } }
        public static F32Vec3 Up        { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec3(Fixed32.Zero, Fixed32.One, Fixed32.Zero); } }
        public static F32Vec3 Left      { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec3(Fixed32.Neg1, Fixed32.Zero, Fixed32.Zero); } }
        public static F32Vec3 Right     { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec3(Fixed32.One, Fixed32.Zero, Fixed32.Zero); } }
        public static F32Vec3 Forward   { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec3(Fixed32.Zero, Fixed32.Zero, Fixed32.One); } }
        public static F32Vec3 Back      { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec3(Fixed32.Zero, Fixed32.Zero, Fixed32.Neg1); } }
        public static F32Vec3 AxisX     { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec3(Fixed32.One, Fixed32.Zero, Fixed32.Zero); } }
        public static F32Vec3 AxisY     { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec3(Fixed32.Zero, Fixed32.One, Fixed32.Zero); } }
        public static F32Vec3 AxisZ     { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec3(Fixed32.Zero, Fixed32.Zero, Fixed32.One); } }

        // Raw components
        public int RawX;
        public int RawY;
        public int RawZ;

        // F32 accessors
        public F32 X { get { return F32.FromRaw(RawX); } set { RawX = value.Raw; } }
        public F32 Y { get { return F32.FromRaw(RawY); } set { RawY = value.Raw; } }
        public F32 Z { get { return F32.FromRaw(RawZ); } set { RawZ = value.Raw; } }

        public F32Vec3(F32 x, F32 y, F32 z)
        {
            RawX = x.Raw;
            RawY = y.Raw;
            RawZ = z.Raw;
        }

        // raw ctor only for internal usage
        private F32Vec3(int x, int y, int z)
        {
            RawX = x;
            RawY = y;
            RawZ = z;
        }

        public static F32Vec3 FromInt(int x, int y, int z) { return new F32Vec3(F32.FromInt(x), F32.FromInt(y), F32.FromInt(z)); }
        public static F32Vec3 FromFloat(float x, float y, float z) { return new F32Vec3(F32.FromFloat(x), F32.FromFloat(y), F32.FromFloat(z)); }
        public static F32Vec3 FromDouble(double x, double y, double z) { return new F32Vec3(F32.FromDouble(x), F32.FromDouble(y), F32.FromDouble(z)); }

        public static F32Vec3 operator -(F32Vec3 a) { return new F32Vec3(-a.RawX, -a.RawY, -a.RawZ); }
        public static F32Vec3 operator +(F32Vec3 a, F32Vec3 b) { return new F32Vec3(a.RawX + b.RawX, a.RawY + b.RawY, a.RawZ + b.RawZ); }
        public static F32Vec3 operator -(F32Vec3 a, F32Vec3 b) { return new F32Vec3(a.RawX - b.RawX, a.RawY - b.RawY, a.RawZ - b.RawZ); }
        public static F32Vec3 operator *(F32Vec3 a, F32Vec3 b) { return new F32Vec3(Fixed32.Mul(a.RawX, b.RawX), Fixed32.Mul(a.RawY, b.RawY), Fixed32.Mul(a.RawZ, b.RawZ)); }
        public static F32Vec3 operator /(F32Vec3 a, F32Vec3 b) { return new F32Vec3(Fixed32.DivPrecise(a.RawX, b.RawX), Fixed32.DivPrecise(a.RawY, b.RawY), Fixed32.DivPrecise(a.RawZ, b.RawZ)); }
        public static F32Vec3 operator %(F32Vec3 a, F32Vec3 b) { return new F32Vec3(a.RawX % b.RawX, a.RawY % b.RawY, a.RawZ % b.RawZ); }

        public static F32Vec3 operator +(F32 a, F32Vec3 b) { return new F32Vec3(a.Raw + b.RawX, a.Raw + b.RawY, a.Raw + b.RawZ); }
        public static F32Vec3 operator +(F32Vec3 a, F32 b) { return new F32Vec3(a.RawX + b.Raw, a.RawY + b.Raw, a.RawZ + b.Raw); }
        public static F32Vec3 operator -(F32 a, F32Vec3 b) { return new F32Vec3(a.Raw - b.RawX, a.Raw - b.RawY, a.Raw - b.RawZ); }
        public static F32Vec3 operator -(F32Vec3 a, F32 b) { return new F32Vec3(a.RawX - b.Raw, a.RawY - b.Raw, a.RawZ - b.Raw); }
        public static F32Vec3 operator *(F32 a, F32Vec3 b) { return new F32Vec3(Fixed32.Mul(a.Raw, b.RawX), Fixed32.Mul(a.Raw, b.RawY), Fixed32.Mul(a.Raw, b.RawZ)); }
        public static F32Vec3 operator *(F32Vec3 a, F32 b) { return new F32Vec3(Fixed32.Mul(a.RawX, b.Raw), Fixed32.Mul(a.RawY, b.Raw), Fixed32.Mul(a.RawZ, b.Raw)); }
        public static F32Vec3 operator /(F32 a, F32Vec3 b) { return new F32Vec3(Fixed32.DivPrecise(a.Raw, b.RawX), Fixed32.DivPrecise(a.Raw, b.RawY), Fixed32.DivPrecise(a.Raw, b.RawZ)); }
        public static F32Vec3 operator /(F32Vec3 a, F32 b) { return new F32Vec3(Fixed32.DivPrecise(a.RawX, b.Raw), Fixed32.DivPrecise(a.RawY, b.Raw), Fixed32.DivPrecise(a.RawZ, b.Raw)); }
        public static F32Vec3 operator %(F32 a, F32Vec3 b) { return new F32Vec3(a.Raw % b.RawX, a.Raw % b.RawY, a.Raw % b.RawZ); }
        public static F32Vec3 operator %(F32Vec3 a, F32 b) { return new F32Vec3(a.RawX % b.Raw, a.RawY % b.Raw, a.RawZ % b.Raw); }

        public static bool operator ==(F32Vec3 a, F32Vec3 b) { return a.RawX == b.RawX && a.RawY == b.RawY && a.RawZ == b.RawZ; }
        public static bool operator !=(F32Vec3 a, F32Vec3 b) { return a.RawX != b.RawX || a.RawY != b.RawY || a.RawZ != b.RawZ; }

        public static F32Vec3 Div(F32Vec3 a, F32 b) { int oob = Fixed32.Rcp(b.Raw); return new F32Vec3(Fixed32.Mul(a.RawX, oob), Fixed32.Mul(a.RawY, oob), Fixed32.Mul(a.RawZ, oob)); }
        public static F32Vec3 DivFast(F32Vec3 a, F32 b) { int oob = Fixed32.RcpFast(b.Raw); return new F32Vec3(Fixed32.Mul(a.RawX, oob), Fixed32.Mul(a.RawY, oob), Fixed32.Mul(a.RawZ, oob)); }
        public static F32Vec3 DivFastest(F32Vec3 a, F32 b) { int oob = Fixed32.RcpFastest(b.Raw); return new F32Vec3(Fixed32.Mul(a.RawX, oob), Fixed32.Mul(a.RawY, oob), Fixed32.Mul(a.RawZ, oob)); }
        public static F32Vec3 Div(F32Vec3 a, F32Vec3 b) { return new F32Vec3(Fixed32.Div(a.RawX, b.RawX), Fixed32.Div(a.RawY, b.RawY), Fixed32.Div(a.RawZ, b.RawZ)); }
        public static F32Vec3 DivFast(F32Vec3 a, F32Vec3 b) { return new F32Vec3(Fixed32.DivFast(a.RawX, b.RawX), Fixed32.DivFast(a.RawY, b.RawY), Fixed32.DivFast(a.RawZ, b.RawZ)); }
        public static F32Vec3 DivFastest(F32Vec3 a, F32Vec3 b) { return new F32Vec3(Fixed32.DivFastest(a.RawX, b.RawX), Fixed32.DivFastest(a.RawY, b.RawY), Fixed32.DivFastest(a.RawZ, b.RawZ)); }
        public static F32Vec3 SqrtPrecise(F32Vec3 a) { return new F32Vec3(Fixed32.SqrtPrecise(a.RawX), Fixed32.SqrtPrecise(a.RawY), Fixed32.SqrtPrecise(a.RawZ)); }
        public static F32Vec3 Sqrt(F32Vec3 a) { return new F32Vec3(Fixed32.Sqrt(a.RawX), Fixed32.Sqrt(a.RawY), Fixed32.Sqrt(a.RawZ)); }
        public static F32Vec3 SqrtFast(F32Vec3 a) { return new F32Vec3(Fixed32.SqrtFast(a.RawX), Fixed32.SqrtFast(a.RawY), Fixed32.SqrtFast(a.RawZ)); }
        public static F32Vec3 SqrtFastest(F32Vec3 a) { return new F32Vec3(Fixed32.SqrtFastest(a.RawX), Fixed32.SqrtFastest(a.RawY), Fixed32.SqrtFastest(a.RawZ)); }
        public static F32Vec3 RSqrt(F32Vec3 a) { return new F32Vec3(Fixed32.RSqrt(a.RawX), Fixed32.RSqrt(a.RawY), Fixed32.RSqrt(a.RawZ)); }
        public static F32Vec3 RSqrtFast(F32Vec3 a) { return new F32Vec3(Fixed32.RSqrtFast(a.RawX), Fixed32.RSqrtFast(a.RawY), Fixed32.RSqrtFast(a.RawZ)); }
        public static F32Vec3 RSqrtFastest(F32Vec3 a) { return new F32Vec3(Fixed32.RSqrtFastest(a.RawX), Fixed32.RSqrtFastest(a.RawY), Fixed32.RSqrtFastest(a.RawZ)); }
        public static F32Vec3 Rcp(F32Vec3 a) { return new F32Vec3(Fixed32.Rcp(a.RawX), Fixed32.Rcp(a.RawY), Fixed32.Rcp(a.RawZ)); }
        public static F32Vec3 RcpFast(F32Vec3 a) { return new F32Vec3(Fixed32.RcpFast(a.RawX), Fixed32.RcpFast(a.RawY), Fixed32.RcpFast(a.RawZ)); }
        public static F32Vec3 RcpFastest(F32Vec3 a) { return new F32Vec3(Fixed32.RcpFastest(a.RawX), Fixed32.RcpFastest(a.RawY), Fixed32.RcpFastest(a.RawZ)); }
        public static F32Vec3 Exp(F32Vec3 a) { return new F32Vec3(Fixed32.Exp(a.RawX), Fixed32.Exp(a.RawY), Fixed32.Exp(a.RawZ)); }
        public static F32Vec3 ExpFast(F32Vec3 a) { return new F32Vec3(Fixed32.ExpFast(a.RawX), Fixed32.ExpFast(a.RawY), Fixed32.ExpFast(a.RawZ)); }
        public static F32Vec3 ExpFastest(F32Vec3 a) { return new F32Vec3(Fixed32.ExpFastest(a.RawX), Fixed32.ExpFastest(a.RawY), Fixed32.ExpFastest(a.RawZ)); }
        public static F32Vec3 Exp2(F32Vec3 a) { return new F32Vec3(Fixed32.Exp2(a.RawX), Fixed32.Exp2(a.RawY), Fixed32.Exp2(a.RawZ)); }
        public static F32Vec3 Exp2Fast(F32Vec3 a) { return new F32Vec3(Fixed32.Exp2Fast(a.RawX), Fixed32.Exp2Fast(a.RawY), Fixed32.Exp2Fast(a.RawZ)); }
        public static F32Vec3 Exp2Fastest(F32Vec3 a) { return new F32Vec3(Fixed32.Exp2Fastest(a.RawX), Fixed32.Exp2Fastest(a.RawY), Fixed32.Exp2Fastest(a.RawZ)); }
        public static F32Vec3 Log(F32Vec3 a) { return new F32Vec3(Fixed32.Log(a.RawX), Fixed32.Log(a.RawY), Fixed32.Log(a.RawZ)); }
        public static F32Vec3 LogFast(F32Vec3 a) { return new F32Vec3(Fixed32.LogFast(a.RawX), Fixed32.LogFast(a.RawY), Fixed32.LogFast(a.RawZ)); }
        public static F32Vec3 LogFastest(F32Vec3 a) { return new F32Vec3(Fixed32.LogFastest(a.RawX), Fixed32.LogFastest(a.RawY), Fixed32.LogFastest(a.RawZ)); }
        public static F32Vec3 Log2(F32Vec3 a) { return new F32Vec3(Fixed32.Log2(a.RawX), Fixed32.Log2(a.RawY), Fixed32.Log2(a.RawZ)); }
        public static F32Vec3 Log2Fast(F32Vec3 a) { return new F32Vec3(Fixed32.Log2Fast(a.RawX), Fixed32.Log2Fast(a.RawY), Fixed32.Log2Fast(a.RawZ)); }
        public static F32Vec3 Log2Fastest(F32Vec3 a) { return new F32Vec3(Fixed32.Log2Fastest(a.RawX), Fixed32.Log2Fastest(a.RawY), Fixed32.Log2Fastest(a.RawZ)); }
        public static F32Vec3 Sin(F32Vec3 a) { return new F32Vec3(Fixed32.Sin(a.RawX), Fixed32.Sin(a.RawY), Fixed32.Sin(a.RawZ)); }
        public static F32Vec3 SinFast(F32Vec3 a) { return new F32Vec3(Fixed32.SinFast(a.RawX), Fixed32.SinFast(a.RawY), Fixed32.SinFast(a.RawZ)); }
        public static F32Vec3 SinFastest(F32Vec3 a) { return new F32Vec3(Fixed32.SinFastest(a.RawX), Fixed32.SinFastest(a.RawY), Fixed32.SinFastest(a.RawZ)); }
        public static F32Vec3 Cos(F32Vec3 a) { return new F32Vec3(Fixed32.Cos(a.RawX), Fixed32.Cos(a.RawY), Fixed32.Cos(a.RawZ)); }
        public static F32Vec3 CosFast(F32Vec3 a) { return new F32Vec3(Fixed32.CosFast(a.RawX), Fixed32.CosFast(a.RawY), Fixed32.CosFast(a.RawZ)); }
        public static F32Vec3 CosFastest(F32Vec3 a) { return new F32Vec3(Fixed32.CosFastest(a.RawX), Fixed32.CosFastest(a.RawY), Fixed32.CosFastest(a.RawZ)); }

        public static F32Vec3 Pow(F32Vec3 a, F32 b) { return new F32Vec3(Fixed32.Pow(a.RawX, b.Raw), Fixed32.Pow(a.RawY, b.Raw), Fixed32.Pow(a.RawZ, b.Raw)); }
        public static F32Vec3 PowFast(F32Vec3 a, F32 b) { return new F32Vec3(Fixed32.PowFast(a.RawX, b.Raw), Fixed32.PowFast(a.RawY, b.Raw), Fixed32.PowFast(a.RawZ, b.Raw)); }
        public static F32Vec3 PowFastest(F32Vec3 a, F32 b) { return new F32Vec3(Fixed32.PowFastest(a.RawX, b.Raw), Fixed32.PowFastest(a.RawY, b.Raw), Fixed32.PowFastest(a.RawZ, b.Raw)); }
        public static F32Vec3 Pow(F32 a, F32Vec3 b) { return new F32Vec3(Fixed32.Pow(a.Raw, b.RawX), Fixed32.Pow(a.Raw, b.RawY), Fixed32.Pow(a.Raw, b.RawZ)); }
        public static F32Vec3 PowFast(F32 a, F32Vec3 b) { return new F32Vec3(Fixed32.PowFast(a.Raw, b.RawX), Fixed32.PowFast(a.Raw, b.RawY), Fixed32.PowFast(a.Raw, b.RawZ)); }
        public static F32Vec3 PowFastest(F32 a, F32Vec3 b) { return new F32Vec3(Fixed32.PowFastest(a.Raw, b.RawX), Fixed32.PowFastest(a.Raw, b.RawY), Fixed32.PowFastest(a.Raw, b.RawZ)); }
        public static F32Vec3 Pow(F32Vec3 a, F32Vec3 b) { return new F32Vec3(Fixed32.Pow(a.RawX, b.RawX), Fixed32.Pow(a.RawY, b.RawY), Fixed32.Pow(a.RawZ, b.RawZ)); }
        public static F32Vec3 PowFast(F32Vec3 a, F32Vec3 b) { return new F32Vec3(Fixed32.PowFast(a.RawX, b.RawX), Fixed32.PowFast(a.RawY, b.RawY), Fixed32.PowFast(a.RawZ, b.RawZ)); }
        public static F32Vec3 PowFastest(F32Vec3 a, F32Vec3 b) { return new F32Vec3(Fixed32.PowFastest(a.RawX, b.RawX), Fixed32.PowFastest(a.RawY, b.RawY), Fixed32.PowFastest(a.RawZ, b.RawZ)); }

        public static F32 Length(F32Vec3 a) { return F32.FromRaw(Fixed32.Sqrt(Fixed32.Mul(a.RawX, a.RawX) + Fixed32.Mul(a.RawY, a.RawY) + Fixed32.Mul(a.RawZ, a.RawZ))); }
        public static F32 LengthFast(F32Vec3 a) { return F32.FromRaw(Fixed32.SqrtFast(Fixed32.Mul(a.RawX, a.RawX) + Fixed32.Mul(a.RawY, a.RawY) + Fixed32.Mul(a.RawZ, a.RawZ))); }
        public static F32 LengthFastest(F32Vec3 a) { return F32.FromRaw(Fixed32.SqrtFastest(Fixed32.Mul(a.RawX, a.RawX) + Fixed32.Mul(a.RawY, a.RawY) + Fixed32.Mul(a.RawZ, a.RawZ))); }
        public static F32 LengthSqr(F32Vec3 a) { return F32.FromRaw(Fixed32.Mul(a.RawX, a.RawX) + Fixed32.Mul(a.RawY, a.RawY) + Fixed32.Mul(a.RawZ, a.RawZ)); }
        public static F32Vec3 Normalize(F32Vec3 a) { F32 ooLen = F32.FromRaw(Fixed32.RSqrt(Fixed32.Mul(a.RawX, a.RawX) + Fixed32.Mul(a.RawY, a.RawY) + Fixed32.Mul(a.RawZ, a.RawZ))); return ooLen * a; }
        public static F32Vec3 NormalizeFast(F32Vec3 a) { F32 ooLen = F32.FromRaw(Fixed32.RSqrtFast(Fixed32.Mul(a.RawX, a.RawX) + Fixed32.Mul(a.RawY, a.RawY) + Fixed32.Mul(a.RawZ, a.RawZ))); return ooLen * a; }
        public static F32Vec3 NormalizeFastest(F32Vec3 a) { F32 ooLen = F32.FromRaw(Fixed32.RSqrtFastest(Fixed32.Mul(a.RawX, a.RawX) + Fixed32.Mul(a.RawY, a.RawY) + Fixed32.Mul(a.RawZ, a.RawZ))); return ooLen * a; }

        public static F32 Dot(F32Vec3 a, F32Vec3 b) { return F32.FromRaw(Fixed32.Mul(a.RawX, b.RawX) + Fixed32.Mul(a.RawY, b.RawY) + Fixed32.Mul(a.RawZ, b.RawZ)); }
        public static F32 Distance(F32Vec3 a, F32Vec3 b) { return Length(a - b); }
        public static F32 DistanceFast(F32Vec3 a, F32Vec3 b) { return LengthFast(a - b); }
        public static F32 DistanceFastest(F32Vec3 a, F32Vec3 b) { return LengthFastest(a - b); }

        public static F32Vec3 Min(F32Vec3 a, F32Vec3 b) { return new F32Vec3(Fixed32.Min(a.RawX, b.RawX), Fixed32.Min(a.RawY, b.RawY), Fixed32.Min(a.RawZ, b.RawZ)); }
        public static F32Vec3 Max(F32Vec3 a, F32Vec3 b) { return new F32Vec3(Fixed32.Max(a.RawX, b.RawX), Fixed32.Max(a.RawY, b.RawY), Fixed32.Max(a.RawZ, b.RawZ)); }

        public static F32Vec3 Clamp(F32Vec3 a, F32 min, F32 max)
        {
            return new F32Vec3(
                Fixed32.Clamp(a.RawX, min.Raw, max.Raw),
                Fixed32.Clamp(a.RawY, min.Raw, max.Raw),
                Fixed32.Clamp(a.RawZ, min.Raw, max.Raw));
        }

        public static F32Vec3 Clamp(F32Vec3 a, F32Vec3 min, F32Vec3 max)
        {
            return new F32Vec3(
                Fixed32.Clamp(a.RawX, min.RawX, max.RawX),
                Fixed32.Clamp(a.RawY, min.RawY, max.RawY),
                Fixed32.Clamp(a.RawZ, min.RawZ, max.RawZ));
        }

        public static F32Vec3 Lerp(F32Vec3 a, F32Vec3 b, F32 t)
        {
            int tr = t.Raw;
            return new F32Vec3(
                Fixed32.Lerp(a.RawX, b.RawX, tr),
                Fixed32.Lerp(a.RawY, b.RawY, tr),
                Fixed32.Lerp(a.RawZ, b.RawZ, tr));
        }

        public static F32Vec3 Cross(F32Vec3 a, F32Vec3 b)
        {
            return new F32Vec3(
                Fixed32.Mul(a.RawY, b.RawZ) - Fixed32.Mul(a.RawZ, b.RawY),
                Fixed32.Mul(a.RawZ, b.RawX) - Fixed32.Mul(a.RawX, b.RawZ),
                Fixed32.Mul(a.RawX, b.RawY) - Fixed32.Mul(a.RawY, b.RawX));
        }

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
            return "(" + Fixed32.ToString(RawX) + ", " + Fixed32.ToString(RawY) + ", " + Fixed32.ToString(RawZ) + ")";
        }

        public override int GetHashCode()
        {
            return RawX.GetHashCode() ^ RawY.GetHashCode() * 7919 ^ RawZ.GetHashCode() * 4513;
        }
    }
}
