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
        private Vector2 _previousPosition;
        private Vector2 _velocity;
        private bool _isGrounded;
        private SpriteEffects _spriteEffect = SpriteEffects.None;

        private Platform _platformPlayerIsOn = null;

        private const float Gravity = 1200f;
        private const float JumpForce = -600f;
        private const float MoveSpeed = 350f;
        private const float Scale = 4f;

        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;

        private const int CollisionWidth = (int)(18 * Scale);
        private const int CollisionHeight = (int)(22 * Scale);

        public int ScaledWidth => (int)(_texture.Width * Scale);
        public int ScaledHeight => (int)(_texture.Height * Scale);

        public Rectangle BoundingBox => new Rectangle(
            (int)(_position.X - CollisionWidth / 2f),
            (int)(_position.Y - CollisionHeight / 2f),
            CollisionWidth,
            CollisionHeight);

        public Player(Texture2D texture, Vector2 position)
        {
            _texture = texture;
            _position = position;
        }

        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice, List<Platform> platforms)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            _previousPosition = _position;

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

            if (!_isGrounded)
            {
                _velocity.Y += Gravity * deltaTime;
            }

            _position += _velocity * deltaTime;

            float halfCollisionWidth = CollisionWidth / 2f;
            int windowWidth = graphicsDevice.Viewport.Width;
            if (_position.X < halfCollisionWidth)
            {
                _position.X = halfCollisionWidth;
                _velocity.X = 0;
            }
            if (_position.X > windowWidth - halfCollisionWidth)
            {
                _position.X = windowWidth - halfCollisionWidth;
                _velocity.X = 0;
            }

            if (_platformPlayerIsOn != null)
            {
                float playerLeft = _position.X - CollisionWidth / 2f;
                float playerRight = _position.X + CollisionWidth / 2f;
                Rectangle platformBox = _platformPlayerIsOn.BoundingBox;

                bool stillHorizontallyOverlapping = playerRight > platformBox.Left && playerLeft < platformBox.Right;

                if (!stillHorizontallyOverlapping)
                {
                    _isGrounded = false;
                    _platformPlayerIsOn = null;
                }
            }

            HandleCollisions(graphicsDevice.Viewport, platforms);

            bool isJumping = _currentKeyboardState.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.Space);
            if (isJumping && _isGrounded)
            {
                _velocity.Y = JumpForce;
                _isGrounded = false;
                _platformPlayerIsOn = null;
            }
        }

        private void HandleCollisions(Viewport viewport, List<Platform> platforms)
        {
            _isGrounded = false;
            _platformPlayerIsOn = null;

            float halfCollisionHeight = CollisionHeight / 2f;
            float playerBottom = _position.Y + halfCollisionHeight;
            float prevPlayerBottom = _previousPosition.Y + halfCollisionHeight;

            if (playerBottom >= viewport.Height)
            {
                _position.Y = viewport.Height - halfCollisionHeight;
                _velocity.Y = 0f;
                _isGrounded = true;
                return;
            }

            if (_velocity.Y >= 0)
            {
                foreach (Platform platform in platforms)
                {
                    Rectangle platformBox = platform.BoundingBox;
                    float platformTop = platformBox.Top;

                    float playerLeft = _position.X - CollisionWidth / 2f;
                    float playerRight = _position.X + CollisionWidth / 2f;

                    bool wasAbove = prevPlayerBottom <= platformTop;
                    bool nowBelow = playerBottom >= platformTop;
                    bool horizontallyOverlapping = playerRight > platformBox.Left && playerLeft < platformBox.Right;

                    if (wasAbove && nowBelow && horizontallyOverlapping)
                    {
                        _position.Y = platformTop - halfCollisionHeight;
                        _velocity.Y = 0f;
                        _isGrounded = true;
                        _platformPlayerIsOn = platform;
                        break;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);

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