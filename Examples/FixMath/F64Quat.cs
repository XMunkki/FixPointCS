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

using FixPointCS;

namespace FixMath
{
    /// <summary>
    /// Quaternion struct with signed 32.32 fixed point components.
    /// </summary>
    [Serializable]
    public struct F64Quat : IEquatable<F64Quat>
    {
        // Constants
        public static F64Quat Identity { get { return new F64Quat(Fixed64.Zero, Fixed64.Zero, Fixed64.Zero, Fixed64.One); } }

        public long RawX;
        public long RawY;
        public long RawZ;
        public long RawW;

        public F64 X { get { return F64.FromRaw(RawX); } set { RawX = value.Raw; } }
        public F64 Y { get { return F64.FromRaw(RawY); } set { RawY = value.Raw; } }
        public F64 Z { get { return F64.FromRaw(RawZ); } set { RawZ = value.Raw; } }
        public F64 W { get { return F64.FromRaw(RawW); } set { RawW = value.Raw; } }

        public F64Quat(F64 x, F64 y, F64 z, F64 w)
        {
            RawX = x.Raw;
            RawY = y.Raw;
            RawZ = z.Raw;
            RawW = w.Raw;
        }

        public F64Quat(F64Vec3 v, F64 w)
        {
            RawX = v.RawX;
            RawY = v.RawY;
            RawZ = v.RawZ;
            RawW = w.Raw;
        }

        private F64Quat(long x, long y, long z, long w)
        {
            RawX = x;
            RawY = y;
            RawZ = z;
            RawW = w;
        }

        public static F64Quat FromAxisAngle(F64Vec3 axis, F64 angle)
        {
            F64 half_angle = F64.Div2(angle);
            return new F64Quat(axis * F64.SinFastest(half_angle), F64.CosFastest(half_angle));
        }

        public static F64Quat FromYawPitchRoll(F64 yaw_y, F64 pitch_x, F64 roll_z)
        {
            //  Roll first, about axis the object is facing, then
            //  pitch upward, then yaw to face into the new heading
            F64 half_roll = F64.Div2(roll_z);
            F64 sr = F64.SinFastest(half_roll);
            F64 cr = F64.CosFastest(half_roll);

            F64 half_pitch = F64.Div2(pitch_x);
            F64 sp = F64.SinFastest(half_pitch);
            F64 cp = F64.CosFastest(half_pitch);

            F64 half_yaw = F64.Div2(yaw_y);
            F64 sy = F64.SinFastest(half_yaw);
            F64 cy = F64.CosFastest(half_yaw);

            return new F64Quat(
                cy * sp * cr + sy * cp * sr,
                sy * cp * cr - cy * sp * sr,
                cy * cp * sr - sy * sp * cr,
                cy * cp * cr + sy * sp * sr);
        }

        // Creates a unit quaternion that represents the rotation from a to b. a and b do not need to be normalized.
        public static F64Quat FromTwoVectors(F64Vec3 a, F64Vec3 b)
        { // From: http://lolengine.net/blog/2014/02/24/quaternion-from-two-vectors-final
            F64 epsilon = F64.Ratio(1, 1000000);

            F64 norm_a_norm_b = F64.SqrtFastest(F64Vec3.LengthSqr(a) * F64Vec3.LengthSqr(b));
            F64 real_part = norm_a_norm_b + F64Vec3.Dot(a, b);

            F64Vec3 v;

            if (real_part < (epsilon * norm_a_norm_b))
            {
                /* If u and v are exactly opposite, rotate 180 degrees
                 * around an arbitrary orthogonal axis. Axis normalization
                 * can happen later, when we normalize the quaternion. */
                real_part = F64.Zero;
                bool cond = F64.Abs(a.X) > F64.Abs(a.Z);
                v = cond ? new F64Vec3(-a.Y, a.X, F64.Zero)
                         : new F64Vec3(F64.Zero, -a.Z, a.Y);
            }
            else
            {
                /* Otherwise, build quaternion the standard way. */
                v = F64Vec3.Cross(a, b);
            }

            return NormalizeFastest(new F64Quat(v, real_part));
        }

        public static F64Quat LookRotation(F64Vec3 dir, F64Vec3 up)
        { // From: https://answers.unity.com/questions/819699/calculate-quaternionlookrotation-manually.html
            if (dir == F64Vec3.Zero)
                return Identity;

            if (up != dir)
            {
                up = F64Vec3.NormalizeFastest(up);
                F64Vec3 v = dir + up * -F64Vec3.Dot(up, dir);
                F64Quat q = FromTwoVectors(F64Vec3.AxisZ, v);
                return FromTwoVectors(v, dir) * q;
            }
            else
                return FromTwoVectors(F64Vec3.AxisZ, dir);
        }

        public static F64Quat LookAtRotation(F64Vec3 from, F64Vec3 to, F64Vec3 up)
        {
            F64Vec3 dir = F64Vec3.NormalizeFastest(to - from);
            return LookRotation(dir, up);
        }

        // Operators
        public static F64Quat operator *(F64Quat a, F64Quat b) { return Multiply(a, b); }

        public static bool operator ==(F64Quat a, F64Quat b) { return a.RawX == b.RawX && a.RawY == b.RawY && a.RawZ == b.RawZ && a.RawW == b.RawW; }
        public static bool operator !=(F64Quat a, F64Quat b) { return a.RawX != b.RawX || a.RawY != b.RawY || a.RawZ != b.RawZ || a.RawW != b.RawW; }

        public static F64Quat Negate(F64Quat a) { return new F64Quat(-a.RawX, -a.RawY, -a.RawZ, -a.RawW); }
        public static F64Quat Conjugate(F64Quat a) { return new F64Quat(-a.RawX, -a.RawY, -a.RawZ, a.RawW); }
        public static F64Quat Inverse(F64Quat a)
        {
            long inv_norm = F64.Rcp(LengthSqr(a)).Raw;
            return new F64Quat(
                -Fixed64.Mul(a.RawX, inv_norm),
                -Fixed64.Mul(a.RawY, inv_norm),
                -Fixed64.Mul(a.RawZ, inv_norm),
                Fixed64.Mul(a.RawW, inv_norm));
        }
        // Inverse for unit quaternions
        public static F64Quat InverseUnit(F64Quat a) { return new F64Quat(-a.RawX, -a.RawY, -a.RawZ, a.RawW); }

        public static F64Quat Multiply(F64Quat value1, F64Quat value2)
        {
            F64 q1x = value1.X;
            F64 q1y = value1.Y;
            F64 q1z = value1.Z;
            F64 q1w = value1.W;

            F64 q2x = value2.X;
            F64 q2y = value2.Y;
            F64 q2z = value2.Z;
            F64 q2w = value2.W;

            // cross(av, bv)
            F64 cx = q1y * q2z - q1z * q2y;
            F64 cy = q1z * q2x - q1x * q2z;
            F64 cz = q1x * q2y - q1y * q2x;

            F64 dot = q1x * q2x + q1y * q2y + q1z * q2z;

            return new F64Quat(
                q1x * q2w + q2x * q1w + cx,
                q1y * q2w + q2y * q1w + cy,
                q1z * q2w + q2z * q1w + cz,
                q1w * q2w - dot);
        }

        public static F64 Length(F64Quat a) { return F64.Sqrt(LengthSqr(a)); }
        public static F64 LengthFast(F64Quat a) { return F64.SqrtFast(LengthSqr(a)); }
        public static F64 LengthFastest(F64Quat a) { return F64.SqrtFastest(LengthSqr(a)); }
        public static F64 LengthSqr(F64Quat a) { return F64.FromRaw(Fixed64.Mul(a.RawX, a.RawX) + Fixed64.Mul(a.RawY, a.RawY) + Fixed64.Mul(a.RawZ, a.RawZ) + Fixed64.Mul(a.RawW, a.RawW)); }
        public static F64Quat Normalize(F64Quat a)
        {
            long inv_norm = F64.Rcp(Length(a)).Raw;
            return new F64Quat(
                Fixed64.Mul(a.RawX, inv_norm),
                Fixed64.Mul(a.RawY, inv_norm),
                Fixed64.Mul(a.RawZ, inv_norm),
                Fixed64.Mul(a.RawW, inv_norm));
        }
        public static F64Quat NormalizeFast(F64Quat a)
        {
            long inv_norm = F64.RcpFast(LengthFast(a)).Raw;
            return new F64Quat(
                Fixed64.Mul(a.RawX, inv_norm),
                Fixed64.Mul(a.RawY, inv_norm),
                Fixed64.Mul(a.RawZ, inv_norm),
                Fixed64.Mul(a.RawW, inv_norm));
        }
        public static F64Quat NormalizeFastest(F64Quat a)
        {
            long inv_norm = F64.RcpFastest(LengthFastest(a)).Raw;
            return new F64Quat(
                Fixed64.Mul(a.RawX, inv_norm),
                Fixed64.Mul(a.RawY, inv_norm),
                Fixed64.Mul(a.RawZ, inv_norm),
                Fixed64.Mul(a.RawW, inv_norm));
        }

        public static F64Quat Slerp(F64Quat q1, F64Quat q2, F64 t)
        {
            F64 epsilon = F64.Ratio(1, 1000000);
            F64 cos_omega = q1.X * q2.X + q1.Y * q2.Y + q1.Z * q2.Z + q1.W * q2.W;

            bool flip = false;

            if (cos_omega < 0)
            {
                flip = true;
                cos_omega = -cos_omega;
            }

            F64 s1, s2;
            if (cos_omega > (F64.One - epsilon))
            {
                // Too close, do straight linear interpolation.
                s1 = F64.One - t;
                s2 = (flip) ? -t : t;
            }
            else
            {
                F64 omega = F64.AcosFastest(cos_omega);
                F64 inv_sin_omega = F64.RcpFastest(F64.SinFastest(omega));

                s1 = F64.SinFastest((F64.One - t) * omega) * inv_sin_omega;
                s2 = (flip)
                    ? -F64.SinFastest(t * omega) * inv_sin_omega
                    : F64.SinFastest(t * omega) * inv_sin_omega;
            }

            return new F64Quat(
                s1 * q1.X + s2 * q2.X,
                s1 * q1.Y + s2 * q2.Y,
                s1 * q1.Z + s2 * q2.Z,
                s1 * q1.W + s2 * q2.W);
        }

        public static F64Quat Lerp(F64Quat q1, F64Quat q2, F64 t)
        {
            F64 t1 = F64.One - t;
            F64 dot = q1.X * q2.X + q1.Y * q2.Y + q1.Z * q2.Z + q1.W * q2.W;

            F64Quat r;
            if (dot >= 0)
                r = new F64Quat(
                    t1 * q1.X + t * q2.X,
                    t1 * q1.Y + t * q2.Y,
                    t1 * q1.Z + t * q2.Z,
                    t1 * q1.W + t * q2.W);
            else
                r = new F64Quat(
                    t1 * q1.X - t * q2.X,
                    t1 * q1.Y - t * q2.Y,
                    t1 * q1.Z - t * q2.Z,
                    t1 * q1.W - t * q2.W);

            return NormalizeFastest(r);
        }

        // Concatenates two Quaternions; the result represents the value1 rotation followed by the value2 rotation.
        public static F64Quat Concatenate(F64Quat value1, F64Quat value2)
        {
            // Concatenate rotation is actually q2 * q1 instead of q1 * q2.
            // So that's why value2 goes q1 and value1 goes q2.
            return Multiply(value2, value1);
        }

        // Rotates a vector by the unit quaternion.
        public static F64Vec3 RotateVector(F64Quat rot, F64Vec3 v)
        { // From https://gamedev.stackexchange.com/questions/28395/rotating-vector3-by-a-quaternion
            F64Vec3 u = new F64Vec3(rot.X, rot.Y, rot.Z);
            F64 s = rot.W;

            return
                (F64.Two * F64Vec3.Dot(u, v)) * u +
                (s * s - F64Vec3.Dot(u, u)) * v +
                (F64.Two * s) * F64Vec3.Cross(u, v);
        }

        public bool Equals(F64Quat other)
        {
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is F64Quat))
                return false;
            return ((F64Quat)obj) == this;
        }

        public override string ToString()
        {
            return "(" + Fixed64.ToString(RawX) + ", " + Fixed64.ToString(RawY) + ", " + Fixed64.ToString(RawZ) + ", " + Fixed64.ToString(RawW) + ")";
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ (Y.GetHashCode() * 7919) ^ (Z.GetHashCode() * 4513) ^ (W.GetHashCode() * 8923);
        }
    }
}
