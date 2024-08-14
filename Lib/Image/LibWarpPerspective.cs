using System;
using Gtk;
using OpenCvSharp;

class LibWarpPerspective : Box
{
    private LibEntryButtonBox _topLeft = new();
    private LibEntryButtonBox _topRight = new();
    private LibEntryButtonBox _bottomLeft = new();
    private LibEntryButtonBox _bottomRight = new();
    private Button _btnRun = new();
    private Mat _originImg = null;
    private LibImShow _cv2c = null;
    private int _selectMode = -1;
    public delegate void eventHandler(Mat img);
    public eventHandler OnChangedImage = null;

    public LibWarpPerspective(LibImShow libImshow)
    {
        _cv2c = libImshow;
        InitGui();
        InitEvents();
    }

    // ****************************************
    // Init Function
    // ****************************************
    private void InitGui()
    {
        _topLeft.Configure(label: "top left", btnLabel: "select pos");
        _topRight.Configure(label: "top right", btnLabel: "select pos");
        _bottomLeft.Configure(label: "bottom left", btnLabel: "select pos");
        _bottomRight.Configure(label: "bottom right", btnLabel: "select pos");
        InitButton(_btnRun, "run", 4);

        this.Add(_topLeft);
        this.Add(_topRight);
        this.Add(_bottomLeft);
        this.Add(_bottomRight);
        this.Add(_btnRun);
        this.Orientation = Orientation.Vertical;
        this.Visible = true;
    }

    private void InitEvents()
    {
        _cv2c.OnAction += OnAction;
        _topLeft.OnCliced += OnClick;
        _topRight.OnCliced += OnClick;
        _bottomLeft.OnCliced += OnClick;
        _bottomRight.OnCliced += OnClick;
        _btnRun.Clicked += OnClickRun;
    }

    private void InitButton(Button btn, string label, int margin)
    {
        btn.Visible = true;
        btn.Label = label;
        btn.MarginTop = margin;
        btn.MarginBottom = margin;
        btn.MarginStart = margin;
        btn.MarginEnd = margin;
    }

    // ****************************************
    // Events Function
    // ****************************************
    private void OnClickRun(object sender, EventArgs e)
    {
        if (_btnRun.Label == "run")
        {
            _btnRun.Label = "reset view";
            Mat img = ImageProcessing(_originImg, GetParam());
            OnChangedImage?.Invoke(img);
        }
        else
        {
            _btnRun.Label = "run";
            OnChangedImage?.Invoke(_originImg);
        }
    }

    private void OnClick(string label)
    {
        if (label == "top left") { _selectMode = 0; }
        else if (label == "top right") { _selectMode = 1; }
        else if (label == "bottom left") { _selectMode = 2; }
        else if (label == "bottom right") { _selectMode = 3; }
        // System.Console.WriteLine($"{label} {_selectMode}");
    }

    private void OnAction(int mouseEvent, string keyEvent, Point pos)
    {
        // System.Console.WriteLine($"{mouseEvent}, {keyEvent}, {pos.X}, {pos.Y}, {_selectMode}");
        if (mouseEvent == MouseEvent.EVENT_LBUTTONUP && _selectMode != -1)
        {
            SetEntryText(pos);
        }
    }

    // ****************************************
    // Private Function
    // ****************************************
    private void SetEntryText(Point pos)
    {
        LibEntryButtonBox[] obj = [_topLeft, _topRight, _bottomLeft, _bottomRight];
        obj[_selectMode].SetText($"{pos.X}, {pos.Y}");
        _selectMode = -1;
    }
    private Mat ImageProcessing(Mat sourceImg, (Point tLeft, Point tRight, Point bLeft, Point bRight) Param)
    {
        if (Param == default) { return sourceImg; }

        Mat distImg = new();
        int rows = sourceImg.Rows;
        int cols = sourceImg.Cols;
        Point2f[] pts1 = [Param.tLeft, Param.tRight, Param.bLeft, Param.bRight];
        float oWidth = (float)Math.Floor(Param.tRight.DistanceTo(Param.tLeft));
        float oHeight = (float)Math.Floor(Param.bLeft.DistanceTo(Param.tLeft));
        Point2f[] pts2 =
        [
            new Point(Param.tLeft.X, Param.tLeft.Y),
            new Point(Param.tLeft.X + oWidth, Param.tLeft.Y),
            new Point(Param.tLeft.X, Param.tLeft.Y + oHeight),
            new Point(Param.tLeft.X + oWidth, Param.tLeft.Y + oHeight)
        ];

        Mat matrix = Cv2.GetPerspectiveTransform(pts1, pts2);
        Cv2.WarpPerspective(sourceImg, distImg, matrix, new Size(cols, rows));
        return distImg;
    }

    private void SetParam((Point tLeft, Point tRight, Point bLeft, Point bRight) Param)
    {
        _topLeft.SetText($"{Param.tLeft.X}, {Param.tLeft.Y}");
        _topRight.SetText($"{Param.tRight.X}, {Param.tRight.Y}");
        _bottomLeft.SetText($"{Param.bLeft.X}, {Param.bLeft.Y}");
        _bottomRight.SetText($"{Param.bRight.X}, {Param.bRight.Y}");
    }

    private Point String2Point(string text)
    {
        Point pos = new();
        string[] spl = text.Split(",");
        if (spl.Length != 2) { return pos; }
        pos.X = int.Parse(spl[0]);
        pos.Y = int.Parse(spl[1]);
        return pos;
    }

    // ****************************************
    // Public Function
    // ****************************************
    public (Point tLeft, Point tRight, Point bRLeft, Point Right) GetParam()
    {
        return (String2Point(_topLeft.Get()), String2Point(_topRight.Get()), String2Point(_bottomLeft.Get()), String2Point(_bottomRight.Get()));
    }

    public Mat Run(Mat sourceImg, (Point tLeft, Point tRight, Point bLeft, Point bRight) Param = default)
    {
        _originImg = sourceImg;
        if (Param == default) { Param = GetParam(); }
        else
        {
            SetParam(Param);
            OnClickRun(null, null);
        }
        return ImageProcessing(_originImg, Param);
    }
}