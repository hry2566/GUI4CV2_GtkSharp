using System;
using Gtk;
using OpenCvSharp;
using UI = Gtk.Builder.ObjectAttribute;

namespace ImShowCustom
{
    class MainWindow : Gtk.Window
    {
        [UI] private Paned _paned = null;
        [UI] private Box _boxLeft = null;
        [UI] private Box _boxRight = null;

        // private LibBlur _imgProc = new();
        // private LibGaussianBlur _imgProc = new();
        // private LibMedianBlur _imgProc=new();
        // private LibBilateralFilter _imgProc = new();
        // private LibFastNlMeansDenoisingColored _imgProc = new();
        // private LibSharp _imgProc = new();
        // private LibUnSharp _imgProc = new();
        // private LibDilate _imgProc = new();
        // private LibErode _imgProc = new();
        // private LibMorphology _imgProc = new();
        // private LibThreshold _imgProc = new();
        // private LibInRangeRGB _imgProc = new();
        // private LibInRangeHSV _imgProc = new();
        // private LibAdaptiveThreshold _imgProc = new();
        // private LibCanny _imgProc = new();
        // private LibLaplacian _imgProc = new();
        // private LibSobel _imgProc = new();
        // private LibConvertScaleAbs _imgProc = new();
        // private LibGamma _imgProc = new();
        // private LibWhiteBalance _imgProc = new();
        // private LibEqualizeHist _imgProc = new();
        // private LibBitwiseNot _imgProc = new();
        // private LibReverseBrightness _imgProc = new();
        // private LibRotate _imgProc = new();
        // private LibWarpPerspective _imgProc = null;
        // private LibTrim _imgProc = new();
        // private LibShadingBlur _imgProc = new();
        // private LibShadingMedianBlur _imgProc = new();
        private LibShadingGaussianBlur _imgProc = new();


        private LibImShow _cv2c = new();
        private Mat _originImg = null;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);
            DeleteEvent += Window_DeleteEvent;

            // _imgProc = new(_cv2c);  // LibWarpPerspective
            InitGui();
            InitEvents();

            _originImg = Cv2.ImRead("lenna.png", ImreadModes.Color);
            // _originImg = Cv2.ImRead("30.png", ImreadModes.Color);               // LibWarpPerspective                   
            _cv2c.ImShow(_imgProc.Run(_originImg));
            // _cv2c.ImShow(_imgProc.Run(_originImg,(20,20)));                     // LibBlur
            // _cv2c.ImShow(_imgProc.Run(_originImg,(20,20,20.6)));                // LibGaussianBlur
            // _cv2c.ImShow(_imgProc.Run(_originImg,20));                          // LibMedianBlur
            // _cv2c.ImShow(_imgProc.Run(_originImg,(10,25,10)));                  // LibBilateralFilter
            // _cv2c.ImShow(_imgProc.Run(_originImg,(3,3,7,21)));                  // LibFastNlMeansDenoisingColored
            // _cv2c.ImShow(_imgProc.Run(_originImg,0.38));                        // LibSharp
            // _cv2c.ImShow(_imgProc.Run(_originImg,(3,3,3.7)));                   // LibUnSharp
            // _cv2c.ImShow(_imgProc.Run(_originImg,(4,4)));                       // LibDilate
            // _cv2c.ImShow(_imgProc.Run(_originImg,(4,4)));                       // LibErode
            // _cv2c.ImShow(_imgProc.Run(_originImg,(4,4,2)));                     // LibMorphology
            // _cv2c.ImShow(_imgProc.Run(_originImg,(0,120,255)));                 // LibThreshold
            // _cv2c.ImShow(_imgProc.Run(_originImg,(100,255,100,255,100,255)));   // LibTInRangeRGB
            // _cv2c.ImShow(_imgProc.Run(_originImg,(90,359,80,255,120,255)));     // LibTInRangeHSV
            // _cv2c.ImShow(_imgProc.Run(_originImg,(1,5,5)));                     // LibAdaptivehreshold
            // _cv2c.ImShow(_imgProc.Run(_originImg,(3,160,70)));                  // LibCanny
            // _cv2c.ImShow(_imgProc.Run(_originImg,3));                           // LibLaplacian
            // _cv2c.ImShow(_imgProc.Run(_originImg,3));                           // LibSobel
            // _cv2c.ImShow(_imgProc.Run(_originImg,(1.87,-80)));                  // LibConvertScaleAbs
            // _cv2c.ImShow(_imgProc.Run(_originImg, 1.5));                        // LibGamma
            // _cv2c.ImShow(_imgProc.Run(_originImg, (false, true, true)));        // LibEqualizeHist
            // _cv2c.ImShow(_imgProc.Run(_originImg, (-10, 0.8)));                 // LibRotate
            // Point pt1 = new(X: 46, Y: 156);                                     // LibWarpPerspective
            // Point pt2 = new(X: 421, Y: 62);                                     // LibWarpPerspective
            // Point pt3 = new(X: 37, Y: 244);                                     // LibWarpPerspective
            // Point pt4 = new(X: 428, Y: 171);                                    // LibWarpPerspective
            // _cv2c.ImShow(_imgProc.Run(_originImg, (pt1, pt2, pt3, pt4)));       // LibWarpPerspective
            // _cv2c.ImShow(_imgProc.Run(_originImg, (175, 175, 393, 393)));       // LibTrim
            // _cv2c.ImShow(_imgProc.Run(_originImg, (2, 2, 1, 25)));              // LibShadingBlur
            // _cv2c.ImShow(_imgProc.Run(_originImg, (2, 2, 0, 1, 15)));           // LibShadingGaussianBlur
        }

        // ****************************************
        // Init Function
        // ****************************************
        private void InitGui()
        {
            _paned.Position = 300;
            _boxLeft.Add(_imgProc);
            _boxRight.Add(_cv2c);
        }

        private void InitEvents()
        {
            _imgProc.OnChangedImage += OnChangedImage;
        }

        // ****************************************
        // events Function
        // ****************************************
        private void OnChangedImage(Mat img)
        {
            _cv2c.ImShow(img);
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        // ****************************************
        // private Function
        // ****************************************

        // ****************************************
        // public Function
        // ****************************************
    }
}
