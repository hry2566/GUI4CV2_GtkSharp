
using Gtk;
using OpenCvSharp;

class LibTrim : Box
{
    private LibScaleBox _scaleX1 = new();
    private LibScaleBox _scaleY1 = new();
    private LibScaleBox _scaleX2 = new();
    private LibScaleBox _scaleY2 = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibTrim()
    {
        InitGui();
        InitEvents();
    }

    // ****************************************
    // Init Function
    // ****************************************
    private void InitGui()
    {
        _scaleX1.Configure(label: "X1", from: 0, to: 1, decimal_place: 0);
        _scaleY1.Configure(label: "Y1", from: 0, to: 1, decimal_place: 0);
        _scaleX2.Configure(label: "X2", from: 0, to: 1, decimal_place: 0);
        _scaleY2.Configure(label: "Y2", from: 0, to: 1, decimal_place: 0);
        this.Add(_scaleX1);
        this.Add(_scaleY1);
        this.Add(_scaleX2);
        this.Add(_scaleY2);
        this.Orientation = Orientation.Vertical;
        this.Visible = true;
    }

    private void InitEvents()
    {
        _scaleX1.OnChangedScale += OnChangeScale;
        _scaleY1.OnChangedScale += OnChangeScale;
        _scaleX2.OnChangedScale += OnChangeScale;
        _scaleY2.OnChangedScale += OnChangeScale;
    }

    // ****************************************
    // Events Function
    // ****************************************
    private void OnChangeScale(double value)
    {
        if (_originImg == null) { return; }
        Mat img = ImageProcessing(_originImg, GetParam());
        OnChangedImage?.Invoke(img);
    }

    // ****************************************
    // Private Function
    // ****************************************
    private Mat ImageProcessing(Mat sourceImg, (int x1, int y1, int x2, int y2) Param)
    {
        if (Param.x1 >= Param.x2 || Param.y1 >= Param.y2) { return sourceImg; }
        Rect roi = new Rect(Param.x1, Param.y1, Param.x2 - Param.x1, Param.y2 - Param.y1);
        return new Mat(sourceImg, roi);
    }

    private void SetParam((int x1, int y1, int x2, int y2) Param)
    {
        _scaleX1.Set(Param.x1);
        _scaleY1.Set(Param.y1);
        _scaleX2.Set(Param.x2);
        _scaleY2.Set(Param.y2);
    }

    private void SetScaleRange()
    {
        _scaleX1.Configure(from: 0, to: _originImg.Width);
        _scaleY1.Configure(from: 0, to: _originImg.Height);
        _scaleX2.Configure(from: 0, to: _originImg.Width);
        _scaleY2.Configure(from: 0, to: _originImg.Height);
        _scaleX2.Set(_originImg.Width);
        _scaleY2.Set(_originImg.Height);
    }

    // ****************************************
    // Public Function
    // ****************************************
    public (int x1, int y1, int x2, int y2) GetParam()
    {
        return ((int)_scaleX1.Get(), (int)_scaleY1.Get(), (int)_scaleX2.Get(), (int)_scaleY2.Get());
    }

    public Mat Run(Mat sourceImg, (int x1, int y1, int x2, int y2) Param = default)
    {
        _originImg = sourceImg;
        SetScaleRange();
        if (Param == default) { Param = GetParam(); }
        else { SetParam(Param); }
        return ImageProcessing(_originImg, Param);
    }
}