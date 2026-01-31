using Game.Core;

namespace Game.Desktop
{
    public class Program
    {
        static void Main() // TODO: We need to wrap this into some method outside Game.Desktop
        {
            Core.GameClient client = new();
            Core.GameServer server = new(); // Server creation here is only for early development to immediately skip game menu (I think)

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
