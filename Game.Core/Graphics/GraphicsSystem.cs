using Game.Common;
using Game.Core.ECS;
using Game.Core.Graphics.Shaders;
using System.Runtime.Versioning;
using Veldrid;
using Veldrid.StartupUtilities;

namespace Game.Core.Graphics
{
    public class GraphicsSystem : EcsSystem
    {
        public GraphicsDevice GraphicsDevice { get; internal set; }
        public IWindow Window => SystemRegistry.Get<IWindow>(); // TODO: Might be slow, assign once to field
        public ShaderRegistry ShaderManager { get; private set; }
        public RenderQueue RenderQueue { get; private set; }
        public Swapchain MainSwapchain { get; internal set; }
        public bool NoSwap { get; set; } = false;

        private readonly GraphicsSystemTaskScheduler _taskScheduler;

        public GraphicsSystem()
        {
            int mainThreadId = Environment.CurrentManagedThreadId;
            _taskScheduler = new GraphicsSystemTaskScheduler(mainThreadId);
            Console.WriteLine("GraphicsSystem::mainThreadId=" + mainThreadId);
            RenderQueue = new();
        }

        [UnsupportedOSPlatform("android", "You have to initialize GraphicsSystem manually on mobile platforms, this method is for desktop only releases. Call CreateGraphicsDevice with correct arguments and then rest!!!!")]
        public void InitializeAll()
        {
            CreateGraphicsDevice();
            CreateResources();
        }

        public void CreateGraphicsDevice(bool recreate = false, AndroidCreateGraphicsDeviceModel? acgdm = null)
        {
            if (recreate)
                GraphicsDevice.Dispose();

            GraphicsDeviceOptions options = new()
            {
                PreferStandardClipSpaceYDirection = true,
                PreferDepthRangeZeroToOne = true,
                SyncToVerticalBlank = false
            };

            if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
            {
                GraphicsDevice = VeldridStartup.CreateGraphicsDevice(Window.Base, options);
                MainSwapchain = GraphicsDevice.MainSwapchain;

                Window.Resized += () =>
                {
                    Resize((uint)Window.Width, (uint)Window.Height);
                };
            }
            else if (OperatingSystem.IsAndroid())
            {
                if (acgdm == null)
                    throw new ArgumentException("ACGDM is null for Android initialization.");

                var model = acgdm.Value;

                GraphicsDevice = GraphicsDevice.CreateVulkan(options);

                var ss = SwapchainSource.CreateAndroidSurface(model.HolderHandle, model.JNIEnvHandle);
                var sd = new SwapchainDescription(
                    ss,
                    model.Width,
                    model.Height,
                    options.SwapchainDepthFormat,
                    options.SyncToVerticalBlank
                );

                MainSwapchain = GraphicsDevice.ResourceFactory.CreateSwapchain(sd);
            }
        }

        private void CreateResources()
        {
            if (GraphicsDevice == null)
                throw new Exception("Can't create resources when GraphicsDevice was not yet created");

            ShaderManager = new(GraphicsDevice.ResourceFactory);
        }

        /// <summary>
        /// Resizes the main swapchain.
        /// </summary>
        public void Resize(uint width, uint height)
        {
            if (width == 0 || height == 0 || GraphicsDevice == null || GraphicsDevice.MainSwapchain == null)
                return;

            GraphicsDevice.ResizeMainWindow(width, height);
        }

        protected override void UpdateCore(double deltaTime)
        {
            if (GraphicsDevice != null && MainSwapchain != null)
            {
                using var cl = GraphicsDevice.ResourceFactory.CreateCommandList();

                cl.Begin();

                // 1. Clear framebuffer first
                cl.SetFramebuffer(MainSwapchain.Framebuffer);
                cl.ClearColorTarget(0, RgbaFloat.Blue);

                // 2. Render all render items
                RenderQueue.RenderAll(cl);

                cl.End();

                // 3. Submit once
                GraphicsDevice.SubmitCommands(cl);

                // 4. Swap buffers if needed
                if (!NoSwap)
                    GraphicsDevice.SwapBuffers(MainSwapchain);

                // 5. Clear queue if using per-frame submission
                RenderQueue.Clear();
            }
        }

    }
}
