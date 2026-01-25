using Game.Core.Diagnostics;
using System.Diagnostics;

namespace Game.Core
{
    public abstract class GameBase
    {
        public bool Initialized { get; protected set; } = false;
        public bool Running { get; private set; } = false;

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
        private double _targetFrameTime;

        protected GameBase()
        {
            
        }

        public abstract void Initialize();
        public abstract void Update(double deltaTime);
        public abstract void Render();
        public abstract void Shutdown();

        protected void LogException(Exception e) => CrashLogHelper.LogUnhandledException(e, this);

        // Main game loop
        public void Run()
        {
            try
            {
                Initialize();
            }
            catch (Exception e)
            {
                LogException(e);
                return;
            }

            Running = true;
            _stopwatch = Stopwatch.StartNew();

            while (Running)
            {
                double frameStart = _stopwatch.Elapsed.TotalSeconds;

                // Calculate deltaTime (variable)
                DeltaTime = frameStart - _accumulatedTime;
                _accumulatedTime = frameStart;

                // Update with variable deltaTime
                try
                {
                    Update(DeltaTime);
                }
                catch (Exception e)
                {
                    LogException(e);
                }

                // Render
                try
                {
                    Render();
                }
                catch (Exception e)
                {
                    LogException(e);
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
