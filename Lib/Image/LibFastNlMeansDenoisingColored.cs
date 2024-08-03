
using Gtk;
using OpenCvSharp;

class LibFastNlMeansDenoisingColored : Box
{
    private LibScaleBox _scaleH = new();
    private LibScaleBox _scaleHColor = new();
    private LibScaleBox _scaleTemplateWindowSize = new();
    private LibScaleBox _scaleSearchWindowSize = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibFastNlMeansDenoisingColored()
    {
        InitGui();
        InitEvents();
    }

    // ****************************************
    // Init Function
    // ****************************************
    private void InitGui()
    {
        _scaleH.Configure(label: "h", from: 1, to: 10, decimal_place: 1);
        _scaleHColor.Configure(label: "hColor", from: 1, to: 10, decimal_place: 1);
        _scaleTemplateWindowSize.Configure(label: "TemplateWindowSize", from: 0, to: 30, decimal_place: 0);
        _scaleSearchWindowSize.Configure(label: "SearchWindowSize", from: 0, to: 30, decimal_place: 0);
        this.Add(_scaleH);
        this.Add(_scaleHColor);
        this.Add(_scaleTemplateWindowSize);
        this.Add(_scaleSearchWindowSize);
        this.Orientation = Orientation.Vertical;
        this.Visible = true;
    }

    private void InitEvents()
    {
        _scaleH.OnChangedScale += OnChangeScale;
        _scaleHColor.OnChangedScale += OnChangeScale;
        _scaleTemplateWindowSize.OnChangedScale += OnChangeScale;
        _scaleSearchWindowSize.OnChangedScale += OnChangeScale;
    }

    // ****************************************
    // Events Function
    // ****************************************
    private void OnChangeScale(double value)
    {
        if (_originImg == null) { return; }
        (float h, float hColor, int templateWindowSize, int searchWindowSize) Param = GetParam();
        Mat img = ImageProcessing(_originImg, Param);
        OnChangedImage?.Invoke(img);
    }

    // ****************************************
    // Private Function
    // ****************************************
    private Mat ImageProcessing(Mat sourceImg, (float h, float hColor, int templateWindowSize, int searchWindowSize) Param)
    {
        Mat dstImg = new();
        Cv2.FastNlMeansDenoisingColored(sourceImg,
                                        dstImg,
                                        Param.h,
                                        Param.hColor,
                                        Param.templateWindowSize,
                                        Param.searchWindowSize);
        return dstImg;
    }

    private void SetParam((float h, float hColor, int templateWindowSize, int searchWindowSize) Param)
    {
        _scaleH.Set(Param.h);
        _scaleHColor.Set(Param.hColor);
        _scaleTemplateWindowSize.Set(Param.templateWindowSize);
        _scaleSearchWindowSize.Set(Param.searchWindowSize);
    }

    // ****************************************
    // Public Function
    // ****************************************
    public (float h, float hColor, int templateWindowSize, int searchWindowSize) GetParam()
    {
        return ((float)_scaleH.Get(), (float)_scaleHColor.Get(), (int)_scaleTemplateWindowSize.Get(), (int)_scaleSearchWindowSize.Get());
    }

    public Mat Run(Mat sourceImg, (float h, float hColor, int templateWindowSize, int searchWindowSize) Param = default)
    {
        _originImg = sourceImg;
        if (Param == default) { Param = GetParam(); }
        else { SetParam(Param); }
        return ImageProcessing(_originImg, Param);
    }
}