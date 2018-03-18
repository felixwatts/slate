namespace Slate.Core
{

    public class Cell
    {
        public string Text { get; }
        public uint Color { get; }    
        TextAlignment Alignment { get; }   

        public Cell(string text, uint color, TextAlignment alignment)
        {
            Text = text;
            Color = color;
            Alignment = alignment;
        } 
    }
}