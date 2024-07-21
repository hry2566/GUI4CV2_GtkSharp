using System;
using System.Runtime.InteropServices;
using Gtk;
using OpenCvSharp;

class LibImShow : LibEventBox
{
    private double minImgScale, imgScale, startX, startY, imgCursolPosX, imgCursolPosY;
    private int imgWidth, imgHeight, frameWidth, frameHeight, trimX, trimY, trimWidth, trimHeight;
    private bool dragFlag = false, ctrlKeyFlag = false;
    private Mat memMat = new();

    private struct POINT
    {
        public int x;

        public int y;
    }
    private SubImShow _sub = new();
    public LibImShow()
    {
        InitGui();
        InitEvents();
    }

    // **************************************************
    // init function
    // **************************************************
    private void InitGui()
    {
        CssProvider cssProvider = new CssProvider();
        cssProvider.LoadFromPath("./Lib/Widgets/css/style.css");
        StyleContext.AddProviderForScreen(Gdk.Screen.Default, cssProvider, uint.MaxValue);
        this.StyleContext.AddClass("box-with-border");
        this.MarginStart = 4;
        this.MarginEnd = 4;
        this.MarginTop = 4;
        this.MarginBottom = 4;
        this.Add(_sub);
    }

    private void InitEvents()
    {
        _sub.eventImShow.AddEvents((int)(Gdk.EventMask.PointerMotionMask |
                                         Gdk.EventMask.ScrollMask |
                                         Gdk.EventMask.KeyPressMask |
                                         Gdk.EventMask.KeyReleaseMask));

        _sub.eventImShow.ButtonPressEvent += OnMouseDown;
        _sub.eventImShow.ButtonReleaseEvent += OnMouseUp;
        _sub.eventImShow.MotionNotifyEvent += OnMouseMove;
        _sub.eventImShow.ScrollEvent += OnMouseScroll;
        _sub.eventImShow.KeyPressEvent += OnKeyPressEvent;
        _sub.eventImShow.KeyReleaseEvent += OnKeyReleaseEvent;
        this.OnChangedSize += OnResize;
    }

    // **************************************************
    // events function
    // **************************************************
    // [ConnectBefore]
    private void OnKeyReleaseEvent(object o, KeyReleaseEventArgs args)
    {
        if (args.Event.Key.ToString() == "Control_L") { ctrlKeyFlag = false; }
    }


    private void OnKeyPressEvent(object o, KeyPressEventArgs args)
    {
        if (args.Event.Key.ToString() == "Control_L") { ctrlKeyFlag = true; }
    }

    private void OnMouseScroll(object o, ScrollEventArgs args)
    {
        if (!ctrlKeyFlag) { return; }

        double step = 0.02;
        GetImgCursolPos(args.Event.X, args.Event.Y);
        double memPosX = imgCursolPosX;
        double memPosY = imgCursolPosY;
        imgScale *= (args.Event.Direction == Gdk.ScrollDirection.Up) ? 1 + step : 1 - step;
        imgScale = Math.Max(imgScale, minImgScale);
        imgScale = Math.Min(imgScale, 200);
        GetImgCursolPos(args.Event.X, args.Event.Y);

        trimX += (int)(memPosX - imgCursolPosX);
        trimY += (int)(memPosY - imgCursolPosY);
        trimX = Math.Max(trimX, 0);
        trimY = Math.Max(trimY, 0);

        ImShow(memMat);
        args.RetVal = true;
    }

    private void OnMouseMove(object o, MotionNotifyEventArgs args)
    {
        _sub.eventImShow.IsFocus = true;
        GetImgCursolPos(args.Event.X, args.Event.Y);
        POINT pos = GetImgPos();
        _sub.lblImShow.Text = $"x={pos.x.ToString()} y={pos.y.ToString()}";

        if (dragFlag && ctrlKeyFlag)
        {
            double mouseX = args.Event.X / imgScale;
            double mouseY = args.Event.Y / imgScale;

            trimX = Math.Max((int)(startX - mouseX), 0);
            trimY = Math.Max((int)(startY - mouseY), 0);
            ImShow(memMat);
        }
    }

    private void OnMouseUp(object o, ButtonReleaseEventArgs args)
    {
        dragFlag = false;
    }

    private void OnMouseDown(object o, ButtonPressEventArgs args)
    {
        if (args.Event.Button == 1)
        {
            dragFlag = true;
            startX = args.Event.X / imgScale + trimX;
            startY = args.Event.Y / imgScale + trimY;
        }
        else if (args.Event.Button == 3 && ctrlKeyFlag)
        {
            ResetView(memMat);
            ImShow(memMat);
        }
    }


    private void OnResize(int width, int height)
    {
        if (memMat.Size() == new Size()) { return; }
        imgWidth = memMat.Width;
        imgHeight = memMat.Height;
        frameWidth = trimWidth = width;
        frameHeight = trimHeight = height;

        double scale = Math.Min((double)frameWidth / imgWidth, (double)frameHeight / imgHeight);
        minImgScale = scale;
        imgScale = Math.Max(minImgScale, imgScale);
        ImShow(memMat);
    }

    // **************************************************
    // private function
    // **************************************************
    private POINT GetImgPos()
    {
        POINT pos = new();
        int offsetX = (int)((frameWidth - imgWidth * imgScale) / imgScale / 2);
        int offsetY = (int)((frameHeight - imgHeight * imgScale) / imgScale / 2);
        if (offsetX < 0) { offsetX = 0; }
        if (offsetY < 0) { offsetY = 0; }
        pos.x = (int)(imgCursolPosX - offsetX);
        pos.y = (int)(imgCursolPosY - offsetY);
        if (pos.x < 0) { pos.x = 0; }
        if (pos.x > imgWidth) { pos.x = imgWidth; }
        if (pos.y < 0) { pos.y = 0; }
        if (pos.y > imgHeight) { pos.y = imgHeight; }
        return pos;
    }

    private void GetImgCursolPos(double x, double y)
    {
        imgCursolPosX = (int)(x / imgScale + trimX);
        imgCursolPosY = (int)(y / imgScale + trimY);
    }

    private Mat GetTrimImage(Mat mat)
    {
        trimWidth = (int)(frameWidth / imgScale);
        trimHeight = (int)(frameHeight / imgScale);

        if (mat.Width < trimX + trimWidth)
        {
            trimX = Math.Max(0, memMat.Width - trimWidth);
            trimWidth = Math.Min(memMat.Width, trimWidth);
        }
        if (memMat.Height < trimY + trimHeight)
        {
            trimY = Math.Max(0, memMat.Height - trimHeight);
            trimHeight = Math.Min(memMat.Height, trimHeight);
        }

        mat = mat.Clone(new Rect(trimX, trimY, trimWidth, trimHeight));
        return mat;
    }

    private Mat GetScaleImage(Mat mat)
    {
        Cv2.Resize(mat, mat, new Size(mat.Width * imgScale, mat.Height * imgScale), 0, 0, InterpolationFlags.Linear);
        return mat;
    }

    private static Gdk.Pixbuf MatToPixbuf(Mat mat)
    {
        using (Mat convertedMat = new())
        {
            Cv2.CvtColor(mat, convertedMat, ColorConversionCodes.BGR2RGB);
            byte[] rawData = new byte[convertedMat.Total() * convertedMat.ElemSize()];
            Marshal.Copy(convertedMat.Data, rawData, 0, rawData.Length);
            return new Gdk.Pixbuf(rawData, Gdk.Colorspace.Rgb, false, 8, convertedMat.Width, convertedMat.Height, convertedMat.Width * 3);
        }
    }

    // **************************************************
    // public function
    // **************************************************
    public void ImShow(Gdk.Pixbuf buf) => _sub.imageImShow.Pixbuf = buf;

    public void ImShow(Mat mat)
    {
        if (imgScale == 0)
        {
            ResetView(mat);
        }
        memMat = mat.Clone();
        mat = GetTrimImage(mat);
        mat = GetScaleImage(mat);

        if (double.IsNaN(imgScale))
        {
            ResetView(mat);
            mat = GetScaleImage(mat);
        }

        using (Gdk.Pixbuf pixbuf = MatToPixbuf(mat)) { _sub.imageImShow.Pixbuf = pixbuf; }
        while (GLib.MainContext.Iteration()) { }
    }

    public void ResetView(Mat mat)
    {
        memMat = mat.Clone();
        imgWidth = mat.Width;
        imgHeight = mat.Height;
        frameWidth = trimWidth = _sub.viewImShow.AllocatedWidth;
        frameHeight = trimHeight = _sub.viewImShow.AllocatedHeight;
        trimX = 0;
        trimY = 0;
        imgScale = 1;

        double scale = Math.Min((double)frameWidth / imgWidth, (double)frameHeight / imgHeight);
        imgScale = minImgScale = scale;
    }

    public Mat GetImage()
    {
        return memMat;
    }
}

