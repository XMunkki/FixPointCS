using System;
using System.Threading.Tasks;

using FixMath;

// Based on https://blogs.msdn.microsoft.com/lukeh/2007/04/03/a-ray-tracer-in-c3-0/

namespace F64Tracer
{
    using Surface = Func<Vector, Material>;

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

        private Color GetNaturalColor(Material material, Vector pos, Vector norm, Vector rd, Scene scene) {
            Color ret = Color.Black;
            foreach (Light light in scene.Lights) {
                Vector ldis = Vector.Minus(light.Pos, pos);
                Vector livec = Vector.Norm(ldis);
                F64 neatIsect = TestRay(new Ray() { Start = pos, Dir = livec }, scene);
                bool isInShadow = !((neatIsect * neatIsect > Vector.MagSqr(ldis)) || (neatIsect == 0));
                if (!isInShadow) {
                    F64 illum = F64.Max(F64.Zero, Vector.Dot(livec, norm));
                    Color lcolor = Color.Times(illum, light.Color);
                    F64 specular = Vector.Dot(livec, Vector.Norm(rd));
                    Color scolor = specular > 0 ? Color.Times(F64.PowFastest(specular, material.Roughness), light.Color) : Color.Black;
                    ret = Color.Plus(ret, Color.Plus(Color.Times(material.Diffuse, lcolor), Color.Times(material.Specular, scolor)));
                }
            }
            return ret;
        }

        private Color GetReflectionColor(Material material, Vector pos, Vector norm, Vector rd, Scene scene, int depth) {
            return Color.Times(material.Reflect, TraceRay(new Ray() { Start = pos, Dir = rd }, scene, depth + 1));
        }

        private Color Shade(ISect isect, Scene scene, int depth) {
            var d = isect.Ray.Dir;
            var pos = Vector.Plus(Vector.Times(isect.Dist, isect.Ray.Dir), isect.Ray.Start);
            var normal = isect.Thing.Normal(pos);
            var reflectDir = Vector.Minus(d, Vector.Times(2 * Vector.Dot(normal, d), normal));
            Color ret = Color.DefaultColor;
            Material material = isect.Thing.Surface(pos);
            ret = Color.Plus(ret, GetNaturalColor(material, pos, normal, reflectDir, scene));
            if (depth >= MaxDepth) {
                return Color.Plus(ret, new Color(F64.Half, F64.Half, F64.Half));
            }
            return Color.Plus(ret, GetReflectionColor(material, Vector.Plus(pos, Vector.Times(F64.FromDouble(.001), reflectDir)), normal, reflectDir, scene, depth));
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
                        Vector rayDir = Vector.Norm(Vector.Plus(camera.Forward, Vector.Plus(Vector.Times(xx, camera.Right), Vector.Times(yy, camera.Up))));
                        Color color = TraceRay(new Ray() { Start = scene.Camera.Pos, Dir = rayDir }, scene, 0);
                        accum = Color.Plus(accum, color);
                    }
                    pixels[x, y] = Color.Times(F64.FromDouble(1.0 / NumSamples), accum).ToDrawingColor();
                }
            });
            return pixels;
        }

        internal readonly Scene DefaultScene =
            new Scene() {
                Things = new SceneObject[] { 
                    new Plane() {
                        Norm = Vector.FromDouble(0,1,0),
                        Offset = F64.FromDouble(0.0),
                        Surface = Surfaces.CheckerBoard
                    },
                    new Sphere() {
                        Center = Vector.FromDouble(0,1,0),
                        Radius = F64.FromDouble(1.0),
                        Surface = Surfaces.Shiny
                    },
                    new Sphere() {
                        Center = Vector.FromDouble(-1,.5,1.5),
                        Radius = F64.FromDouble(.5),
                        Surface = Surfaces.Shiny
                    }},
                Lights = new Light[] { 
                    new Light() {
                        Pos = Vector.FromDouble(-2,2.5,0),
                        Color = Color.FromDouble(.49,.07,.07)
                    },
                    new Light() {
                        Pos = Vector.FromDouble(1.5,2.5,1.5),
                        Color = Color.FromDouble(.07,.07,.49)
                    },
                    new Light() {
                        Pos = Vector.FromDouble(1.5,2.5,-1.5),
                        Color = Color.FromDouble(.07,.49,.071)
                    },
                    new Light() {
                        Pos = Vector.FromDouble(0,3.5,0),
                        Color = Color.FromDouble(.21,.21,.35)
                    }},
                Camera = Camera.Create(Vector.FromDouble(3,2,4), Vector.FromDouble(-1,.5,0))
            };
    }

    static class Surfaces
    {
        // Only works with X-Z plane.
        public static readonly Surface CheckerBoard = pos =>
        {
            bool isWhite = (F64.FloorToInt(pos.Z) + F64.FloorToInt(pos.X)) % 2 != 0;
            return new Material
            {
                Diffuse = isWhite ? Color.White : Color.Black,
                Specular = Color.White,
                Reflect = isWhite ? F64.FromDouble(.1) : F64.FromDouble(.7),
                Roughness = F64.FromInt(150)
            };
        };

        public static readonly Surface Shiny = pos =>
        {
            return new Material
            {
                Diffuse = Color.White,
                Specular = new Color(F64.Half, F64.Half, F64.Half),
                Reflect = F64.FromDouble(.6),
                Roughness = F64.FromInt(50)
            };
        };
    }

    struct Vector {
        public F64 X;
        public F64 Y;
        public F64 Z;

        public Vector(F64 x, F64 y, F64 z) { X = x; Y = y; Z = z; }
//        public Vector(string str) {
//            string[] nums = str.Split(',');
//            if (nums.Length != 3) throw new ArgumentException();
//            X = F64.Parse(nums[0]);
//            Y = F64.Parse(nums[1]);
//            Z = F64.Parse(nums[2]);
//        }
        public static Vector FromDouble(double x, double y, double z) { return new Vector(F64.FromDouble(x), F64.FromDouble(y), F64.FromDouble(z)); }
        public static Vector Times(F64 n, Vector v) {
            return new Vector(v.X * n, v.Y * n, v.Z * n);
        }
        public static Vector Minus(Vector v1, Vector v2) {
            return new Vector(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }
        public static Vector Plus(Vector v1, Vector v2) {
            return new Vector(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }
        public static F64 Dot(Vector v1, Vector v2) {
            return (v1.X * v2.X) + (v1.Y * v2.Y) + (v1.Z * v2.Z);
        }
        public static F64 Mag(Vector v) { return F64.SqrtFast(Dot(v, v)); }
        public static F64 MagSqr(Vector v) { return Dot(v, v); }
        public static Vector Norm(Vector v) {
            F64 ooLen = F64.RSqrtFast(Dot(v, v));
            return Times(ooLen, v);
//            F64 mag = Mag(v);
//            F64 div = mag == 0 ? F64.MaxValue : 1 / mag;
//            return Times(div, v);
        }
        public static Vector Cross(Vector v1, Vector v2) {
            return new Vector(((v1.Y * v2.Z) - (v1.Z * v2.Y)),
                              ((v1.Z * v2.X) - (v1.X * v2.Z)),
                              ((v1.X * v2.Y) - (v1.Y * v2.X)));
        }
        public static bool Equals(Vector v1, Vector v2) {
            return (v1.X == v2.X) && (v1.Y == v2.Y) && (v1.Z == v2.Z);
        }
    }

    public struct Color {
        public F64 R;
        public F64 G;
        public F64 B;

        public Color(F64 r, F64 g, F64 b) { R = r; G = g; B = b; }
//        public Color(string str) {
//            string[] nums = str.Split(',');
//            if (nums.Length != 3) throw new ArgumentException();
//            R = F64.Parse(nums[0]);
//            G = F64.Parse(nums[1]);
//            B = F64.Parse(nums[2]);
//        }

        public static Color FromInt(int r, int g, int b) { return new Color(F64.FromInt(r), F64.FromInt(g), F64.FromInt(b)); }
        public static Color FromDouble(double r, double g, double b) { return new Color(F64.FromDouble(r), F64.FromDouble(g), F64.FromDouble(b)); }

        public static Color Times(F64 n, Color v) {
            return new Color(n * v.R, n * v.G, n * v.B);
        }
        public static Color Times(Color v1, Color v2) { 
            return new Color(v1.R * v2.R, v1.G * v2.G, v1.B * v2.B);
        }
        public static Color Plus(Color v1, Color v2) { 
            return new Color(v1.R + v2.R, v1.G + v2.G, v1.B + v2.B);
        }
        public static Color Minus(Color v1, Color v2) { 
            return new Color(v1.R - v2.R, v1.G - v2.G, v1.B - v2.B);
        }

        public static Color Black { get { return new Color(F64.Zero, F64.Zero, F64.Zero); } }
        public static Color White { get { return new Color(F64.One, F64.One, F64.One); } }
        public static Color Background { get { return Color.FromDouble(0.4, 0.6, 1.0); } }
        public static Color DefaultColor { get { return new Color(F64.Zero, F64.Zero, F64.Zero); } }

        public int ToInt(F64 d) {
            F64 clamped = F64.Max(F64.Zero, F64.Min(d, F64.One));
            return F64.RoundToInt(clamped * 255);
        }

        internal System.Drawing.Color ToDrawingColor() {
            return System.Drawing.Color.FromArgb(ToInt(R), ToInt(G), ToInt(B));
        }
    }

    struct Ray {
        public Vector Start;
        public Vector Dir;
    }

    class ISect {
        public SceneObject Thing;
        public Ray Ray;
        public F64 Dist;
    }

    class Material
    {
        public Color Diffuse;
        public Color Specular;
        public F64 Reflect;
        public F64 Roughness;
    }

//    class Surface {
//        public Func<Vector, Color> Diffuse;
//        public Func<Vector, Color> Specular;
//        public Func<Vector, F64> Reflect;
//        public F64 Roughness;
//    }

    class Camera {
        public Vector Pos;
        public Vector Forward;
        public Vector Up;
        public Vector Right;

        public static Camera Create(Vector pos, Vector lookAt) {
            Vector forward = Vector.Norm(Vector.Minus(lookAt, pos));
            Vector down = new Vector(F64.Zero, F64.Neg1, F64.Zero);
            Vector right = Vector.Times(F64.FromDouble(1.5), Vector.Norm(Vector.Cross(forward, down)));
            Vector up = Vector.Times(F64.FromDouble(1.5), Vector.Norm(Vector.Cross(forward, right)));

            return new Camera() { Pos = pos, Forward = forward, Up = up, Right = right };
        }
    }

    class Light {
        public Vector Pos;
        public Color Color;
    }

    abstract class SceneObject {
        public Surface Surface;
        public abstract ISect Intersect(Ray ray);
        public abstract Vector Normal(Vector pos);
    }

    class Sphere : SceneObject {
        public Vector Center;
        public F64 Radius;

        public override ISect Intersect(Ray ray) {
            Vector eo = Vector.Minus(Center, ray.Start);
            F64 v = Vector.Dot(eo, ray.Dir);
            if (v < F64.Zero)
                return null;

            //F64 disc = F64.Pow(Radius, F64.FromInt(2)) - (Vector.Dot(eo, eo) - F64.Pow(v, F64.FromInt(2)));
            F64 disc = (Radius * Radius) - (Vector.Dot(eo, eo) - (v * v));
            F64 dist = disc < F64.Zero ? F64.Zero : v - F64.SqrtFast(disc);

            if (dist == F64.Zero)
                return null;

            return new ISect() {
                Thing = this,
                Ray = ray,
                Dist = dist
            };
        }

        public override Vector Normal(Vector pos) {
            return Vector.Norm(Vector.Minus(pos, Center));
        }
    }

    class Plane : SceneObject {
        public Vector Norm;
        public F64 Offset;

        public override ISect Intersect(Ray ray) {
            F64 epsilon = F64.FromRaw(256 * 65536);
            F64 denom = Vector.Dot(Norm, ray.Dir);
            if (denom > -epsilon) return null;
            return new ISect() {
                Thing = this,
                Ray = ray,
                Dist = (Vector.Dot(Norm, ray.Start) + Offset) * F64.RcpFast(-denom)
            };
        }

        public override Vector Normal(Vector pos) {
            return Norm;
        }
    }

    class Scene {
        public SceneObject[] Things;
        public Light[] Lights;
        public Camera Camera;
    }
}
