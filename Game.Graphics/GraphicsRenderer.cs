using Game.Common.Windowing;
using Game.Graphics.Shaders;
using Veldrid;
using Veldrid.StartupUtilities;

namespace Game.Graphics
{
    public class GraphicsRenderer
    {
        public static GraphicsRenderer Singleton { get; private set; }
        public GraphicsDevice GraphicsDevice { get; internal set; }
        public AbstractWindow Window { get; set; }
        public ShaderManager ShaderManager { get; private set; }
        public Swapchain MainSwapchain { get; set; }
        public bool NoSwap { get; set; } = false;

        public GraphicsRenderer()
        {
            if (Singleton == null)
                Singleton = this;
            else
                throw new Exception();
        }

        public void Initialize()
        {
            CreateGraphicsDevice(false);
            //CreateResouces();
        }

        public void AttachExistingGraphicsDeviceAndroid(GraphicsDevice graphicsDevice)
        {
            if (OperatingSystem.IsAndroid())
                GraphicsDevice = graphicsDevice;
        }

        public void CreateGraphicsDevice(bool recreate = false) // Also handle android
        {
            if (recreate)
                GraphicsDevice.Dispose();

            GraphicsDeviceOptions options = new()
            {
                PreferStandardClipSpaceYDirection = true,
                PreferDepthRangeZeroToOne = true,
                SyncToVerticalBlank = false
            };

            GraphicsDevice = VeldridStartup.CreateGraphicsDevice(Window.Base, options);
            MainSwapchain = GraphicsDevice.MainSwapchain;

            Window.Base.Resized += () =>
            {
                Resize((uint)Window.Base.Width, (uint)Window.Base.Height);
            };
        }

        private void CreateResouces()
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
            {
                GraphicsDevice.SwapBuffers(MainSwapchain);
            }
        }
    }
}
