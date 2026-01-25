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
            Core.Game game = new();
            game.Initialize();

            dw.Run(graphicsRenderer.GraphicsDevice, () =>
            {
                game.Update(game.DeltaTime);
                game.Render();
            });

            game.Shutdown();
        }
    }
}
