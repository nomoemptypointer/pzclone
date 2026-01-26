namespace Game.Core.ECS
{
    public abstract class EcsSystem
    {
        public bool Enabled { get; set; } = true;

        internal SystemRegistry SystemRegistry { get; private set; }

        internal void AttachRegistry(SystemRegistry registry) => SystemRegistry = registry;

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
