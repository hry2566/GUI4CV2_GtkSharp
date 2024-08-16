
using Gtk;
using OpenCvSharp;

class LibRotate : Box
{
    private LibScaleBox _scaleAngle = new();
    private LibScaleBox _scaleScale = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibRotate()
    {
        InitGui();
        InitEvents();
    }

    // ****************************************
    // Init Function
    // ****************************************
    private void InitGui()
    {
        _scaleAngle.Configure(label: "kernel X", from: -180, to: 180, decimal_place: 2);
        _scaleScale.Configure(label: "kernel Y", from: 0, to: 2, decimal_place: 2);
        _scaleAngle.Set(0);
        _scaleScale.Set(1);
        this.Add(_scaleAngle);
        this.Add(_scaleScale);
        this.Orientation = Orientation.Vertical;
        this.Visible = true;
    }

    private void InitEvents()
    {
        _scaleAngle.OnChangedScale += OnChangeScale;
        _scaleScale.OnChangedScale += OnChangeScale;
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
    private Mat ImageProcessing(Mat sourceImg, (double angle, double scale) Param)
    {
        Mat dstImg = new();
        int height = sourceImg.Rows;
        int width = sourceImg.Cols;
        Point2f center = new Point2f(width / 2f, height / 2f);
        Mat trans = Cv2.GetRotationMatrix2D(center, Param.angle, Param.scale);
        Cv2.WarpAffine(sourceImg, dstImg, trans, new Size(width, height), InterpolationFlags.Linear, BorderTypes.Constant, new Scalar(0, 0, 0));
        return dstImg;
    }

    private void SetParam((double angle, double scale) Param)
    {
        _scaleAngle.Set(Param.angle);
        _scaleScale.Set(Param.scale);
    }

    // ****************************************
    // Public Function
    // ****************************************
    public (double angle, double scale) GetParam()
    {
        return (_scaleAngle.Get(), _scaleScale.Get());
    }

    public Mat Run(Mat sourceImg, (double angle, double scale) Param = default)
    {
        _originImg = sourceImg;
        if (Param == default) { Param = GetParam(); }
        else { SetParam(Param); }
        return ImageProcessing(_originImg, Param);
    }
}