
using System.Collections.Generic;
using Gtk;
using OpenCvSharp;

class LibAdaptiveThreshold : Box
{
    private LibOptionMenuBox _optMenuType = new();
    private LibScaleBox _scaleBlockSize = new();
    private LibScaleBox _scaleC = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibAdaptiveThreshold()
    {
        InitGui();
        InitEvents();
    }

    // ****************************************
    // Init Function
    // ****************************************
    private void InitGui()
    {
        _optMenuType.Configure(label: "Type", menuList: new List<string>(["Mean", "Gaussian"]));
        _scaleBlockSize.Configure(label: "Block Size", from: 3, to: 50, decimal_place: 0);
        _scaleC.Configure(label: "c", from: 0, to: 30, decimal_place: 0);
        this.Add(_optMenuType);
        this.Add(_scaleBlockSize);
        this.Add(_scaleC);
        this.Orientation = Orientation.Vertical;
        this.Visible = true;
    }

    private void InitEvents()
    {
        _optMenuType.OnChangedMenu += OnChangedMenu;
        _scaleBlockSize.OnChangedScale += OnChangeScale;
        _scaleC.OnChangedScale += OnChangeScale;
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
        Mat img = ImageProcessing(_originImg, GetParam());
        OnChangedImage?.Invoke(img);
    }

    // ****************************************
    // Private Function
    // ****************************************
    private Mat ImageProcessing(Mat sourceImg, (int index, int blockSize, double c) Param)
    {
        AdaptiveThresholdTypes type = AdaptiveThresholdTypes.MeanC;
        if (Param.index == 1) { type = AdaptiveThresholdTypes.GaussianC; }
        if (Param.blockSize % 2 == 0) { Param.blockSize += 1; }
        Mat dstImg = new();
        Cv2.CvtColor(sourceImg, dstImg, ColorConversionCodes.BGR2GRAY);
        Cv2.AdaptiveThreshold(dstImg,
                              dstImg,
                              255,
                              type,
                              ThresholdTypes.Binary,
                              Param.blockSize,
                              Param.c);
        return dstImg;
    }

    private void SetParam((int index, int blockSize, double c) Param)
    {
        _optMenuType.SetIndex(Param.index);
        _scaleBlockSize.Set(Param.blockSize);
        _scaleC.Set(Param.c);
    }

    // ****************************************
    // Public Function
    // ****************************************
    public (int index, int blockSize, double c) GetParam()
    {
        return (_optMenuType.GetIndex(), (int)_scaleBlockSize.Get(), _scaleC.Get());
    }

    public Mat Run(Mat sourceImg, (int index, int blockSize, double c) Param = default)
    {
        _originImg = sourceImg;
        if (Param == default) { Param = GetParam(); }
        else { SetParam(Param); }
        return ImageProcessing(_originImg, Param);
    }
}