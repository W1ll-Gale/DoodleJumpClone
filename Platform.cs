using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DoodleJumpClone
{
    class Platform
    {
        private Texture2D _texture;
        private Rectangle _boundingBox;

        public Rectangle BoundingBox => _boundingBox;

        public Platform(Texture2D texture, Vector2 position, int width, int height)
        {
            _texture = texture;
            _boundingBox = new Rectangle((int)position.X, (int)position.Y, width, height);
        }

        public void Update(GameTime gameTime, Viewport viewport)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _boundingBox, Color.White);
        }
    }
}
