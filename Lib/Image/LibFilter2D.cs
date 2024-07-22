
using Gtk;
using OpenCvSharp;

class LibFilter2D : Box
{
    private LibScaleBox _scaleKernel = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibFilter2D()
    {
        InitGui();
        InitEvents();
    }

    // ****************************************
    // Init Function
    // ****************************************
    private void InitGui()
    {
        _scaleKernel.Configure(label: "Kernel", from: 0, to: 3, decimal_place: 2);
        this.Add(_scaleKernel);
        this.Orientation = Orientation.Vertical;
        this.Visible = true;
    }

    private void InitEvents()
    {
        _scaleKernel.OnChangedScale += OnChangeScale;
    }

    // ****************************************
    // Events Function
    // ****************************************
    private void OnChangeScale(double value)
    {
        if (_originImg == null) { return; }
        double Param = _scaleKernel.Get();
        Mat img = Filter2D(_originImg, Param);
        if (OnChangedImage == null) { return; }
        OnChangedImage(img);
    }

    // ****************************************
    // Private Function
    // ****************************************
    private Mat Filter2D(Mat sourceImg, double Param)
    {
        Mat dstImg = new();
        double[,] kernel = {
            { -Param,        -Param, -Param},
            { -Param, 1.0+8.0*Param, -Param},
            { -Param,        -Param, -Param},
        };
        Cv2.Filter2D(sourceImg, dstImg, -1, InputArray.Create(kernel));
        return dstImg;
    }

    // ****************************************
    // Public Function
    // ****************************************
    public Mat Run(Mat sourceImg, double Param = default)
    {
        _originImg = sourceImg;
        if (Param == default) { Param = _scaleKernel.Get(); }
        else { _scaleKernel.Set(Param); }
        return Filter2D(_originImg, Param);
    }

    public double GetParam()
    {
        return _scaleKernel.Get();
    }
}