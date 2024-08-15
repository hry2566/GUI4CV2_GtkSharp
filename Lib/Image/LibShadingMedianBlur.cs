using System;
using Gtk;
using OpenCvSharp;

class LibShadingMedianBlur : Box
{
    private LibScaleBox _scaleKernel = new();
    private LibScaleBox _scaleRemove = new();
    private LibOptionMenuBox _optMenu = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibShadingMedianBlur()
    {
        InitGui();
        InitEvents();
    }

    // ****************************************
    // Init Function
    // ****************************************
    private void InitGui()
    {
        _scaleKernel.Configure(label: "kernel", from: 1, to: 100, decimal_place: 0);
        _optMenu.Configure(label: "mode", menuList: ["none", "light", "dark", "light & dark"]);
        _scaleRemove.Configure(label: "rmove noise", from: 1, to: 128, decimal_place: 0);
        this.Add(_scaleKernel);
        this.Add(_optMenu);
        this.Add(_scaleRemove);
        this.Orientation = Orientation.Vertical;
        this.Visible = true;
    }

    private void InitEvents()
    {
        _scaleKernel.OnChangedScale += OnChangeScale;
        _optMenu.OnChangedMenu += OnChangeMenu;
        _scaleRemove.OnChangedScale += OnChangeScale;
    }

    // ****************************************
    // Events Function
    // ****************************************
    private void OnChangeScale(double value)
    {
        OnChange();
    }

    private void OnChangeMenu(string value)
    {
        OnChange();
    }

    // ****************************************
    // Private Function
    // ****************************************
    private void OnChange()
    {
        if (_originImg == null) { return; }
        Mat img = ImageProcessing(_originImg, GetParam());
        OnChangedImage?.Invoke(img);
    }
    private Mat ImageProcessing(Mat sourceImg, (int kernel, int selectMenu, int noiseCut) Param)
    {
        Mat dstImg = new();
        sourceImg = sourceImg.CvtColor(ColorConversionCodes.BGR2GRAY);
        if (Param.kernel % 2 == 0) { Param.kernel += 1; }
        Cv2.MedianBlur(sourceImg, dstImg, Param.kernel);
        Cv2.Divide(sourceImg, dstImg, dstImg, scale: 128);
        if (Param.selectMenu != 0)
        {
            for (int y = 0; y < dstImg.Rows; y++)
            {
                for (int x = 0; x < dstImg.Cols; x++)
                {
                    byte val = dstImg.At<byte>(y, x);
                    if (Param.selectMenu == 1) { dstImg.Set(y, x, (byte)(val < 128 + Param.noiseCut ? 0 : 255)); }
                    else if (Param.selectMenu == 2) { dstImg.Set(y, x, (byte)(val > 128 - Param.noiseCut ? 255 : 0)); }
                    else if (Param.selectMenu == 3) { dstImg.Set(y, x, (byte)((val > 128 - Param.noiseCut && val < 128 + Param.noiseCut) ? 128 : 255)); }
                }
            }
        }
        return dstImg.CvtColor(ColorConversionCodes.GRAY2BGR);
    }

    private void SetParam((int kernel, int selectMenu, int noiseCut) Param)
    {
        _scaleKernel.Set(Param.kernel);
        _optMenu.SetIndex(Param.selectMenu);
        _scaleRemove.Set(Param.noiseCut);
    }

    // ****************************************
    // Public Function
    // ****************************************
    public (int kernel, int selectMenu, int noiseCut) GetParam()
    {
        return ((int)_scaleKernel.Get(), _optMenu.GetIndex(), (int)_scaleRemove.Get());
    }

    public Mat Run(Mat sourceImg, (int kernel, int selectMenu, int noiseCut) Param = default)
    {
        _originImg = sourceImg;
        if (Param == default) { Param = GetParam(); }
        else { SetParam(Param); }
        return ImageProcessing(_originImg, Param);
    }
}