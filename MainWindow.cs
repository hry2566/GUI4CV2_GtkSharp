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
        private LibInRangeHSV _imgProc = new();

        private LibImShow _cv2c = new();
        private Mat _originImg = null;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);
            DeleteEvent += Window_DeleteEvent;

            InitGui();
            InitEvents();

            _originImg = Cv2.ImRead("lenna.png", ImreadModes.Color);
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
            // _cv2c.ImShow(_imgProc.Run(_originImg,(100,255,100,255,100,255)));   // LibTInRGB
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
