
using Gtk;
using OpenCvSharp;

class LibConvertScaleAbs : Box
{
    private LibScaleBox _scaleContrast = new();
    private LibScaleBox _scaleBrightness = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibConvertScaleAbs()
    {
        InitGui();
        InitEvents();
    }

    // ****************************************
    // Init Function
    // ****************************************
    private void InitGui()
    {
        _scaleContrast.Configure(label: "Contrast", from: 1, to: 3, decimal_place: 2);
        _scaleBrightness.Configure(label: "Brightness", from: -128, to: 128, decimal_place: 0);
        _scaleBrightness.Set(1);
        this.Add(_scaleContrast);
        this.Add(_scaleBrightness);
        this.Orientation = Orientation.Vertical;
        this.Visible = true;
    }

    private void InitEvents()
    {
        _scaleContrast.OnChangedScale += OnChangeScale;
        _scaleBrightness.OnChangedScale += OnChangeScale;
    }

    // ****************************************
    // Events Function
    // ****************************************
    private void OnChangeScale(double value)
    {
        if (_originImg == null) { return; }
        (double contrast, int brightness) Param = GetParam();
        Mat img = ImageProcessing(_originImg, Param);
        if (OnChangedImage == null) { return; }
        OnChangedImage(img);
    }

    // ****************************************
    // Private Function
    // ****************************************
    private Mat ImageProcessing(Mat sourceImg, (double contrast, int brightness) Param)
    {
        Mat distImg = new();
        Cv2.ConvertScaleAbs(sourceImg, distImg, Param.contrast, Param.brightness);
        return distImg;
    }

    private void SetParam((double contrast, int brightness) Param)
    {
        _scaleContrast.Set(Param.contrast);
        _scaleBrightness.Set(Param.brightness);
    }

    // ****************************************
    // Public Function
    // ****************************************
    public (double contrast, int brightness) GetParam()
    {
        return ((double)_scaleContrast.Get(), (int)_scaleBrightness.Get());
    }

    public Mat Run(Mat sourceImg, (double contrast, int brightness) Param = default)
    {
        _originImg = sourceImg;
        if (Param == default) { Param = GetParam(); }
        else { SetParam(Param); }
        return ImageProcessing(_originImg, Param);
    }
}