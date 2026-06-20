using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mario
{
    public class Camera
    {
        private Vector2 _position;
        private Viewport _viewport;

        public Camera(Viewport viewport)
        {
            _viewport = viewport;
            _position = Vector2.Zero;
        }

        public void Follow(Player player)
        {
            // Center camera on player
            _position.X = player.Position.X - _viewport.Width / 2;
            _position.Y = player.Position.Y - _viewport.Height / 2;

            // Clamp camera to level bounds
            if (_position.X < 0) _position.X = 0;
            if (_position.Y < 0) _position.Y = 0;
            if (_position.X > 2560 - _viewport.Width) _position.X = 2560 - _viewport.Width;
            if (_position.Y > 480 - _viewport.Height) _position.Y = 480 - _viewport.Height;
        }

        public Matrix GetViewMatrix()
        {
            return Matrix.CreateTranslation(-_position.X, -_position.Y, 0);
        }
    }
}
