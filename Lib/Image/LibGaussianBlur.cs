
using Gtk;
using OpenCvSharp;

class LibGaussianBlur : Box
{
    private LibScaleBox _scaleX = new();
    private LibScaleBox _scaleY = new();
    private LibScaleBox _sigma = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibGaussianBlur()
    {
        InitGui();
        InitEvents();
    }

    // ****************************************
    // Init Function
    // ****************************************
    private void InitGui()
    {
        _scaleX.Configure(label: "kernel X", from: 1, to: 50, decimal_place: 0);
        _scaleY.Configure(label: "kernel Y", from: 1, to: 50, decimal_place: 0);
        _sigma.Configure(label: "sigma", from: 0, to: 50, decimal_place: 1);
        this.Add(_scaleX);
        this.Add(_scaleY);
        this.Add(_sigma);
        this.Orientation = Orientation.Vertical;
        this.Visible = true;
    }

    private void InitEvents()
    {
        _scaleX.OnChangedScale += OnChangeScale;
        _scaleY.OnChangedScale += OnChangeScale;
        _sigma.OnChangedScale += OnChangeScale;
    }

    // ****************************************
    // Events Function
    // ****************************************
    private void OnChangeScale(double value)
    {
        if (_originImg == null) { return; }
        (int x, int y, double sigma) Param;
        Param.x = (int)_scaleX.Get();
        Param.y = (int)_scaleY.Get();
        Param.sigma = _sigma.Get();
        if (Param.x % 2 == 0) { Param.x += 1; }
        if (Param.y % 2 == 0) { Param.y += 1; }

        Mat img = GaussianBlur(_originImg, Param);
        if (OnChangedImage == null) { return; }
        OnChangedImage(img);
    }

    // ****************************************
    // Private Function
    // ****************************************
    private Mat GaussianBlur(Mat sourceImg, (int x, int y, double sigma) Param)
    {
        Mat dstImg = new();
        Cv2.GaussianBlur(sourceImg, dstImg, new OpenCvSharp.Size(Param.x, Param.y), Param.sigma);
        return dstImg;
    }

    // ****************************************
    // Public Function
    // ****************************************
    public Mat Run(Mat sourceImg, (int x, int y, double sigma) Param = default)
    {
        _originImg = sourceImg;

        if (Param == default)
        {
            Param.x = (int)_scaleX.Get();
            Param.y = (int)_scaleY.Get();
            Param.sigma = _sigma.Get();
        }
        else
        {
            if (Param.x % 2 == 0) { Param.x += 1; }
            if (Param.y % 2 == 0) { Param.y += 1; }
            _scaleX.Set(Param.x);
            _scaleY.Set(Param.y);
            _sigma.Set(Param.sigma);
        }
        return GaussianBlur(_originImg, Param);
    }

    public (int x, int y, double sigma) GetParam()
    {
        return ((int)_scaleX.Get(), (int)_scaleY.Get(), _sigma.Get());
    }
}