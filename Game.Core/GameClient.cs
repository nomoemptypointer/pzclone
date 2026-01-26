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
            SystemRegistry.Register(new GraphicsSystem());
            SystemRegistry.GetSystem<GraphicsSystem>().Initialize();
            base.Initialize();

            _player = new EcsEntity();

            AnnounceInitialized(); // Signals that the game has been initialized and should handle window stuff etc
        }

        public override void Shutdown()
        {
            base.Shutdown();
            // TODO: cleanup resources
        }
    }
}
