namespace Game.Core.ECS
{
    public abstract class EcsSystem
    {
        public bool Enabled { get; set; } = true;

        public void Update(double deltaTime)
        {
            if (Enabled)
                UpdateCore(deltaTime);
        }

        protected abstract void UpdateCore(double deltaTime);

        public void OnNewSceneLoaded() => OnNewSceneLoadedCore();

        protected virtual void OnNewSceneLoadedCore() { }
    }
}
