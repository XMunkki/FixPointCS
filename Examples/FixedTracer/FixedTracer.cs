using System;
using System.Threading.Tasks;

using FixPointCS;
using FixMath;

// Based on https://blogs.msdn.microsoft.com/lukeh/2007/04/03/a-ray-tracer-in-c3-0/

namespace FixedTracer
{
    public static class RayTracer
    {
        private const int MaxDepth = 5;

        public static System.Drawing.Color[,] RenderDefaultScene(int width, int height, int numSamples)
        {
            return Render(width, height, numSamples, DefaultScene);
        }

        private static ISect IntersectRay(Ray ray, Scene scene)
        {
            F64 bestDist = F64.MaxValue;
            ISect best = null;

            foreach (SceneObject obj in scene.Things)
            {
                ISect isect = obj.Intersect(ray);
                if (isect != null)
                {
                    if (isect.Dist < bestDist)
                    {
                        best = isect;
                        bestDist = isect.Dist;
                    }
                }
            }

            return best;
        }

        private static Color TraceRay(Ray ray, Scene scene, int depth)
        {
            ISect isect = IntersectRay(ray, scene);
            if (isect == null)
                return Color.Background;
            return Shade(ray, isect, scene, depth);
        }

        private static Color GetNaturalColor(Material material, F64Vec3 pos, F64Vec3 norm, F64Vec3 rd, Scene scene)
        {
            Color ret = Color.Black;
            foreach (Light light in scene.Lights)
            {
                F64Vec3 ldis = light.Pos - pos;
                F64Vec3 livec = F64Vec3.NormalizeFastest(ldis);
                ISect isect = IntersectRay(new Ray(pos, livec), scene);
                bool isInShadow = (isect != null) && (isect.Dist * isect.Dist) < F64Vec3.LengthSqr(ldis);
                if (!isInShadow)
                {
                    F32 illum = F32.Max(F32.Zero, F64Vec3.Dot(livec, norm).F32);
                    Color lcolor = illum * light.Color;
                    F32 specular = F64Vec3.Dot(livec, F64Vec3.NormalizeFastest(rd)).F32;
                    Color scolor = specular > 0 ? F32.PowFastest(specular, material.Roughness) * light.Color : Color.Black;
                    ret += material.Diffuse * lcolor + material.Specular * scolor;
                }
            }
            return ret;
        }

        private static Color GetReflectionColor(Material material, F64Vec3 pos, F64Vec3 norm, F64Vec3 rd, Scene scene, int depth)
        {
            return material.Reflect * TraceRay(new Ray(pos, rd), scene, depth + 1);
        }

        private static Color Shade(Ray ray, ISect isect, Scene scene, int depth)
        {
            var pos = isect.Pos;
            var normal = isect.Normal;
            var reflectDir = ray.Dir - 2 * F64Vec3.Dot(normal, ray.Dir) * normal;
            Color ret = Color.Black;
            Material material = isect.Material;
            ret += GetNaturalColor(material, pos, normal, reflectDir, scene);
            if (depth >= MaxDepth)
                return ret + new Color(F32.Half, F32.Half, F32.Half);
            return ret + GetReflectionColor(material, pos + F64.Ratio(1, 1000) * reflectDir, normal, reflectDir, scene, depth);
        }

        internal static System.Drawing.Color[,] Render(int screenWidth, int screenHeight, int numSamples, Scene scene)
        {
            System.Drawing.Color[,] pixels = new System.Drawing.Color[screenWidth, screenHeight];
            Camera camera = scene.Camera;
            F64 sx = F64.Ratio(1, 2 * screenWidth);
            F64 ox = F64.Half * F64.FromInt(screenWidth);
            F64 sy = -F64.Ratio(1, 2 * screenHeight);
            F64 oy = F64.Half * F64.FromInt(screenHeight);
            F32 ooNumSamples = F32.Ratio(1, numSamples);

            //for (int y = 0; y < screenHeight; y++)
            Parallel.For(0, screenHeight, y =>
            {
                Random rnd = new Random(y * 29827341 + 23427343);
                for (int x = 0; x < screenWidth; x++)
                {
                    Color accum = Color.Black;
                    for (int i = 0; i < numSamples; i++)
                    {
                        // rx, ry == random offset in range [-0.5, 0.5]
                        F64 rx = F64.FromRaw(rnd.Next()) * 2 - F64.Half;
                        F64 ry = F64.FromRaw(rnd.Next()) * 2 - F64.Half;
                        F64 xx = (F64.FromInt(x) + rx - ox) * sx;
                        F64 yy = (F64.FromInt(y) + ry - oy) * sy;
                        F64Vec3 rayDir = F64Vec3.Normalize(camera.Forward + xx * camera.Right + yy * camera.Up);
                        Color color = TraceRay(new Ray(camera.Pos, rayDir), scene, 0);
                        accum += color;
                    }
                    pixels[x, y] = (ooNumSamples * accum).ToDrawingColor();
                }
            });
            return pixels;
        }

        static F64Vec3 F64Vec3Ratio100(int x, int y, int z)
        {
            return new F64Vec3(F64.Ratio100(x), F64.Ratio100(y), F64.Ratio100(z));
        }

        internal static readonly Scene DefaultScene =
            new Scene()
            {
                Things = new SceneObject[]
                {
                    new Checkerboard(F64Vec3Ratio100(0_00, 1_00, 0_00), F64.Zero, Materials.White, Materials.Black),
                    new Sphere(F64Vec3Ratio100(0_00, 1_00, 0_00), F64.One, Materials.Shiny),
                    new Sphere(F64Vec3Ratio100(-1_00, 0_50, 1_50), F64.Half, Materials.Shiny),
                },
                Lights = new Light[]
                {
                    new Light(F64Vec3Ratio100(-2_00, 2_50, 0_00), Color.Ratio100(49, 7, 7)),
                    new Light(F64Vec3Ratio100(1_50, 2_50, 1_50), Color.Ratio100(7, 7, 49)),
                    new Light(F64Vec3Ratio100(1_50, 2_50, -1_50), Color.Ratio100(7, 49, 7)),
                    new Light(F64Vec3Ratio100(0_00, 3_50, 0_00), Color.Ratio100(21, 21, 35)),
                },
                Camera = Camera.Create(F64Vec3.FromInt(3, 2, 4), F64Vec3Ratio100(-1_00, 0_50, 0_00))
            };
    }

    static class Materials
    {
        public static readonly Material White = new Material
        {
            Diffuse = Color.White,
            Specular = Color.White,
            Reflect = F32.Ratio(1, 10),
            Roughness = F32.FromInt(150)
        };

        public static readonly Material Black = new Material
        {
            Diffuse = Color.Black,
            Specular = Color.White,
            Reflect = F32.Ratio(7, 10),
            Roughness = F32.FromInt(150)
        };

        public static readonly Material Shiny = new Material
        {
            Diffuse = Color.White,
            Specular = Color.Ratio100(50, 50, 50),
            Reflect = F32.Ratio(6, 10),
            Roughness = F32.FromInt(50)
        };
    }

    public struct Color
    {
        public int RawR;
        public int RawG;
        public int RawB;

        private Color(int r, int g, int b) { RawR = r; RawG = g; RawB = b; }
        public Color(F32 r, F32 g, F32 b) { RawR = r.Raw; RawG = g.Raw; RawB = b.Raw; }

        public static Color Ratio100(int r, int g, int b) { return new Color((r << 16) / 100, (g << 16) / 100, (b << 16) / 100); }

        public static Color operator +(Color a, Color b) { return new Color(a.RawR + b.RawR, a.RawG + b.RawG, a.RawB + b.RawB); }
        public static Color operator -(Color a, Color b) { return new Color(a.RawR - b.RawR, a.RawG - b.RawG, a.RawB - b.RawB); }
        public static Color operator *(Color a, Color b) { return new Color(Fixed32.Mul(a.RawR, b.RawR), Fixed32.Mul(a.RawG, b.RawG), Fixed32.Mul(a.RawB, b.RawB)); }
        public static Color operator *(F32 a, Color b) { return new Color(Fixed32.Mul(a.Raw, b.RawR), Fixed32.Mul(a.Raw, b.RawG), Fixed32.Mul(a.Raw, b.RawB)); }
        public static Color operator *(Color a, F32 b) { return new Color(Fixed32.Mul(a.RawR, b.Raw), Fixed32.Mul(a.RawG, b.Raw), Fixed32.Mul(a.RawB, b.Raw)); }

        public static Color Black { get { return new Color(0, 0, 0); } }
        public static Color White { get { return new Color(Fixed32.One, Fixed32.One, Fixed32.One); } }
        public static Color Background { get { return new Color(F32.Ratio(4, 10), F32.Ratio(6, 10), F32.One); } }

        private int ToInt(int raw)
        {
            int clamped = Fixed32.Clamp(raw, 0, Fixed32.One);
            return Fixed32.RoundToInt(clamped * 255);
        }

        internal System.Drawing.Color ToDrawingColor()
        {
            return System.Drawing.Color.FromArgb(ToInt(RawR), ToInt(RawG), ToInt(RawB));
        }
    }

    struct Ray
    {
        public readonly F64Vec3 Start;
        public readonly F64Vec3 Dir;

        public Ray(F64Vec3 start, F64Vec3 dir)
        {
            Start = start;
            Dir = dir;
        }
    }

    class ISect
    {
        public Material Material;
        public F64      Dist;
        public F64Vec3  Pos;
        public F64Vec3  Normal;
    }

    class Material
    {
        public Color    Diffuse;
        public Color    Specular;
        public F32      Reflect;
        public F32      Roughness;
    }

    class Camera
    {
        public F64Vec3 Pos;
        public F64Vec3 Forward;
        public F64Vec3 Up;
        public F64Vec3 Right;

        public static Camera Create(F64Vec3 pos, F64Vec3 lookAt)
        {
            F64Vec3 forward = F64Vec3.Normalize(lookAt - pos);
            F64Vec3 down = new F64Vec3(F64.Zero, F64.Neg1, F64.Zero);
            F64Vec3 right = F64.Ratio(15, 10) * F64Vec3.Normalize(F64Vec3.Cross(forward, down));
            F64Vec3 up = F64.Ratio(15, 10) * F64Vec3.Normalize(F64Vec3.Cross(forward, right));

            return new Camera() { Pos = pos, Forward = forward, Up = up, Right = right };
        }
    }

    class Light
    {
        public F64Vec3  Pos;
        public Color    Color;

        public Light(F64Vec3 pos, Color color)
        {
            Pos = pos;
            Color = color;
        }
    }

    abstract class SceneObject
    {
        public abstract ISect Intersect(Ray ray);
    }

    class Sphere : SceneObject
    {
        public readonly F64Vec3     Center;
        public readonly F64         Radius;
        public readonly Material    Material;

        public Sphere(F64Vec3 center, F64 radius, Material material)
        {
            Center = center;
            Radius = radius;
            Material = material;
        }

        public override ISect Intersect(Ray ray)
        {
            F64Vec3 eo = Center - ray.Start;
            F64 v = F64Vec3.Dot(eo, ray.Dir);
            if (v < F64.Zero)
                return null;

            F64 disc = (Radius * Radius) - (F64Vec3.Dot(eo, eo) - (v * v));
            F64 dist = disc < F64.Zero ? F64.Zero : v - F64.SqrtFast(disc);

            if (dist == F64.Zero)
                return null;

            F64Vec3 pos = ray.Start + dist * ray.Dir;
            return new ISect
            {
                Material = Material,
                Dist = dist,
                Pos = pos,
                Normal = F64Vec3.Normalize(pos - Center)
            };
        }
    }

    class Checkerboard : SceneObject
    {
        public F64Vec3  Norm;
        public F64      Offset;
        public Material White;
        public Material Black;

        public Checkerboard(F64Vec3 normal, F64 offset, Material white, Material black)
        {
            Norm = normal;
            Offset = offset;
            White = white;
            Black = black;
        }

        public override ISect Intersect(Ray ray)
        {
            F64 epsilon = F64.FromRaw(1 << 24);
            F64 denom = F64Vec3.Dot(Norm, ray.Dir);
            if (denom > -epsilon) return null;

            F64 dist = (F64Vec3.Dot(Norm, ray.Start) + Offset) * F64.RcpFast(-denom);
            F64Vec3 pos = ray.Start + dist * ray.Dir;
            bool isWhite = ((F64.FloorToInt(pos.Z) + F64.FloorToInt(pos.X)) & 1) != 0;
            return new ISect
            {
                Material = isWhite ? White : Black,
                Pos = pos,
                Dist = dist,
                Normal = Norm
            };
        }
    }

    class Scene
    {
        public SceneObject[]    Things;
        public Light[]          Lights;
        public Camera           Camera;
    }
}
