using System.Linq;
using System;
using System.Collections.Generic;

// Based on https://blogs.msdn.microsoft.com/lukeh/2007/04/03/a-ray-tracer-in-c3-0/

namespace DoubleTracer
{
    public class RayTracer
    {
        private int screenWidth;
        private int screenHeight;
        private const int MaxDepth = 5;

        public RayTracer(int screenWidth, int screenHeight)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
        }

        public static System.Drawing.Color[,] RenderDefaultScene(int width, int height)
        {
            RayTracer tracer = new RayTracer(width, height);
            return tracer.Render(tracer.DefaultScene);
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

        private double TestRay(Ray ray, Scene scene)
        {
            ISect isect = IntersectRay(ray, scene);
            if (isect == null)
                return 0;
            return isect.Dist;
        }

        private Color TraceRay(Ray ray, Scene scene, int depth)
        {
            ISect isect = IntersectRay(ray, scene);
            if (isect == null)
                return Color.Background;
            return Shade(isect, scene, depth);
        }

        private Color GetNaturalColor(SceneObject thing, Vector pos, Vector norm, Vector rd, Scene scene)
        {
            Color ret = Color.Make(0, 0, 0);
            foreach (Light light in scene.Lights)
            {
                Vector ldis = Vector.Minus(light.Pos, pos);
                Vector livec = Vector.Norm(ldis);
                double neatIsect = TestRay(new Ray() { Start = pos, Dir = livec }, scene);
                bool isInShadow = !((neatIsect > Vector.Mag(ldis)) || (neatIsect == 0));
                if (!isInShadow)
                {
                    double illum = Vector.Dot(livec, norm);
                    Color lcolor = illum > 0 ? Color.Times(illum, light.Color) : Color.Make(0, 0, 0);
                    double specular = Vector.Dot(livec, Vector.Norm(rd));
                    Color scolor = specular > 0 ? Color.Times(Math.Pow(specular, thing.Surface.Roughness), light.Color) : Color.Make(0, 0, 0);
                    ret = Color.Plus(ret, Color.Plus(Color.Times(thing.Surface.Diffuse(pos), lcolor),
                                                     Color.Times(thing.Surface.Specular(pos), scolor)));
                }
            }
            return ret;
        }

        private Color GetReflectionColor(SceneObject thing, Vector pos, Vector norm, Vector rd, Scene scene, int depth)
        {
            return Color.Times(thing.Surface.Reflect(pos), TraceRay(new Ray() { Start = pos, Dir = rd }, scene, depth + 1));
        }

        private Color Shade(ISect isect, Scene scene, int depth)
        {
            var d = isect.Ray.Dir;
            var pos = Vector.Plus(Vector.Times(isect.Dist, isect.Ray.Dir), isect.Ray.Start);
            var normal = isect.Thing.Normal(pos);
            var reflectDir = Vector.Minus(d, Vector.Times(2 * Vector.Dot(normal, d), normal));
            Color ret = Color.DefaultColor;
            ret = Color.Plus(ret, GetNaturalColor(isect.Thing, pos, normal, reflectDir, scene));
            if (depth >= MaxDepth)
            {
                return Color.Plus(ret, Color.Make(.5, .5, .5));
            }
            return Color.Plus(ret, GetReflectionColor(isect.Thing, Vector.Plus(pos, Vector.Times(.001, reflectDir)), normal, reflectDir, scene, depth));
        }

        private double RecenterX(double x)
        {
            return (x - (screenWidth / 2.0)) / (2.0 * screenWidth);
        }
        private double RecenterY(double y)
        {
            return -(y - (screenHeight / 2.0)) / (2.0 * screenHeight);
        }

        private Vector GetPoint(double x, double y, Camera camera)
        {
            return Vector.Norm(Vector.Plus(camera.Forward, Vector.Plus(Vector.Times(RecenterX(x), camera.Right),
                                                                       Vector.Times(RecenterY(y), camera.Up))));
        }

        internal System.Drawing.Color[,] Render(Scene scene)
        {
            System.Drawing.Color[,] pixels = new System.Drawing.Color[screenWidth, screenHeight];
            for (int y = 0; y < screenHeight; y++)
            {
                for (int x = 0; x < screenWidth; x++)
                {
                    Color color = TraceRay(new Ray() { Start = scene.Camera.Pos, Dir = GetPoint(x, y, scene.Camera) }, scene, 0);
                    pixels[x, y] = color.ToDrawingColor();
                }
            }
            return pixels;
        }

        internal readonly Scene DefaultScene =
            new Scene()
            {
                Things = new SceneObject[] {
                                new Plane() {
                                    Norm = Vector.Make(0,1,0),
                                    Offset = 0,
                                    Surface = Surfaces.CheckerBoard
                                },
                                new Sphere() {
                                    Center = Vector.Make(0,1,0),
                                    Radius = 1,
                                    Surface = Surfaces.Shiny
                                },
                                new Sphere() {
                                    Center = Vector.Make(-1,.5,1.5),
                                    Radius = .5,
                                    Surface = Surfaces.Shiny
                                }},
                Lights = new Light[] {
                                new Light() {
                                    Pos = Vector.Make(-2,2.5,0),
                                    Color = Color.Make(.49,.07,.07)
                                },
                                new Light() {
                                    Pos = Vector.Make(1.5,2.5,1.5),
                                    Color = Color.Make(.07,.07,.49)
                                },
                                new Light() {
                                    Pos = Vector.Make(1.5,2.5,-1.5),
                                    Color = Color.Make(.07,.49,.071)
                                },
                                new Light() {
                                    Pos = Vector.Make(0,3.5,0),
                                    Color = Color.Make(.21,.21,.35)
                                }},
                Camera = Camera.Create(Vector.Make(3, 2, 4), Vector.Make(-1, .5, 0))
            };
    }

    static class Surfaces
    {
        // Only works with X-Z plane.
        public static readonly Surface CheckerBoard =
            new Surface()
            {
                Diffuse = pos => ((Math.Floor(pos.Z) + Math.Floor(pos.X)) % 2 != 0)
                                    ? Color.Make(1, 1, 1)
                                    : Color.Make(0, 0, 0),
                Specular = pos => Color.Make(1, 1, 1),
                Reflect = pos => ((Math.Floor(pos.Z) + Math.Floor(pos.X)) % 2 != 0)
                                    ? .1
                                    : .7,
                Roughness = 150
            };


        public static readonly Surface Shiny =
            new Surface()
            {
                Diffuse = pos => Color.Make(1, 1, 1),
                Specular = pos => Color.Make(.5, .5, .5),
                Reflect = pos => .6,
                Roughness = 50
            };
    }

    struct Vector
    {
        public double X;
        public double Y;
        public double Z;

        public Vector(double x, double y, double z) { X = x; Y = y; Z = z; }
        public Vector(string str)
        {
            string[] nums = str.Split(',');
            if (nums.Length != 3) throw new ArgumentException();
            X = double.Parse(nums[0]);
            Y = double.Parse(nums[1]);
            Z = double.Parse(nums[2]);
        }
        public static Vector Make(double x, double y, double z) { return new Vector(x, y, z); }
        public static Vector Times(double n, Vector v)
        {
            return new Vector(v.X * n, v.Y * n, v.Z * n);
        }
        public static Vector Minus(Vector v1, Vector v2)
        {
            return new Vector(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }
        public static Vector Plus(Vector v1, Vector v2)
        {
            return new Vector(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }
        public static double Dot(Vector v1, Vector v2)
        {
            return (v1.X * v2.X) + (v1.Y * v2.Y) + (v1.Z * v2.Z);
        }
        public static double Mag(Vector v) { return Math.Sqrt(Dot(v, v)); }
        public static Vector Norm(Vector v)
        {
            double mag = Mag(v);
            double div = mag == 0 ? double.PositiveInfinity : 1 / mag;
            return Times(div, v);
        }
        public static Vector Cross(Vector v1, Vector v2)
        {
            return new Vector(((v1.Y * v2.Z) - (v1.Z * v2.Y)),
                              ((v1.Z * v2.X) - (v1.X * v2.Z)),
                              ((v1.X * v2.Y) - (v1.Y * v2.X)));
        }
        public static bool Equals(Vector v1, Vector v2)
        {
            return (v1.X == v2.X) && (v1.Y == v2.Y) && (v1.Z == v2.Z);
        }
    }

    public struct Color
    {
        public double R;
        public double G;
        public double B;

        public Color(double r, double g, double b) { R = r; G = g; B = b; }
        public Color(string str)
        {
            string[] nums = str.Split(',');
            if (nums.Length != 3) throw new ArgumentException();
            R = double.Parse(nums[0]);
            G = double.Parse(nums[1]);
            B = double.Parse(nums[2]);
        }

        public static Color Make(double r, double g, double b) { return new Color(r, g, b); }

        public static Color Times(double n, Color v)
        {
            return new Color(n * v.R, n * v.G, n * v.B);
        }
        public static Color Times(Color v1, Color v2)
        {
            return new Color(v1.R * v2.R, v1.G * v2.G, v1.B * v2.B);
        }

        public static Color Plus(Color v1, Color v2)
        {
            return new Color(v1.R + v2.R, v1.G + v2.G, v1.B + v2.B);
        }
        public static Color Minus(Color v1, Color v2)
        {
            return new Color(v1.R - v2.R, v1.G - v2.G, v1.B - v2.B);
        }

        public static readonly Color Background = Make(0, 0, 0);
        public static readonly Color DefaultColor = Make(0, 0, 0);

        public double Legalize(double d)
        {
            return d > 1 ? 1 : d;
        }

        internal System.Drawing.Color ToDrawingColor()
        {
            return System.Drawing.Color.FromArgb((int)(Legalize(R) * 255), (int)(Legalize(G) * 255), (int)(Legalize(B) * 255));
        }

    }

    struct Ray
    {
        public Vector Start;
        public Vector Dir;
    }

    class ISect
    {
        public SceneObject Thing;
        public Ray Ray;
        public double Dist;
    }

    class Surface
    {
        public Func<Vector, Color> Diffuse;
        public Func<Vector, Color> Specular;
        public Func<Vector, double> Reflect;
        public double Roughness;
    }

    class Camera
    {
        public Vector Pos;
        public Vector Forward;
        public Vector Up;
        public Vector Right;

        public static Camera Create(Vector pos, Vector lookAt)
        {
            Vector forward = Vector.Norm(Vector.Minus(lookAt, pos));
            Vector down = new Vector(0, -1, 0);
            Vector right = Vector.Times(1.5, Vector.Norm(Vector.Cross(forward, down)));
            Vector up = Vector.Times(1.5, Vector.Norm(Vector.Cross(forward, right)));

            return new Camera() { Pos = pos, Forward = forward, Up = up, Right = right };
        }
    }

    class Light
    {
        public Vector Pos;
        public Color Color;
    }

    abstract class SceneObject
    {
        public Surface Surface;
        public abstract ISect Intersect(Ray ray);
        public abstract Vector Normal(Vector pos);
    }

    class Sphere : SceneObject
    {
        public Vector Center;
        public double Radius;

        public override ISect Intersect(Ray ray)
        {
            Vector eo = Vector.Minus(Center, ray.Start);
            double v = Vector.Dot(eo, ray.Dir);
            double dist;
            if (v < 0)
            {
                dist = 0;
            }
            else
            {
                double disc = Math.Pow(Radius, 2) - (Vector.Dot(eo, eo) - Math.Pow(v, 2));
                dist = disc < 0 ? 0 : v - Math.Sqrt(disc);
            }
            if (dist == 0) return null;
            return new ISect()
            {
                Thing = this,
                Ray = ray,
                Dist = dist
            };
        }

        public override Vector Normal(Vector pos)
        {
            return Vector.Norm(Vector.Minus(pos, Center));
        }
    }

    class Plane : SceneObject
    {
        public Vector Norm;
        public double Offset;

        public override ISect Intersect(Ray ray)
        {
            double denom = Vector.Dot(Norm, ray.Dir);
            if (denom > 0) return null;
            return new ISect()
            {
                Thing = this,
                Ray = ray,
                Dist = (Vector.Dot(Norm, ray.Start) + Offset) / (-denom)
            };
        }

        public override Vector Normal(Vector pos)
        {
            return Norm;
        }
    }

    class Scene
    {
        public SceneObject[] Things;
        public Light[] Lights;
        public Camera Camera;

        public IEnumerable<ISect> Intersect(Ray r)
        {
            return from thing in Things
                   select thing.Intersect(r);
        }
    }
}
