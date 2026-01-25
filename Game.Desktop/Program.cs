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

            dw.Tick(graphicsRenderer.GraphicsDevice); // Needs to be called by game.Run();
            game.TargetFramesPerSecond = double.MaxValue;
            game.SetRenderAction(graphicsRenderer.Render);

            game.Run(
                () => dw.Tick(graphicsRenderer.GraphicsDevice),
                () => Console.WriteLine($"DeltaTime: {game.DeltaTime:F4}s") // example
            );

            game.Shutdown();
        }
    }
}
