using Game.Core.ECS;

namespace Game.Core
{
    public class GameClient : GameBase
    {
        public static GameClient Instance { get; private set; }

        private GameObject _player;

        public GameClient()
        {
            if (Instance != null)
                throw new InvalidOperationException("Game instance already exists!");

            Instance = this;
        }

        public override void Initialize()
        {
            base.Initialize();

            _player = new GameObject();

            AnnounceInitialized(); // Signals that the game has been initialized and should handle window stuff etc
        }

        public override void Shutdown()
        {
            base.Shutdown();
            // TODO: cleanup resources
        }
    }
}
