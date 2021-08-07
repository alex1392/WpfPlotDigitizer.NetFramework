using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;


namespace My_EMGU_Program
{
    public partial class Form1 : Form
    {
        Image<Bgr, Byte> My_Image;
        Image<Gray, Byte> gray_image;
        Image<Bgr, Byte> My_image_copy;

        bool gray_in_use = false;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Load_BTN_Click(object sender, EventArgs e)
        {
            OpenFileDialog Openfile = new OpenFileDialog();
            if (Openfile.ShowDialog() == DialogResult.OK)
            {
                //Load the Image
                My_Image = new Image<Bgr, Byte>(Openfile.FileName);

                //Display the Image
                image_PCBX.Image = My_Image.ToBitmap();

                My_image_copy = My_Image.Copy();

                //Set sepctrum choice
                Red_Spectrum_CHCK.Checked = true;
                Red_Spectrum_CHCK.Enabled = true;
                Green_Spectrum_CHCK.Checked = true;
                Green_Spectrum_CHCK.Enabled = true;
                Blue_Spectrum_CHCK.Checked = true;
                Blue_Spectrum_CHCK.Enabled = true;

                
            }
        }

        private void image_PCBX_MouseMove(object sender, MouseEventArgs e)
        {
            if (image_PCBX.Image != null)
            {
                X_pos_LBL.Text = "X: " + e.X.ToString();
                Y_pos_LBL.Text = "Y: " + e.Y.ToString();

                if (gray_in_use)
                {
                    Val_LBL.Text = "Value: " + gray_image[e.Y, e.X].ToString();
                }
                else
                {
                    Val_LBL.Text = "Value: " + My_Image[e.Y, e.X].ToString();
                }
                //It is much more stable with large images to access the image.Data propert directley than use code like bellow
                //Bitmap tmp_img = new Bitmap(image_PCBX.Image);
                //Val_LBL.Text = "Value: " + tmp_img.GetPixel(e.X, e.Y).ToString();
            }
        }

        private void Convert_btn_Click(object sender, EventArgs e)
        {
            if (My_Image != null)
            {
                if (gray_in_use)
                {
                    gray_in_use = false;
                    image_PCBX.Image = My_Image.ToBitmap();
                    Convert_btn.Text = "Convert to Gray";

                    Red_Spectrum_CHCK.Checked = true;
                    Red_Spectrum_CHCK.Enabled = true;
                    Green_Spectrum_CHCK.Checked = true;
                    Green_Spectrum_CHCK.Enabled = true;
                    Blue_Spectrum_CHCK.Checked = true;
                    Blue_Spectrum_CHCK.Enabled = true;
                }
                else
                {
                    gray_image = My_Image.Convert<Gray, Byte>();
                    gray_in_use = true;
                    image_PCBX.Image = gray_image.ToBitmap();
                    Convert_btn.Text = "Convert to Colour";
                   
                    Red_Spectrum_CHCK.Enabled = false;
                    Green_Spectrum_CHCK.Enabled = false;
                    Blue_Spectrum_CHCK.Enabled = false;
                }
            }
            
        }

        private void Red_Spectrum_CHCK_CheckedChanged(object sender, EventArgs e)
        {
            if (!Red_Spectrum_CHCK.Checked)
            {
                //Remove Red Spectrum programatically
                Suppress(2);
            }
            else
            {
                //Add Red Spectrum programatically
                Un_Suppress(2);
            }
            image_PCBX.Image = My_image_copy.ToBitmap();
        }

        private void Green_Spectrum_CHCK_CheckedChanged(object sender, EventArgs e)
        {
            if (!Green_Spectrum_CHCK.Checked)
            {
                //Remove Green Spectrum programatically
                Suppress(1);
            }
            else
            {
                //Add Green Spectrum programatically
                Un_Suppress(1);
            }
            image_PCBX.Image = My_image_copy.ToBitmap();
        }

        private void Blue_Spectrum_CHCK_CheckedChanged(object sender, EventArgs e)
        {
            if (!Blue_Spectrum_CHCK.Checked)
            {
                //Remove Blue Spectrum programatically
                Suppress(0);
            }
            else
            {
                //Add Blue Spectrum programatically
                Un_Suppress(0);
            }
            image_PCBX.Image = My_image_copy.ToBitmap();
        }

        private void Suppress(int spectrum)
        {

            for (int i = 0; i < My_Image.Height; i++)
            {
                for (int j = 0; j < My_Image.Width; j++)
                {
                    My_image_copy.Data[i, j, spectrum] = 0;
                }
            }

        }

        private void Un_Suppress(int spectrum)
        {
            for (int i = 0; i < My_Image.Height; i++)
            {
                for (int j = 0; j < My_Image.Width; j++)
                {
                    My_image_copy.Data[i, j, spectrum] = My_Image.Data[i, j, spectrum];
                }
            }
        }
    }
}
