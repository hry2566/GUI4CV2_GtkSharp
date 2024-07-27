
using Gtk;
using OpenCvSharp;

class LibErode : Box
{
    private LibScaleBox _scaleX = new();
    private LibScaleBox _scaleY = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibErode()
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
        (int x, int y) Param = GetParam();
        Mat img = Erode(_originImg, Param);
        if (OnChangedImage == null) { return; }
        OnChangedImage(img);
    }

    // ****************************************
    // Private Function
    // ****************************************
    private Mat Erode(Mat sourceImg, (int x, int y) Param)
    {
        Mat distImg = new();
        Cv2.Erode(sourceImg, distImg, Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(Param.x, Param.y)));
        return distImg;
    }

    private void SetParam((int x, int y) Param)
    {
        _scaleX.Set(Param.x);
        _scaleY.Set(Param.y);
    }

    // ****************************************
    // Public Function
    // ****************************************
    public (int x, int y) GetParam()
    {
        return ((int)_scaleX.Get(), (int)_scaleY.Get());
    }

    public Mat Run(Mat sourceImg, (int x, int y) Param = default)
    {
        _originImg = sourceImg;
        if (Param == default) { Param = GetParam(); }
        else { SetParam(Param); }
        return Erode(_originImg, Param);
    }
}