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

        // private LibBlur _blur = new();
        // private LibGaussianBlur _blur = new();
        // private LibMedianBlur _blur=new();
        // private LibBilateralFilter _blur = new();
        private LibFastNlMeansDenoisingColored _blur = new();
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
            _cv2c.ImShow(_blur.Run(_originImg));
            // _cv2c.ImShow(_blur.Run(_originImg,(20,20)));
            // _cv2c.ImShow(_blur.Run(_originImg,(20,20,20.6)));
            // _cv2c.ImShow(_blur.Run(_originImg,20));
            // _cv2c.ImShow(_blur.Run(_originImg,(10,25,10)));
            // _cv2c.ImShow(_blur.Run(_originImg,(3,3,7,21)));
        }

        // ****************************************
        // Init Function
        // ****************************************
        private void InitGui()
        {
            _paned.Position = 300;
            _boxLeft.Add(_blur);
            _boxRight.Add(_cv2c);
        }

        private void InitEvents()
        {
            _blur.OnChangedImage += OnChangedImage;
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
