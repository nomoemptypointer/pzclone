namespace Game.Core.ECS
{
    public abstract class GameSystem
    {
        public bool Enabled { get; set; } = true;

        public void Update(double deltaSeconds)
        {
            if (Enabled)
                UpdateCore(deltaSeconds);
        }

        protected abstract void UpdateCore(double deltaSeconds);

        public void OnNewSceneLoaded() => OnNewSceneLoadedCore();

        protected virtual void OnNewSceneLoadedCore() { }
    }
}
