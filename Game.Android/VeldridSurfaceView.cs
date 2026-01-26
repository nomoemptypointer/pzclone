using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Graphics;
using Game.Common;
using Game.Core.Graphics;

namespace Game.Android
{
    internal class VeldridSurfaceView : SurfaceView, ISurfaceHolderCallback
    {
        private GraphicsSystem _renderer;
        private bool _running;
        private CancellationTokenSource _cts;

        public event Action OnGraphicsDeviceCreated;

        public VeldridSurfaceView(Context context, GraphicsSystem renderer) : base(context)
        {
            _renderer = renderer;
            Holder.AddCallback(this);
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            var model = new AndroidCreateGraphicsDeviceModel(
                holderHandle: holder.Surface.Handle,
                jniEnvHandle: JNIEnv.Handle,
                width: (uint)Width,
                height: (uint)Height
            );

            _renderer.CreateGraphicsDevice(recreate: false, acgdm: model);

            OnGraphicsDeviceCreated?.Invoke();
        }


        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            PauseRenderingLoop();
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            _renderer.Resize((uint)width, (uint)height);
        }

        public void StartRenderingLoop(Action renderAction)
        {
            if (_running) return;

            _cts = new CancellationTokenSource();
            _running = true;

            Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    renderAction?.Invoke();

                    // Simple ~60fps
                    await Task.Delay(16, _cts.Token);
                }
            });
        }

        public void PauseRenderingLoop()
        {
            _cts?.Cancel();
            _running = false;
        }

        public void ResumeRenderingLoop(Action renderAction)
        {
            if (!_running)
            {
                StartRenderingLoop(renderAction);
            }
        }
    }
}
