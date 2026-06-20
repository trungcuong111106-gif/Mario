using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Mario
{
    public class Player
    {
        private Vector2 _position;
        private Vector2 _velocity;
        private const float MOVE_SPEED = 150f;
        private const float JUMP_FORCE = -300f;
        private const float GRAVITY = 600f;
        private const float MAX_FALL_SPEED = 400f;
        
        private int _width = 32;
        private int _height = 48;
        private bool _isJumping;
        private bool _isGrounded;

        public Vector2 Position => _position;
        public Rectangle BoundingBox => new Rectangle((int)_position.X, (int)_position.Y, _width, _height);

        public Player(Vector2 startPosition)
        {
            _position = startPosition;
            _velocity = Vector2.Zero;
            _isJumping = false;
            _isGrounded = false;
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Handle input
            HandleInput(keyboardState);

            // Apply gravity
            ApplyGravity(deltaTime);

            // Check collisions
            CheckCollisions(tileMap);

            // Update position
            _position += _velocity * deltaTime;

            // Keep player in bounds
            if (_position.X < 0) _position.X = 0;
            if (_position.X + _width > 2560) _position.X = 2560 - _width;
        }

        private void HandleInput(KeyboardState keyboardState)
        {
            // Horizontal movement
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                _velocity.X = -MOVE_SPEED;
            }
            else if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                _velocity.X = MOVE_SPEED;
            }
            else
            {
                _velocity.X = 0;
            }

            // Jump
            if ((keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W)) && _isGrounded)
            {
                _velocity.Y = JUMP_FORCE;
                _isJumping = true;
                _isGrounded = false;
            }
        }

        private void ApplyGravity(float deltaTime)
        {
            if (!_isGrounded)
            {
                _velocity.Y += GRAVITY * deltaTime;
                if (_velocity.Y > MAX_FALL_SPEED)
                    _velocity.Y = MAX_FALL_SPEED;
            }
        }

        private void CheckCollisions(TileMap tileMap)
        {
            _isGrounded = false;

            // Check collision with tiles
            Rectangle playerBounds = BoundingBox;

            // Simple collision detection
            for (int x = (int)_position.X / 32; x < (_position.X + _width) / 32 + 1; x++)
            {
                for (int y = (int)_position.Y / 32; y < (_position.Y + _height) / 32 + 1; y++)
                {
                    if (tileMap.IsSolid(x, y))
                    {
                        Rectangle tileBounds = new Rectangle(x * 32, y * 32, 32, 32);

                        if (playerBounds.Intersects(tileBounds))
                        {
                            // Landing on tile
                            if (_velocity.Y > 0 && _position.Y + _height - _velocity.Y * 0.016f <= tileBounds.Y)
                            {
                                _position.Y = tileBounds.Y - _height;
                                _velocity.Y = 0;
                                _isGrounded = true;
                                _isJumping = false;
                            }
                            // Hit head
                            else if (_velocity.Y < 0 && _position.Y - _velocity.Y * 0.016f >= tileBounds.Y + tileBounds.Height)
                            {
                                _position.Y = tileBounds.Y + tileBounds.Height;
                                _velocity.Y = 0;
                            }
                            // Collision from sides
                            else if (_velocity.X > 0)
                            {
                                _position.X = tileBounds.X - _width;
                            }
                            else if (_velocity.X < 0)
                            {
                                _position.X = tileBounds.X + tileBounds.Width;
                            }
                        }
                    }
                }
            }

            // Check if fell off world
            if (_position.Y > 600)
            {
                _position = new Vector2(100, 300);
                _velocity = Vector2.Zero;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw player as red rectangle (placeholder)
            spriteBatch.Draw(
                texture: GetWhiteTexture(spriteBatch),
                destinationRectangle: BoundingBox,
                color: Color.Red
            );
        }

        private Texture2D GetWhiteTexture(SpriteBatch spriteBatch)
        {
            Texture2D whiteTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            whiteTexture.SetData(new[] { Color.White });
            return whiteTexture;
        }
    }
}
