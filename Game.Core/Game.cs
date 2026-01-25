using Game.Core.ECS;

namespace Game.Core
{
    public class Game : GameBase
    {
        public static Game Instance { get; private set; }

        private GameObject _player;

        public Game()
        {
            if (Instance != null)
                throw new InvalidOperationException("Game instance already exists!");

            Instance = this;
        }

        public override void Initialize()
        {
            _player = new GameObject();

            AnnounceInitialized(); // Signals that the game has been initialized and should handle window stuff etc
        }

        public override void Shutdown()
        {
            // TODO: cleanup resources
        }
    }
}
