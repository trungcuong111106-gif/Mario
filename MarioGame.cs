using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Mario
{
    public class MarioGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Player _player;
        private TileMap _tileMap;
        private Camera _camera;
        private SpriteFont _debugFont;

        public MarioGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

#if __ANDROID__
            // Set fullscreen for Android
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.IsFullScreen = true;
#else
            // Set window properties for Windows
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.IsFullScreen = false;
#endif
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            _tileMap = new TileMap(20, 15);
            _player = new Player(new Vector2(100, 300));
            _camera = new Camera(GraphicsDevice.Viewport);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            // Exit on ESC (Windows only)
#if !__ANDROID__
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
#endif

            KeyboardState keyboardState = Keyboard.GetState();
            TouchCollection touchCollection = TouchPanel.GetState();

            _player.Update(gameTime, keyboardState, _tileMap, touchCollection, GraphicsDevice.Viewport);
            _camera.Follow(_player);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
            _tileMap.Draw(_spriteBatch);
            _player.Draw(_spriteBatch);
            _spriteBatch.End();

            // Draw touch zones on Android (for debugging)
#if __ANDROID__
            DrawTouchZones();
#endif

            base.Draw(gameTime);
        }

        private void DrawTouchZones()
        {
            _spriteBatch.Begin();
            
            Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            float leftBoundary = GraphicsDevice.Viewport.Width * 0.25f;
            float rightBoundary = GraphicsDevice.Viewport.Width * 0.75f;

            // Draw left zone (semi-transparent)
            _spriteBatch.Draw(pixel, new Rectangle(0, 0, (int)leftBoundary, GraphicsDevice.Viewport.Height), 
                Color.Green * 0.2f);

            // Draw right zone (semi-transparent)
            _spriteBatch.Draw(pixel, new Rectangle((int)rightBoundary, 0, 
                GraphicsDevice.Viewport.Width - (int)rightBoundary, GraphicsDevice.Viewport.Height), 
                Color.Green * 0.2f);

            // Draw jump zone (top)
            _spriteBatch.Draw(pixel, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, 
                (int)(GraphicsDevice.Viewport.Height * 0.3f)), 
                Color.Yellow * 0.2f);

            _spriteBatch.End();
        }
    }
}
