using Game.Core.ECS;
using Game.Core.ECS.Systems;
using Game.Core.Graphics;
using Game.Core.Graphics.Components;

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
            base.Initialize();
            var window = new DesktopWindowSystem();
            SystemRegistry.Register<IWindow, DesktopWindowSystem>(window);
            var graphicsSystem = new GraphicsSystem();
            SystemRegistry.Register(graphicsSystem);

            if (!OperatingSystem.IsAndroid())
                graphicsSystem.InitializeAll();

            SystemRegistry.Register(new InputSystem(window));

            // -- game stuff now
            _player = new EcsEntity();
            _player.AddComponent(new Text2D());

            AnnounceInitialized();
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }
    }
}
