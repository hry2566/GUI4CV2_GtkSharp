using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

class SubImShow : Box
{
    [UI] public Image imageImShow = null;
    [UI] public EventBox eventImShow = null;
    [UI] public Label lblImShow = null;
    [UI] public Viewport viewImShow = null;

    public SubImShow() : this(new Builder("SubImShow.glade")) { }

    private SubImShow(Builder builder) : base(builder.GetRawOwnedObject("_boxImShow"))
    {
        builder.Autoconnect(this);
    }
}

