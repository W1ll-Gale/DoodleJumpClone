using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace DoodleJumpClone
{
    class Player
    {
        private Texture2D _texture;
        private SpriteEffects _spriteEffect = SpriteEffects.None;

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        private Vector2 _previousPosition;
        public bool IsGrounded {get; private set; }
        private Platform _platformPlayerIsOn = null;

        private const float Gravity = 1200f;
        private const float JumpForce = -600f;
        private const float MoveSpeed = 350f;
        private const float Scale = 4f;

        private const int CollisionWidth = (int)(18 * Scale);
        private const int CollisionHeight = (int)(22 * Scale);

        public int ScaledWidth => (int)(_texture.Width * Scale);
        public int ScaledHeight => (int)(_texture.Height * Scale);

        public Rectangle BoundingBox => new Rectangle(
            (int)(Position.X - CollisionWidth / 2f),
            (int)(Position.Y - CollisionHeight / 2f),
            CollisionWidth,
            CollisionHeight);

        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;

        public Player(Texture2D texture, Vector2 position)
        {
            _texture = texture;
            Position = position;
        }

        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice, List<Platform> platforms)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            _previousPosition = Position;

            Vector2 velocity = Velocity;
            Vector2 position = Position;

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
            velocity.X = horizontalMovement;

            if (!IsGrounded)
            {
                velocity.Y += Gravity * deltaTime;
            }

            if (_platformPlayerIsOn != null && _platformPlayerIsOn.Type == PlatformType.Moving)
            {
                position.X += _platformPlayerIsOn.Velocity.X * deltaTime;
            }

            position += velocity * deltaTime;

            float halfCollisionWidth = CollisionWidth / 2f;
            int windowWidth = graphicsDevice.Viewport.Width;
            if (position.X < -halfCollisionWidth)
            {
                position.X = windowWidth - halfCollisionWidth;
            }
            if (position.X > windowWidth + halfCollisionWidth)
            {
                position.X = halfCollisionWidth;
            }

            if (_platformPlayerIsOn != null && !_platformPlayerIsOn.IsCollidable)
            {
                IsGrounded = false;
                _platformPlayerIsOn = null;
            }


            HandleCollisions(graphicsDevice.Viewport, platforms, ref position, ref velocity);

            Position = position;
            Velocity = velocity;
        }

        private void HandleCollisions(Viewport viewport, List<Platform> platforms, ref Vector2 position, ref Vector2 velocity)
        {
            IsGrounded = false;
            _platformPlayerIsOn = null;

            float halfCollisionHeight = CollisionHeight / 2f;
            float playerBottom = position.Y + halfCollisionHeight;
            float prevPlayerBottom = _previousPosition.Y + halfCollisionHeight;

            if (velocity.Y >= 0)
            {
                foreach (Platform platform in platforms.Where(p => p.IsCollidable))
                {
                    Rectangle platformBox = platform.BoundingBox;
                    float platformTop = platformBox.Top;

                    float playerLeft = position.X - CollisionWidth / 2f;
                    float playerRight = position.X + CollisionWidth / 2f;

                    bool wasAbove = prevPlayerBottom <= platformTop;
                    bool nowBelow = playerBottom >= platformTop;
                    bool horizontallyOverlapping = playerRight > platformBox.Left && playerLeft < platformBox.Right;

                    if (wasAbove && nowBelow && horizontallyOverlapping)
                    {
                        position.Y = platformTop - halfCollisionHeight;
                        velocity.Y = JumpForce;
                        IsGrounded = true;
                        _platformPlayerIsOn = platform;

                        platform.OnPlayerContact();
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
                Position,
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