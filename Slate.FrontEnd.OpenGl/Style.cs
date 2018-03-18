namespace Slate.FrontEnd.OpenGl
{
    public class Style
    {
        public SpriteFont Font { get; }
        public Texture Texture { get; }
        public int MaxCellWidth { get; }
        public int CellHeight { get; }
        public int CellPaddingX { get; }
        public int CellPaddingY { get; }

        public Style(SpriteFont font, Texture texture)
        {
            Font = font;
            Texture = texture;

            CellPaddingX = 1;
            CellPaddingY = 1;

            CellHeight = Font.MeasureString("A").Y + 2 * CellPaddingY + 1;
            MaxCellWidth = Font.MeasureString(new string('A', 100));
        }

        public virtual Microsoft.Xna.Framework.Color GetColor(uint color)
        {
            return new Microsoft.Xna.Framework.Color(color);
        }
    }
}