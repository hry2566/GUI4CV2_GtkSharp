
using Gtk;
using OpenCvSharp;

class LibMedianBlur : Box
{
    private LibScaleBox _scaleKernel = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibMedianBlur()
    {
        InitGui();
        InitEvents();
    }

    // ****************************************
    // Init Function
    // ****************************************
    private void InitGui()
    {
        _scaleKernel.Configure(label: "Kernel", from: 1, to: 50, decimal_place: 0);
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
        int Param = GetParam();
        if (Param % 2 == 0) { Param += 1; }

        Mat img = ImageProcessing(_originImg, Param);
        OnChangedImage?.Invoke(img);
    }

    // ****************************************
    // Private Function
    // ****************************************
    private Mat ImageProcessing(Mat sourceImg, int Param)
    {
        Mat dstImg = new();
        Cv2.MedianBlur(sourceImg, dstImg, Param);
        return dstImg;
    }

    private void SetParam(int Param)
    {
        _scaleKernel.Set(Param);
    }

    // ****************************************
    // Public Function
    // ****************************************
    public int GetParam()
    {
        return (int)_scaleKernel.Get();
    }

    public Mat Run(Mat sourceImg, int Param = default)
    {
        _originImg = sourceImg;
        if (Param == default) { Param = GetParam(); }
        else { SetParam(Param); }
        return ImageProcessing(_originImg, Param);
    }
}