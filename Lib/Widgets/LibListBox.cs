using System;
using System.Collections.Generic;
using GLib;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

class LibListBox : Box
{

    [UI] private TreeView _lst = null;
    private Gtk.TreeViewColumn _column = new Gtk.TreeViewColumn();
    private Gtk.CellRendererText _cell = new Gtk.CellRendererText();
    private Gtk.ListStore _store = new Gtk.ListStore(typeof(string));
    private List<string> _strList = new();
    public delegate void eventHandler(double value);
    public eventHandler OnSelected = null;

    public LibListBox() : this(new Builder("LibListBox.glade")) { }

    private LibListBox(Builder builder) : base(builder.GetRawOwnedObject("vbox"))
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
        this.Expand = true;

        _column.PackStart(_cell, true);
        _lst.AppendColumn(_column);
        _column.AddAttribute(_cell, "text", 0);
        _lst.Model = _store;
    }

    private void InitEvents()
    {

    }

    // **************************************************
    // events function
    // **************************************************

    // **************************************************
    // private function
    // **************************************************
    private void InsertItemAt(int index, string item)
    {
        TreeIter iter;
        if (_store.IterNthChild(out iter, index)) { _store.InsertWithValues(index, item); }
        else { Add(item); }
    }
    // **************************************************
    // public function
    // **************************************************

    public void Add(string str)
    {
        _store.AppendValues(str);
    }

    public int GetIndex()
    {
        TreeSelection selection = _lst.Selection;
        if (selection.GetSelected(out TreeIter iter))
        {
            TreePath path = _store.GetPath(iter);
            return path.Indices[0];
        }
        return -1;
    }

    public void SetIndex(int index)
    {
        TreeIter iter;
        if (_store.IterNthChild(out iter, index))
        {
            TreePath path = _store.GetPath(iter);
            _lst.Selection.SelectPath(path);
        }
    }

    public void Del(int index)
    {
        TreeIter iter;
        if (_store.IterNthChild(out iter, index)) { _store.Remove(ref iter); }
    }

    public string GetItem(int index)
    {
        TreeIter iter;
        if (_store.IterNthChild(out iter, index)) { return (string)_store.GetValue(iter, 0); }
        return null;
    }

    public void MoveItem(int index)
    {
        int currentIndex = GetIndex();
        if (currentIndex == -1 || index < 0 || index >= _store.IterNChildren()) { return; }
        string selectedItem = GetItem(currentIndex);
        Del(currentIndex);
        InsertItemAt(index, selectedItem);
        SetIndex(index);
    }
}

