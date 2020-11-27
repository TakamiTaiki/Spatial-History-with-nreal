using System;

public class TextModel
{
    public delegate void TextEvent(string text);
    public event TextEvent OnTextChanged;

    private string text;
    public string Text
    {
        get { return this.text; }
        set
        {
            if (!string.Equals(this.text, value))
            {
                this.text = value;
                OnTextChanged?.Invoke(value);
            }
        }
    }
}
