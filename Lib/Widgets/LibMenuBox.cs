using System.Collections.Generic;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

class LibMenuBox : Box
{
    [UI] private MenuBar _menuBar = null;
    private SubColumMenu _columMenu = null;
    public delegate void eventHandler(string label);
    public eventHandler OnMenuClicked = null;
    public LibMenuBox() : this(new Builder("LibMenuBox.glade")) { }

    private LibMenuBox(Builder builder) : base(builder.GetRawOwnedObject("vbox"))
    {
        builder.Autoconnect(this);
    }

    private void OnClick(string label)
    {
        OnMenuClicked?.Invoke(label);
    }

    // **************************************************
    // public function
    // **************************************************
    public void AddMenu(string label, List<string> subItem)
    {
        _columMenu = new(label, subItem);
        _columMenu.UseUnderline = true;
        _columMenu.OnClicked += OnClick;
        _menuBar.Add(_columMenu);
    }
}

