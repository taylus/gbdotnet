using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GBDotNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameBoy
{
    public class MonoGameBoy : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameBoyScreen screen;
        private KeyboardState previousKeyboardState;
        private KeyboardState currentKeyboardState;
        private readonly Emulator emulator;
        private readonly string romPath;
        private string RomName => Path.GetFileName(romPath);
        private readonly bool useBootRom;
        private readonly GameBoyColorPalette palette;
        private bool paused = true;
        private DisplayMode currentDisplayMode;
        private readonly bool runInBackground = true;
        private readonly bool loggingEnabled;
        private readonly IConfiguration config;
        private long frontendFrameCount = 0;
        private double frontendFps = 60;
        private bool runAtMaxSpeed = false;

        public MonoGameBoy(Emulator emulator, string romPath, bool useBootRom, bool loggingEnabled, IConfiguration config)
        {
            this.emulator = emulator;
            this.romPath = romPath;
            this.useBootRom = useBootRom;
            this.loggingEnabled = loggingEnabled;
            this.config = config;
            palette = GetColorPalette();
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            currentKeyboardState = previousKeyboardState = Keyboard.GetState();
            ShowScreen();
            if (runInBackground) Task.Run(RunEmulator).ContinueWith(deadEmulator =>
            {
                Console.WriteLine($"Emulator task unexpectedly faulted while executing instruction at ${emulator.CPU.Registers.LastPC:x4}:");
                Console.WriteLine($"Exception (if any): {deadEmulator.Exception}");
            });
        }

        private void RunEmulator()
        {
            var cyclesToRunPerFrame = CPU.ClockSpeed / PPU.FramesPerSecond;
            var millisecondsPerFrame = 1000 / PPU.FramesPerSecond;
            var stopwatch = new Stopwatch();
            while (true)
            {
                var cycleCount = 0;
                stopwatch.Restart();
                //emulate one frame's worth of cycles
                while (cycleCount < cyclesToRunPerFrame)
                {
                    if (paused) continue;
                    if (loggingEnabled) Console.WriteLine(emulator);
                    emulator.Tick();
                    cycleCount += emulator.CPU.CyclesLastTick;
                }
                stopwatch.Stop();

                //let real time "catch up" to the emulator, if needed, by sleeping
                if (!runAtMaxSpeed && stopwatch.ElapsedMilliseconds < millisecondsPerFrame)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(millisecondsPerFrame - stopwatch.ElapsedMilliseconds));
                }
            }
        }

        private void SetWindowSize(int width, int height)
        {
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            currentKeyboardState = Keyboard.GetState();

            if (!runInBackground)
            {
                for (int i = 0; i < 512; i++)
                {
                    if (loggingEnabled) Console.WriteLine(emulator);
                    emulator.Tick();
                }
            }

            if (IsKeyDown(Keys.Escape)) Exit();

            HandleJoypadInput();
            HandleDebugInput();

            if (currentDisplayMode == DisplayMode.Screen) screen.PutPixels(palette, emulator.PPU.ScreenBackBuffer);
            else if (currentDisplayMode == DisplayMode.TileSet) screen.PutPixels(palette, emulator.PPU.RenderTileSet());
            else if (currentDisplayMode == DisplayMode.BackgroundMap) screen.PutPixels(palette, emulator.PPU.RenderBackgroundMap());
            else if (currentDisplayMode == DisplayMode.WindowLayer) screen.PutPixels(palette, emulator.PPU.RenderWindow());
            else if (currentDisplayMode == DisplayMode.SpriteLayer) screen.PutPixels(palette, emulator.PPU.RenderSprites());

            frontendFrameCount++;
            frontendFps = frontendFrameCount / gameTime.TotalGameTime.TotalSeconds;
            SetWindowTitle(currentDisplayMode == DisplayMode.Screen ? "MonoGameBoy" : $"MonoGameBoy - {currentDisplayMode}", gameTime);

            previousKeyboardState = currentKeyboardState;
            base.Update(gameTime);
        }

        private void HandleJoypadInput()
        {
            if (IsKeyDown(Keys.X)) emulator.Joypad.Press(Button.A);
            else emulator.Joypad.Release(Button.A);

            if (IsKeyDown(Keys.Z)) emulator.Joypad.Press(Button.B);
            else emulator.Joypad.Release(Button.B);

            if (IsKeyDown(Keys.Enter)) emulator.Joypad.Press(Button.Start);
            else emulator.Joypad.Release(Button.Start);

            if (IsKeyDown(Keys.RightShift)) emulator.Joypad.Press(Button.Select);
            else emulator.Joypad.Release(Button.Select);

            if (IsKeyDown(Keys.Up)) emulator.Joypad.Press(Button.Up);
            else emulator.Joypad.Release(Button.Up);

            if (IsKeyDown(Keys.Down)) emulator.Joypad.Press(Button.Down);
            else emulator.Joypad.Release(Button.Down);

            if (IsKeyDown(Keys.Left)) emulator.Joypad.Press(Button.Left);
            else emulator.Joypad.Release(Button.Left);

            if (IsKeyDown(Keys.Right)) emulator.Joypad.Press(Button.Right);
            else emulator.Joypad.Release(Button.Right);
        }

        private void HandleDebugInput()
        {
            if (WasJustPressed(Keys.Space)) ShowScreen();
            else if (WasJustPressed(Keys.T)) ShowTileSet();
            else if (WasJustPressed(Keys.B)) ShowBackgroundMap();
            else if (WasJustPressed(Keys.W)) ShowWindowLayer();
            else if (WasJustPressed(Keys.S)) ShowSpriteLayer();
            else if (WasJustPressed(Keys.P)) ShowPalettes();
            else if (WasJustPressed(Keys.F1)) SaveMemoryDump(openAfterSaving: true);
            else if (WasJustPressed(Keys.F2) || WasJustPressed(Keys.R)) RestartEmulator();
            else if (WasJustPressed(Keys.Tab)) runAtMaxSpeed = !runAtMaxSpeed;
        }

        private bool IsKeyDown(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }

        private bool WasJustPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key);
        }

        private void ShowTileSet()
        {
            screen = new GameBoyScreen(GraphicsDevice, TileSet.WidthInPixels, TileSet.HeightInPixels);
            SetWindowSize(screen.Width * 4, screen.Height * 4);
            currentDisplayMode = DisplayMode.TileSet;
            //paused = true;
        }

        private void ShowBackgroundMap()
        {
            screen = new GameBoyScreen(GraphicsDevice, TileMap.WidthInPixels, TileMap.HeightInPixels);
            SetWindowSize(screen.Width * 2, screen.Height * 2);
            currentDisplayMode = DisplayMode.BackgroundMap;
            //paused = true;
        }

        private void ShowWindowLayer()
        {
            screen = new GameBoyScreen(GraphicsDevice, TileMap.WidthInPixels, TileMap.HeightInPixels);
            SetWindowSize(screen.Width * 2, screen.Height * 2);
            currentDisplayMode = DisplayMode.WindowLayer;
            //paused = true;
        }

        private void ShowSpriteLayer()
        {
            screen = new GameBoyScreen(GraphicsDevice, PPU.ScreenWidthInPixels, PPU.ScreenHeightInPixels);
            SetWindowSize(screen.Width * 3, screen.Height * 3);
            currentDisplayMode = DisplayMode.SpriteLayer;
            //paused = true;
        }

        private void ShowPalettes()
        {
            //TODO: implement
        }

        private void ShowScreen()
        {
            screen = new GameBoyScreen(GraphicsDevice, PPU.ScreenWidthInPixels, PPU.ScreenHeightInPixels);
            SetWindowSize(screen.Width * 3, screen.Height * 3);
            currentDisplayMode = DisplayMode.Screen;
            paused = false;
        }

        private void SetWindowTitle(string title, GameTime gameTime, bool showRomName = true, bool showFps = true, bool showCpuTime = true)
        {
            string romNamePart = showRomName ? $" [{RomName}]" : "";
            string fpsPart = showFps ? $" [{emulator.CalculateAverageFramesPerSecond(gameTime.TotalGameTime):n0}/{frontendFps:n0} FPS]" : "";
            string cpuTimePart = showCpuTime ? $" [{emulator.CPU.ElapsedCpuTime.TotalSeconds:n0}/{gameTime.TotalGameTime.TotalSeconds:n0} sec]" : "";
            Window.Title = $"{title}{romNamePart}{fpsPart}{cpuTimePart}";
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            screen.Draw(spriteBatch, GraphicsDevice.Viewport.Bounds);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void SaveMemoryDump(bool openAfterSaving = false)
        {
            string path = Guid.NewGuid() + ".bin";
            File.WriteAllBytes(path, emulator.CPU.Memory.Data.ToArray());
            if (openAfterSaving) OpenFile(path);
        }

        protected static void OpenFile(string path)
        {
            Process.Start(new ProcessStartInfo() { FileName = path, UseShellExecute = true });
        }

        private void RestartEmulator()
        {
            emulator.Restart(useBootRom);
            if (loggingEnabled) Console.Clear();
            frontendFrameCount = 0;
            frontendFps = 60;
        }

        private GameBoyColorPalette GetColorPalette()
        {
            try
            {
                return new GameBoyColorPalette(new string[]
                {
                    config.GetSection("palette:0").Value,
                    config.GetSection("palette:1").Value,
                    config.GetSection("palette:2").Value,
                    config.GetSection("palette:3").Value
                });
            }
            catch
            {
                return GameBoyColorPalette.Dmg;
            }
        }
    }
}