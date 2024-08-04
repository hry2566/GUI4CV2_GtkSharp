using System;
using GLib;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

class Lib3CheckButton : Box
{
    [UI] private CheckButton _check1 = null;
    [UI] private CheckButton _check2 = null;
    [UI] private CheckButton _check3 = null;
    [UI] private Label _lbl = null;
    public delegate void eventHandler(bool[] check);
    public eventHandler OnChangedCheck;

    public Lib3CheckButton() : this(new Builder("Lib3CheckButton.glade")) { }

    private Lib3CheckButton(Builder builder) : base(builder.GetRawOwnedObject("vbox"))
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
        _check1.Clicked += OnClick;
        _check2.Clicked += OnClick;
        _check3.Clicked += OnClick;
    }

    // **************************************************
    // events function
    // **************************************************
    private void OnClick(object sender, EventArgs e)
    {
        if (OnChangedCheck != null)
        {
            bool[] check = Get();
            OnChangedCheck?.Invoke(check);
        }
    }

    // **************************************************
    // private function
    // **************************************************

    // **************************************************
    // public function
    // **************************************************
    public void Configure(string label = "", string[] checkLbl = null)
    {
        _lbl.Text = label;
        if (checkLbl != null)
        {
            _check1.Label = checkLbl[0];
            _check2.Label = checkLbl[1];
            _check3.Label = checkLbl[2];
        }
    }

    public bool[] Get()
    {
        return [_check1.Active, _check2.Active, _check3.Active];
    }

    public void Set(bool[] check = null)
    {
        bool isDefault = check == null;
        _check1.Active = !isDefault && check[0];
        _check2.Active = !isDefault && check[1];
        _check3.Active = !isDefault && check[2];
        OnClick(null, null);
    }
}

