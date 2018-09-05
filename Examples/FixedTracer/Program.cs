using System.Drawing;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace RayTracer
{
    public partial class RayTracerForm : Form
    {
        Bitmap bitmap;
        PictureBox pictureBox;
        const int width = 600;
        const int height = 600;

        public RayTracerForm()
        {
            bitmap = new Bitmap(width,height);

            pictureBox = new PictureBox();
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.Image = bitmap;

            ClientSize = new System.Drawing.Size(width, height + 24);
            Controls.Add(pictureBox);
            Text = "Ray Tracer";
            Load += RayTracerForm_Load;

            Show();
        }

        private void RayTracerForm_Load(object sender, EventArgs e)
        {
            this.Show();

            Stopwatch sw = Stopwatch.StartNew();
            System.Drawing.Color[,] pixels = FixedTracer.RayTracer.RenderDefaultScene(width, height, 16);
            //System.Drawing.Color[,] pixels = DoubleTracer.RayTracer.RenderDefaultScene(width, height);
            sw.Stop();
            Console.WriteLine("Elapsed: {0:0.00}s", sw.Elapsed.TotalSeconds);

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    bitmap.SetPixel(x, y, pixels[x, y]);

            pictureBox.Refresh();
            pictureBox.Invalidate();
        }

        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();

            Application.Run(new RayTracerForm());
        }
    }
}
