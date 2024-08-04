
using System;
using Gtk;
using OpenCvSharp;

class LibEqualizeHist : Box
{
    private Lib3CheckButton _checkHSV = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibEqualizeHist()
    {
        InitGui();
        InitEvents();
    }

    // ****************************************
    // Init Function
    // ****************************************
    private void InitGui()
    {
        _checkHSV.Configure(label: "EqualizeHist", checkLbl: ["Hue", "Saturation", "Brightness"]);
        this.Add(_checkHSV);
        this.Orientation = Orientation.Vertical;
        this.Visible = true;
    }

    private void InitEvents()
    {
        _checkHSV.OnChangedCheck += OnChangeCheck;
    }

    // ****************************************
    // Events Function
    // ****************************************
    private void OnChangeCheck(bool[] check)
    {
        (bool Hue, bool Saturation, bool Brightness) Param = (check[0], check[1], check[2]);
        Mat img = ImageProcessing(_originImg, Param);
        OnChangedImage?.Invoke(img);
    }

    // ****************************************
    // Private Function
    // ****************************************
    private Mat ImageProcessing(Mat sourceImg, (bool Hue, bool Saturation, bool Brightness) Param)
    {
        Mat distImg = new();
        Cv2.CvtColor(sourceImg, distImg, ColorConversionCodes.BGR2HSV);
        Mat[] hsvChannels = Cv2.Split(distImg);
        var clahe = Cv2.CreateCLAHE(1.0, new Size(1, 1));
        if (Param.Hue) { clahe.Apply(hsvChannels[0], hsvChannels[0]); }
        if (Param.Saturation) { clahe.Apply(hsvChannels[1], hsvChannels[1]); }
        if (Param.Brightness) { clahe.Apply(hsvChannels[2], hsvChannels[2]); }
        Cv2.Merge(hsvChannels, distImg);
        Cv2.CvtColor(distImg, distImg, ColorConversionCodes.HSV2BGR);
        return distImg;
    }

    private void SetParam((bool Hue, bool Saturation, bool Brightness) Param)
    {
        _checkHSV.Set([Param.Hue, Param.Saturation, Param.Brightness]);
    }

    // ****************************************
    // Public Function
    // ****************************************
    public (bool Hue, bool Saturation, bool Brightness) GetParam()
    {
        bool[] check = _checkHSV.Get();
        return (check[0], check[1], check[2]);
    }

    public Mat Run(Mat sourceImg, (bool Hue, bool Saturation, bool Brightness) Param = default)
    {
        _originImg = sourceImg;
        if (Param == default) { Param = GetParam(); }
        else { SetParam(Param); }
        return ImageProcessing(_originImg, Param);
    }
}