using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;

namespace Slate.FrontEnd.OpenGl
{
    public class Style
    {
        public BitmapFont FontRegular { get; }
        public BitmapFont FontBold { get; }
        public Texture2D Texture { get; }
        
        public int MinCellWidth { get; }
        public int MaxCellWidth { get; }
        public int CellHeight { get; }
        public int CellPaddingX { get; }
        public int CellPaddingY { get; }
        public int CellTextYOffset { get; }
        public Microsoft.Xna.Framework.Color GridLines { get; }

        public Style(BitmapFont fontRegular, BitmapFont fontBold, Texture2D texture)
        {
            FontRegular = fontRegular;
            FontBold = fontBold;
            Texture = texture;

            CellPaddingX = 1;
            CellPaddingY = 1;            

            CellHeight = (int)(FontBold.MeasureString("A").Height + (2 * CellPaddingY) + 1);
            MinCellWidth = (int)FontBold.MeasureString(new string('A', 1)).Width;
            MaxCellWidth = (int)FontBold.MeasureString(new string('A', 100)).Width;

            GridLines = new Microsoft.Xna.Framework.Color(37, 37, 38, 255);
        }

        public virtual Microsoft.Xna.Framework.Color GetColor(uint color)
        {
            return new Microsoft.Xna.Framework.Color(color);
        }
    }
}