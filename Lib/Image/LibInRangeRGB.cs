
using Gtk;
using OpenCvSharp;

class LibInRangeRGB : Box
{
    private LibScaleBox _scaleRMin = new();
    private LibScaleBox _scaleRMax = new();
    private LibScaleBox _scaleGMin = new();
    private LibScaleBox _scaleGMax = new();
    private LibScaleBox _scaleBMin = new();
    private LibScaleBox _scaleBMax = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibInRangeRGB()
    {
        InitGui();
        InitEvents();
    }

    // ****************************************
    // Init Function
    // ****************************************
    private void InitGui()
    {
        _scaleRMin.Configure(label: "R Min", from: 0, to: 255, decimal_place: 0);
        _scaleRMax.Configure(label: "R Max", from: 0, to: 255, decimal_place: 0);
        _scaleGMin.Configure(label: "G Min", from: 0, to: 255, decimal_place: 0);
        _scaleGMax.Configure(label: "G Max", from: 0, to: 255, decimal_place: 0);
        _scaleBMin.Configure(label: "B Min", from: 0, to: 255, decimal_place: 0);
        _scaleBMax.Configure(label: "B Max", from: 0, to: 255, decimal_place: 0);
        _scaleRMax.Set(255);
        _scaleGMax.Set(255);
        _scaleBMax.Set(255);
        this.Add(_scaleRMin);
        this.Add(_scaleRMax);
        this.Add(_scaleGMin);
        this.Add(_scaleGMax);
        this.Add(_scaleBMin);
        this.Add(_scaleBMax);
        this.Orientation = Orientation.Vertical;
        this.Visible = true;
    }

    private void InitEvents()
    {
        _scaleRMin.OnChangedScale += OnChangeScale;
        _scaleRMax.OnChangedScale += OnChangeScale;
        _scaleGMin.OnChangedScale += OnChangeScale;
        _scaleGMax.OnChangedScale += OnChangeScale;
        _scaleBMin.OnChangedScale += OnChangeScale;
        _scaleBMax.OnChangedScale += OnChangeScale;
    }

    // ****************************************
    // Events Function
    // ****************************************
    private void OnChangeScale(double value)
    {
        if (_originImg == null) { return; }
        (int RMin, int RMax, int GMin, int GMax, int BMin, int BMax) Param = GetParam();
        Mat img = ImageProcessing(_originImg, Param);
        OnChangedImage?.Invoke(img);
    }

    // ****************************************
    // Private Function
    // ****************************************
    private Mat ImageProcessing(Mat sourceImg, (int RMin, int RMax, int GMin, int GMax, int BMin, int BMax) Param)
    {
        Mat dstImg = new();
        Scalar s_min = new Scalar(Param.BMin, Param.GMin, Param.RMin);
        Scalar s_max = new Scalar(Param.BMax, Param.GMax, Param.RMax);
        Cv2.InRange(sourceImg, s_min, s_max, dstImg);
        return dstImg;
    }

    private void SetParam((int RMin, int RMax, int GMin, int GMax, int BMin, int BMax) Param)
    {
        _scaleRMin.Set(Param.RMin);
        _scaleRMax.Set(Param.RMax);
        _scaleGMin.Set(Param.GMin);
        _scaleGMax.Set(Param.GMax);
        _scaleBMin.Set(Param.BMin);
        _scaleBMax.Set(Param.BMax);
    }

    // ****************************************
    // Public Function
    // ****************************************
    public (int RMin, int RMax, int GMin, int GMax, int BMin, int BMax) GetParam()
    {
        return ((int)_scaleRMin.Get(), (int)_scaleRMax.Get(), (int)_scaleGMin.Get(), (int)_scaleGMax.Get(), (int)_scaleBMin.Get(), (int)_scaleBMax.Get());
    }

    public Mat Run(Mat sourceImg, (int RMin, int RMax, int GMin, int GMax, int BMin, int BMax) Param = default)
    {
        _originImg = sourceImg;
        if (Param == default) { Param = GetParam(); }
        else { SetParam(Param); }
        return ImageProcessing(_originImg, Param);
    }
}