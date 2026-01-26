using Game.Graphics;
using System.Reflection;
using Veldrid;

namespace Game.Desktop
{
    public class Program
    {
        static void Main() // TODO: We need to wrap this into some method outside Game.Desktop
        {
            foreach (var name in Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                Console.WriteLine(name);
            }

            DesktopWindow dw = new();

            GraphicsRenderer graphicsRenderer = new()
            {
                Window = dw
            };

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
