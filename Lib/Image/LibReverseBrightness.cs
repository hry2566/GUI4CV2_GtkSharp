
using Gtk;
using OpenCvSharp;

class LibReverseBrightness : Box
{
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibReverseBrightness()
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
        Mat dstImg = new();
        Mat hsvImg = new();
        Mat resultHsv = new();
        Cv2.CvtColor(sourceImg, hsvImg, ColorConversionCodes.BGR2HSV);
        Mat[] hsvChannels = Cv2.Split(hsvImg);
        Mat value = hsvChannels[2];
        Cv2.BitwiseNot(value, value);
        hsvChannels[2] = value;
        Cv2.Merge(hsvChannels, resultHsv);
        Cv2.CvtColor(resultHsv, dstImg, ColorConversionCodes.HSV2BGR);
        return dstImg;
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