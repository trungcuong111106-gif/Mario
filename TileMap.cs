using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mario
{
    public class TileMap
    {
        private int[,] _tiles;
        private int _width;
        private int _height;
        private const int TILE_SIZE = 32;

        public TileMap(int width, int height)
        {
            _width = width;
            _height = height;
            _tiles = new int[height, width];

            GenerateMap();
        }

        private void GenerateMap()
        {
            // Clear map
            for (int y = 0; y < _height; y++)
                for (int x = 0; x < _width; x++)
                    _tiles[y, x] = 0;

            // Create ground
            for (int x = 0; x < _width; x++)
            {
                _tiles[_height - 1, x] = 1; // Ground
                _tiles[_height - 2, x] = 1; // Ground
            }

            // Add some platforms
            for (int x = 5; x < 8; x++)
                _tiles[10, x] = 1;

            for (int x = 10; x < 14; x++)
                _tiles[8, x] = 1;

            for (int x = 15; x < 19; x++)
                _tiles[6, x] = 1;

            // Add some obstacles
            for (int x = 3; x < 6; x++)
                _tiles[12, x] = 1;

            for (int x = 12; x < 15; x++)
                _tiles[11, x] = 1;
        }

        public bool IsSolid(int x, int y)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
                return false;

            return _tiles[y, x] == 1;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Texture2D whiteTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            whiteTexture.SetData(new[] { Color.White });

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (_tiles[y, x] == 1)
                    {
                        Rectangle tileRect = new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                        spriteBatch.Draw(whiteTexture, tileRect, Color.Green);
                    }
                }
            }

            whiteTexture.Dispose();
        }
    }
}
