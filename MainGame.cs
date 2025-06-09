using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DoodleJumpClone;

public class MainGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _playerTexture;

    private Vector2 _playerPosition;
    private Vector2 _playerVelocity;
    private bool _isGrounded;

    private const float Gravity = 2000f;
    private const float JumpForce = -800f;
    private const float MoveSpeed = 350f;
    private const float Friction = 0.90f;

    private KeyboardState _currentKeyboardState;
    private KeyboardState _previousKeyboardState;

    private float _scale = 4f;

    private SpriteEffects _spriteEffect = SpriteEffects.None;

    public MainGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _playerPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2f, _graphics.PreferredBackBufferHeight /2f);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _playerTexture = Content.Load<Texture2D>("TestPlayerSprite");
    }

    protected override void Update(GameTime gameTime)
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

        _playerVelocity.X = horizontalMovement;

        _isGrounded = _playerPosition.Y + (_playerTexture.Height * _scale) / 2f >= _graphics.GraphicsDevice.Viewport.Height;

        bool isJumping = _currentKeyboardState.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.Space);

        if(isJumping && _isGrounded)
        {
            _playerVelocity.Y = JumpForce;
        }

        if (!_isGrounded)
        {
            _playerVelocity.Y += Gravity * deltaTime;
        }

        _playerPosition += _playerVelocity * deltaTime;

        int windowHeight = _graphics.GraphicsDevice.Viewport.Height;
        float halfScaledHeight = (_playerTexture.Height * _scale) / 2f;
        if (_playerPosition.Y + halfScaledHeight > windowHeight)
        {
            _playerPosition.Y = windowHeight - halfScaledHeight;

            if (_playerVelocity.Y > 0)
            {
                _playerVelocity.Y = 0f;
            }
        }

        float halfScaledWidth = (_playerTexture.Width * _scale) / 2f;
        int windowWidth = _graphics.GraphicsDevice.Viewport.Width;

        if (_playerPosition.X - halfScaledWidth < 0)
        {
            _playerPosition.X = halfScaledWidth;
            _playerVelocity.X = 0;
        }
        if (_playerPosition.X + halfScaledWidth > windowWidth)
        {
            _playerPosition.X = windowWidth - halfScaledWidth;
            _playerVelocity.X = 0;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        var origin = new Vector2(_playerTexture.Width / 2f, _playerTexture.Height / 2f);

        _spriteBatch.Draw(
            _playerTexture,
            _playerPosition,
            null,
            Color.White,
            0f,
            origin,
            _scale,
            _spriteEffect,
            0f
        );

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
