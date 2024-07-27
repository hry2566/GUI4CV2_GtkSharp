
using System.Collections.Generic;
using Gtk;
using OpenCvSharp;

class LibMorphology : Box
{
    private LibOptionMenuBox _optMenu = new();
    private LibScaleBox _scaleX = new();
    private LibScaleBox _scaleY = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibMorphology()
    {
        InitGui();
        InitEvents();
    }

    // ****************************************
    // Init Function
    // ****************************************
    private void InitGui()
    {
        _optMenu.Configure(label: "operation", menuList: new List<string>(["Open", "Close", "Gradient"]));
        _scaleX.Configure(label: "kernel X", from: 1, to: 50, decimal_place: 0);
        _scaleY.Configure(label: "kernel Y", from: 1, to: 50, decimal_place: 0);

        this.Add(_optMenu);
        this.Add(_scaleX);
        this.Add(_scaleY);
        this.Orientation = Orientation.Vertical;
        this.Visible = true;
    }

    private void InitEvents()
    {
        _optMenu.OnChangedMenu += OnChangedMenu;
        _scaleX.OnChangedScale += OnChangeScale;
        _scaleY.OnChangedScale += OnChangeScale;
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
        (int x, int y, int index) Param;
        Param.x = (int)_scaleX.Get();
        Param.y = (int)_scaleY.Get();
        Param.index = _optMenu.GetIndex();
        Mat img = Morphology(_originImg, Param);
        if (OnChangedImage == null) { return; }
        OnChangedImage(img);
    }

    // ****************************************
    // Private Function
    // ****************************************
    private Mat Morphology(Mat sourceImg, (int x, int y, int index) Param)
    {
        var operation = MorphTypes.Open;
        if (Param.index == 1) { operation = MorphTypes.Close; }
        if (Param.index == 2) { operation = MorphTypes.Gradient; }

        Mat distImg = new();
        Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(Param.x, Param.y));
        Cv2.MorphologyEx(sourceImg, distImg, operation, kernel);
        return distImg;
    }

    // ****************************************
    // Public Function
    // ****************************************
    public Mat Run(Mat sourceImg, (int x, int y, int index) Param = default)
    {
        _originImg = sourceImg;
        if (Param == default)
        {
            Param.x = (int)_scaleX.Get();
            Param.y = (int)_scaleY.Get();
            Param.index = _optMenu.GetIndex();
        }
        else
        {
            _scaleX.Set(Param.x);
            _scaleY.Set(Param.y);
            _optMenu.SetIndex(Param.index);
        }
        return Morphology(_originImg, Param);
    }

    public (int x, int y, int index) GetParam()
    {
        return ((int)_scaleX.Get(), (int)_scaleY.Get(), _optMenu.GetIndex());
    }
}