using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;
using AForge.Imaging.Filters;

namespace ocrProjesi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private string OCR(Bitmap b)
        {
            string res = "";
            using (var engine = new TesseractEngine(@"tessdata", "eng", EngineMode.Default))
            {
                engine.SetVariable("tessedit_char_whitelist", "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ");
                engine.SetVariable("tessedit_unrej_any_wd", true);

                using (var page = engine.Process(b, PageSegMode.SingleLine))
                    res = page.GetText();
            }
            return res;
        }

        private string reconhecerCaptcha(Image img)
        {
            Bitmap _bitmap = new Bitmap(img);
            _bitmap = _bitmap.Clone(new Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Erosion erosion = new Erosion();
            Dilatation dilatation = new Dilatation();
            Invert inverter = new Invert();
            ColorFiltering cor = new ColorFiltering();
            cor.Blue = new AForge.IntRange(200, 255);
            cor.Red = new AForge.IntRange(200, 255);
            cor.Green = new AForge.IntRange(200, 255);
            Opening open = new Opening();
            BlobsFiltering bc = new BlobsFiltering();
            Closing close = new Closing();
            GaussianSharpen gs = new GaussianSharpen();
            ContrastCorrection cc = new ContrastCorrection();
            bc.MinHeight = 10;
            FiltersSequence seq = new FiltersSequence(gs, inverter, open, inverter, bc, inverter, open, cc, cor, bc, inverter);
            pictureBox1.Image = seq.Apply(_bitmap);
            string recognedString = OCR((Bitmap)pictureBox1.Image);
            return recognedString;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            Bitmap image = new Bitmap(openFileDialog1.FileName);
            if (image != null)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(openFileDialog1.FileName);
                label1.Text = reconhecerCaptcha(image);
            }
            image.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }
    }
}
