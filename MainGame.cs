using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ImGuiNET;
using MonoGame.ImGuiNet;
using System.Collections.Generic;

namespace DoodleJumpClone;

public class MainGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private ImGuiRenderer _imGuiRenderer;

    private bool _showDebugVisuals = false;
    private bool _showDebugMenu = false;
    private KeyboardState _currentKeyboardState;
    private KeyboardState _previousKeyboardState;

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
        _graphics.SynchronizeWithVerticalRetrace = false; 
        IsFixedTimeStep = false; 
    }

    protected override void Initialize()
    {
        _cameraY = 0;
        _imGuiRenderer = new ImGuiRenderer(this);
        _imGuiRenderer.RebuildFontAtlas();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _playerTexture = Content.Load<Texture2D>("TestPlayerSprite");
        _player = new Player(_playerTexture, new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight - 50));

        _platformTexture = new Texture2D(GraphicsDevice, 1, 1);
        _platformTexture.SetData(new[] { Color.White });
        _platforms = new List<Platform>();

        _platforms.Add(new Platform(_platformTexture, new Vector2(100, 400), 100, 20));
        _platforms.Add(new Platform(_platformTexture, new Vector2(300, 300), 100, 20));

    }

    protected override void Update(GameTime gameTime)
    {

        _previousKeyboardState = _currentKeyboardState;
        _currentKeyboardState = Keyboard.GetState();

        if (_currentKeyboardState.IsKeyDown(Keys.F1) && _previousKeyboardState.IsKeyUp(Keys.F1))
        {
            _showDebugMenu = !_showDebugMenu;
        }

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        foreach (Platform platform in _platforms)
        {
            platform.Update(gameTime, GraphicsDevice.Viewport);
        }

        _player.Update(gameTime, GraphicsDevice, _platforms);

        base.Update(gameTime);
    }

    private void DrawDebugUI()
    {
        ImGui.Begin("Game Debug");
        ImGui.Text($"FPS: {ImGui.GetIO().Framerate:F1}");
        ImGui.Separator();

        if (_player != null)
        {
            ImGui.Text("Player Info");
            ImGui.Text($"Position: {_player.Position}");
            ImGui.Text($"Velocity: {_player.Velocity}");
            ImGui.Text($"IsGrounded: {_player.IsGrounded}");
        }

        ImGui.Separator();
        ImGui.Checkbox("Show Hitboxes", ref _showDebugVisuals);
        ImGui.End();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        Matrix cameraTransform = Matrix.CreateTranslation(0, (int)_cameraY, 0);

        _spriteBatch.Begin(transformMatrix: cameraTransform, samplerState: SamplerState.PointClamp);
        _player.Draw(_spriteBatch);

        if (_showDebugVisuals)
        {
            _spriteBatch.Draw(_platformTexture, _player.BoundingBox, Color.Red * 0.5f);

        }

        _spriteBatch.End();

        _spriteBatch.Begin(transformMatrix: cameraTransform);
        foreach (Platform platform in _platforms)
        {
            platform.Draw(_spriteBatch);

            if (_showDebugVisuals)
            {
                _spriteBatch.Draw(_platformTexture, platform.BoundingBox, Color.Green * 0.5f);
            }
        }
        _spriteBatch.End();

        if (_showDebugMenu)
        {
            _imGuiRenderer.BeginLayout(gameTime);
            DrawDebugUI();
            _imGuiRenderer.EndLayout();
        }

        base.Draw(gameTime);
    }
}
