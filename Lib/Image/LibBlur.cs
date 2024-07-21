
using Gtk;
using OpenCvSharp;

class LibBlur : Box
{
    private LibScaleBox _scaleX = new();
    private LibScaleBox _scaleY = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibBlur()
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
        this.Add(_scaleX);
        this.Add(_scaleY);
        this.Orientation = Orientation.Vertical;
        this.Visible = true;
    }

    private void InitEvents()
    {
        _scaleX.OnChangedScale += OnChangeScale;
        _scaleY.OnChangedScale += OnChangeScale;
    }

    // ****************************************
    // Events Function
    // ****************************************
    private void OnChangeScale(double value)
    {
        if (_originImg == null) { return; }
        (int x, int y) Param;
        Param.x = (int)_scaleX.Get();
        Param.y = (int)_scaleY.Get();
        Mat img = Blur(_originImg, Param);
        if (OnChangedImage == null) { return; }
        OnChangedImage(img);
    }

    // ****************************************
    // Private Function
    // ****************************************
    private Mat Blur(Mat sourceImg, (int x, int y) Param)
    {
        Mat distImg = new();
        Cv2.Blur(sourceImg, distImg, new OpenCvSharp.Size(Param.x, Param.y));
        return distImg;
    }

    // ****************************************
    // Public Function
    // ****************************************
    public Mat Run(Mat sourceImg, (int x, int y) Param = default)
    {
        _originImg = sourceImg;
        if (Param == default)
        {
            Param.x = (int)_scaleX.Get();
            Param.y = (int)_scaleY.Get();
        }
        else
        {
            _scaleX.Set(Param.x);
            _scaleY.Set(Param.y);
        }
        return Blur(_originImg, Param);
    }

    public (int x, int y) GetParam()
    {
        return ((int)_scaleX.Get(), (int)_scaleY.Get());
    }
}