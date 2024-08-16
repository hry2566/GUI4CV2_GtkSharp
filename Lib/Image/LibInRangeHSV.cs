
using Gtk;
using OpenCvSharp;

class LibInRangeHSV : Box
{
    private LibScaleBox _scaleHMin = new();
    private LibScaleBox _scaleHMax = new();
    private LibScaleBox _scaleSMin = new();
    private LibScaleBox _scaleSMax = new();
    private LibScaleBox _scaleVMin = new();
    private LibScaleBox _scaleVMax = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibInRangeHSV()
    {
        InitGui();
        InitEvents();
    }

    // ****************************************
    // Init Function
    // ****************************************
    private void InitGui()
    {
        _scaleHMin.Configure(label: "H Min", from: 0, to: 359, decimal_place: 0);
        _scaleHMax.Configure(label: "H Max", from: 0, to: 359, decimal_place: 0);
        _scaleSMin.Configure(label: "S Min", from: 0, to: 255, decimal_place: 0);
        _scaleSMax.Configure(label: "S Max", from: 0, to: 255, decimal_place: 0);
        _scaleVMin.Configure(label: "V Min", from: 0, to: 255, decimal_place: 0);
        _scaleVMax.Configure(label: "V Max", from: 0, to: 255, decimal_place: 0);
        _scaleHMax.Set(359);
        _scaleSMax.Set(255);
        _scaleVMax.Set(255);
        this.Add(_scaleHMin);
        this.Add(_scaleHMax);
        this.Add(_scaleSMin);
        this.Add(_scaleSMax);
        this.Add(_scaleVMin);
        this.Add(_scaleVMax);
        this.Orientation = Orientation.Vertical;
        this.Visible = true;
    }

    private void InitEvents()
    {
        _scaleHMin.OnChangedScale += OnChangeScale;
        _scaleHMax.OnChangedScale += OnChangeScale;
        _scaleSMin.OnChangedScale += OnChangeScale;
        _scaleSMax.OnChangedScale += OnChangeScale;
        _scaleVMin.OnChangedScale += OnChangeScale;
        _scaleVMax.OnChangedScale += OnChangeScale;
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
    private Mat ImageProcessing(Mat sourceImg, (int HMin, int HMax, int SMin, int SMax, int VMin, int VMax) Param)
    {
        Mat dstImg = new();
        Cv2.CvtColor(sourceImg, dstImg, ColorConversionCodes.BGR2HSV_FULL);
        Scalar s_min = new Scalar(Param.HMin, Param.SMin, Param.VMin);
        Scalar s_max = new Scalar(Param.HMax, Param.SMax, Param.VMax);

        if (Param.HMin > Param.HMax)
        {
            Mat mask1 = new();
            Mat mask2 = new();
            s_min = new Scalar(Param.HMin, Param.SMin, Param.VMin);
            s_max = new Scalar(255, Param.SMax, Param.VMax);
            Cv2.InRange(sourceImg, s_min, s_max, mask1);
            s_min = new Scalar(0, Param.SMin, Param.VMin);
            s_max = new Scalar(Param.HMax, Param.SMax, Param.VMax);
            Cv2.InRange(sourceImg, s_min, s_max, mask2);
            dstImg = mask1 + mask2;
        }
        else { Cv2.InRange(sourceImg, s_min, s_max, dstImg); }
        return dstImg;
    }

    private void SetParam((int HMin, int HMax, int SMin, int SMax, int VMin, int VMax) Param)
    {
        _scaleHMin.Set(Param.HMin);
        _scaleHMax.Set(Param.HMax);
        _scaleSMin.Set(Param.SMin);
        _scaleSMax.Set(Param.SMax);
        _scaleVMin.Set(Param.VMin);
        _scaleVMax.Set(Param.VMax);
    }

    // ****************************************
    // Public Function
    // ****************************************
    public (int HMin, int HMax, int SMin, int SMax, int VMin, int VMax) GetParam()
    {
        return ((int)_scaleHMin.Get(), (int)_scaleHMax.Get(), (int)_scaleSMin.Get(), (int)_scaleSMax.Get(), (int)_scaleVMin.Get(), (int)_scaleVMax.Get());
    }

    public Mat Run(Mat sourceImg, (int HMin, int HMax, int SMin, int SMax, int VMin, int VMax) Param = default)
    {
        _originImg = sourceImg;
        if (Param == default) { Param = GetParam(); }
        else { SetParam(Param); }
        return ImageProcessing(_originImg, Param);
    }
}