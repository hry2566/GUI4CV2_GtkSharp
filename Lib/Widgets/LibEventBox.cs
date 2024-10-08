
using Gtk;

class LibEventBox : EventBox
{
    private int width, height;
    public delegate void eventHandler2(int width, int height);
    public eventHandler2 OnChangedSize = null;

    public LibEventBox()
    {
        this.Visible = true;
        this.Expand = true;
    }

    protected override void OnSizeAllocated(Gdk.Rectangle allocation)
    {
        if (OnChangedSize == null) { return; }
        base.OnSizeAllocated(allocation);
        if (width != this.AllocatedWidth || height != this.AllocatedHeight)
        {
            OnChangedSize(this.AllocatedWidth, this.AllocatedHeight);
            width = this.AllocatedWidth;
            height = this.AllocatedHeight;
        }
    }
}