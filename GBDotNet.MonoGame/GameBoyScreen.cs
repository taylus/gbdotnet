using System.IO;
using System.Linq;
using GBDotNet.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameBoy
{
    public class GameBoyScreen
    {
        public int Width { get => renderTarget.Width; }
        public int Height { get => renderTarget.Height; }

        private readonly RenderTarget2D renderTarget;

        public GameBoyScreen(GraphicsDevice graphicsDevice, int width = PPU.ScreenWidthInPixels, int height = PPU.ScreenHeightInPixels)
        {
            renderTarget = new RenderTarget2D(graphicsDevice, width, height);
        }

        public void PutPixel(Color color, int x, int y)
        {
            var pixels = new Color[renderTarget.Width * renderTarget.Height];
            renderTarget.GetData(pixels);
            pixels[renderTarget.Width * y + x] = color;
            renderTarget.SetData(pixels);
        }

        public void PutPixelsFromFile(GameBoyColorPalette palette, string path)
        {
            PutPixels(palette, File.ReadAllBytes(path));
        }

        public void PutPixels(GameBoyColorPalette palette, byte[] colors)
        {
            //map colors through the given palette
            PutPixels(colors.Select(c => palette[c]).ToArray());
        }

        public void PutPixels(Color[] color)
        {
            renderTarget.SetData(color);
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Rectangle? sourceRectangle = null)
        {
            spriteBatch.Draw(renderTarget, destinationRectangle, sourceRectangle, Color.White);
        }
    }
}
