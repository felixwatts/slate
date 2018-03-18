using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Concurrent;
using Slate.Core;
using System;
using System.Linq;

namespace Slate.FrontEnd.OpenGl
{
    public class FrontEnd : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private readonly ISlate _slate;
        private ConcurrentQueue<Update> _pendingUpdates;
        private Style _style;
        private int _numRowsInView;
        private Cell[] _cellCache;
        private Region _visibleRegion;

        public FrontEnd(ISlate slate)
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _slate = slate;
            _pendingUpdates = new ConcurrentQueue<Update>();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var fontRegular = Content.Load<SpriteFont>("FontSmallRegular");
            var fontBold = Content.Load<SpriteFont>("FontSmallBold");
            var texture = Content.Load<Texture2D>("oneWhitePixel");
            _style = new Style(fontRegular, fontBold, texture);

            _slate.Updates.Subscribe(u => _pendingUpdates.Enqueue(u));   

            _numRowsInView = (int) _graphics.GraphicsDevice.Viewport.Height / _style.CellHeight;
            _cellCache = new Cell[_numRowsInView];

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if(!IsRedrawRequired()) return;

            _spriteBatch.Begin();

            int column = 0; int columnStartPx = 0;
            
            while(columnStartPx < _graphics.GraphicsDevice.Viewport.Width)
            {
                using(_slate.Lock())
                {
                    for(int y = 0; y < _numRowsInView; y++)
                    {
                        _cellCache[y] = _slate.GetCell(new Slate.Core.Point(column, y));
                    }
                }

                var columnWidthPx = _cellCache.Max(MeasureCellWidth);
                
                for(int y = 0; y < _numRowsInView; y++)
                {
                    var cell = _cellCache[y];
                    DrawCell(columnStartPx, y * _style.CellHeight, columnWidthPx, cell);
                }

                column++;
                columnStartPx += columnWidthPx;                
            }

            _visibleRegion = new Region(Slate.Core.Point.Zero, new Slate.Core.Point(column, _numRowsInView));            
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private int MeasureCellWidth(Cell cell)
        {
            if(cell == null) return _style.MinCellWidth;
            var font = cell.IsTextBold ?_style.FontBold : _style.FontRegular;
            return (int)(font.MeasureString(cell.Text).X + 2 * _style.CellPaddingX + 1);
        }    

        private void DrawCell(int xPx, int yPx, int widthPx, Cell cell)
        {
            _spriteBatch.Draw(
                _style.Texture, 
                new Rectangle(xPx, yPx, widthPx, _style.CellHeight), 
                _style.GridLines);

            if(cell == null) return;

            _spriteBatch.Draw(
                _style.Texture, 
                new Rectangle(xPx + 1, yPx + 1, widthPx - 1, _style.CellHeight - 1), 
                _style.GetColor(cell.Color));

            if(string.IsNullOrEmpty(cell.Text)) return;

            var font = cell.IsTextBold ? _style.FontBold : _style.FontRegular;
            var textColor = Slate.Core.Color.ContrastingColor(cell.Color);

            _spriteBatch.DrawString(
                font, 
                cell.Text, 
                GetTextPosition(xPx, yPx, widthPx, cell.Alignment, cell.Text, font),
                _style.GetColor(textColor));
        }

        private Vector2 GetTextPosition(int xPx, int yPx, int cellWidthPx, TextAlignment alignment, string text, SpriteFont font)
        {
            var y = yPx + 1 + _style.CellPaddingY;
            var x = 0;
            switch(alignment)
            {
                case TextAlignment.Left:
                    x = xPx + 1 + _style.CellPaddingX;
                    break;
                case TextAlignment.Right:
                {
                    var size = font.MeasureString(text);
                    x = (int)(xPx + cellWidthPx - size.X - _style.CellPaddingX);
                    break;
                }
                case TextAlignment.Center:
                {
                    var size = font.MeasureString(text);
                    x = (int)(xPx + ((cellWidthPx - size.X) / 2));
                    break;
                }                
            }

            return new Vector2(x, y);
        }

        private bool IsRedrawRequired()
        {
            bool isRedrawRequired = false;

            if(_visibleRegion == null) return true;

            while(_pendingUpdates.TryDequeue(out var update))
            {
                switch(update.Type)
                {
                    case UpdateType.SizeChanged:
                        isRedrawRequired = true;
                        break;
                    case UpdateType.RegionDirty:
                        if(!update.Region.IntersectionWith(_visibleRegion).IsEmpty)
                            isRedrawRequired = true;
                        break;
                }
            }

            return isRedrawRequired;
        }
    }
}
