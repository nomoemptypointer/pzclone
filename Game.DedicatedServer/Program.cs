using Game.Core;

namespace Game.DedicatedServer
{
    public static class Program
    {
        public static GameServer ServerInstance { get; private set; }

        static void Main()
        {
            ServerInstance = new();
            ServerInstance.Initialize();
            ServerInstance.Shutdown();
        }
    }
}