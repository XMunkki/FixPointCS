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
    /// Vector4 struct with signed 16.16 fixed point components.
    /// </summary>
    [Serializable]
    public struct F64Vec4 : IEquatable<F64Vec4>
    {
        public static F64Vec4 Zero      { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec4(Fixed64.Zero, Fixed64.Zero, Fixed64.Zero, Fixed64.Zero); } }
        public static F64Vec4 One       { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec4(Fixed64.One, Fixed64.One, Fixed64.One, Fixed64.One); } }
        public static F64Vec4 AxisX     { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec4(Fixed64.One, Fixed64.Zero, Fixed64.Zero, Fixed64.Zero); } }
        public static F64Vec4 AxisY     { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec4(Fixed64.Zero, Fixed64.One, Fixed64.Zero, Fixed64.Zero); } }
        public static F64Vec4 AxisZ     { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec4(Fixed64.Zero, Fixed64.Zero, Fixed64.One, Fixed64.Zero); } }
        public static F64Vec4 AxisW     { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F64Vec4(Fixed64.Zero, Fixed64.Zero, Fixed64.Zero, Fixed64.One); } }

        // Raw components
        public long RawX;
        public long RawY;
        public long RawZ;
        public long RawW;

        // F64 accessors
        public F64 X { get { return F64.FromRaw(RawX); } set { RawX = value.Raw; } }
        public F64 Y { get { return F64.FromRaw(RawY); } set { RawY = value.Raw; } }
        public F64 Z { get { return F64.FromRaw(RawZ); } set { RawZ = value.Raw; } }
        public F64 W { get { return F64.FromRaw(RawW); } set { RawW = value.Raw; } }

        public F64Vec4(F64 x, F64 y, F64 z, F64 w)
        {
            RawX = x.Raw;
            RawY = y.Raw;
            RawZ = z.Raw;
            RawW = w.Raw;
        }

        // raw ctor only for internal usage
        private F64Vec4(long x, long y, long z, long w)
        {
            RawX = x;
            RawY = y;
            RawZ = z;
            RawW = w;
        }

        public static F64Vec4 FromInt(int x, int y, int z, int w) { return new F64Vec4(F64.FromInt(x), F64.FromInt(y), F64.FromInt(z), F64.FromInt(w)); }
        public static F64Vec4 FromFloat(float x, float y, float z, float w) { return new F64Vec4(F64.FromFloat(x), F64.FromFloat(y), F64.FromFloat(z), F64.FromFloat(w)); }
        public static F64Vec4 FromDouble(double x, double y, double z, double w) { return new F64Vec4(F64.FromDouble(x), F64.FromDouble(y), F64.FromDouble(z), F64.FromDouble(w)); }

        public static F64Vec4 operator -(F64Vec4 a) { return new F64Vec4(-a.RawX, -a.RawY, -a.RawZ, -a.RawW); }
        public static F64Vec4 operator +(F64Vec4 a, F64Vec4 b) { return new F64Vec4(a.RawX + b.RawX, a.RawY + b.RawY, a.RawZ + b.RawZ, a.RawW + b.RawW); }
        public static F64Vec4 operator -(F64Vec4 a, F64Vec4 b) { return new F64Vec4(a.RawX - b.RawX, a.RawY - b.RawY, a.RawZ - b.RawZ, a.RawW - b.RawW); }
        public static F64Vec4 operator *(F64Vec4 a, F64Vec4 b) { return new F64Vec4(Fixed64.Mul(a.RawX, b.RawX), Fixed64.Mul(a.RawY, b.RawY), Fixed64.Mul(a.RawZ, b.RawZ), Fixed64.Mul(a.RawW, b.RawW)); }
        public static F64Vec4 operator /(F64Vec4 a, F64Vec4 b) { return new F64Vec4(Fixed64.DivPrecise(a.RawX, b.RawX), Fixed64.DivPrecise(a.RawY, b.RawY), Fixed64.DivPrecise(a.RawZ, b.RawZ), Fixed64.DivPrecise(a.RawW, b.RawW)); }
        public static F64Vec4 operator %(F64Vec4 a, F64Vec4 b) { return new F64Vec4(a.RawX % b.RawX, a.RawY % b.RawY, a.RawZ % b.RawZ, a.RawW % b.RawW); }

        public static F64Vec4 operator +(F64 a, F64Vec4 b) { return new F64Vec4(a.Raw + b.RawX, a.Raw + b.RawY, a.Raw + b.RawZ, a.Raw + b.RawW); }
        public static F64Vec4 operator +(F64Vec4 a, F64 b) { return new F64Vec4(a.RawX + b.Raw, a.RawY + b.Raw, a.RawZ + b.Raw, a.RawW + b.Raw); }
        public static F64Vec4 operator -(F64 a, F64Vec4 b) { return new F64Vec4(a.Raw - b.RawX, a.Raw - b.RawY, a.Raw - b.RawZ, a.Raw - b.RawW); }
        public static F64Vec4 operator -(F64Vec4 a, F64 b) { return new F64Vec4(a.RawX - b.Raw, a.RawY - b.Raw, a.RawZ - b.Raw, a.RawW - b.Raw); }
        public static F64Vec4 operator *(F64 a, F64Vec4 b) { return new F64Vec4(Fixed64.Mul(a.Raw, b.RawX), Fixed64.Mul(a.Raw, b.RawY), Fixed64.Mul(a.Raw, b.RawZ), Fixed64.Mul(a.Raw, b.RawW)); }
        public static F64Vec4 operator *(F64Vec4 a, F64 b) { return new F64Vec4(Fixed64.Mul(a.RawX, b.Raw), Fixed64.Mul(a.RawY, b.Raw), Fixed64.Mul(a.RawZ, b.Raw), Fixed64.Mul(a.RawW, b.Raw)); }
        public static F64Vec4 operator /(F64 a, F64Vec4 b) { return new F64Vec4(Fixed64.DivPrecise(a.Raw, b.RawX), Fixed64.DivPrecise(a.Raw, b.RawY), Fixed64.DivPrecise(a.Raw, b.RawZ), Fixed64.DivPrecise(a.Raw, b.RawW)); }
        public static F64Vec4 operator /(F64Vec4 a, F64 b) { return new F64Vec4(Fixed64.DivPrecise(a.RawX, b.Raw), Fixed64.DivPrecise(a.RawY, b.Raw), Fixed64.DivPrecise(a.RawZ, b.Raw), Fixed64.DivPrecise(a.RawW, b.Raw)); }
        public static F64Vec4 operator %(F64 a, F64Vec4 b) { return new F64Vec4(a.Raw % b.RawX, a.Raw % b.RawY, a.Raw % b.RawZ, a.Raw % b.RawW); }
        public static F64Vec4 operator %(F64Vec4 a, F64 b) { return new F64Vec4(a.RawX % b.Raw, a.RawY % b.Raw, a.RawZ % b.Raw, a.RawW % b.Raw); }

        public static bool operator ==(F64Vec4 a, F64Vec4 b) { return a.RawX == b.RawX && a.RawY == b.RawY && a.RawZ == b.RawZ && a.RawW == b.RawW; }
        public static bool operator !=(F64Vec4 a, F64Vec4 b) { return a.RawX != b.RawX || a.RawY != b.RawY || a.RawZ != b.RawZ || a.RawW != b.RawW; }

        public static F64Vec4 Div(F64Vec4 a, F64 b) { long oob = Fixed64.Rcp(b.Raw); return new F64Vec4(Fixed64.Mul(a.RawX, oob), Fixed64.Mul(a.RawY, oob), Fixed64.Mul(a.RawZ, oob), Fixed64.Mul(a.RawW, oob)); }
        public static F64Vec4 DivFast(F64Vec4 a, F64 b) { long oob = Fixed64.RcpFast(b.Raw); return new F64Vec4(Fixed64.Mul(a.RawX, oob), Fixed64.Mul(a.RawY, oob), Fixed64.Mul(a.RawZ, oob), Fixed64.Mul(a.RawW, oob)); }
        public static F64Vec4 DivFastest(F64Vec4 a, F64 b) { long oob = Fixed64.RcpFastest(b.Raw); return new F64Vec4(Fixed64.Mul(a.RawX, oob), Fixed64.Mul(a.RawY, oob), Fixed64.Mul(a.RawZ, oob), Fixed64.Mul(a.RawW, oob)); }
        public static F64Vec4 Div(F64Vec4 a, F64Vec4 b) { return new F64Vec4(Fixed64.Div(a.RawX, b.RawX), Fixed64.Div(a.RawY, b.RawY), Fixed64.Div(a.RawZ, b.RawZ), Fixed64.Div(a.RawW, b.RawW)); }
        public static F64Vec4 DivFast(F64Vec4 a, F64Vec4 b) { return new F64Vec4(Fixed64.DivFast(a.RawX, b.RawX), Fixed64.DivFast(a.RawY, b.RawY), Fixed64.DivFast(a.RawZ, b.RawZ), Fixed64.DivFast(a.RawW, b.RawW)); }
        public static F64Vec4 DivFastest(F64Vec4 a, F64Vec4 b) { return new F64Vec4(Fixed64.DivFastest(a.RawX, b.RawX), Fixed64.DivFastest(a.RawY, b.RawY), Fixed64.DivFastest(a.RawZ, b.RawZ), Fixed64.DivFastest(a.RawW, b.RawW)); }
        public static F64Vec4 SqrtPrecise(F64Vec4 a) { return new F64Vec4(Fixed64.SqrtPrecise(a.RawX), Fixed64.SqrtPrecise(a.RawY), Fixed64.SqrtPrecise(a.RawZ), Fixed64.SqrtPrecise(a.RawW)); }
        public static F64Vec4 Sqrt(F64Vec4 a) { return new F64Vec4(Fixed64.Sqrt(a.RawX), Fixed64.Sqrt(a.RawY), Fixed64.Sqrt(a.RawZ), Fixed64.Sqrt(a.RawW)); }
        public static F64Vec4 SqrtFast(F64Vec4 a) { return new F64Vec4(Fixed64.SqrtFast(a.RawX), Fixed64.SqrtFast(a.RawY), Fixed64.SqrtFast(a.RawZ), Fixed64.SqrtFast(a.RawW)); }
        public static F64Vec4 SqrtFastest(F64Vec4 a) { return new F64Vec4(Fixed64.SqrtFastest(a.RawX), Fixed64.SqrtFastest(a.RawY), Fixed64.SqrtFastest(a.RawZ), Fixed64.SqrtFastest(a.RawW)); }
        public static F64Vec4 RSqrt(F64Vec4 a) { return new F64Vec4(Fixed64.RSqrt(a.RawX), Fixed64.RSqrt(a.RawY), Fixed64.RSqrt(a.RawZ), Fixed64.RSqrt(a.RawW)); }
        public static F64Vec4 RSqrtFast(F64Vec4 a) { return new F64Vec4(Fixed64.RSqrtFast(a.RawX), Fixed64.RSqrtFast(a.RawY), Fixed64.RSqrtFast(a.RawZ), Fixed64.RSqrtFast(a.RawW)); }
        public static F64Vec4 RSqrtFastest(F64Vec4 a) { return new F64Vec4(Fixed64.RSqrtFastest(a.RawX), Fixed64.RSqrtFastest(a.RawY), Fixed64.RSqrtFastest(a.RawZ), Fixed64.RSqrtFastest(a.RawW)); }
        public static F64Vec4 Rcp(F64Vec4 a) { return new F64Vec4(Fixed64.Rcp(a.RawX), Fixed64.Rcp(a.RawY), Fixed64.Rcp(a.RawZ), Fixed64.Rcp(a.RawW)); }
        public static F64Vec4 RcpFast(F64Vec4 a) { return new F64Vec4(Fixed64.RcpFast(a.RawX), Fixed64.RcpFast(a.RawY), Fixed64.RcpFast(a.RawZ), Fixed64.RcpFast(a.RawW)); }
        public static F64Vec4 RcpFastest(F64Vec4 a) { return new F64Vec4(Fixed64.RcpFastest(a.RawX), Fixed64.RcpFastest(a.RawY), Fixed64.RcpFastest(a.RawZ), Fixed64.RcpFastest(a.RawW)); }
        public static F64Vec4 Exp(F64Vec4 a) { return new F64Vec4(Fixed64.Exp(a.RawX), Fixed64.Exp(a.RawY), Fixed64.Exp(a.RawZ), Fixed64.Exp(a.RawW)); }
        public static F64Vec4 ExpFast(F64Vec4 a) { return new F64Vec4(Fixed64.ExpFast(a.RawX), Fixed64.ExpFast(a.RawY), Fixed64.ExpFast(a.RawZ), Fixed64.ExpFast(a.RawW)); }
        public static F64Vec4 ExpFastest(F64Vec4 a) { return new F64Vec4(Fixed64.ExpFastest(a.RawX), Fixed64.ExpFastest(a.RawY), Fixed64.ExpFastest(a.RawZ), Fixed64.ExpFastest(a.RawW)); }
        public static F64Vec4 Exp2(F64Vec4 a) { return new F64Vec4(Fixed64.Exp2(a.RawX), Fixed64.Exp2(a.RawY), Fixed64.Exp2(a.RawZ), Fixed64.Exp2(a.RawW)); }
        public static F64Vec4 Exp2Fast(F64Vec4 a) { return new F64Vec4(Fixed64.Exp2Fast(a.RawX), Fixed64.Exp2Fast(a.RawY), Fixed64.Exp2Fast(a.RawZ), Fixed64.Exp2Fast(a.RawW)); }
        public static F64Vec4 Exp2Fastest(F64Vec4 a) { return new F64Vec4(Fixed64.Exp2Fastest(a.RawX), Fixed64.Exp2Fastest(a.RawY), Fixed64.Exp2Fastest(a.RawZ), Fixed64.Exp2Fastest(a.RawW)); }
        public static F64Vec4 Log(F64Vec4 a) { return new F64Vec4(Fixed64.Log(a.RawX), Fixed64.Log(a.RawY), Fixed64.Log(a.RawZ), Fixed64.Log(a.RawW)); }
        public static F64Vec4 LogFast(F64Vec4 a) { return new F64Vec4(Fixed64.LogFast(a.RawX), Fixed64.LogFast(a.RawY), Fixed64.LogFast(a.RawZ), Fixed64.LogFast(a.RawW)); }
        public static F64Vec4 LogFastest(F64Vec4 a) { return new F64Vec4(Fixed64.LogFastest(a.RawX), Fixed64.LogFastest(a.RawY), Fixed64.LogFastest(a.RawZ), Fixed64.LogFastest(a.RawW)); }
        public static F64Vec4 Log2(F64Vec4 a) { return new F64Vec4(Fixed64.Log2(a.RawX), Fixed64.Log2(a.RawY), Fixed64.Log2(a.RawZ), Fixed64.Log2(a.RawW)); }
        public static F64Vec4 Log2Fast(F64Vec4 a) { return new F64Vec4(Fixed64.Log2Fast(a.RawX), Fixed64.Log2Fast(a.RawY), Fixed64.Log2Fast(a.RawZ), Fixed64.Log2Fast(a.RawW)); }
        public static F64Vec4 Log2Fastest(F64Vec4 a) { return new F64Vec4(Fixed64.Log2Fastest(a.RawX), Fixed64.Log2Fastest(a.RawY), Fixed64.Log2Fastest(a.RawZ), Fixed64.Log2Fastest(a.RawW)); }
        public static F64Vec4 Sin(F64Vec4 a) { return new F64Vec4(Fixed64.Sin(a.RawX), Fixed64.Sin(a.RawY), Fixed64.Sin(a.RawZ), Fixed64.Sin(a.RawW)); }
        public static F64Vec4 SinFast(F64Vec4 a) { return new F64Vec4(Fixed64.SinFast(a.RawX), Fixed64.SinFast(a.RawY), Fixed64.SinFast(a.RawZ), Fixed64.SinFast(a.RawW)); }
        public static F64Vec4 SinFastest(F64Vec4 a) { return new F64Vec4(Fixed64.SinFastest(a.RawX), Fixed64.SinFastest(a.RawY), Fixed64.SinFastest(a.RawZ), Fixed64.SinFastest(a.RawW)); }
        public static F64Vec4 Cos(F64Vec4 a) { return new F64Vec4(Fixed64.Cos(a.RawX), Fixed64.Cos(a.RawY), Fixed64.Cos(a.RawZ), Fixed64.Cos(a.RawW)); }
        public static F64Vec4 CosFast(F64Vec4 a) { return new F64Vec4(Fixed64.CosFast(a.RawX), Fixed64.CosFast(a.RawY), Fixed64.CosFast(a.RawZ), Fixed64.CosFast(a.RawW)); }
        public static F64Vec4 CosFastest(F64Vec4 a) { return new F64Vec4(Fixed64.CosFastest(a.RawX), Fixed64.CosFastest(a.RawY), Fixed64.CosFastest(a.RawZ), Fixed64.CosFastest(a.RawW)); }

        public static F64Vec4 Pow(F64Vec4 a, F64 b) { return new F64Vec4(Fixed64.Pow(a.RawX, b.Raw), Fixed64.Pow(a.RawY, b.Raw), Fixed64.Pow(a.RawZ, b.Raw), Fixed64.Pow(a.RawW, b.Raw)); }
        public static F64Vec4 PowFast(F64Vec4 a, F64 b) { return new F64Vec4(Fixed64.PowFast(a.RawX, b.Raw), Fixed64.PowFast(a.RawY, b.Raw), Fixed64.PowFast(a.RawZ, b.Raw), Fixed64.PowFast(a.RawW, b.Raw)); }
        public static F64Vec4 PowFastest(F64Vec4 a, F64 b) { return new F64Vec4(Fixed64.PowFastest(a.RawX, b.Raw), Fixed64.PowFastest(a.RawY, b.Raw), Fixed64.PowFastest(a.RawZ, b.Raw), Fixed64.PowFastest(a.RawW, b.Raw)); }
        public static F64Vec4 Pow(F64 a, F64Vec4 b) { return new F64Vec4(Fixed64.Pow(a.Raw, b.RawX), Fixed64.Pow(a.Raw, b.RawY), Fixed64.Pow(a.Raw, b.RawZ), Fixed64.Pow(a.Raw, b.RawW)); }
        public static F64Vec4 PowFast(F64 a, F64Vec4 b) { return new F64Vec4(Fixed64.PowFast(a.Raw, b.RawX), Fixed64.PowFast(a.Raw, b.RawY), Fixed64.PowFast(a.Raw, b.RawZ), Fixed64.PowFast(a.Raw, b.RawW)); }
        public static F64Vec4 PowFastest(F64 a, F64Vec4 b) { return new F64Vec4(Fixed64.PowFastest(a.Raw, b.RawX), Fixed64.PowFastest(a.Raw, b.RawY), Fixed64.PowFastest(a.Raw, b.RawZ), Fixed64.PowFastest(a.Raw, b.RawW)); }
        public static F64Vec4 Pow(F64Vec4 a, F64Vec4 b) { return new F64Vec4(Fixed64.Pow(a.RawX, b.RawX), Fixed64.Pow(a.RawY, b.RawY), Fixed64.Pow(a.RawZ, b.RawZ), Fixed64.Pow(a.RawW, b.RawW)); }
        public static F64Vec4 PowFast(F64Vec4 a, F64Vec4 b) { return new F64Vec4(Fixed64.PowFast(a.RawX, b.RawX), Fixed64.PowFast(a.RawY, b.RawY), Fixed64.PowFast(a.RawZ, b.RawZ), Fixed64.PowFast(a.RawW, b.RawW)); }
        public static F64Vec4 PowFastest(F64Vec4 a, F64Vec4 b) { return new F64Vec4(Fixed64.PowFastest(a.RawX, b.RawX), Fixed64.PowFastest(a.RawY, b.RawY), Fixed64.PowFastest(a.RawZ, b.RawZ), Fixed64.PowFastest(a.RawW, b.RawW)); }

        public static F64 Length(F64Vec4 a) { return F64.FromRaw(Fixed64.Sqrt(Fixed64.Mul(a.RawX, a.RawX) + Fixed64.Mul(a.RawY, a.RawY) + Fixed64.Mul(a.RawZ, a.RawZ) + Fixed64.Mul(a.RawW, a.RawW))); }
        public static F64 LengthFast(F64Vec4 a) { return F64.FromRaw(Fixed64.SqrtFast(Fixed64.Mul(a.RawX, a.RawX) + Fixed64.Mul(a.RawY, a.RawY) + Fixed64.Mul(a.RawZ, a.RawZ) + Fixed64.Mul(a.RawW, a.RawW))); }
        public static F64 LengthFastest(F64Vec4 a) { return F64.FromRaw(Fixed64.SqrtFastest(Fixed64.Mul(a.RawX, a.RawX) + Fixed64.Mul(a.RawY, a.RawY) + Fixed64.Mul(a.RawZ, a.RawZ) + Fixed64.Mul(a.RawW, a.RawW))); }
        public static F64 LengthSqr(F64Vec4 a) { return F64.FromRaw(Fixed64.Mul(a.RawX, a.RawX) + Fixed64.Mul(a.RawY, a.RawY) + Fixed64.Mul(a.RawZ, a.RawZ) + Fixed64.Mul(a.RawW, a.RawW)); }
        public static F64Vec4 Normalize(F64Vec4 a) { F64 ooLen = F64.FromRaw(Fixed64.RSqrt(Fixed64.Mul(a.RawX, a.RawX) + Fixed64.Mul(a.RawY, a.RawY) + Fixed64.Mul(a.RawZ, a.RawZ) + Fixed64.Mul(a.RawW, a.RawW))); return ooLen * a; }
        public static F64Vec4 NormalizeFast(F64Vec4 a) { F64 ooLen = F64.FromRaw(Fixed64.RSqrtFast(Fixed64.Mul(a.RawX, a.RawX) + Fixed64.Mul(a.RawY, a.RawY) + Fixed64.Mul(a.RawZ, a.RawZ) + Fixed64.Mul(a.RawW, a.RawW))); return ooLen * a; }
        public static F64Vec4 NormalizeFastest(F64Vec4 a) { F64 ooLen = F64.FromRaw(Fixed64.RSqrtFastest(Fixed64.Mul(a.RawX, a.RawX) + Fixed64.Mul(a.RawY, a.RawY) + Fixed64.Mul(a.RawZ, a.RawZ) + Fixed64.Mul(a.RawW, a.RawW))); return ooLen * a; }

        public static F64 Dot(F64Vec4 a, F64Vec4 b) { return F64.FromRaw(Fixed64.Mul(a.RawX, b.RawX) + Fixed64.Mul(a.RawY, b.RawY) + Fixed64.Mul(a.RawZ, b.RawZ) + Fixed64.Mul(a.RawW, b.RawW)); }
        public static F64 Distance(F64Vec4 a, F64Vec4 b) { return Length(a - b); }
        public static F64 DistanceFast(F64Vec4 a, F64Vec4 b) { return LengthFast(a - b); }
        public static F64 DistanceFastest(F64Vec4 a, F64Vec4 b) { return LengthFastest(a - b); }

        public static F64Vec4 Min(F64Vec4 a, F64Vec4 b) { return new F64Vec4(Fixed64.Min(a.RawX, b.RawX), Fixed64.Min(a.RawY, b.RawY), Fixed64.Min(a.RawZ, b.RawZ), Fixed64.Min(a.RawW, b.RawW)); }
        public static F64Vec4 Max(F64Vec4 a, F64Vec4 b) { return new F64Vec4(Fixed64.Max(a.RawX, b.RawX), Fixed64.Max(a.RawY, b.RawY), Fixed64.Max(a.RawZ, b.RawZ), Fixed64.Max(a.RawW, b.RawW)); }

        public static F64Vec4 Clamp(F64Vec4 a, F64 min, F64 max)
        {
            return new F64Vec4(
                Fixed64.Clamp(a.RawX, min.Raw, max.Raw),
                Fixed64.Clamp(a.RawY, min.Raw, max.Raw),
                Fixed64.Clamp(a.RawZ, min.Raw, max.Raw),
                Fixed64.Clamp(a.RawW, min.Raw, max.Raw));
        }

        public static F64Vec4 Clamp(F64Vec4 a, F64Vec4 min, F64Vec4 max)
        {
            return new F64Vec4(
                Fixed64.Clamp(a.RawX, min.RawX, max.RawX),
                Fixed64.Clamp(a.RawY, min.RawY, max.RawY),
                Fixed64.Clamp(a.RawZ, min.RawZ, max.RawZ),
                Fixed64.Clamp(a.RawW, min.RawW, max.RawW));
        }

        public static F64Vec4 Lerp(F64Vec4 a, F64Vec4 b, F64 t)
        {
            long tr = t.Raw;
            return new F64Vec4(
                Fixed64.Lerp(a.RawX, b.RawX, tr),
                Fixed64.Lerp(a.RawY, b.RawY, tr),
                Fixed64.Lerp(a.RawZ, b.RawZ, tr),
                Fixed64.Lerp(a.RawW, b.RawW, tr));
        }

        public bool Equals(F64Vec4 other)
        {
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is F64Vec4))
                return false;
            return ((F64Vec4)obj) == this;
        }

        public override string ToString()
        {
            return "(" + Fixed64.ToString(RawX) + ", " + Fixed64.ToString(RawY) + ", " + Fixed64.ToString(RawZ) + ", " + Fixed64.ToString(RawW) + ")";
        }

        public override int GetHashCode()
        {
            return RawX.GetHashCode() ^ (RawY.GetHashCode() * 7919) ^ (RawZ.GetHashCode() * 4513) ^ (RawW.GetHashCode() * 8923);
        }
    }
}
