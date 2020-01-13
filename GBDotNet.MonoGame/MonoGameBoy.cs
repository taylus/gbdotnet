using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameBoy
{
    public class MonoGameBoy : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameBoyScreen screen;
        private GameBoyColorPalette palette = GameBoyColorPalette.Dmg;

        public MonoGameBoy()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            InitializeGraphics();
            screen = new GameBoyScreen(GraphicsDevice);
            screen.PutPixels(Enumerable.Repeat(palette[0], 160 * 144).ToArray());
            DrawDemoSprite(new Point(74, 60));
        }

        private void DrawDemoSprite(Point offset)
        {
            //demo: draw a coffee cup sprite (very inefficiently + not how the Game Boy itself does)
            screen.PutPixel(palette[3], 2 + offset.X, 0 + offset.Y);
            screen.PutPixel(palette[3], 3 + offset.X, 0 + offset.Y);
            screen.PutPixel(palette[3], 4 + offset.X, 0 + offset.Y);
            screen.PutPixel(palette[3], 1 + offset.X, 1 + offset.Y);
            screen.PutPixel(palette[3], 5 + offset.X, 1 + offset.Y);
            screen.PutPixel(palette[3], 0 + offset.X, 2 + offset.Y);
            screen.PutPixel(palette[3], 2 + offset.X, 2 + offset.Y);
            screen.PutPixel(palette[3], 3 + offset.X, 2 + offset.Y);
            screen.PutPixel(palette[3], 4 + offset.X, 2 + offset.Y);
            screen.PutPixel(palette[3], 6 + offset.X, 2 + offset.Y);
            screen.PutPixel(palette[3], 0 + offset.X, 3 + offset.Y);
            screen.PutPixel(palette[3], 2 + offset.X, 3 + offset.Y);
            screen.PutPixel(palette[3], 3 + offset.X, 3 + offset.Y);
            screen.PutPixel(palette[3], 4 + offset.X, 3 + offset.Y);
            screen.PutPixel(palette[3], 6 + offset.X, 3 + offset.Y);
            screen.PutPixel(palette[3], 7 + offset.X, 3 + offset.Y);
            screen.PutPixel(palette[3], 0 + offset.X, 4 + offset.Y);
            screen.PutPixel(palette[3], 7 + offset.X, 4 + offset.Y);
            screen.PutPixel(palette[3], 0 + offset.X, 5 + offset.Y);
            screen.PutPixel(palette[3], 5 + offset.X, 5 + offset.Y);
            screen.PutPixel(palette[3], 7 + offset.X, 5 + offset.Y);
            screen.PutPixel(palette[3], 1 + offset.X, 6 + offset.Y);
            screen.PutPixel(palette[3], 5 + offset.X, 6 + offset.Y);
            screen.PutPixel(palette[3], 6 + offset.X, 6 + offset.Y);
            screen.PutPixel(palette[3], 2 + offset.X, 7 + offset.Y);
            screen.PutPixel(palette[3], 3 + offset.X, 7 + offset.Y);
            screen.PutPixel(palette[3], 4 + offset.X, 7 + offset.Y);

            screen.PutPixel(palette[1], 1 + offset.X, 4 + offset.Y);
            screen.PutPixel(palette[1], 5 + offset.X, 4 + offset.Y);
            screen.PutPixel(palette[1], 6 + offset.X, 4 + offset.Y);
            screen.PutPixel(palette[1], 1 + offset.X, 5 + offset.Y);
            screen.PutPixel(palette[1], 2 + offset.X, 5 + offset.Y);
            screen.PutPixel(palette[1], 3 + offset.X, 5 + offset.Y);
            screen.PutPixel(palette[1], 4 + offset.X, 5 + offset.Y);
            screen.PutPixel(palette[1], 2 + offset.X, 6 + offset.Y);
            screen.PutPixel(palette[1], 3 + offset.X, 6 + offset.Y);
            screen.PutPixel(palette[1], 4 + offset.X, 6 + offset.Y);
        }

        private void InitializeGraphics()
        {
            graphics.PreferredBackBufferWidth = GameBoyScreen.WidthInPixels * 4;
            graphics.PreferredBackBufferHeight = GameBoyScreen.HeightInPixels * 4;
            graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().GetPressedKeys().Any()) Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(new Color(70, 70, 70));

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            screen.Draw(spriteBatch, GraphicsDevice.Viewport.Bounds);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}