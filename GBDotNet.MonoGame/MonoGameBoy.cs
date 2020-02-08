using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GBDotNet.Core;
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
        private readonly CPU cpu;
        private readonly PPU ppu;
        private readonly string romPath;
        private string RomName => Path.GetFileName(romPath);
        private static readonly GameBoyColorPalette palette = GameBoyColorPalette.Dmg;
        private bool paused = true;
        private readonly bool runInBackground = true;

        public MonoGameBoy(CPU cpu, PPU ppu, string romPath)
        {
            this.cpu = cpu;
            this.ppu = ppu;
            this.romPath = romPath;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            currentKeyboardState = previousKeyboardState = Keyboard.GetState();
            ShowScreen();
            if (runInBackground) Task.Run(RunEmulator);
        }

        private void RunEmulator()
        {
            while (true)
            {
                if (paused) continue;
                Console.WriteLine($"{cpu} {ppu}");
                cpu.Tick();
                ppu.Tick(cpu.CyclesLastTick);
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

            if (currentKeyboardState.IsKeyDown(Keys.Escape)) Exit();
            else if (WasJustPressed(Keys.Space)) ShowScreen();
            if (WasJustPressed(Keys.T)) ShowTileSet();
            else if (WasJustPressed(Keys.B)) ShowBackgroundMap();
            else if (WasJustPressed(Keys.W)) ShowWindowLayer();
            else if (WasJustPressed(Keys.S)) ShowSpriteLayer();
            else if (WasJustPressed(Keys.P)) ShowPalettes();
            else if (WasJustPressed(Keys.F1)) SaveMemoryDump(openAfterSaving: true);

            if (!runInBackground)
            {
                for (int i = 0; i < 1024; i++)
                {
                    Console.WriteLine($"{cpu} {ppu}");
                    cpu.Tick();
                    ppu.Tick(cpu.CyclesLastTick);
                }
            }

            previousKeyboardState = currentKeyboardState;
            base.Update(gameTime);
        }

        private bool WasJustPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key);
        }

        private void ShowTileSet()
        {
            screen = new GameBoyScreen(GraphicsDevice, TileSet.WidthInPixels, TileSet.HeightInPixels);
            screen.PutPixels(palette, ppu.RenderTileSet());
            SetWindowSize(screen.Width * 4, screen.Height * 4);
            Window.Title = $"MonoGameBoy - Tileset [{RomName}]";
            paused = true;
        }

        private void ShowBackgroundMap()
        {
            screen = new GameBoyScreen(GraphicsDevice, TileMap.WidthInPixels, TileMap.HeightInPixels);
            screen.PutPixels(palette, ppu.RenderBackgroundMap());
            SetWindowSize(screen.Width * 2, screen.Height * 2);
            Window.Title = $"MonoGameBoy - Background Map [{RomName}]";
            paused = true;
        }

        private void ShowWindowLayer()
        {
            screen = new GameBoyScreen(GraphicsDevice, TileMap.WidthInPixels, TileMap.HeightInPixels);
            screen.PutPixels(palette, ppu.RenderWindow());
            SetWindowSize(screen.Width * 2, screen.Height * 2);
            Window.Title = $"MonoGameBoy - Window Layer [{RomName}]";
            paused = true;
        }

        private void ShowSpriteLayer()
        {
            screen = new GameBoyScreen(GraphicsDevice, PPU.ScreenWidthInPixels, PPU.ScreenHeightInPixels);
            screen.PutPixels(palette, ppu.RenderSprites());
            SetWindowSize(screen.Width * 3, screen.Height * 3);
            Window.Title = $"MonoGameBoy - Sprites [{RomName}]";
            paused = true;
        }

        private void ShowPalettes()
        {
            //TODO: implement
        }

        private void ShowScreen()
        {
            screen = new GameBoyScreen(GraphicsDevice, PPU.ScreenWidthInPixels, PPU.ScreenHeightInPixels);
            SetWindowSize(screen.Width * 3, screen.Height * 3);
            screen.PutPixels(palette, ppu.RenderScreen());
            Window.Title = $"MonoGameBoy [{RomName}]";
            paused = false;
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!paused) screen.PutPixels(palette, ppu.RenderScreen());

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            screen.Draw(spriteBatch, GraphicsDevice.Viewport.Bounds);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void SaveMemoryDump(bool openAfterSaving = false)
        {
            string path = Guid.NewGuid() + ".bin";
            File.WriteAllBytes(path, cpu.Memory.Data.ToArray());
            if (openAfterSaving) OpenFile(path);
        }

        protected static void OpenFile(string path)
        {
            Process.Start(new ProcessStartInfo() { FileName = path, UseShellExecute = true });
        }
    }
}