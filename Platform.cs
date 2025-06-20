using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DoodleJumpClone
{
    public enum PlatformType
    {
        Static,
        Moving,
        Destructible
    }

    class Platform
    {
        private Texture2D _texture;
        private Rectangle _boundingBox;
        public PlatformType Type { get; }

        private float _movementRemainder = 0f;

        public Vector2 Velocity;
        private int _direction = 1;
        private const float Speed = 100f;

        private enum State { Intact, Hit, Disappeared }
        private State _currentState = State.Intact;
        private float _timer;
        private const float DisappearDuration = 0.3f; 

        public bool IsCollidable { get; private set; } = true;

        public Rectangle BoundingBox => _boundingBox;

        public Platform(Texture2D texture, Vector2 position, int width, int height, PlatformType type)
        {
            _texture = texture;
            _boundingBox = new Rectangle((int)position.X, (int)position.Y, width, height);
            Type = type;

            if (Type == PlatformType.Moving)
            {
                Velocity.X = Speed * _direction;
            }
        }
        public void Update(GameTime gameTime, Viewport viewport)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Type == PlatformType.Destructible)
            {
                switch (_currentState)
                {
                    case State.Hit:
                        _timer -= deltaTime;
                        if (_timer <= 0)
                        {
                            _currentState = State.Disappeared;
                            IsCollidable = false;
                        }
                        break;
                }
            }

            if (Type == PlatformType.Moving)
            {
                float totalMovement = (Velocity.X * deltaTime) + _movementRemainder;

                float predictedNextX = _boundingBox.X + totalMovement;

                if (predictedNextX <= 0)
                {
                    _boundingBox.X = 0;
                    _direction = 1;
                    Velocity.X = Speed * _direction;
                    _movementRemainder = 0;
                }
                else if (predictedNextX + _boundingBox.Width >= viewport.Width)
                {
                    _boundingBox.X = viewport.Width - _boundingBox.Width;
                    _direction = -1;
                    Velocity.X = Speed * _direction;
                    _movementRemainder = 0;
                }
                else
                {
                    int pixelsToMove = (int)totalMovement;
                    _boundingBox.X += pixelsToMove;
                    _movementRemainder = totalMovement - pixelsToMove;
                }
            }

        }

        public void OnPlayerContact()
        {
            if (Type == PlatformType.Destructible && _currentState == State.Intact)
            {
                _currentState = State.Hit;
                _timer = DisappearDuration;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_currentState == State.Disappeared)
            {
                return;
            }

            Color color = Type switch
            {
                PlatformType.Static => Color.White,
                PlatformType.Moving => Color.LightBlue,
                PlatformType.Destructible => _currentState == State.Hit ? Color.OrangeRed : Color.Brown,
                _ => Color.White
            };

            spriteBatch.Draw(_texture, _boundingBox, color);
        }
    }
}
