using System;

using FixMath;

// Based on https://blogs.msdn.microsoft.com/lukeh/2007/04/03/a-ray-tracer-in-c3-0/

namespace F32Tracer
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

        private F32 TestRay(Ray ray, Scene scene)
        {
            ISect isect = IntersectRay(ray, scene);
            if (isect == null)
                return F32.Zero;
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
                F32 neatIsect = TestRay(new Ray() { Start = pos, Dir = livec }, scene);
                bool isInShadow = !((neatIsect * neatIsect > Vector.MagSqr(ldis)) || (neatIsect == 0));
                if (!isInShadow) {
                    F32 illum = F32.Max(F32.Zero, Vector.Dot(livec, norm));
                    Color lcolor = Color.Times(illum, light.Color);
                    F32 specular = Vector.Dot(livec, Vector.Norm(rd));
                    Color scolor = specular > 0 ? Color.Times(F32.PowFastest(specular, material.Roughness), light.Color) : Color.Black;
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
                return Color.Plus(ret, new Color(F32.Half, F32.Half, F32.Half));
            }
            return Color.Plus(ret, GetReflectionColor(material, Vector.Plus(pos, Vector.Times(F32.FromDouble(.001), reflectDir)), normal, reflectDir, scene, depth));
        }

        private F32 RecenterX(F32 x) {
            return (x - (F32.FromInt(screenWidth) * F32.Half)) / (F32.Two * F32.FromInt(screenWidth));
        }
        private F32 RecenterY(F32 y) {
            return -(y - (F32.FromInt(screenHeight) * F32.Half)) / (F32.Two * F32.FromInt(screenHeight));
        }

        internal System.Drawing.Color[,] Render(Scene scene) {
            System.Drawing.Color[,] pixels = new System.Drawing.Color[screenWidth, screenHeight];
            Camera camera = scene.Camera;
            F32 sx = F32.One / (F32.Two * F32.FromInt(screenWidth));
            F32 ox = F32.Half * F32.FromInt(screenWidth);
            F32 sy = -F32.One / (F32.Two * F32.FromInt(screenHeight));
            F32 oy = F32.Half * F32.FromInt(screenHeight);
            for (int y = 0; y < screenHeight; y++)
            {
                for (int x = 0; x < screenWidth; x++)
                {
                    F32 xx = (F32.FromInt(x) - ox) * sx;
                    F32 yy = (F32.FromInt(y) - oy) * sy;
                    Vector rayDir = Vector.Norm(Vector.Plus(camera.Forward, Vector.Plus(Vector.Times(xx, camera.Right), Vector.Times(yy, camera.Up))));
                    Color color = TraceRay(new Ray() { Start = scene.Camera.Pos, Dir = rayDir }, scene, 0);
                    pixels[x, y] = color.ToDrawingColor();
                }
            }
            return pixels;
        }

        internal readonly Scene DefaultScene =
            new Scene() {
                Things = new SceneObject[] { 
                    new Plane() {
                        Norm = Vector.FromDouble(0,1,0),
                        Offset = F32.FromDouble(0.0),
                        Surface = Surfaces.CheckerBoard
                    },
                    new Sphere() {
                        Center = Vector.FromDouble(0,1,0),
                        Radius = F32.FromDouble(1.0),
                        Surface = Surfaces.Shiny
                    },
                    new Sphere() {
                        Center = Vector.FromDouble(-1,.5,1.5),
                        Radius = F32.FromDouble(.5),
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
            bool isWhite = (F32.FloorToInt(pos.Z) + F32.FloorToInt(pos.X)) % 2 != 0;
            return new Material
            {
                Diffuse = isWhite ? Color.White : Color.Black,
                Specular = Color.White,
                Reflect = isWhite ? F32.FromDouble(.1) : F32.FromDouble(.7),
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

    struct Vector {
        public F32 X;
        public F32 Y;
        public F32 Z;

        public Vector(F32 x, F32 y, F32 z) { X = x; Y = y; Z = z; }
//        public Vector(string str) {
//            string[] nums = str.Split(',');
//            if (nums.Length != 3) throw new ArgumentException();
//            X = F32.Parse(nums[0]);
//            Y = F32.Parse(nums[1]);
//            Z = F32.Parse(nums[2]);
//        }
        public static Vector FromDouble(double x, double y, double z) { return new Vector(F32.FromDouble(x), F32.FromDouble(y), F32.FromDouble(z)); }
        public static Vector Times(F32 n, Vector v) {
            return new Vector(v.X * n, v.Y * n, v.Z * n);
        }
        public static Vector Minus(Vector v1, Vector v2) {
            return new Vector(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }
        public static Vector Plus(Vector v1, Vector v2) {
            return new Vector(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }
        public static F32 Dot(Vector v1, Vector v2) {
            return (v1.X * v2.X) + (v1.Y * v2.Y) + (v1.Z * v2.Z);
        }
        public static F32 Mag(Vector v) { return F32.SqrtFast(Dot(v, v)); }
        public static F32 MagSqr(Vector v) { return Dot(v, v); }
        public static Vector Norm(Vector v) {
            F32 ooLen = F32.RSqrtFast(Dot(v, v));
            return Times(ooLen, v);
//            F32 mag = Mag(v);
//            F32 div = mag == 0 ? F32.MaxValue : 1 / mag;
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
        public F32 R;
        public F32 G;
        public F32 B;

        public Color(F32 r, F32 g, F32 b) { R = r; G = g; B = b; }
//        public Color(string str) {
//            string[] nums = str.Split(',');
//            if (nums.Length != 3) throw new ArgumentException();
//            R = F32.Parse(nums[0]);
//            G = F32.Parse(nums[1]);
//            B = F32.Parse(nums[2]);
//        }

        public static Color FromInt(int r, int g, int b) { return new Color(F32.FromInt(r), F32.FromInt(g), F32.FromInt(b)); }
        public static Color FromDouble(double r, double g, double b) { return new Color(F32.FromDouble(r), F32.FromDouble(g), F32.FromDouble(b)); }

        public static Color Times(F32 n, Color v) {
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

        public static Color Black { get { return new Color(F32.Zero, F32.Zero, F32.Zero); } }
        public static Color White { get { return new Color(F32.One, F32.One, F32.One); } }
        public static Color Background { get { return new Color(F32.Zero, F32.Zero, F32.Zero); } }
        public static Color DefaultColor { get { return new Color(F32.Zero, F32.Zero, F32.Zero); } }

        public F32 Legalize(F32 d) {
            return F32.Max(F32.Min(d, F32.One), F32.Zero);
        }

        internal System.Drawing.Color ToDrawingColor() {
            F32 s = F32.FromInt(255);
            return System.Drawing.Color.FromArgb(F32.FloorToInt(Legalize(R) * s), F32.FloorToInt(Legalize(G) * s), F32.FloorToInt(Legalize(B) * s));
        }
    }

    struct Ray {
        public Vector Start;
        public Vector Dir;
    }

    class ISect {
        public SceneObject Thing;
        public Ray Ray;
        public F32 Dist;
    }

    class Material
    {
        public Color Diffuse;
        public Color Specular;
        public F32 Reflect;
        public F32 Roughness;
    }

    class Camera {
        public Vector Pos;
        public Vector Forward;
        public Vector Up;
        public Vector Right;

        public static Camera Create(Vector pos, Vector lookAt) {
            Vector forward = Vector.Norm(Vector.Minus(lookAt, pos));
            Vector down = new Vector(F32.Zero, F32.Neg1, F32.Zero);
            Vector right = Vector.Times(F32.FromDouble(1.5), Vector.Norm(Vector.Cross(forward, down)));
            Vector up = Vector.Times(F32.FromDouble(1.5), Vector.Norm(Vector.Cross(forward, right)));

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
        public F32 Radius;

        public override ISect Intersect(Ray ray) {
            Vector eo = Vector.Minus(Center, ray.Start);
            F32 v = Vector.Dot(eo, ray.Dir);
            if (v < F32.Zero)
                return null;

            //F32 disc = F32.Pow(Radius, F32.FromInt(2)) - (Vector.Dot(eo, eo) - F32.Pow(v, F32.FromInt(2)));
            F32 disc = (Radius * Radius) - (Vector.Dot(eo, eo) - (v * v));
            F32 dist = disc < F32.Zero ? F32.Zero : v - F32.SqrtFast(disc);

            if (dist == F32.Zero)
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
        public F32 Offset;

        public override ISect Intersect(Ray ray) {
            F32 denom = Vector.Dot(Norm, ray.Dir);
            if (denom > F32.Zero) return null;
            return new ISect() {
                Thing = this,
                Ray = ray,
                Dist = (Vector.Dot(Norm, ray.Start) + Offset) * F32.RcpFast(-denom)
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
