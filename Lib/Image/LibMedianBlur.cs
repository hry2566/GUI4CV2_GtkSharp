
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
        int Param = (int)_scaleKernel.Get();
        if (Param % 2 == 0) { Param += 1; }

        Mat img = GaussianBlur(_originImg, Param);
        if (OnChangedImage == null) { return; }
        OnChangedImage(img);
    }

    // ****************************************
    // Private Function
    // ****************************************
    private Mat GaussianBlur(Mat sourceImg, int Param)
    {
        Mat dstImg = new();
        Cv2.MedianBlur(sourceImg, dstImg, Param);
        return dstImg;
    }

    // ****************************************
    // Public Function
    // ****************************************
    public Mat Run(Mat sourceImg, int Param = default)
    {
        _originImg = sourceImg;

        if (Param == default)
        {
            Param = (int)_scaleKernel.Get();
        }
        else
        {
            if (Param % 2 == 0) { Param += 1; }
            _scaleKernel.Set(Param);
        }
        return GaussianBlur(_originImg, Param);
    }

    public int GetParam()
    {
        return (int)_scaleKernel.Get();
    }
}