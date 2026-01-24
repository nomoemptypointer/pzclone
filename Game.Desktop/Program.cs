using Game.Graphics;

namespace Game.Desktop
{
    public class Program
    {
        static void Main()
        {
            GraphicsRenderer graphicsRenderer = new();
            DesktopWindow dw = new(graphicsRenderer.GraphicsDevice);
            dw.Run();
        }
    }
}