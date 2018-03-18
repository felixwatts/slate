namespace Slate.Core
{

    public class Cell
    {
        public string Text { get; }
        public uint Color { get; }    
        public TextAlignment Alignment { get; }  
        public bool IsTextBold { get; }

        public Cell(string text, uint color, TextAlignment alignment, bool isTextBold = false)
        {
            Text = text;
            Color = color;
            Alignment = alignment;
            IsTextBold = isTextBold;
        } 
    }
}