using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DoodleJumpClone;

public class MainGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private float _cameraY;

    private Player _player;

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

        Texture2D playerTexture = Content.Load<Texture2D>("TestPlayerSprite");
        _player = new Player(playerTexture, Vector2.Zero);

    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _player.Update(gameTime, GraphicsDevice);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        var cameraTransform = Matrix.CreateTranslation(0, (int)_cameraY, 0);
        _spriteBatch.Begin(transformMatrix: cameraTransform, samplerState: SamplerState.PointClamp);
        _player.Draw(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
