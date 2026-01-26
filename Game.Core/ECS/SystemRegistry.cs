namespace Game.Core.ECS
{
    public class SystemRegistry
    {
        private Dictionary<Type, EcsSystem> _systems = [];

        public T GetSystem<T>() where T : EcsSystem
        {
            if (!_systems.TryGetValue(typeof(T), out EcsSystem gs))
                throw new InvalidOperationException($"No system of type {typeof(T).Name} was found.");

            return (T)gs;
        }

        public void Register<T>(T system) where T : EcsSystem => _systems.Add(typeof(T), system);
        public Dictionary<Type, EcsSystem>.Enumerator GetSystemsEnumerator() => _systems.GetEnumerator();
    }
}
