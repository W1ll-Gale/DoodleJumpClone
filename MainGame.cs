using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ImGuiNET;
using MonoGame.ImGuiNet;
using System.Collections.Generic;
using System;

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
    private Random _random = new Random();
    private Platform _highestPlatform;
    private float _maxHeightReached;

    private Player _player;
    private Texture2D _playerTexture;

    private List<Platform> _platforms;
    private Texture2D _platformTexture;

    private int _currentScore;
    private float _initialPlayerY;
    private const int MovingPlatformScoreThreshold = 200;

    public MainGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.SynchronizeWithVerticalRetrace = false; 
        IsFixedTimeStep = false;

        _graphics.PreferredBackBufferWidth = 600;
        _graphics.PreferredBackBufferHeight = 800;
        _graphics.ApplyChanges();
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

        ResetGame();
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

        float playerCameraY = -_player.Position.Y + _graphics.PreferredBackBufferHeight / 2f;
        if (playerCameraY > _cameraY)
        {
            _cameraY = playerCameraY;


            if (_player.Position.Y < _maxHeightReached)
            {
                _maxHeightReached = _player.Position.Y;
                _currentScore = (int)((_initialPlayerY - _maxHeightReached) / 10f);
            }
        }

        while (_highestPlatform.BoundingBox.Y > -_cameraY - 50)
        {
            GeneratePlatforms();
        }

        _platforms.RemoveAll(p => p.BoundingBox.Y > -_cameraY + _graphics.PreferredBackBufferHeight);

        if (_player.Position.Y > -_cameraY + _graphics.PreferredBackBufferHeight)
        {
            ResetGame();
        }

        base.Update(gameTime);
    }

    private void GeneratePlatforms()
    {
        PlatformType platformType = PlatformType.Static;

        if (_currentScore > MovingPlatformScoreThreshold)
        {
            if (_random.Next(0, 5) == 0)
            {
                platformType = PlatformType.Moving;
            }
        }
        int y = _highestPlatform.BoundingBox.Y - _random.Next(80, 145);
        const float maxHorizontalReach = 280f;
        float previousPlatformX = _highestPlatform.BoundingBox.Center.X;
        float minX = previousPlatformX - maxHorizontalReach;
        float maxX = previousPlatformX + maxHorizontalReach;

        minX = Math.Max(0, minX);
        maxX = Math.Min(_graphics.PreferredBackBufferWidth - 100, maxX);
        if (minX >= maxX)
        {
            minX = _graphics.PreferredBackBufferWidth / 2f - 150;
            maxX = _graphics.PreferredBackBufferWidth / 2f + 150;
        }
        int x = _random.Next((int)minX, (int)maxX);

        Platform newPlatform = new Platform(_platformTexture, new Vector2(x, y), 100, 20, platformType);
        _platforms.Add(newPlatform);
        _highestPlatform = newPlatform;
    }

    private void ResetGame()
    {
        _platforms.Clear();
        _cameraY = 0;
        _currentScore = 0;

        Platform startPlatform = new Platform(_platformTexture, new Vector2(_graphics.PreferredBackBufferWidth / 2f - 50, _graphics.PreferredBackBufferHeight - 50), 100, 20, PlatformType.Static);
        _platforms.Add(startPlatform);
        _highestPlatform = startPlatform;

        int startY = startPlatform.BoundingBox.Top - _player.ScaledHeight;
        _player.Velocity = Vector2.Zero;
        _player.Position = new Vector2(_graphics.PreferredBackBufferWidth / 2f, startY);

        _initialPlayerY = startY;
        _maxHeightReached = _initialPlayerY;

        while (_highestPlatform.BoundingBox.Y > -_cameraY)
        {
            GeneratePlatforms();
        }
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

        foreach(Platform platform in _platforms)
        {
            ImGui.Text($"Platform: {platform.Type}");
            ImGui.Text($"Position: {platform.BoundingBox.Location}");
            ImGui.Text($"Velocity: {platform.Velocity}");
            ImGui.Text($"Size: {platform.BoundingBox.Size}");
        }

        ImGui.Separator();
        ImGui.Checkbox("Show Hitboxes", ref _showDebugVisuals);
        ImGui.End();
    }


    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        Matrix cameraTransform = Matrix.CreateTranslation(0, (int)_cameraY, 0);

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

        _spriteBatch.Begin(transformMatrix: cameraTransform, samplerState: SamplerState.PointClamp);
        _player.Draw(_spriteBatch);

        if (_showDebugVisuals)
        {
            _spriteBatch.Draw(_platformTexture, _player.BoundingBox, Color.Red * 0.5f);

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
