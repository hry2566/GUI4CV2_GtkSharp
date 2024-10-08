
using Gtk;
using OpenCvSharp;

class LibCanny : Box
{
    private LibScaleBox _scaleKernel = new();
    private LibScaleBox _scaleMax = new();
    private LibScaleBox _scaleMin = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibCanny()
    {
        InitGui();
        InitEvents();
    }

    // ****************************************
    // Init Function
    // ****************************************
    private void InitGui()
    {
        _scaleKernel.Configure(label: "Kernel", from: 1, to: 20, decimal_place: 0);
        _scaleMax.Configure(label: "Max", from: 0, to: 500, decimal_place: 0);
        _scaleMin.Configure(label: "Min", from: 0, to: 500, decimal_place: 0);
        this.Add(_scaleKernel);
        this.Add(_scaleMax);
        this.Add(_scaleMin);
        this.Orientation = Orientation.Vertical;
        this.Visible = true;
    }

    private void InitEvents()
    {
        _scaleKernel.OnChangedScale += OnChangeScale;
        _scaleMax.OnChangedScale += OnChangeScale;
        _scaleMin.OnChangedScale += OnChangeScale;
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
    private Mat ImageProcessing(Mat sourceImg, (int kernel, int max, int min) Param)
    {
        Mat dstImg = new();
        Cv2.Blur(sourceImg, dstImg, new OpenCvSharp.Size(Param.kernel, Param.kernel));
        Cv2.CvtColor(dstImg, dstImg, ColorConversionCodes.BGR2GRAY);
        Cv2.Canny(dstImg, dstImg, Param.max, Param.min);
        return dstImg;
    }

    private void SetParam((int kernel, int max, int min) Param)
    {
        _scaleKernel.Set(Param.kernel);
        _scaleMax.Set(Param.max);
        _scaleMin.Set(Param.min);
    }

    // ****************************************
    // Public Function
    // ****************************************
    public (int kernel, int max, int min) GetParam()
    {
        return ((int)_scaleKernel.Get(), (int)_scaleMax.Get(), (int)_scaleMin.Get());
    }

    public Mat Run(Mat sourceImg, (int kernel, int max, int min) Param = default)
    {
        _originImg = sourceImg;
        if (Param == default) { Param = GetParam(); }
        else { SetParam(Param); }
        return ImageProcessing(_originImg, Param);
    }
}