using System;
using System.Threading.Tasks;

using FixMath;

// Based on https://blogs.msdn.microsoft.com/lukeh/2007/04/03/a-ray-tracer-in-c3-0/

namespace F64Tracer
{
    using Surface = Func<F64Vec3, Material>;

    public class RayTracer
    {
        private int screenWidth;
        private int screenHeight;
        private const int MaxDepth = 5;

        public RayTracer(int screenWidth, int screenHeight) {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
        }

        public static System.Drawing.Color[,] RenderDefaultScene(int width, int height)
        {
            RayTracer rayTracer = new RayTracer(width, height);
            return rayTracer.Render(rayTracer.DefaultScene);
        }

        private ISect IntersectRay(Ray ray, Scene scene)
        {
            ISect best = null;
            foreach (SceneObject obj in scene.Things)
            {
                ISect isect = obj.Intersect(ray);
                if (isect != null)
                {
                    if (best == null || isect.Dist < best.Dist)
                        best = isect;
                }
            }

            return best;
        }

        private F64 TestRay(Ray ray, Scene scene)
        {
            ISect isect = IntersectRay(ray, scene);
            if (isect == null)
                return F64.Zero;
            return isect.Dist;
        }

        private Color TraceRay(Ray ray, Scene scene, int depth)
        {
            ISect isect = IntersectRay(ray, scene);
            if (isect == null)
                return Color.Background;
            return Shade(isect, scene, depth);
        }

        private Color GetNaturalColor(Material material, F64Vec3 pos, F64Vec3 norm, F64Vec3 rd, Scene scene) {
            Color ret = Color.Black;
            foreach (Light light in scene.Lights) {
                F64Vec3 ldis = light.Pos - pos;
                F64Vec3 livec = F64Vec3.Normalize(ldis);
                F64 neatIsect = TestRay(new Ray() { Start = pos, Dir = livec }, scene);
                bool isInShadow = !((neatIsect * neatIsect > F64Vec3.LengthSqr(ldis)) || (neatIsect == 0));
                if (!isInShadow) {
                    F64 illum = F64.Max(F64.Zero, F64Vec3.Dot(livec, norm));
                    Color lcolor = illum.F32 * light.Color;
                    F32 specular = F64Vec3.Dot(livec, F64Vec3.Normalize(rd)).F32;
                    Color scolor = specular > 0 ? F32.PowFastest(specular, material.Roughness) * light.Color : Color.Black;
                    ret = ret + material.Diffuse * lcolor + material.Specular * scolor;
                }
            }
            return ret;
        }

        private Color GetReflectionColor(Material material, F64Vec3 pos, F64Vec3 norm, F64Vec3 rd, Scene scene, int depth) {
            return material.Reflect * TraceRay(new Ray() { Start = pos, Dir = rd }, scene, depth + 1);
        }

        private Color Shade(ISect isect, Scene scene, int depth) {
            var d = isect.Ray.Dir;
            var pos = isect.Dist * isect.Ray.Dir + isect.Ray.Start;
            var normal = isect.Thing.Normal(pos);
            var reflectDir = d - F64.FromInt(2) * F64Vec3.Dot(normal, d) * normal;
            Color ret = Color.Black;
            Material material = isect.Thing.Surface(pos);
            ret += GetNaturalColor(material, pos, normal, reflectDir, scene);
            if (depth >= MaxDepth) {
                return ret + new Color(F32.Half, F32.Half, F32.Half);
            }
            return ret + GetReflectionColor(material, pos + F64.FromDouble(.001) * reflectDir, normal, reflectDir, scene, depth);
        }

        private F64 RecenterX(F64 x) {
            return (x - (F64.FromInt(screenWidth) * F64.Half)) / (F64.Two * F64.FromInt(screenWidth));
        }
        private F64 RecenterY(F64 y) {
            return -(y - (F64.FromInt(screenHeight) * F64.Half)) / (F64.Two * F64.FromInt(screenHeight));
        }

        internal System.Drawing.Color[,] Render(Scene scene) {
            System.Drawing.Color[,] pixels = new System.Drawing.Color[screenWidth, screenHeight];
            Camera camera = scene.Camera;
            F64 sx = F64.One / (F64.Two * F64.FromInt(screenWidth));
            F64 ox = F64.Half * F64.FromInt(screenWidth);
            F64 sy = -F64.One / (F64.Two * F64.FromInt(screenHeight));
            F64 oy = F64.Half * F64.FromInt(screenHeight);
            //for (int y = 0; y < screenHeight; y++)
            Parallel.For(0, screenHeight, y =>
            {
                Random rnd = new Random(y * 29827341 + 23427343);
                for (int x = 0; x < screenWidth; x++)
                {
                    const int NumSamples = 16;
                    Color accum = Color.Black;
                    for (int i = 0; i < NumSamples; i++)
                    {
                        F64 xx = (F64.FromInt(x) + F64.FromDouble(rnd.NextDouble() - 0.5) - ox) * sx;
                        F64 yy = (F64.FromInt(y) + F64.FromDouble(rnd.NextDouble() - 0.5) - oy) * sy;
                        F64Vec3 rayDir = F64Vec3.Normalize(camera.Forward + xx * camera.Right + yy * camera.Up);
                        Color color = TraceRay(new Ray() { Start = scene.Camera.Pos, Dir = rayDir }, scene, 0);
                        accum += color;
                    }
                    pixels[x, y] = (F32.FromDouble(1.0 / NumSamples) * accum).ToDrawingColor();
                }
            });
            return pixels;
        }

        internal readonly Scene DefaultScene =
            new Scene() {
                Things = new SceneObject[] { 
                    new Plane() {
                        Norm = F64Vec3.FromDouble(0,1,0),
                        Offset = F64.FromDouble(0.0),
                        Surface = Surfaces.CheckerBoard
                    },
                    new Sphere() {
                        Center = F64Vec3.FromDouble(0,1,0),
                        Radius = F64.FromDouble(1.0),
                        Surface = Surfaces.Shiny
                    },
                    new Sphere() {
                        Center = F64Vec3.FromDouble(-1,.5,1.5),
                        Radius = F64.FromDouble(.5),
                        Surface = Surfaces.Shiny
                    }},
                Lights = new Light[] { 
                    new Light() {
                        Pos = F64Vec3.FromDouble(-2,2.5,0),
                        Color = Color.FromDouble(.49,.07,.07)
                    },
                    new Light() {
                        Pos = F64Vec3.FromDouble(1.5,2.5,1.5),
                        Color = Color.FromDouble(.07,.07,.49)
                    },
                    new Light() {
                        Pos = F64Vec3.FromDouble(1.5,2.5,-1.5),
                        Color = Color.FromDouble(.07,.49,.071)
                    },
                    new Light() {
                        Pos = F64Vec3.FromDouble(0,3.5,0),
                        Color = Color.FromDouble(.21,.21,.35)
                    }},
                Camera = Camera.Create(F64Vec3.FromDouble(3,2,4), F64Vec3.FromDouble(-1,.5,0))
            };
    }

    static class Surfaces
    {
        // Only works with X-Z plane.
        public static readonly Surface CheckerBoard = pos =>
        {
            bool isWhite = (F64.FloorToInt(pos.z) + F64.FloorToInt(pos.x)) % 2 != 0;
            return new Material
            {
                Diffuse = isWhite ? Color.White : Color.Black,
                Specular = Color.White,
                Reflect = F32.FromDouble(isWhite ? 0.1 : 0.7),
                Roughness = F32.FromInt(150)
            };
        };

        public static readonly Surface Shiny = pos =>
        {
            return new Material
            {
                Diffuse = Color.White,
                Specular = new Color(F32.Half, F32.Half, F32.Half),
                Reflect = F32.FromDouble(.6),
                Roughness = F32.FromInt(50)
            };
        };
    }

    public struct Color {
        public F32 R;
        public F32 G;
        public F32 B;

        public Color(F32 r, F32 g, F32 b) { R = r; G = g; B = b; }

        public static Color FromInt(int r, int g, int b) { return new Color(F32.FromInt(r), F32.FromInt(g), F32.FromInt(b)); }
        public static Color FromDouble(double r, double g, double b) { return new Color(F32.FromDouble(r), F32.FromDouble(g), F32.FromDouble(b)); }

        public static Color operator +(Color a, Color b) { return new Color(a.R + b.R, a.G + b.G, a.B + b.B); }
        public static Color operator -(Color a, Color b) { return new Color(a.R - b.R, a.G - b.G, a.B - b.B); }
        public static Color operator *(Color a, Color b) { return new Color(a.R * b.R, a.G * b.G, a.B * b.B); }
        public static Color operator *(F32 a, Color b) { return new Color(a * b.R, a * b.G, a * b.B); }
        public static Color operator *(Color a, F32 b) { return new Color(a.R * b, a.G * b, a.B * b); }

        public static Color Black { get { return new Color(F32.Zero, F32.Zero, F32.Zero); } }
        public static Color White { get { return new Color(F32.One, F32.One, F32.One); } }
        public static Color Background { get { return Color.FromDouble(0.4, 0.6, 1.0); } }

        public int ToInt(F32 d) {
            F32 clamped = F32.Max(F32.Zero, F32.Min(d, F32.One));
            return F32.RoundToInt(clamped * 255);
        }

        internal System.Drawing.Color ToDrawingColor() {
            return System.Drawing.Color.FromArgb(ToInt(R), ToInt(G), ToInt(B));
        }
    }

    struct Ray
    {
        public F64Vec3 Start;
        public F64Vec3 Dir;
    }

    class ISect
    {
        public SceneObject Thing;
        public Ray Ray;
        public F64 Dist;
    }

    class Material
    {
        public Color Diffuse;
        public Color Specular;
        public F32 Reflect;
        public F32 Roughness;
    }

    class Camera {
        public F64Vec3 Pos;
        public F64Vec3 Forward;
        public F64Vec3 Up;
        public F64Vec3 Right;

        public static Camera Create(F64Vec3 pos, F64Vec3 lookAt) {
            F64Vec3 forward = F64Vec3.Normalize(lookAt - pos);
            F64Vec3 down = new F64Vec3(F64.Zero, F64.Neg1, F64.Zero);
            F64Vec3 right = F64.FromDouble(1.5) * F64Vec3.Normalize(F64Vec3.Cross(forward, down));
            F64Vec3 up = F64.FromDouble(1.5) * F64Vec3.Normalize(F64Vec3.Cross(forward, right));

            return new Camera() { Pos = pos, Forward = forward, Up = up, Right = right };
        }
    }

    class Light {
        public F64Vec3 Pos;
        public Color Color;
    }

    abstract class SceneObject {
        public Surface Surface;
        public abstract ISect Intersect(Ray ray);
        public abstract F64Vec3 Normal(F64Vec3 pos);
    }

    class Sphere : SceneObject {
        public F64Vec3 Center;
        public F64 Radius;

        public override ISect Intersect(Ray ray) {
            F64Vec3 eo = Center - ray.Start;
            F64 v = F64Vec3.Dot(eo, ray.Dir);
            if (v < F64.Zero)
                return null;

            //F64 disc = F64.Pow(Radius, F64.FromInt(2)) - (F64Vec3.Dot(eo, eo) - F64.Pow(v, F64.FromInt(2)));
            F64 disc = (Radius * Radius) - (F64Vec3.Dot(eo, eo) - (v * v));
            F64 dist = disc < F64.Zero ? F64.Zero : v - F64.SqrtFast(disc);

            if (dist == F64.Zero)
                return null;

            return new ISect() {
                Thing = this,
                Ray = ray,
                Dist = dist
            };
        }

        public override F64Vec3 Normal(F64Vec3 pos) {
            return F64Vec3.Normalize(pos - Center);
        }
    }

    class Plane : SceneObject {
        public F64Vec3 Norm;
        public F64 Offset;

        public override ISect Intersect(Ray ray) {
            F64 epsilon = F64.FromRaw(256 * 65536);
            F64 denom = F64Vec3.Dot(Norm, ray.Dir);
            if (denom > -epsilon) return null;
            return new ISect() {
                Thing = this,
                Ray = ray,
                Dist = (F64Vec3.Dot(Norm, ray.Start) + Offset) * F64.RcpFast(-denom)
            };
        }

        public override F64Vec3 Normal(F64Vec3 pos) {
            return Norm;
        }
    }

    class Scene {
        public SceneObject[] Things;
        public Light[] Lights;
        public Camera Camera;
    }
}
