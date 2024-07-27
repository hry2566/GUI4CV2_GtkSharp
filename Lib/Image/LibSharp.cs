
using Gtk;
using OpenCvSharp;

class LibSharp : Box
{
    private LibScaleBox _scaleKernel = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibSharp()
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
        double Param = GetParam();
        Mat img = Sharp(_originImg, Param);
        if (OnChangedImage == null) { return; }
        OnChangedImage(img);
    }

    // ****************************************
    // Private Function
    // ****************************************
    private Mat Sharp(Mat sourceImg, double Param)
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

    private void SetParam(double Param)
    {
        _scaleKernel.Set(Param);
    }

    // ****************************************
    // Public Function
    // ****************************************
    public double GetParam()
    {
        return _scaleKernel.Get();
    }

    public Mat Run(Mat sourceImg, double Param = default)
    {
        _originImg = sourceImg;
        if (Param == default) { Param = GetParam(); }
        else { SetParam(Param); }
        return Sharp(_originImg, Param);
    }
}