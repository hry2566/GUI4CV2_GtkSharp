
using Gtk;
using OpenCvSharp;

class LibUnSharp : Box
{
    private LibScaleBox _scaleKernelX = new();
    private LibScaleBox _scaleKernelY = new();
    private LibScaleBox _scaleK = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibUnSharp()
    {
        InitGui();
        InitEvents();
    }

    // ****************************************
    // Init Function
    // ****************************************
    private void InitGui()
    {
        _scaleKernelX.Configure(label: "Kernel X", from: 1, to: 10, decimal_place: 0);
        _scaleKernelY.Configure(label: "Kernel Y", from: 1, to: 10, decimal_place: 0);
        _scaleK.Configure(label: "K", from: 0, to: 20, decimal_place: 1);
        this.Add(_scaleKernelX);
        this.Add(_scaleKernelY);
        this.Add(_scaleK);
        this.Orientation = Orientation.Vertical;
        this.Visible = true;
    }

    private void InitEvents()
    {
        _scaleKernelX.OnChangedScale += OnChangeScale;
        _scaleKernelY.OnChangedScale += OnChangeScale;
        _scaleK.OnChangedScale += OnChangeScale;
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
    private Mat ImageProcessing(Mat sourceImg, (int x, int y, double k) Param)
    {
        Mat dstImg = new();
        Cv2.Blur(sourceImg, dstImg, new OpenCvSharp.Size(Param.x, Param.y));
        double[,] kernel = {
            { -Param.k/9.0d,        -Param.k/9.0d, -Param.k/9.0d},
            { -Param.k/9.0d, 1.0+8.0*Param.k/9.0d, -Param.k/9.0d},
            { -Param.k/9.0d,        -Param.k/9.0d, -Param.k/9.0d},
        };
        Cv2.Filter2D(dstImg, dstImg, -1, InputArray.Create(kernel));
        return dstImg;
    }

    private void SetParam((int x, int y, double k) Param)
    {
        _scaleKernelX.Set(Param.x);
        _scaleKernelY.Set(Param.y);
        _scaleK.Set(Param.k);
    }

    // ****************************************
    // Public Function
    // ****************************************
    public (int x, int y, double k) GetParam()
    {
        return ((int)_scaleKernelX.Get(), (int)_scaleKernelY.Get(), _scaleK.Get());
    }

    public Mat Run(Mat sourceImg, (int x, int y, double k) Param = default)
    {
        _originImg = sourceImg;
        if (Param == default) { Param = GetParam(); }
        else { SetParam(Param); }
        return ImageProcessing(_originImg, Param);
    }
}