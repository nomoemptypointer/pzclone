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
            Game game = new();
            game.Initialize();

            dw.Run(graphicsRenderer.GraphicsDevice, () =>
            {
                // This lambda is called every frame by the window
                float deltaTime = graphicsRenderer.DeltaTime; // Get time between frames

                // Update game logic
                game.Update(deltaTime);

                // Render the game
                game.Render();
            });

            game.Shutdown();
        }
    }
}
