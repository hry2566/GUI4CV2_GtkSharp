using System;
using System.Collections.Generic;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

class LibOptionMenuBox : Box
{
    [UI] private Label _lbl = null;
    [UI] private ComboBoxText _comb = null;
    public delegate void eventHandler(string value);
    public eventHandler OnChangedMenu = null;

    public LibOptionMenuBox() : this(new Builder("LibOptionMenuBox.glade")) { }

    private LibOptionMenuBox(Builder builder) : base(builder.GetRawOwnedObject("vbox"))
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
        _comb.Changed += OnChangeMenu;
    }

    // **************************************************
    // events function
    // **************************************************
    private void OnChangeMenu(object sender, EventArgs e)
    {
        string val = _comb.ActiveText;
        if (OnChangedMenu != null)
        {
            OnChangedMenu(val);
        }
    }

    // **************************************************
    // private function
    // **************************************************

    // **************************************************
    // public function
    // **************************************************
    public void Configure(string label = "", List<string> menuList = default)
    {
        if (label != "") { _lbl.Text = label; }
        if (menuList == default) { return; }

        _comb.RemoveAll();
        for (int i = 0; i < menuList.Count; i++)
        {
            _comb.AppendText(menuList[i]);
        }
        SetIndex(0);
    }

    public int GetIndex()
    {
        return _comb.Active;
    }

    public void SetIndex(int index)
    {
        _comb.Active = index;
    }
}

