namespace Game.Core.ECS
{
    public class GameObjectQuerySystem : GameSystem
    {
        private readonly IReadOnlyList<GameObject> _gameObjects;

        public GameObjectQuerySystem(IReadOnlyList<GameObject> gameObjectList) => _gameObjects = gameObjectList;

        protected override void UpdateCore(double deltaSeconds) { }

        public GameObject FindByName(string name) => _gameObjects.FirstOrDefault(go => go.Name == name);

        public IEnumerable<GameObject> GetUnparentedGameObjects() => _gameObjects.Where(go => go.Transform.Parent == null);

        public IEnumerable<GameObject> GetAllGameObjects() => _gameObjects;

        public string GetCloneName(string name)
        {
            while (FindByName(name) != null)
                name += " (Clone)";

            return name;
        }
    }
}
