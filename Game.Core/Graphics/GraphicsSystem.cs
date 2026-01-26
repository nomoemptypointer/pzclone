using Game.Common;
using Game.Core.ECS;
using Game.Core.Graphics.Shaders;
using Veldrid;
using Veldrid.StartupUtilities;

namespace Game.Core.Graphics
{
    public class GraphicsSystem : EcsSystem
    {
        public GraphicsDevice GraphicsDevice { get; internal set; }
        public IWindow Window => SystemRegistry.Get<IWindow>();
        public ShaderManager ShaderManager { get; private set; }
        public Swapchain MainSwapchain { get; internal set; }
        public bool NoSwap { get; set; } = false;

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

        public void Render()
        {
            using var cl = GraphicsDevice.ResourceFactory.CreateCommandList();

            cl.Begin();

            cl.SetFramebuffer(MainSwapchain.Framebuffer);
            cl.ClearColorTarget(0, RgbaFloat.Blue);

            cl.End();

            GraphicsDevice.SubmitCommands(cl);

            GraphicsDevice.WaitForIdle();

            if (!NoSwap)
                GraphicsDevice.SwapBuffers(MainSwapchain);
        }

        protected override void UpdateCore(double deltaTime)
        {
            if (GraphicsDevice != null || MainSwapchain != null)
                Render();
        }
    }
}
