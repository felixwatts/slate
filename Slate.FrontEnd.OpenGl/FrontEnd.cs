using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Concurrent;
using Slate.Core;
using System;
using System.Linq;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System.Collections.Generic;

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
        private Keys[] _lastPressedKeys = new Keys[0];
        private Slate.Core.MouseButton[] _lastPressedMouseButtons = new Slate.Core.MouseButton[0];
        private int[] _columnStartPx;

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

            var fontRegular = Content.Load<BitmapFont>("verdana-13px");
            var fontBold = Content.Load<BitmapFont>("verdana-13px-bold");
            var texture = Content.Load<Texture2D>("oneWhitePixel");
            _style = new Style(fontRegular, fontBold, texture);

            _slate.Updates.Subscribe(u => _pendingUpdates.Enqueue(u));   

            _numRowsInView = (int) _graphics.GraphicsDevice.Viewport.Height / _style.CellHeight;
            _cellCache = new Cell[_numRowsInView];

            var maxNumColsInView = (int) _graphics.GraphicsDevice.Viewport.Width / _style.MinCellWidth;
            _columnStartPx = new int[maxNumColsInView];
        }        

        protected override void Update(GameTime gameTime)
        {            
            ProcessKeyboard();
            ProcessMouse();
            
            base.Update(gameTime);
        }        

        private void ProcessMouse()
        {
            var state = Mouse.GetState();

            var currentPressedMouseButtons = new List<Slate.Core.MouseButton>(3);
            if(state.LeftButton == ButtonState.Pressed) currentPressedMouseButtons.Add(Slate.Core.MouseButton.Left);
            if(state.MiddleButton == ButtonState.Pressed) currentPressedMouseButtons.Add(Slate.Core.MouseButton.Middle);
            if(state.RightButton == ButtonState.Pressed) currentPressedMouseButtons.Add(Slate.Core.MouseButton.Right);

            var downButtons = currentPressedMouseButtons.Except(_lastPressedMouseButtons);
            var upButtons = _lastPressedMouseButtons.Except(currentPressedMouseButtons);

            if(!(upButtons.Any() || downButtons.Any()))
                return;

            var cell = ToCell(state.Position);
            var modifierKeys = GetModifierKeys(Keyboard.GetState());

            using(_slate.Lock())
            {
                foreach(var button in upButtons)
                {
                    _slate.MouseUp(cell, button, modifierKeys);
                }

                foreach(var button in downButtons)
                {
                    _slate.MouseDown(cell, button, modifierKeys);
                }
            }

            _lastPressedMouseButtons = currentPressedMouseButtons.ToArray();;
        }

        private void ProcessKeyboard()
        {
            KeyboardState state = Keyboard.GetState();
            var currentPressedKeys = state.GetPressedKeys();
            var keysDown = currentPressedKeys.Except(_lastPressedKeys);
            var keysUp = _lastPressedKeys.Except(currentPressedKeys);
            _lastPressedKeys = currentPressedKeys; 

            if(keysUp.Any() || keysDown.Any())
            {
                var modiferKeys = GetModifierKeys(state);

                using(_slate.Lock())
                {
                    foreach(var key in keysUp)
                    {
                        _slate.KeyUp(Translate(key), modiferKeys);
                    }

                    foreach(var key in keysDown)
                    {
                        _slate.KeyDown(Translate(key), modiferKeys);
                    }
                }
            }
        }

        private Slate.Core.Point ToCell(Microsoft.Xna.Framework.Point screenPoint)
        {
            var x = 0;
            for(;x < _columnStartPx.Length-1; x++)
            {
                if(_columnStartPx[x+1] > screenPoint.X)
                    break;
            }

            var y = screenPoint.Y / _style.CellHeight;

            return new Slate.Core.Point(x, y);
        }

        private Slate.Core.Key Translate(Keys key) => (Slate.Core.Key)key; 

        private Slate.Core.ModifierKeys GetModifierKeys(KeyboardState state)
        {
            var result = Slate.Core.ModifierKeys.None;
            if(state.IsKeyDown(Keys.LeftControl) || state.IsKeyDown(Keys.RightControl))
                result |= Slate.Core.ModifierKeys.Ctrl;
            if(state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift))
                result |= Slate.Core.ModifierKeys.Shift;
            if(state.IsKeyDown(Keys.LeftAlt) || state.IsKeyDown(Keys.RightAlt))
                result |= Slate.Core.ModifierKeys.Alt;

            return result;
        }       

        protected override void Draw(GameTime gameTime)
        {
            if(!IsRedrawRequired()) return;

            _spriteBatch.Begin();

            int column = 0; int columnStartPx = 0;
            
            while(columnStartPx < _graphics.GraphicsDevice.Viewport.Width)
            {
                _columnStartPx[column] = columnStartPx;

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

            var newVisibleRegion = new Region(Slate.Core.Point.Zero, new Slate.Core.Point(column, _numRowsInView));
            if(!newVisibleRegion.Equals(_visibleRegion))
            {
                _visibleRegion = newVisibleRegion;
                using(_slate.Lock())
                {
                    _slate.SetVisibleRegions(new[]{_visibleRegion});
                }
            }                   
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private int MeasureCellWidth(Cell cell)
        {
            if(cell == null) return _style.MinCellWidth;
            var font = cell.IsTextBold ?_style.FontBold : _style.FontRegular;
            return (int)(font.MeasureString(cell.Text).Width + 2 * _style.CellPaddingX + 1);
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

        private Vector2 GetTextPosition(int xPx, int yPx, int cellWidthPx, TextAlignment alignment, string text, BitmapFont font)
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
                    x = (int)(xPx + cellWidthPx - size.Width - _style.CellPaddingX);
                    break;
                }
                case TextAlignment.Center:
                {
                    var size = font.MeasureString(text);
                    x = (int)(xPx + ((cellWidthPx - size.Width) / 2));
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
