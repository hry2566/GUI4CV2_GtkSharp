using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using System;

class LibMaxMinEntryBox : Box
{
    [UI] private Label _lbl = null;
    [UI] private Entry _entryMin = null;
    [UI] private Entry _entryMax = null;

    public delegate void eventHandler(int min, int max);
    public eventHandler OnChanged = null;

    public LibMaxMinEntryBox() : this(new Builder("LibMaxMinEntryBox.glade")) { }

    private LibMaxMinEntryBox(Builder builder) : base(builder.GetRawOwnedObject("vbox"))
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
        _entryMin.Changed += OnChange;
        _entryMax.Changed += OnChange;
    }

    // **************************************************
    // events function
    // **************************************************
    private void OnChange(object sender, EventArgs e)
    {
        if (!int.TryParse(_entryMin.Text, out int num) || !int.TryParse(_entryMax.Text, out num)) { return; }
        OnChanged?.Invoke(int.Parse(_entryMin.Text), int.Parse(_entryMax.Text));
    }

    // private void OnClick(object sender, EventArgs e)
    // {
    //     OnCliced?.Invoke(_lbl.Text);
    // }

    // **************************************************
    // private function
    // **************************************************

    // **************************************************
    // public function
    // **************************************************
    public void Configure(string label = "")
    {
        if (label != "") { _lbl.Text = label; }
        // if (btnLabel != "") { _btn.Label = btnLabel; }
    }

    // public void SetText(string text) { _entry.Text = text; }

    public (int min, int max) Get()
    {
        if (!int.TryParse(_entryMin.Text, out int num) || !int.TryParse(_entryMax.Text, out num)) { return default; }
        return (int.Parse(_entryMin.Text), int.Parse(_entryMax.Text));
    }
}

