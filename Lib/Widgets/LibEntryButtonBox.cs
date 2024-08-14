using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using System;

class LibEntryButtonBox : Box
{
    [UI] private Label _lbl = null;
    [UI] private Entry _entry = null;
    [UI] private Button _btn = null;

    public delegate void eventHandler(string label);
    public eventHandler OnCliced = null;

    public LibEntryButtonBox() : this(new Builder("LibEntryButtonBox.glade")) { }

    private LibEntryButtonBox(Builder builder) : base(builder.GetRawOwnedObject("vbox"))
    {
        builder.Autoconnect(this);
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
    }

    private void InitEvents()
    {
        _btn.Clicked += OnClick;
    }

    // **************************************************
    // events function
    // **************************************************
    private void OnClick(object sender, EventArgs e)
    {
        OnCliced?.Invoke(_lbl.Text);
    }

    // **************************************************
    // private function
    // **************************************************

    // **************************************************
    // public function
    // **************************************************
    public void Configure(string label = "", string btnLabel = "")
    {
        if (label != "") { _lbl.Text = label; }
        if (btnLabel != "") { _btn.Label = btnLabel; }
    }

    public void SetText(string text) { _entry.Text = text; }

    public string Get() { return _entry.Text; }
}

