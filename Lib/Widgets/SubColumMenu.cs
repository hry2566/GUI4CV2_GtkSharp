using System.Collections.Generic;
using Gtk;

class SubColumMenu : MenuItem
{
    private Menu _menu = new();
    private List<MenuItem> _menuList = new();
    public delegate void eventHandler(string label);
    public eventHandler OnClicked = null;

    public SubColumMenu(string label, List<string> subItem)
    {
        this.Label = label;
        this.Visible = true;
        this.Submenu = _menu;
        foreach (string menu in subItem) { AddItem(menu); }
        foreach (MenuItem subMenu in _menuList) { _menu.Add(subMenu); }
    }

    // **************************************************
    // events function
    // **************************************************
    private void OnClick(object o, ButtonPressEventArgs args)
    {
        MenuItem item = (MenuItem)o;
        OnClicked?.Invoke($"{this.Label}-{item.Label}");
    }

    // **************************************************
    // private function
    // **************************************************
    private void AddItem(string label)
    {
        MenuItem item = new();
        item.Visible = true;
        item.Label = label;
        _menuList.Add(item);
        item.ButtonPressEvent += OnClick;
    }
}

