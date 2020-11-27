using System;

public class ExplainTextController
{
    public TextModel Model { get; private set; }
    public TextView View_Text { get; private set; }
    public TextIndicatorView View_Indicator { get; private set; }

    public ExplainTextController(TextModel model, TextView view_Text, TextIndicatorView View_Indicator)
    {
        this.Model = model;
        this.View_Text = view_Text;
        this.View_Indicator = View_Indicator;

        this.Model.OnTextChanged += OnTextChanged;
    }

    private void OnTextChanged(string text)
    {
        this.View_Text.SetText(text);
        this.View_Indicator.SetIndicator(!string.IsNullOrEmpty(text));
    }

    public void SetText(string text)
    {
        this.Model.Text = text;
    }
}
