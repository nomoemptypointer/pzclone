using Game.Graphics;

namespace Game.Desktop
{
    public class Program
    {
        static void Main() // TODO: We need to wrap this into some method outside Game.Desktop
        {
            DesktopWindow dw = new();
            GraphicsRenderer graphicsRenderer = new() { Window = dw.Base };
            Core.GameClient client = new();
            Core.GameServer server = new();

            var serverThread = new Thread(() =>
            {
                server.Initialize();
                server.Run();
                server.Shutdown();
            })
            { IsBackground = true };

            client.Initialize();

            client.SetRenderAction(graphicsRenderer.Render);
            client.Run(dw.Tick);

            client.Shutdown();
        }
    }
}
