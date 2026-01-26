namespace Game.Core.ECS.Systems
{
    public class EntityQuerySystem(IReadOnlyList<EcsEntity> gameObjectList) : EcsSystem
    {
        private readonly IReadOnlyList<EcsEntity> _entities = gameObjectList;

        protected override void UpdateCore(double deltaTime) { }

        public EcsEntity FindByName(string name) => _entities.FirstOrDefault(go => go.Name == name);

        public IEnumerable<EcsEntity> GetUnparentedGameObjects() => _entities.Where(go => go.Transform.Parent == null);

        public IEnumerable<EcsEntity> GetAllGameObjects() => _entities;

        public string GetCloneName(string name)
        {
            while (FindByName(name) != null)
                name += " (Clone)";
            return name;
        }
    }
}
