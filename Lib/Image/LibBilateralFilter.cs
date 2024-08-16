
using Gtk;
using OpenCvSharp;

class LibBilateralFilter : Box
{
    private LibScaleBox _scaleD = new();
    private LibScaleBox _scaleSigmaColor = new();
    private LibScaleBox _scaleSigmaSpace = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibBilateralFilter()
    {
        InitGui();
        InitEvents();
    }

    // ****************************************
    // Init Function
    // ****************************************
    private void InitGui()
    {
        _scaleD.Configure(label: "d", from: 1, to: 20, decimal_place: 0);
        _scaleSigmaColor.Configure(label: "SigmaColor", from: 1, to: 255, decimal_place: 0);
        _scaleSigmaSpace.Configure(label: "SigmaSpace", from: 1, to: 255, decimal_place: 0);
        this.Add(_scaleD);
        this.Add(_scaleSigmaColor);
        this.Add(_scaleSigmaSpace);
        this.Orientation = Orientation.Vertical;
        this.Visible = true;
    }

    private void InitEvents()
    {
        _scaleD.OnChangedScale += OnChangeScale;
        _scaleSigmaColor.OnChangedScale += OnChangeScale;
        _scaleSigmaSpace.OnChangedScale += OnChangeScale;
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
    private Mat ImageProcessing(Mat sourceImg, (int d, int sigmaColor, int sigmaSpace) Param)
    {
        Mat dstImg = new();
        Cv2.BilateralFilter(sourceImg, dstImg, Param.d, Param.sigmaColor, Param.sigmaSpace);
        return dstImg;
    }

    private void SetParam((int d, int sigmaColor, int sigmaSpace) Param)
    {
        _scaleD.Set(Param.d);
        _scaleSigmaColor.Set(Param.sigmaColor);
        _scaleSigmaSpace.Set(Param.sigmaSpace);
    }

    // ****************************************
    // Public Function
    // ****************************************
    public (int d, int sigmaColor, int sigmaSpace) GetParam()
    {
        return ((int)_scaleD.Get(), (int)_scaleSigmaColor.Get(), (int)_scaleSigmaSpace.Get());
    }

    public Mat Run(Mat sourceImg, (int d, int sigmaColor, int sigmaSpace) Param = default)
    {
        _originImg = sourceImg;
        if (Param == default) { Param = GetParam(); }
        else { SetParam(Param); }
        return ImageProcessing(_originImg, Param);
    }
}