
using Gtk;
using OpenCvSharp;

class LibWhiteBalance : Box
{
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibWhiteBalance()
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
        Mat labImage = new Mat();
        Mat lNormalized = new Mat();
        Cv2.CvtColor(sourceImg, labImage, ColorConversionCodes.BGR2Lab);
        Mat[] labPlanes = Cv2.Split(labImage);
        Mat l = labPlanes[0];
        Mat a = labPlanes[1];
        Mat b = labPlanes[2];
        Cv2.Normalize(l, lNormalized, 0, 1, NormTypes.MinMax, -1, l / 255.0);
        a -= lNormalized * (Cv2.Mean(a).Val0 - 128) * 1.1;
        b -= lNormalized * (Cv2.Mean(b).Val0 - 128) * 1.1;
        Cv2.Merge(new Mat[] { l, a, b }, labImage);
        Cv2.CvtColor(labImage, dstImg, ColorConversionCodes.Lab2BGR);
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