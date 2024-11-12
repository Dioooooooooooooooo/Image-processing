using ImageProcess2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using AForge.Video;
using System.Drawing.Imaging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace ImageProcessing
{
    public partial class Form1 : Form
    {
        Boolean webcam;
        String webcamMode = "";
        FilterInfoCollection _filterInfoCollection;
        VideoCaptureDevice _videoCaptureDevice;

        Bitmap loaded, processed;
        Bitmap imageB, imageA, colorGreen;
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

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog3.ShowDialog();
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            imageB = new Bitmap(openFileDialog2.FileName);
            pictureBox1.Image = imageB;
        }

        private void openFileDialog3_FileOk(object sender, CancelEventArgs e)
        {
            imageA = new Bitmap(openFileDialog3.FileName);
            pictureBox2.Image = imageA;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Color mygreen = Color.FromArgb(0, 0, 255);
            int greygreen = (mygreen.R +  mygreen.G + mygreen.B) / 3;
            int threshold = 5;
            processed = new Bitmap(imageB.Width, imageB.Height);

            for (int x = 0; x < imageB.Width; x++)
            {
               for (int y = 0; y < imageB.Height; y++)
                {
                    Color pixel = imageB.GetPixel(x, y);
                    Color backpixel = imageA.GetPixel(x, y);
                    int grey = (pixel.R + pixel.G + pixel.B) / 3;
                    int subtractvalue = Math.Abs(grey - greygreen);
                    if (subtractvalue > threshold)
                        processed.SetPixel(x, y, pixel);
                    else
                        processed.SetPixel(x, y, backpixel);
                          
                }
            }

            pictureBox3.Image = processed;
        }

        private void onToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_videoCaptureDevice == null)
            {
                _videoCaptureDevice = new VideoCaptureDevice(_filterInfoCollection[0].MonikerString);
                _videoCaptureDevice.NewFrame += FinalFrame_NewFrame;
                _videoCaptureDevice.Start();
            }
        }

        private void offToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_videoCaptureDevice != null)
            {
                _videoCaptureDevice.Stop();
                pictureBox1 = null;
                webcam = false;
            }
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
            if (_filterInfoCollection == null)
            {
                _filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }


        private void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            using (Bitmap originalFrame = (Bitmap)eventArgs.Frame.Clone())
            {
                if (originalFrame != null)
                {
                    pictureBox1.Image = (Bitmap)originalFrame.Clone();
                    Bitmap processed = null;

                    switch (webcamMode)
                    {
                        case "PixelCopy":
                            processed = DIP.PixelCopy((Bitmap)originalFrame.Clone());
                            break;
                        case "Greyscaling":
                            processed = DIP.Greyscale((Bitmap)originalFrame.Clone());
                            break;
                        case "Inversion":
                            processed = DIP.ColorInversion((Bitmap)originalFrame.Clone());
                            break;
                        case "MirrorHorizontal":
                            processed = DIP.MirrorHorizontal((Bitmap)originalFrame.Clone());
                            break;
                        case "MirrorVertical":
                            processed = DIP.MirrorVertical((Bitmap)originalFrame.Clone());
                            break;
                        case "Sepia":
                            processed = DIP.Sepia((Bitmap)originalFrame.Clone());
                            break;
                    }
                    if(processed != null)
                    {
                        pictureBox2.Image = processed;
                    }
                }
            }
        }

        private void pixelCopyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            webcamMode = "PixelCopy";
        }

        private void greyscalingToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            webcamMode = "Greyscaling";
        }

        private void inversionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            webcamMode = "Inversion";
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void convuToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void smoothingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded);
            BitmapFilter.Smooth(processed, 50);
            pictureBox2.Image = processed;
        }

        private void gaussianBLurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded);
            BitmapFilter.GaussianBlur(processed, 300);
            pictureBox2.Image = processed;
        }

        private void sharpenToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded);
            BitmapFilter.Sharpen(processed, 11);
            pictureBox2.Image = processed;
        }

        private void meanRemovalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded);
            BitmapFilter.Sharpen(processed, 9);
            pictureBox2.Image = processed; 
        }

        private void embossLaplascianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded);
            BitmapFilter.EmbossLaplacian(processed);
            pictureBox2.Image = processed;
        }

        private void mirrorHorizontalToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            webcamMode = "MirrorHorizontal";
        }

        private void mirrorVerticalToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            webcamMode = "MirrorVertical";
        }
        private void sepiaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            webcamMode = "Sepia";
        }
    }
} 