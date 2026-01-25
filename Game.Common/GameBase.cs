using System.Diagnostics;
using Game.Common;

namespace Game
{
    public abstract class GameBase
    {
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
            _accumulatedTime = 0f;

            while (Running)
            {
                float deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;
                _stopwatch.Restart();

                _accumulatedTime += deltaTime;

                // Fixed update step
                while (_accumulatedTime >= _targetFrameTime)
                {
                    try
                    {
                        Update(_targetFrameTime);
                    }
                    catch (Exception e)
                    {
                        LogException(e);
                    }
                    _accumulatedTime -= _targetFrameTime;
                }

                try
                {
                    Render();
                }
                catch (Exception e)
                {
                    LogException(e);
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
