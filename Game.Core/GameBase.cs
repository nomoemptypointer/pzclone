using Game.Core.Diagnostics;
using Game.Core.ECS;
using Game.Core.Graphics;
using System.Diagnostics;

namespace Game.Core
{
    public abstract class GameBase
    {
        public bool Running { get; private set; } = false;
        public SystemRegistry SystemRegistry { get; } = new SystemRegistry();

        private readonly List<EcsEntity> _gameObjects = [];
        private readonly List<EcsEntity> _destroyList = [];

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
            EcsEntity.InternalConstructed += OnGameObjectConstructed;
            EcsEntity.InternalDestroyRequested += OnGameObjectDestroyRequested;
            EcsEntity.InternalDestroyCommitted += OnGameObjectDestroyCommitted;
        }

        public virtual void Initialize()
        {

        }

        public virtual void Shutdown()
        {
            Running = false;
        }

        protected void FlushDeletedObjects()
        {
            foreach (EcsEntity go in _destroyList)
                go.CommitDestroy();
            _destroyList.Clear();
        }

        private void OnGameObjectConstructed(EcsEntity go)
        {
            go.SetRegistry(SystemRegistry);
            lock (_gameObjects)
                _gameObjects.Add(go);
        }

        private void OnGameObjectDestroyRequested(EcsEntity go)
        {
            _destroyList.Add(go);
        }

        private void OnGameObjectDestroyCommitted(EcsEntity go)
        {
            lock (_gameObjects)
                _gameObjects.Remove(go);
        }

        public void AnnounceInitialized()
        {
            RunLoop();
        }

        public void RunLoop()
        {
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


                foreach (var system in SystemRegistry.GetAllSystems())
                {
                    system.Update(DeltaTime);
                }
#if RELEASE
                }
                catch (Exception e)
                {
                    CrashLogHelper.LogUnhandledException(e);
                }
#endif

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
                CrashLogHelper.LogUnhandledException(e);
            }
        }
    }
}
