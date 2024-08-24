using System;
using GLib;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

class LibScaleBox : Box
{
    [UI] private Button _btn_up = null;
    [UI] private Button _btn_down = null;
    [UI] private Label _lbl = null;
    [UI] private Scale _scale = null;
    [UI] private Entry _scaleValue = null;
    private int _decimalPlace = 0;
    private double memVal = 0;
    public delegate void eventHandler(double value);
    public eventHandler OnChangedScale = null;

    public LibScaleBox() : this(new Builder("LibScaleBox.glade")) { }

    private LibScaleBox(Builder builder) : base(builder.GetRawOwnedObject("vbox"))
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
        _scale.StyleContext.AddClass("box-with-border");
        _btn_up.StyleContext.AddClass("box-with-border");
        _btn_down.StyleContext.AddClass("box-with-border");
        _scaleValue.StyleContext.AddClass("box-with-border");
        this.StyleContext.AddClass("box-with-border");
    }

    private void InitEvents()
    {
        _scale.ChangeValue += OnChangeScale;
        _btn_up.Clicked += OnClickUp;
        _btn_down.Clicked += OnClickDown;
        _scaleValue.KeyPressEvent += OnChangeValue;
    }

    // **************************************************
    // events function
    // **************************************************
    [ConnectBefore]
    private void OnChangeValue(object o, KeyPressEventArgs args)
    {
        if (args.Event.Key != Gdk.Key.Return) { return; }
        try
        {
            double val = double.Parse(_scaleValue.Text);
            if (_decimalPlace == 0)
            {
                val = (int)val;
                _scaleValue.Text = val.ToString();
            }
            Adjustment adjustment = _scale.Adjustment;
            if (adjustment.Lower > val) { val = adjustment.Lower; _scaleValue.Text = val.ToString(); }
            else if (adjustment.Upper < val) { val = adjustment.Upper; _scaleValue.Text = val.ToString(); }
            _scale.Value = val;

            if (OnChangedScale != null && memVal != val)
            {
                OnChangedScale(val);
                memVal = val;
            }
        }
        catch
        {
            _scaleValue.Text = ((int)_scale.Value).ToString();
        }
    }

    private void OnChangeScale(object o, ChangeValueArgs args)
    {
        double val = Math.Round(_scale.Value, _decimalPlace, MidpointRounding.AwayFromZero);
        _scaleValue.Text = val.ToString();

        if (OnChangedScale != null && memVal != val)
        {
            OnChangedScale(val);
            memVal = val;
        }
    }

    private void OnClickDown(object sender, EventArgs e)
    {
        _scale.Value -= 1 / Math.Pow(10, _decimalPlace);
        OnChangeScale(null, null);
    }

    private void OnClickUp(object sender, EventArgs e)
    {
        _scale.Value += 1 / Math.Pow(10, _decimalPlace);
        OnChangeScale(null, null);
    }


    // **************************************************
    // private function
    // **************************************************

    // **************************************************
    // public function
    // **************************************************
    public void Configure(string label = "", int from = 0, int to = 255, int decimal_place = 0)
    {
        _lbl.Text = label;
        _scale.SetRange(from, to);
        _decimalPlace = decimal_place;
        _scaleValue.Text = from.ToString();
    }

    public double Get()
    {
        return double.Parse(_scaleValue.Text);
    }

    public void Set(double val)
    {
        _scale.Value = val;
        OnChangeScale(null, null);
    }
}

