namespace Game
{
    public class Game : GameBase
    {
        public static Game Instance { get; private set; }

        public Game()
        {
            if (Instance != null)
                throw new InvalidOperationException("Game instance already exists!");

            Instance = this;
        }

        public override void Initialize()
        {
            // TODO: load resources, setup game state
        }

        public override void Update(double deltaTime)
        {
            // TODO: update game logic using deltaTime
        }

        public override void Render()
        {
            // TODO: render your game
        }

        public override void Shutdown()
        {
            // TODO: cleanup resources
        }
    }
}
