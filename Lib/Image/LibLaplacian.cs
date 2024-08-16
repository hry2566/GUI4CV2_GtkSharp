
using Gtk;
using OpenCvSharp;

class LibLaplacian : Box
{
    private LibScaleBox _scaleKernel = new();
    private Mat _originImg = null;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibLaplacian()
    {
        InitGui();
        InitEvents();
    }

    // ****************************************
    // Init Function
    // ****************************************
    private void InitGui()
    {
        _scaleKernel.Configure(label: "Kernel", from: 1, to: 10, decimal_place: 0);
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
        Mat img = ImageProcessing(_originImg, GetParam());
        OnChangedImage?.Invoke(img);
    }

    // ****************************************
    // Private Function
    // ****************************************
    private Mat ImageProcessing(Mat sourceImg, int Param)
    {
        Mat dstImg = new();
        if (Param % 2 == 0) { Param += 1; }
        Cv2.CvtColor(sourceImg, dstImg, ColorConversionCodes.BGR2GRAY);
        Cv2.Laplacian(dstImg, dstImg, MatType.CV_8UC1, Param);
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