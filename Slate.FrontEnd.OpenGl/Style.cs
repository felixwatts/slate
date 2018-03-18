using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Slate.FrontEnd.OpenGl
{
    public class Style
    {
        public SpriteFont FontRegular { get; }
        public SpriteFont FontBold { get; }
        public Texture2D Texture { get; }
        
        public int MinCellWidth { get; }
        public int MaxCellWidth { get; }
        public int CellHeight { get; }
        public int CellPaddingX { get; }
        public int CellPaddingY { get; }
        public int CellTextYOffset { get; }
        public Microsoft.Xna.Framework.Color GridLines { get; }

        public Style(SpriteFont fontRegular, SpriteFont fontBold, Texture2D texture)
        {
            FontRegular = fontRegular;
            FontBold = fontBold;
            Texture = texture;

            CellPaddingX = 1;
            CellPaddingY = 1;            

            CellHeight = (int)(FontBold.MeasureString("A").Y + (2 * CellPaddingY) + 1);
            MinCellWidth = (int)FontBold.MeasureString(new string('A', 1)).X;
            MaxCellWidth = (int)FontBold.MeasureString(new string('A', 100)).X;

            GridLines = new Microsoft.Xna.Framework.Color(37, 37, 38, 255);
        }

        public virtual Microsoft.Xna.Framework.Color GetColor(uint color)
        {
            return new Microsoft.Xna.Framework.Color(color);
        }
    }
}