using Game.Core.Diagnostics;
using Game.Core.ECS;
using System.Diagnostics;

namespace Game.Core
{
    public abstract class GameBase
    {
        public bool Running { get; private set; } = false;
        public SystemRegistry SystemRegistry { get; } = new SystemRegistry();

        private readonly List<EcsEntity> _entites = [];
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
            EcsEntity.InternalConstructed += OnEntityConstructed;
            EcsEntity.InternalDestroyRequested += OnEntityDestroyRequested;
            EcsEntity.InternalDestroyCommitted += OnEntityDestroyCommitted;
        }

        public virtual void Initialize()
        {
            //SystemRegistry.Register(new EntityQuerySystem(_entites));
        }

        public virtual void Shutdown()
        {
            Running = false;
        }

        private void FlushDeletedEntities()
        {
            foreach (EcsEntity entity in _destroyList)
                entity.CommitDestroy();
            _destroyList.Clear();
        }

        private void OnEntityConstructed(EcsEntity entity)
        {
            entity.SetRegistry(SystemRegistry);
            lock (_entites)
                _entites.Add(entity);
        }

        private void OnEntityDestroyRequested(EcsEntity entity) => _destroyList.Add(entity);

        private void OnEntityDestroyCommitted(EcsEntity entity) { lock (_entites) _entites.Remove(entity); }

        protected void AnnounceInitialized()
        {
            Running = true;
            RunLoop();
        }

        private void RunLoop()
        {
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
                Console.WriteLine("tick");
                FlushDeletedEntities();
                foreach (var system in SystemRegistry.GetAllSystems())
                    system.Update(DeltaTime);
#if RELEASE
        }
        catch (Exception e)
        {
            CrashLogHelper.LogUnhandledException(e);
        }
#endif

                // Frame limiting
                double frameEnd = _stopwatch.Elapsed.TotalSeconds;
                double frameTime = frameEnd - frameStart;
                double sleepTime = TargetFrameTime - frameTime;

                if (sleepTime > 0)
                {
                    int sleepMs = (int)(sleepTime * 1000) - 1; // coarse sleep
                    if (sleepMs > 0)
                        Thread.Sleep(sleepMs);

                    double targetEnd = frameStart + TargetFrameTime;
                    while (_stopwatch.Elapsed.TotalSeconds < targetEnd)
                    {
                        // spin wait for sub-millisecond precision
                    }
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
