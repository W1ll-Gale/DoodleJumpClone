using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace DoodleJumpClone;

public class MainGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private float _cameraY;

    private Player _player;
    private Texture2D _playerTexture;

    private List<Platform> _platforms;
    private Texture2D _platformTexture;

    public MainGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _cameraY = 0;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _playerTexture = Content.Load<Texture2D>("TestPlayerSprite");
        _player = new Player(_playerTexture, Vector2.Zero);

        _platformTexture = new Texture2D(GraphicsDevice, 1, 1);
        _platformTexture.SetData(new[] { Color.White });
        _platforms = new List<Platform>();

        _platforms.Add(new Platform(_platformTexture, new Vector2(100, 400), 100, 20));
        _platforms.Add(new Platform(_platformTexture, new Vector2(300, 300), 100, 20));

    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        foreach (Platform platform in _platforms)
        {
            platform.Update(gameTime, GraphicsDevice.Viewport);
        }

        _player.Update(gameTime, GraphicsDevice, _platforms);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        Matrix cameraTransform = Matrix.CreateTranslation(0, (int)_cameraY, 0);

        _spriteBatch.Begin(transformMatrix: cameraTransform, samplerState: SamplerState.PointClamp);
        _player.Draw(_spriteBatch);
        _spriteBatch.End();

        _spriteBatch.Begin(transformMatrix: cameraTransform);
        foreach (Platform platform in _platforms)
        {
            platform.Draw(_spriteBatch);
        }
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
