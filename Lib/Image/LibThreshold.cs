
using System.Collections.Generic;
using Gtk;
using OpenCvSharp;

class LibThreshold : Box
{
    private LibOptionMenuBox _optMenuType = new();
    private LibScaleBox _scaleTh = new();
    private LibScaleBox _scaleVal = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibThreshold()
    {
        InitGui();
        InitEvents();
    }

    // ****************************************
    // Init Function
    // ****************************************
    private void InitGui()
    {
        _optMenuType.Configure(label: "Type", menuList: new List<string>(["Binary", "BynaryInv", "Otsu", "Tozero", "TozeroInv"]));
        _scaleTh.Configure(label: "Threshold", from: 0, to: 255, decimal_place: 0);
        _scaleVal.Configure(label: "Max Value", from: 0, to: 255, decimal_place: 0);
        _scaleVal.Set(255);
        this.Add(_optMenuType);
        this.Add(_scaleTh);
        this.Add(_scaleVal);
        this.Orientation = Orientation.Vertical;
        this.Visible = true;
    }

    private void InitEvents()
    {
        _optMenuType.OnChangedMenu += OnChangedMenu;
        _scaleTh.OnChangedScale += OnChangeScale;
        _scaleVal.OnChangedScale += OnChangeScale;
    }

    // ****************************************
    // Events Function
    // ****************************************
    private void OnChangedMenu(string value)
    {
        OnChangeScale(0);
    }
    private void OnChangeScale(double value)
    {
        if (_originImg == null) { return; }
        (int type, double th, double val) Param = GetParam();
        Mat img = ImageProcessing(_originImg, Param);
        OnChangedImage?.Invoke(img);
    }

    // ****************************************
    // Private Function
    // ****************************************
    private Mat ImageProcessing(Mat sourceImg, (int type, double th, double val) Param)
    {
        var type = ThresholdTypes.Binary;
        if (Param.type == 1) { type = ThresholdTypes.BinaryInv; }
        if (Param.type == 2) { type = ThresholdTypes.Otsu; }
        if (Param.type == 3) { type = ThresholdTypes.Tozero; }
        if (Param.type == 4) { type = ThresholdTypes.TozeroInv; }

        Mat dstImg = new();
        Cv2.CvtColor(sourceImg, dstImg, ColorConversionCodes.BGR2GRAY);
        Cv2.Threshold(dstImg, dstImg, Param.th, Param.val, type);
        return dstImg;
    }

    private void SetParam((int type, double th, double val) Param)
    {
        _optMenuType.SetIndex(Param.type);
        _scaleTh.Set(Param.th);
        _scaleVal.Set(Param.val);
    }

    // ****************************************
    // Public Function
    // ****************************************
    public (int type, double th, double val) GetParam()
    {
        return (_optMenuType.GetIndex(), (double)_scaleTh.Get(), (double)_scaleVal.Get());
    }

    public Mat Run(Mat sourceImg, (int type, double th, double val) Param = default)
    {
        _originImg = sourceImg;
        if (Param == default) { Param = GetParam(); }
        else { SetParam(Param); }
        return ImageProcessing(_originImg, Param);
    }
}