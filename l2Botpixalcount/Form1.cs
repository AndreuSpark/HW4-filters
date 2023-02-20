using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using System.Threading;
using Emgu.CV.Structure;

namespace l2Botpixalcount
{
    public partial class Form1 : Form
    {
        private VideoCapture _capture;
        private Thread thread;
        private int HugeThreshholdMax = 00;
        private int HugeThreshholdMin = 0;
        private int HugeThreshholdSatMax = 0;
        private int HugeThreshholdSatMin = 0;
        private int HugeThreshholdValMax = 0;
        private int HugeThreshholdValMin = 0;
        private int  BiniryThreshhold=0;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            _capture = new VideoCapture(1);
            thread = new Thread(DisplayWebcam);
            thread.Start();
            Hugemax.Value = HugeThreshholdMax;
            Hugemin.Value = HugeThreshholdMin;
            trackBar1.Value = HugeThreshholdSatMax;
            trackBar2.Value = HugeThreshholdSatMin;
            trackBar3.Value = HugeThreshholdValMax;
            trackBar4.Value = HugeThreshholdValMin;
            trackBar5.Value = BiniryThreshhold;
        }
        private void DisplayWebcam()
        {
            while (_capture.IsOpened)
            {//frame maintinens

                Mat frame = _capture.QueryFrame(); //gets frame
                Mat resizedFrame = Resize(frame);//Resize frame
                pictureBox1.Image = resizedFrame.ToBitmap();//displays frame

                Mat[] hsvChannels = ConvetToHVS(frame);//converts frame to hvs
                Mat Hue=displayHue(hsvChannels);//displays hue
                Mat Sat = displaySaterashion(hsvChannels);
                Mat Val =displayValue(hsvChannels);
                displaybinery(frame);
                displayCombined(Hue, Sat, Val);


            }

        }
        private void displaybinery(Mat frame)
        {
            Image<Bgr, Byte> imeg = frame.ToImage<Bgr, byte>();
            Image<Gray, Byte> imgGray = imeg.Convert<Gray, byte>().ThresholdBinary(new Gray( BiniryThreshhold), new Gray(255));
            Mat newframe = imgGray.Mat;
            pictureBox5.Image = Resize(newframe).ToBitmap();
        }
        
        private void displayCombined(Mat hue ,Mat sat ,Mat val) 
        {
            Mat combinedImige = new Mat();
            CvInvoke.BitwiseAnd(hue, sat, combinedImige);
            CvInvoke.BitwiseAnd(combinedImige, val, combinedImige);
            Mat filterdcombinedImige = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Cross, new Size(4, 4), new Point(1, 1));
            CvInvoke.Dilate(combinedImige, combinedImige, filterdcombinedImige, new Point(1, 1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());
            CvInvoke.Erode(combinedImige, combinedImige, filterdcombinedImige, new Point(1, 1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());
            Invoke(new Action(() => { pictureBox6.Image = Resize(combinedImige).ToBitmap(); }));
        }
        private Mat displayHue(Mat[] hsvChannel)
        {
            Mat hueFillter = new Mat();
            CvInvoke.InRange(hsvChannel[0], new ScalarArray(HugeThreshholdMin),new ScalarArray(HugeThreshholdMax),hueFillter);//sets thresholdse
            Mat resizedhueFillter = Resize(hueFillter);//resizes frame and stors as new mat
            Invoke(new Action(() => { pictureBox2.Image = resizedhueFillter.ToBitmap();}));//turns to bitmap 
            return hueFillter;
        }
        private Mat displaySaterashion(Mat[] hsvChannel)
        {
            Mat SatFillter = new Mat();
            CvInvoke.InRange(hsvChannel[1], new ScalarArray(HugeThreshholdSatMin), new ScalarArray(HugeThreshholdSatMax), SatFillter);//sets thresholdse
            Mat resizedhueFillter = Resize(SatFillter);//resizes frame and stors as new mat
            Invoke(new Action(() => { pictureBox3.Image = resizedhueFillter.ToBitmap(); }));//turns to bitmap 
            return SatFillter;
        }
        private Mat displayValue(Mat[] hsvChannel)
        {
            Mat SatFillter = new Mat();
            CvInvoke.InRange(hsvChannel[2], new ScalarArray(HugeThreshholdValMin), new ScalarArray(HugeThreshholdValMax), SatFillter);//sets thresholdse
            Mat resizedhueFillter = Resize(SatFillter);//resizes frame and stors as new mat
            Invoke(new Action(() => { pictureBox4.Image = resizedhueFillter.ToBitmap(); }));//turns to bitmap 
            return SatFillter;
        }
        private Mat[] ConvetToHVS(Mat framein)
        {
            // convert the image to an HVS image:
            Mat hvsframe = new Mat();
            CvInvoke.CvtColor(framein, hvsframe, Emgu.CV.CvEnum.ColorConversion.Bgr2Hsv);

            Mat[] hsvChannels = hvsframe.Split();
            return hsvChannels;

        }
        private Mat Resize(Mat framein)
        {
            Mat frame = framein.Clone();
            int newHight = (frame.Size.Height * pictureBox1.Size.Width) / frame.Size.Width;
            Size newSize = new Size(pictureBox1.Size.Width, newHight);
            CvInvoke.Resize(frame, frame, newSize);
            return frame;
        }
        private void form1_closing(object sender, FormClosingEventArgs e)
        {
            thread.Abort();
        }
      
        private void Hugemax_Scroll(object sender, EventArgs e)
        {
            HugeThreshholdMax = Hugemax.Value;
        }

        private void Hugemin_Scroll(object sender, EventArgs e)
        {
            HugeThreshholdMin = Hugemin.Value;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            HugeThreshholdSatMin = trackBar2.Value;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            HugeThreshholdSatMax = trackBar1.Value;
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            HugeThreshholdValMax = trackBar4.Value;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            HugeThreshholdValMin = trackBar3.Value;
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            BiniryThreshhold = trackBar5.Value;
        }
    }
}
