using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Concurrent;
using Slate.Core;
using System;

namespace Slate.FrontEnd.OpenGl
{
    public class FrontEnd : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private readonly ISlate _slate;
        private ConcurrentQueue<Update> _pendingUpdates;
        private Style _style;
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
            _slate.Updates.Subscribe(u => _pendingUpdates.Enqueue(u));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var font = Content.Load<SpriteFont>("FontSmallRegular");
            var texture = Content.Load<Texture>("oneWhitePixel");
            _style = new Style(font, texture);

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

            using(_slate.Lock())
            {
                foreach(var point in _visibleRegion)
                {
                    var cell = _slate.GetCell(point);
                    DrawCell(point, cell);
                }
            }
 
            _spriteBatch.DrawString(_font, "Hello, World!", new Vector2(100, 100), Microsoft.Xna.Framework.Color.Black);
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawCell(Point at, Cell cell)
        {
            _spriteBatch.Draw(_style.Texture, )
        }

        private Microsoft.Xna.Framework.Rectangle ToScreen(Point cell)
        {
            return new Microsoft.Xna.Framework.Rectangle(cell.X * _style.CellWi)
        }

        private bool IsRedrawRequired()
        {
            bool isRedrawRequired = false;
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
        }
    }
}
