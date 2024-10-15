using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcessing
{
    public partial class Form1 : Form
    {
        Bitmap loaded, processed;
        public Form1()
        {
            InitializeComponent();
            this.Text = "Image processing part 1";

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            loaded = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = loaded;

        }

        private void pixelCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel=loaded.GetPixel(x, y);
                    processed.SetPixel(x, y, pixel);
                }
            }

            pictureBox2.Image = processed;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            processed.Save(saveFileDialog1.FileName);
        }

        private void greyscalingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //used to compress the img when you have limited computing power
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            int ave;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    ave = (int)(pixel.R + pixel.G + pixel.B)/3;
                    Color grey = Color.FromArgb(ave,ave,ave);
                    processed.SetPixel(x, y, grey);
                }
            }

            pictureBox2.Image = processed;
        }

        private void inversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // use dark images to see something on that img
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    Color invert = Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B);
                    processed.SetPixel(x, y, invert);
                }
            }

            pictureBox2.Image = processed;
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
              
            tr = 0.393R + 0.769G + 0.189B
            tg = 0.349R + 0.686G + 0.168B
            tb = 0.272R + 0.534G + 0.131B

             */

            // use brown images to see something on that img looks like art
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            int r,g,b;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    int tr = (int)(0.393 * (pixel.R) + 0.769 * (pixel.G) + 0.189 * (pixel.B));
                    int tg = (int)(0.349 * (pixel.R) + 0.686 * (pixel.G) + 0.168 * (pixel.B));
                    int tb = (int)(0.272 * (pixel.R) + 0.534 * (pixel.G) + 0.131 * (pixel.B));
                    r = tr; 
                    g = tg; 
                    b = tb;
                    if (tr > 255)
                    {
                        r = 255;
                    }

                    if (tg > 255)
                    {
                        g = 255;
                    }
                  

                    if (tb > 255)
                    {
                        b = 255;
                    }
                   
                    Color sepia = Color.FromArgb(r, g, b);
                    processed.SetPixel(x, y, sepia);
                }
            }

            pictureBox2.Image = processed;
        }

        private void mirrorHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {

            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    processed.SetPixel(loaded.Width-1-x, y, pixel);
                }
            }

            pictureBox2.Image = processed;
        }

        private void mirrorVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {

            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    processed.SetPixel(x, loaded.Height-1-y, pixel);
                }
            }

            pictureBox2.Image = processed;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }
    }
}
