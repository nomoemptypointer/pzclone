using Game.Core;

namespace Game.Desktop
{
    public class Program
    {
        static void Main() // TODO: We need to wrap this into some method outside Game.Desktop
        {
            DesktopWindow dw = new();
            Core.GameClient client = new();
            Core.GameServer server = new();

            var serverThread = new Thread(() =>
            {
                server.Initialize();
                server.Shutdown();
            })
            { IsBackground = true };

            client.Initialize();
            client.Shutdown();
        }
    }
}
