using Android.Content;
using Android.Runtime;
using Android.Views;
using Game.Graphics;
using Veldrid;
using Android.Graphics;

namespace Game.Android
{
    internal class VeldridSurfaceView : SurfaceView, ISurfaceHolderCallback
    {
        private GraphicsRenderer _renderer;
        private bool _running;
        private CancellationTokenSource _cts;

        public event Action OnGraphicsDeviceCreated;

        public VeldridSurfaceView(Context context, GraphicsRenderer renderer) : base(context)
        {
            _renderer = renderer;
            Holder.AddCallback(this);
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            GraphicsDeviceOptions options = new()
            {
                Debug = false,
                PreferStandardClipSpaceYDirection = true,
                PreferDepthRangeZeroToOne = true,
                SyncToVerticalBlank = false
            };

            var gd = GraphicsDevice.CreateVulkan(options);
            _renderer.AttachExistingGraphicsDeviceAndroid(gd);

            // Create the Swapchain for Android
            SwapchainSource ss = SwapchainSource.CreateAndroidSurface(holder.Surface.Handle, JNIEnv.Handle);
            SwapchainDescription sd = new(
                ss,
                (uint)Width,
                (uint)Height,
                null,
                options.SyncToVerticalBlank
            );
            _renderer.MainSwapchain = gd.ResourceFactory.CreateSwapchain(sd);


            // Fire the event so MainActivity can start the game loop
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
