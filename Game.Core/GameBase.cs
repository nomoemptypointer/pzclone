using Game.Core.Diagnostics;
using Game.Core.ECS;
using Game.Graphics;
using System.Diagnostics;

namespace Game.Core
{
    public abstract class GameBase
    {
        public bool Initialized { get; private set; } = false;
        public bool Running { get; private set; } = false;
        public SystemRegistry SystemRegistry { get; } = new SystemRegistry();

        private readonly List<GameObject> _gameObjects = [];
        private readonly List<GameObject> _destroyList = [];
        private Action _renderAction;

        public double TargetFrameTime
        {
            get => _targetFrameTime;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Frame time must be positive.");
                _targetFrameTime = value;
            }
        }
        public double TargetFramesPerSecond
        {
            get => 1.0 / _targetFrameTime;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "FPS must be positive.");
                _targetFrameTime = 1.0 / value;
            }
        }

        public double DeltaTime { get; private set; } = 0;

        private Stopwatch _stopwatch;
        private double _accumulatedTime;
        private double _targetFrameTime = 0;

        protected GameBase()
        {
            Console.WriteLine("protected GameBase ctor called");
            SystemRegistry.Register(new GameObjectQuerySystem(_gameObjects));
            GameObject.InternalConstructed += OnGameObjectConstructed;
            GameObject.InternalDestroyRequested += OnGameObjectDestroyRequested;
            GameObject.InternalDestroyCommitted += OnGameObjectDestroyCommitted;
        }

        public virtual void Initialize()
        {
            if (!OperatingSystem.IsAndroid()) // TODO: Is this still required?
                GraphicsRenderer.Singleton.Initialize();
        }

        public virtual void Shutdown()
        {

        }

        protected void LogException(Exception e) => CrashLogHelper.LogUnhandledException(e, this);

        protected void FlushDeletedObjects()
        {
            foreach (GameObject go in _destroyList)
                go.CommitDestroy();
            _destroyList.Clear();
        }

        private void OnGameObjectConstructed(GameObject go)
        {
            go.SetRegistry(SystemRegistry);
            lock (_gameObjects)
                _gameObjects.Add(go);
        }

        private void OnGameObjectDestroyRequested(GameObject go)
        {
            _destroyList.Add(go);
        }

        private void OnGameObjectDestroyCommitted(GameObject go)
        {
            lock (_gameObjects)
                _gameObjects.Remove(go);
        }

        public void AnnounceInitialized() => Initialized = true;

        public void SetRenderAction(Action action)
        {
            if (_renderAction != null) throw new Exception("Render action is already set");
            _renderAction = action;
        }

        // Main game loop
        public void Run(params Action[] perFrameActions)
        {
#if RELEASE
            try
            {
#endif
            Initialize();
#if RELEASE
            }
            catch (Exception e)
            {
                LogException(e);
                return;
            }
#endif

            Running = true;
            _stopwatch = Stopwatch.StartNew();

            while (Running)
            {
                double frameStart = _stopwatch.Elapsed.TotalSeconds;

                DeltaTime = frameStart - _accumulatedTime;
                _accumulatedTime = frameStart;

#if RELEASE
                try
                {
#endif
                FlushDeletedObjects();


                    var systemEnumerator = SystemRegistry.GetSystemsEnumerator();
                    while (systemEnumerator.MoveNext())
                    {
                        var kvp = systemEnumerator.Current;
                        GameSystem system = kvp.Value;
                        system.Update(DeltaTime);
                    }
#if RELEASE
                }
                catch (Exception e)
                {
                    LogException(e);
                }
#endif

                // Render
                try
                {
                    _renderAction?.Invoke();
                }
                catch (Exception e)
                {
                    LogException(e);
                }

                foreach (var action in perFrameActions)
                {
                    try
                    {
                        action?.Invoke();
                    }
                    catch (Exception e)
                    {
                        LogException(e);
                    }
                }

                double frameEnd = _stopwatch.Elapsed.TotalSeconds;
                double frameTime = frameEnd - frameStart;
                double sleepTime = TargetFrameTime - frameTime;

                if (sleepTime > 0)
                {
                    int sleepMs = (int)(sleepTime * 1000);
                    if (sleepMs > 0)
                        Thread.Sleep(sleepMs);
                }
            }

            try
            {
                Shutdown();
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        public void Exit()
        {
            Running = false;
        }
    }
}
