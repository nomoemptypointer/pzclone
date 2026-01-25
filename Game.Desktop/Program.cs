using Game.Graphics;
using Veldrid;

namespace Game.Desktop
{
    public class Program
    {
        static void Main()
        {
            DesktopWindow dw = new(GraphicsBackend.Vulkan);
            GraphicsRenderer graphicsRenderer = new(dw);
            dw.Run(graphicsRenderer.GraphicsDevice);
        }
    }
}