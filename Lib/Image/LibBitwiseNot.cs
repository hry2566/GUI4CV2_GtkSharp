
using Gtk;
using OpenCvSharp;

class LibBitwiseNot : Box
{
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibBitwiseNot()
    {
        InitGui();
    }

    // ****************************************
    // Init Function
    // ****************************************
    private void InitGui()
    {
        this.Orientation = Orientation.Vertical;
        this.Visible = true;
    }

    // ****************************************
    // Events Function
    // ****************************************

    // ****************************************
    // Private Function
    // ****************************************
    private Mat ImageProcessing(Mat sourceImg)
    {
        Mat distImg = new();
        Cv2.BitwiseNot(sourceImg,distImg);
        return distImg;
    }

    // ****************************************
    // Public Function
    // ****************************************
    public Mat Run(Mat sourceImg)
    {
        _originImg = sourceImg;
        return ImageProcessing(_originImg);
    }
}