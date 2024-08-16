
using System;
using Gtk;
using OpenCvSharp;

class LibLabelingNoiseRemoval : Box
{
    private LibMaxMinEntryBox _maxminWidth = new();
    private LibMaxMinEntryBox _maxminHeight = new();
    private LibMaxMinEntryBox _maxminArea = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibLabelingNoiseRemoval()
    {
        InitGui();
        InitEvents();
    }

    // ****************************************
    // Init Function
    // ****************************************
    private void InitGui()
    {
        _maxminWidth.Configure(label: "width");
        _maxminHeight.Configure(label: "height");
        _maxminArea.Configure(label: "area");
        this.Add(_maxminWidth);
        this.Add(_maxminHeight);
        this.Add(_maxminArea);
        this.Orientation = Orientation.Vertical;
        this.Visible = true;
    }

    private void InitEvents()
    {
        _maxminWidth.OnChanged += OnChanged;
        _maxminHeight.OnChanged += OnChanged;
        _maxminArea.OnChanged += OnChanged;
    }



    // ****************************************
    // Events Function
    // ****************************************
    private void OnChanged(int min, int max)
    {
        if (_originImg == null) { return; }
        Mat img = ImageProcessing(_originImg, GetParam());
        OnChangedImage?.Invoke(img);
    }


    // ****************************************
    // Private Function
    // ****************************************
    private Mat ImageProcessing(Mat sourceImg, (int minWidth, int maxWidth, int minHeight, int maxHeight, int minArea, int maxArea) Param)
    {
        if (Param == default) { return sourceImg; }

        Mat dstImg = new Mat();
        if (sourceImg.Channels() == 3)
        {
            Cv2.CvtColor(sourceImg, dstImg, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(dstImg, dstImg, 0, 255, ThresholdTypes.Otsu);
        }
        else { dstImg = sourceImg; }
        Mat labels = new Mat();
        Mat stats = new Mat();
        Mat centroids = new Mat();
        int numLabels = Cv2.ConnectedComponentsWithStats(dstImg, labels, stats, centroids, PixelConnectivity.Connectivity8, MatType.CV_32S);
        for (int i = 1; i < numLabels; i++)
        {
            int width = stats.At<int>(i, (int)ConnectedComponentsTypes.Width);
            int height = stats.At<int>(i, (int)ConnectedComponentsTypes.Height);
            int area = stats.At<int>(i, (int)ConnectedComponentsTypes.Area);

            if ((Param.minWidth <= width && width <= Param.maxWidth) ||
                (Param.minHeight <= height && height <= Param.maxHeight) ||
                (Param.minArea <= area && area <= Param.maxArea))
            {
                Mat mask = new Mat();
                Cv2.InRange(labels, new Scalar(i), new Scalar(i), mask);
                dstImg.SetTo(0, mask);
            }
        }
        return dstImg;
    }

    private void SetParam((int minWidth, int maxWidth, int minHeight, int maxHeight, int minArea, int maxArea) Param)
    {
        _maxminWidth.Set((Param.minWidth, Param.maxWidth));
        _maxminHeight.Set((Param.minHeight, Param.maxHeight));
        _maxminArea.Set((Param.minArea, Param.maxArea));
    }

    // ****************************************
    // Public Function
    // ****************************************
    public (int minWidth, int maxWidth, int minHeight, int maxHeight, int minArea, int maxArea) GetParam()
    {
        (int minWidth, int maxWidth) = _maxminWidth.Get();
        (int minHeight, int maxHeight) = _maxminHeight.Get();
        (int minArea, int maxArea) = _maxminArea.Get();
        return (minWidth, maxWidth, minHeight, maxHeight, minArea, maxArea);
    }

    public Mat Run(Mat sourceImg, (int minWidth, int maxWidth, int minHeight, int maxHeight, int minArea, int maxArea) Param = default)
    {
        _originImg = sourceImg;
        if (Param == default) { Param = GetParam(); }
        else { SetParam(Param); }
        return ImageProcessing(_originImg, Param);
    }
}