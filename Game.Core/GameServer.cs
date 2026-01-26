namespace Game.Core
{
    public class GameServer : GameBase
    {
        public GameServer()
        {
            TargetFramesPerSecond = 480;
        }

        public override void Initialize()
        {
            base.Initialize();

            AnnounceInitialized(); // Signals that the game has been initialized and should handle window stuff etc
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }
    }
}
