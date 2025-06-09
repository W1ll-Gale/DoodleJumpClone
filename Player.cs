using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace DoodleJumpClone
{
    class Player
    {
        private Texture2D _texture;
        public Vector2 _position;
        private Vector2 _velocity;
        private bool _isGrounded;
        private SpriteEffects _spriteEffect = SpriteEffects.None;

        private const float Gravity = 1200f;
        private const float JumpForce = -600f;
        private const float MoveSpeed = 350f;
        private const float Scale = 4f;

        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;

        public int ScaledWidth => (int)(_texture.Width * Scale);
        public int ScaledHeight => (int)(_texture.Height * Scale);

        public Player(Texture2D texture, Vector2 position)
        {
            _texture = texture;
            _position = position;
        }

        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            float horizontalMovement = 0f;
            if (_currentKeyboardState.IsKeyDown(Keys.A))
            {
                horizontalMovement -= MoveSpeed;
                _spriteEffect = SpriteEffects.FlipHorizontally;
            }
            if (_currentKeyboardState.IsKeyDown(Keys.D))
            {
                horizontalMovement += MoveSpeed;
                _spriteEffect = SpriteEffects.None;
            }
            _velocity.X = horizontalMovement;

            _isGrounded = _position.Y + (_texture.Height * Scale) / 2f >= graphicsDevice.Viewport.Height;

            bool isJumping = _currentKeyboardState.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.Space);

            if (isJumping && _isGrounded)
            {
                _velocity.Y = JumpForce;
            }

            if (!_isGrounded)
            {
                _velocity.Y += Gravity * deltaTime;
            }

            _position += _velocity * deltaTime;

            int windowHeight = graphicsDevice.Viewport.Height;
            float halfScaledHeight = (_texture.Height * Scale) / 2f;
            if (_position.Y + halfScaledHeight > windowHeight)
            {
                _position.Y = windowHeight - halfScaledHeight;

                if (_velocity.Y > 0)
                {
                    _velocity.Y = 0f;
                }
            }

            float halfScaledWidth = (_texture.Width * Scale) / 2f;
            int windowWidth = graphicsDevice.Viewport.Width;

            if (_position.X - halfScaledWidth < 0)
            {
                _position.X = halfScaledWidth;
                _velocity.X = 0;
            }
            if (_position.X + halfScaledWidth > windowWidth)
            {
                _position.X = windowWidth - halfScaledWidth;
                _velocity.X = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var origin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);

            spriteBatch.Draw(
                _texture,
                _position,
                null,
                Color.White,
                0f,
                origin,
                Scale,
                _spriteEffect,
                0f
            );
        }
    }
}
