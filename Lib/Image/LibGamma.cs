
using System;
using Gtk;
using OpenCvSharp;

class LibGamma : Box
{
    private LibScaleBox _scaleGamma = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibGamma()
    {
        InitGui();
        InitEvents();
    }

    // ****************************************
    // Init Function
    // ****************************************
    private void InitGui()
    {
        _scaleGamma.Configure(label: "Gammma", from: 0, to: 2, decimal_place: 1);
        _scaleGamma.Set(1);
        this.Add(_scaleGamma);
        this.Orientation = Orientation.Vertical;
        this.Visible = true;
    }

    private void InitEvents()
    {
        _scaleGamma.OnChangedScale += OnChangeScale;
    }

    // ****************************************
    // Events Function
    // ****************************************
    private void OnChangeScale(double value)
    {
        if (_originImg == null) { return; }
        double Param = GetParam();
        Mat img = ImageProcessing(_originImg, Param);
        OnChangedImage?.Invoke(img);
    }

    // ****************************************
    // Private Function
    // ****************************************
    private Mat ImageProcessing(Mat sourceImg, double Param)
    {
        Mat distImg = new();
        byte[] lut = new byte[256];
        for (int i = 0; i < lut.Length; i++)
        {
            lut[i] = (byte)(Math.Pow((float)i / 255, 1.0f / Param) * 255);
        }
        Cv2.LUT(sourceImg, lut, distImg);
        return distImg;
    }

    private void SetParam(double Param)
    {
        _scaleGamma.Set(Param);
    }

    // ****************************************
    // Public Function
    // ****************************************
    public double GetParam()
    {
        return _scaleGamma.Get();
    }

    public Mat Run(Mat sourceImg, double Param = default)
    {
        _originImg = sourceImg;
        if (Param == default) { Param = GetParam(); }
        else { SetParam(Param); }
        return ImageProcessing(_originImg, Param);
    }
}