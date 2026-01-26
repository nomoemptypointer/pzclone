using Game.Core.ECS;
using Game.Core.Graphics;

namespace Game.Core
{
    public class GameClient : GameBase
    {
        public static GameClient Instance { get; private set; }

        private EcsEntity _player;

        public GameClient()
        {
            if (Instance != null)
                throw new InvalidOperationException("Game instance already exists!");

            Instance = this;
        }

        public override void Initialize()
        {
            if (!OperatingSystem.IsAndroid())
                SystemRegistry.Register<IWindow, DesktopWindow>(new DesktopWindow());

            var graphicsSystem = new GraphicsSystem();
            SystemRegistry.Register(graphicsSystem);

            if (!OperatingSystem.IsAndroid())
                graphicsSystem.InitializeAll();

            base.Initialize();

            _player = new EcsEntity();

            AnnounceInitialized();
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }
    }
}
