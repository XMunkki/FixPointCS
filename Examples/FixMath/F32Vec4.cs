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
    public struct F32Vec4 : IEquatable<F32Vec4>
    {
        public static F32Vec4 Zero      { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec4(Fixed32.Zero, Fixed32.Zero, Fixed32.Zero, Fixed32.Zero); } }
        public static F32Vec4 One       { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec4(Fixed32.One, Fixed32.One, Fixed32.One, Fixed32.One); } }
        public static F32Vec4 AxisX     { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec4(Fixed32.One, Fixed32.Zero, Fixed32.Zero, Fixed32.Zero); } }
        public static F32Vec4 AxisY     { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec4(Fixed32.Zero, Fixed32.One, Fixed32.Zero, Fixed32.Zero); } }
        public static F32Vec4 AxisZ     { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec4(Fixed32.Zero, Fixed32.Zero, Fixed32.One, Fixed32.Zero); } }
        public static F32Vec4 AxisW     { [MethodImpl(FixedUtil.AggressiveInlining)] get { return new F32Vec4(Fixed32.Zero, Fixed32.Zero, Fixed32.Zero, Fixed32.One); } }

        // Raw components
        public int RawX;
        public int RawY;
        public int RawZ;
        public int RawW;

        // F32 accessors
        public F32 X { get { return F32.FromRaw(RawX); } set { RawX = value.Raw; } }
        public F32 Y { get { return F32.FromRaw(RawY); } set { RawY = value.Raw; } }
        public F32 Z { get { return F32.FromRaw(RawZ); } set { RawZ = value.Raw; } }
        public F32 W { get { return F32.FromRaw(RawW); } set { RawW = value.Raw; } }

        public F32Vec4(F32 x, F32 y, F32 z, F32 w)
        {
            RawX = x.Raw;
            RawY = y.Raw;
            RawZ = z.Raw;
            RawW = w.Raw;
        }

        // raw ctor only for internal usage
        private F32Vec4(int x, int y, int z, int w)
        {
            RawX = x;
            RawY = y;
            RawZ = z;
            RawW = w;
        }

        public static F32Vec4 FromInt(int x, int y, int z, int w) { return new F32Vec4(F32.FromInt(x), F32.FromInt(y), F32.FromInt(z), F32.FromInt(w)); }
        public static F32Vec4 FromFloat(float x, float y, float z, float w) { return new F32Vec4(F32.FromFloat(x), F32.FromFloat(y), F32.FromFloat(z), F32.FromFloat(w)); }
        public static F32Vec4 FromDouble(double x, double y, double z, double w) { return new F32Vec4(F32.FromDouble(x), F32.FromDouble(y), F32.FromDouble(z), F32.FromDouble(w)); }

        public static F32Vec4 operator -(F32Vec4 a) { return new F32Vec4(-a.RawX, -a.RawY, -a.RawZ, -a.RawW); }
        public static F32Vec4 operator +(F32Vec4 a, F32Vec4 b) { return new F32Vec4(a.RawX + b.RawX, a.RawY + b.RawY, a.RawZ + b.RawZ, a.RawW + b.RawW); }
        public static F32Vec4 operator -(F32Vec4 a, F32Vec4 b) { return new F32Vec4(a.RawX - b.RawX, a.RawY - b.RawY, a.RawZ - b.RawZ, a.RawW - b.RawW); }
        public static F32Vec4 operator *(F32Vec4 a, F32Vec4 b) { return new F32Vec4(Fixed32.Mul(a.RawX, b.RawX), Fixed32.Mul(a.RawY, b.RawY), Fixed32.Mul(a.RawZ, b.RawZ), Fixed32.Mul(a.RawW, b.RawW)); }
        public static F32Vec4 operator /(F32Vec4 a, F32Vec4 b) { return new F32Vec4(Fixed32.DivPrecise(a.RawX, b.RawX), Fixed32.DivPrecise(a.RawY, b.RawY), Fixed32.DivPrecise(a.RawZ, b.RawZ), Fixed32.DivPrecise(a.RawW, b.RawW)); }
        public static F32Vec4 operator %(F32Vec4 a, F32Vec4 b) { return new F32Vec4(a.RawX % b.RawX, a.RawY % b.RawY, a.RawZ % b.RawZ, a.RawW % b.RawW); }

        public static F32Vec4 operator +(F32 a, F32Vec4 b) { return new F32Vec4(a.Raw + b.RawX, a.Raw + b.RawY, a.Raw + b.RawZ, a.Raw + b.RawW); }
        public static F32Vec4 operator +(F32Vec4 a, F32 b) { return new F32Vec4(a.RawX + b.Raw, a.RawY + b.Raw, a.RawZ + b.Raw, a.RawW + b.Raw); }
        public static F32Vec4 operator -(F32 a, F32Vec4 b) { return new F32Vec4(a.Raw - b.RawX, a.Raw - b.RawY, a.Raw - b.RawZ, a.Raw - b.RawW); }
        public static F32Vec4 operator -(F32Vec4 a, F32 b) { return new F32Vec4(a.RawX - b.Raw, a.RawY - b.Raw, a.RawZ - b.Raw, a.RawW - b.Raw); }
        public static F32Vec4 operator *(F32 a, F32Vec4 b) { return new F32Vec4(Fixed32.Mul(a.Raw, b.RawX), Fixed32.Mul(a.Raw, b.RawY), Fixed32.Mul(a.Raw, b.RawZ), Fixed32.Mul(a.Raw, b.RawW)); }
        public static F32Vec4 operator *(F32Vec4 a, F32 b) { return new F32Vec4(Fixed32.Mul(a.RawX, b.Raw), Fixed32.Mul(a.RawY, b.Raw), Fixed32.Mul(a.RawZ, b.Raw), Fixed32.Mul(a.RawW, b.Raw)); }
        public static F32Vec4 operator /(F32 a, F32Vec4 b) { return new F32Vec4(Fixed32.DivPrecise(a.Raw, b.RawX), Fixed32.DivPrecise(a.Raw, b.RawY), Fixed32.DivPrecise(a.Raw, b.RawZ), Fixed32.DivPrecise(a.Raw, b.RawW)); }
        public static F32Vec4 operator /(F32Vec4 a, F32 b) { return new F32Vec4(Fixed32.DivPrecise(a.RawX, b.Raw), Fixed32.DivPrecise(a.RawY, b.Raw), Fixed32.DivPrecise(a.RawZ, b.Raw), Fixed32.DivPrecise(a.RawW, b.Raw)); }
        public static F32Vec4 operator %(F32 a, F32Vec4 b) { return new F32Vec4(a.Raw % b.RawX, a.Raw % b.RawY, a.Raw % b.RawZ, a.Raw % b.RawW); }
        public static F32Vec4 operator %(F32Vec4 a, F32 b) { return new F32Vec4(a.RawX % b.Raw, a.RawY % b.Raw, a.RawZ % b.Raw, a.RawW % b.Raw); }

        public static bool operator ==(F32Vec4 a, F32Vec4 b) { return a.RawX == b.RawX && a.RawY == b.RawY && a.RawZ == b.RawZ && a.RawW == b.RawW; }
        public static bool operator !=(F32Vec4 a, F32Vec4 b) { return a.RawX != b.RawX || a.RawY != b.RawY || a.RawZ != b.RawZ || a.RawW != b.RawW; }

        public static F32Vec4 Div(F32Vec4 a, F32 b) { int oob = Fixed32.Rcp(b.Raw); return new F32Vec4(Fixed32.Mul(a.RawX, oob), Fixed32.Mul(a.RawY, oob), Fixed32.Mul(a.RawZ, oob), Fixed32.Mul(a.RawW, oob)); }
        public static F32Vec4 DivFast(F32Vec4 a, F32 b) { int oob = Fixed32.RcpFast(b.Raw); return new F32Vec4(Fixed32.Mul(a.RawX, oob), Fixed32.Mul(a.RawY, oob), Fixed32.Mul(a.RawZ, oob), Fixed32.Mul(a.RawW, oob)); }
        public static F32Vec4 DivFastest(F32Vec4 a, F32 b) { int oob = Fixed32.RcpFastest(b.Raw); return new F32Vec4(Fixed32.Mul(a.RawX, oob), Fixed32.Mul(a.RawY, oob), Fixed32.Mul(a.RawZ, oob), Fixed32.Mul(a.RawW, oob)); }
        public static F32Vec4 Div(F32Vec4 a, F32Vec4 b) { return new F32Vec4(Fixed32.Div(a.RawX, b.RawX), Fixed32.Div(a.RawY, b.RawY), Fixed32.Div(a.RawZ, b.RawZ), Fixed32.Div(a.RawW, b.RawW)); }
        public static F32Vec4 DivFast(F32Vec4 a, F32Vec4 b) { return new F32Vec4(Fixed32.DivFast(a.RawX, b.RawX), Fixed32.DivFast(a.RawY, b.RawY), Fixed32.DivFast(a.RawZ, b.RawZ), Fixed32.DivFast(a.RawW, b.RawW)); }
        public static F32Vec4 DivFastest(F32Vec4 a, F32Vec4 b) { return new F32Vec4(Fixed32.DivFastest(a.RawX, b.RawX), Fixed32.DivFastest(a.RawY, b.RawY), Fixed32.DivFastest(a.RawZ, b.RawZ), Fixed32.DivFastest(a.RawW, b.RawW)); }
        public static F32Vec4 SqrtPrecise(F32Vec4 a) { return new F32Vec4(Fixed32.SqrtPrecise(a.RawX), Fixed32.SqrtPrecise(a.RawY), Fixed32.SqrtPrecise(a.RawZ), Fixed32.SqrtPrecise(a.RawW)); }
        public static F32Vec4 Sqrt(F32Vec4 a) { return new F32Vec4(Fixed32.Sqrt(a.RawX), Fixed32.Sqrt(a.RawY), Fixed32.Sqrt(a.RawZ), Fixed32.Sqrt(a.RawW)); }
        public static F32Vec4 SqrtFast(F32Vec4 a) { return new F32Vec4(Fixed32.SqrtFast(a.RawX), Fixed32.SqrtFast(a.RawY), Fixed32.SqrtFast(a.RawZ), Fixed32.SqrtFast(a.RawW)); }
        public static F32Vec4 SqrtFastest(F32Vec4 a) { return new F32Vec4(Fixed32.SqrtFastest(a.RawX), Fixed32.SqrtFastest(a.RawY), Fixed32.SqrtFastest(a.RawZ), Fixed32.SqrtFastest(a.RawW)); }
        public static F32Vec4 RSqrt(F32Vec4 a) { return new F32Vec4(Fixed32.RSqrt(a.RawX), Fixed32.RSqrt(a.RawY), Fixed32.RSqrt(a.RawZ), Fixed32.RSqrt(a.RawW)); }
        public static F32Vec4 RSqrtFast(F32Vec4 a) { return new F32Vec4(Fixed32.RSqrtFast(a.RawX), Fixed32.RSqrtFast(a.RawY), Fixed32.RSqrtFast(a.RawZ), Fixed32.RSqrtFast(a.RawW)); }
        public static F32Vec4 RSqrtFastest(F32Vec4 a) { return new F32Vec4(Fixed32.RSqrtFastest(a.RawX), Fixed32.RSqrtFastest(a.RawY), Fixed32.RSqrtFastest(a.RawZ), Fixed32.RSqrtFastest(a.RawW)); }
        public static F32Vec4 Rcp(F32Vec4 a) { return new F32Vec4(Fixed32.Rcp(a.RawX), Fixed32.Rcp(a.RawY), Fixed32.Rcp(a.RawZ), Fixed32.Rcp(a.RawW)); }
        public static F32Vec4 RcpFast(F32Vec4 a) { return new F32Vec4(Fixed32.RcpFast(a.RawX), Fixed32.RcpFast(a.RawY), Fixed32.RcpFast(a.RawZ), Fixed32.RcpFast(a.RawW)); }
        public static F32Vec4 RcpFastest(F32Vec4 a) { return new F32Vec4(Fixed32.RcpFastest(a.RawX), Fixed32.RcpFastest(a.RawY), Fixed32.RcpFastest(a.RawZ), Fixed32.RcpFastest(a.RawW)); }
        public static F32Vec4 Exp(F32Vec4 a) { return new F32Vec4(Fixed32.Exp(a.RawX), Fixed32.Exp(a.RawY), Fixed32.Exp(a.RawZ), Fixed32.Exp(a.RawW)); }
        public static F32Vec4 ExpFast(F32Vec4 a) { return new F32Vec4(Fixed32.ExpFast(a.RawX), Fixed32.ExpFast(a.RawY), Fixed32.ExpFast(a.RawZ), Fixed32.ExpFast(a.RawW)); }
        public static F32Vec4 ExpFastest(F32Vec4 a) { return new F32Vec4(Fixed32.ExpFastest(a.RawX), Fixed32.ExpFastest(a.RawY), Fixed32.ExpFastest(a.RawZ), Fixed32.ExpFastest(a.RawW)); }
        public static F32Vec4 Exp2(F32Vec4 a) { return new F32Vec4(Fixed32.Exp2(a.RawX), Fixed32.Exp2(a.RawY), Fixed32.Exp2(a.RawZ), Fixed32.Exp2(a.RawW)); }
        public static F32Vec4 Exp2Fast(F32Vec4 a) { return new F32Vec4(Fixed32.Exp2Fast(a.RawX), Fixed32.Exp2Fast(a.RawY), Fixed32.Exp2Fast(a.RawZ), Fixed32.Exp2Fast(a.RawW)); }
        public static F32Vec4 Exp2Fastest(F32Vec4 a) { return new F32Vec4(Fixed32.Exp2Fastest(a.RawX), Fixed32.Exp2Fastest(a.RawY), Fixed32.Exp2Fastest(a.RawZ), Fixed32.Exp2Fastest(a.RawW)); }
        public static F32Vec4 Log(F32Vec4 a) { return new F32Vec4(Fixed32.Log(a.RawX), Fixed32.Log(a.RawY), Fixed32.Log(a.RawZ), Fixed32.Log(a.RawW)); }
        public static F32Vec4 LogFast(F32Vec4 a) { return new F32Vec4(Fixed32.LogFast(a.RawX), Fixed32.LogFast(a.RawY), Fixed32.LogFast(a.RawZ), Fixed32.LogFast(a.RawW)); }
        public static F32Vec4 LogFastest(F32Vec4 a) { return new F32Vec4(Fixed32.LogFastest(a.RawX), Fixed32.LogFastest(a.RawY), Fixed32.LogFastest(a.RawZ), Fixed32.LogFastest(a.RawW)); }
        public static F32Vec4 Log2(F32Vec4 a) { return new F32Vec4(Fixed32.Log2(a.RawX), Fixed32.Log2(a.RawY), Fixed32.Log2(a.RawZ), Fixed32.Log2(a.RawW)); }
        public static F32Vec4 Log2Fast(F32Vec4 a) { return new F32Vec4(Fixed32.Log2Fast(a.RawX), Fixed32.Log2Fast(a.RawY), Fixed32.Log2Fast(a.RawZ), Fixed32.Log2Fast(a.RawW)); }
        public static F32Vec4 Log2Fastest(F32Vec4 a) { return new F32Vec4(Fixed32.Log2Fastest(a.RawX), Fixed32.Log2Fastest(a.RawY), Fixed32.Log2Fastest(a.RawZ), Fixed32.Log2Fastest(a.RawW)); }
        public static F32Vec4 Sin(F32Vec4 a) { return new F32Vec4(Fixed32.Sin(a.RawX), Fixed32.Sin(a.RawY), Fixed32.Sin(a.RawZ), Fixed32.Sin(a.RawW)); }
        public static F32Vec4 SinFast(F32Vec4 a) { return new F32Vec4(Fixed32.SinFast(a.RawX), Fixed32.SinFast(a.RawY), Fixed32.SinFast(a.RawZ), Fixed32.SinFast(a.RawW)); }
        public static F32Vec4 SinFastest(F32Vec4 a) { return new F32Vec4(Fixed32.SinFastest(a.RawX), Fixed32.SinFastest(a.RawY), Fixed32.SinFastest(a.RawZ), Fixed32.SinFastest(a.RawW)); }
        public static F32Vec4 Cos(F32Vec4 a) { return new F32Vec4(Fixed32.Cos(a.RawX), Fixed32.Cos(a.RawY), Fixed32.Cos(a.RawZ), Fixed32.Cos(a.RawW)); }
        public static F32Vec4 CosFast(F32Vec4 a) { return new F32Vec4(Fixed32.CosFast(a.RawX), Fixed32.CosFast(a.RawY), Fixed32.CosFast(a.RawZ), Fixed32.CosFast(a.RawW)); }
        public static F32Vec4 CosFastest(F32Vec4 a) { return new F32Vec4(Fixed32.CosFastest(a.RawX), Fixed32.CosFastest(a.RawY), Fixed32.CosFastest(a.RawZ), Fixed32.CosFastest(a.RawW)); }

        public static F32Vec4 Pow(F32Vec4 a, F32 b) { return new F32Vec4(Fixed32.Pow(a.RawX, b.Raw), Fixed32.Pow(a.RawY, b.Raw), Fixed32.Pow(a.RawZ, b.Raw), Fixed32.Pow(a.RawW, b.Raw)); }
        public static F32Vec4 PowFast(F32Vec4 a, F32 b) { return new F32Vec4(Fixed32.PowFast(a.RawX, b.Raw), Fixed32.PowFast(a.RawY, b.Raw), Fixed32.PowFast(a.RawZ, b.Raw), Fixed32.PowFast(a.RawW, b.Raw)); }
        public static F32Vec4 PowFastest(F32Vec4 a, F32 b) { return new F32Vec4(Fixed32.PowFastest(a.RawX, b.Raw), Fixed32.PowFastest(a.RawY, b.Raw), Fixed32.PowFastest(a.RawZ, b.Raw), Fixed32.PowFastest(a.RawW, b.Raw)); }
        public static F32Vec4 Pow(F32 a, F32Vec4 b) { return new F32Vec4(Fixed32.Pow(a.Raw, b.RawX), Fixed32.Pow(a.Raw, b.RawY), Fixed32.Pow(a.Raw, b.RawZ), Fixed32.Pow(a.Raw, b.RawW)); }
        public static F32Vec4 PowFast(F32 a, F32Vec4 b) { return new F32Vec4(Fixed32.PowFast(a.Raw, b.RawX), Fixed32.PowFast(a.Raw, b.RawY), Fixed32.PowFast(a.Raw, b.RawZ), Fixed32.PowFast(a.Raw, b.RawW)); }
        public static F32Vec4 PowFastest(F32 a, F32Vec4 b) { return new F32Vec4(Fixed32.PowFastest(a.Raw, b.RawX), Fixed32.PowFastest(a.Raw, b.RawY), Fixed32.PowFastest(a.Raw, b.RawZ), Fixed32.PowFastest(a.Raw, b.RawW)); }
        public static F32Vec4 Pow(F32Vec4 a, F32Vec4 b) { return new F32Vec4(Fixed32.Pow(a.RawX, b.RawX), Fixed32.Pow(a.RawY, b.RawY), Fixed32.Pow(a.RawZ, b.RawZ), Fixed32.Pow(a.RawW, b.RawW)); }
        public static F32Vec4 PowFast(F32Vec4 a, F32Vec4 b) { return new F32Vec4(Fixed32.PowFast(a.RawX, b.RawX), Fixed32.PowFast(a.RawY, b.RawY), Fixed32.PowFast(a.RawZ, b.RawZ), Fixed32.PowFast(a.RawW, b.RawW)); }
        public static F32Vec4 PowFastest(F32Vec4 a, F32Vec4 b) { return new F32Vec4(Fixed32.PowFastest(a.RawX, b.RawX), Fixed32.PowFastest(a.RawY, b.RawY), Fixed32.PowFastest(a.RawZ, b.RawZ), Fixed32.PowFastest(a.RawW, b.RawW)); }

        public static F32 Length(F32Vec4 a) { return F32.FromRaw(Fixed32.Sqrt(Fixed32.Mul(a.RawX, a.RawX) + Fixed32.Mul(a.RawY, a.RawY) + Fixed32.Mul(a.RawZ, a.RawZ) + Fixed32.Mul(a.RawW, a.RawW))); }
        public static F32 LengthFast(F32Vec4 a) { return F32.FromRaw(Fixed32.SqrtFast(Fixed32.Mul(a.RawX, a.RawX) + Fixed32.Mul(a.RawY, a.RawY) + Fixed32.Mul(a.RawZ, a.RawZ) + Fixed32.Mul(a.RawW, a.RawW))); }
        public static F32 LengthFastest(F32Vec4 a) { return F32.FromRaw(Fixed32.SqrtFastest(Fixed32.Mul(a.RawX, a.RawX) + Fixed32.Mul(a.RawY, a.RawY) + Fixed32.Mul(a.RawZ, a.RawZ) + Fixed32.Mul(a.RawW, a.RawW))); }
        public static F32 LengthSqr(F32Vec4 a) { return F32.FromRaw(Fixed32.Mul(a.RawX, a.RawX) + Fixed32.Mul(a.RawY, a.RawY) + Fixed32.Mul(a.RawZ, a.RawZ) + Fixed32.Mul(a.RawW, a.RawW)); }
        public static F32Vec4 Normalize(F32Vec4 a) { F32 ooLen = F32.FromRaw(Fixed32.RSqrt(Fixed32.Mul(a.RawX, a.RawX) + Fixed32.Mul(a.RawY, a.RawY) + Fixed32.Mul(a.RawZ, a.RawZ) + Fixed32.Mul(a.RawW, a.RawW))); return ooLen * a; }
        public static F32Vec4 NormalizeFast(F32Vec4 a) { F32 ooLen = F32.FromRaw(Fixed32.RSqrtFast(Fixed32.Mul(a.RawX, a.RawX) + Fixed32.Mul(a.RawY, a.RawY) + Fixed32.Mul(a.RawZ, a.RawZ) + Fixed32.Mul(a.RawW, a.RawW))); return ooLen * a; }
        public static F32Vec4 NormalizeFastest(F32Vec4 a) { F32 ooLen = F32.FromRaw(Fixed32.RSqrtFastest(Fixed32.Mul(a.RawX, a.RawX) + Fixed32.Mul(a.RawY, a.RawY) + Fixed32.Mul(a.RawZ, a.RawZ) + Fixed32.Mul(a.RawW, a.RawW))); return ooLen * a; }

        public static F32 Dot(F32Vec4 a, F32Vec4 b) { return F32.FromRaw(Fixed32.Mul(a.RawX, b.RawX) + Fixed32.Mul(a.RawY, b.RawY) + Fixed32.Mul(a.RawZ, b.RawZ) + Fixed32.Mul(a.RawW, b.RawW)); }
        public static F32 Distance(F32Vec4 a, F32Vec4 b) { return Length(a - b); }
        public static F32 DistanceFast(F32Vec4 a, F32Vec4 b) { return LengthFast(a - b); }
        public static F32 DistanceFastest(F32Vec4 a, F32Vec4 b) { return LengthFastest(a - b); }

        public static F32Vec4 Min(F32Vec4 a, F32Vec4 b) { return new F32Vec4(Fixed32.Min(a.RawX, b.RawX), Fixed32.Min(a.RawY, b.RawY), Fixed32.Min(a.RawZ, b.RawZ), Fixed32.Min(a.RawW, b.RawW)); }
        public static F32Vec4 Max(F32Vec4 a, F32Vec4 b) { return new F32Vec4(Fixed32.Max(a.RawX, b.RawX), Fixed32.Max(a.RawY, b.RawY), Fixed32.Max(a.RawZ, b.RawZ), Fixed32.Max(a.RawW, b.RawW)); }

        public static F32Vec4 Clamp(F32Vec4 a, F32 min, F32 max)
        {
            return new F32Vec4(
                Fixed32.Clamp(a.RawX, min.Raw, max.Raw),
                Fixed32.Clamp(a.RawY, min.Raw, max.Raw),
                Fixed32.Clamp(a.RawZ, min.Raw, max.Raw),
                Fixed32.Clamp(a.RawW, min.Raw, max.Raw));
        }

        public static F32Vec4 Clamp(F32Vec4 a, F32Vec4 min, F32Vec4 max)
        {
            return new F32Vec4(
                Fixed32.Clamp(a.RawX, min.RawX, max.RawX),
                Fixed32.Clamp(a.RawY, min.RawY, max.RawY),
                Fixed32.Clamp(a.RawZ, min.RawZ, max.RawZ),
                Fixed32.Clamp(a.RawW, min.RawW, max.RawW));
        }

        public static F32Vec4 Lerp(F32Vec4 a, F32Vec4 b, F32 t)
        {
            int tr = t.Raw;
            return new F32Vec4(
                Fixed32.Lerp(a.RawX, b.RawX, tr),
                Fixed32.Lerp(a.RawY, b.RawY, tr),
                Fixed32.Lerp(a.RawZ, b.RawZ, tr),
                Fixed32.Lerp(a.RawW, b.RawW, tr));
        }

        public bool Equals(F32Vec4 other)
        {
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is F32Vec4))
                return false;
            return ((F32Vec4)obj) == this;
        }

        public override string ToString()
        {
            return "(" + Fixed32.ToString(RawX) + ", " + Fixed32.ToString(RawY) + ", " + Fixed32.ToString(RawZ) + ", " + Fixed32.ToString(RawW) + ")";
        }

        public override int GetHashCode()
        {
            return RawX.GetHashCode() ^ (RawY.GetHashCode() * 7919) ^ (RawZ.GetHashCode() * 4513) ^ (RawW.GetHashCode() * 8923);
        }
    }
}
