namespace Game.Core.ECS
{
    public class GameObjectQuerySystem : EcsSystem
    {
        private readonly IReadOnlyList<EcsEntity> _gameObjects;

        public GameObjectQuerySystem(IReadOnlyList<EcsEntity> gameObjectList) => _gameObjects = gameObjectList;

        protected override void UpdateCore(double deltaTime) { }

        public EcsEntity FindByName(string name) => _gameObjects.FirstOrDefault(go => go.Name == name);

        public IEnumerable<EcsEntity> GetUnparentedGameObjects() => _gameObjects.Where(go => go.Transform.Parent == null);

        public IEnumerable<EcsEntity> GetAllGameObjects() => _gameObjects;

        public string GetCloneName(string name)
        {
            while (FindByName(name) != null)
                name += " (Clone)";

            return name;
        }
    }
}
