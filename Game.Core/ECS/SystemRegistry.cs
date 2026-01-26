namespace Game.Core.ECS
{
    public sealed class SystemRegistry
    {
        private readonly Dictionary<Type, EcsSystem> _systems = [];

        // Register by concrete type (old behavior, still valid)
        public void Register<TSystem>(TSystem system)
            where TSystem : EcsSystem
        {
            system.AttachRegistry(this);
            _systems.Add(typeof(TSystem), system);
        }

        // Register by interface (preferred)
        public void Register<TInterface, TSystem>(TSystem system)
            where TSystem : EcsSystem, TInterface
        {
            system.AttachRegistry(this);
            _systems.Add(typeof(TInterface), system);
        }

        // Get by concrete type
        public TSystem GetSystem<TSystem>()
            where TSystem : EcsSystem
        {
            if (!_systems.TryGetValue(typeof(TSystem), out var system))
                throw new InvalidOperationException(
                    $"No system of type {typeof(TSystem).Name} was found.");

            return (TSystem)system;
        }

        // Get by interface
        public TInterface Get<TInterface>()
        {
            if (!_systems.TryGetValue(typeof(TInterface), out var system))
                throw new InvalidOperationException(
                    $"No system implementing {typeof(TInterface).Name} was found.");

            return (TInterface)(object)system;
        }

        public IEnumerable<EcsSystem> GetAllSystems()
            => _systems.Values;
    }
}
