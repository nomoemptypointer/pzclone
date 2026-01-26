using Game.Common.Windowing;
using Game.Graphics.Shaders;
using Veldrid;

namespace Game.Graphics
{
    public class GraphicsRenderer
    {
        public static GraphicsRenderer Singleton { get; private set; }
        public GraphicsDevice GraphicsDevice { get; private set; }
        public AbstractWindow Window { get; set; }
        public ShaderManager ShaderManager { get; private set; }
        public bool FastFrame { get; private set; } = false;

        public GraphicsRenderer(GraphicsBackend backend = GraphicsBackend.Vulkan)
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

        public void CreateGraphicsDevice(bool recreate = false)
        {
            if (recreate)
                GraphicsDevice.Dispose();

            var options = new GraphicsDeviceOptions(
                debug: true,
                swapchainDepthFormat: null, // no depth buffer by default
                syncToVerticalBlank: false,
                resourceBindingModel: ResourceBindingModel.Improved
            );

            if (Window.BaseSDL3 == 0)
                throw new Exception("Window handle is null (CreateGraphicsDevice)"); // never gets fired because window exists here (?)

            SwapchainSource source = SwapchainSourceExtensions.CreateSDL(Window.BaseSDL3); // BUG (Android): Passed Window.BaseSDL3 from here exists but inside the method it is null/zero

            var swapchainDesc = new SwapchainDescription(
                source,
                1920,
                1080,
                null,
                false // vsync
            );

            GraphicsDevice = GraphicsDevice.CreateVulkan( // TODO: Legacy backend (opengl/es)
                options,
                swapchainDesc
            );
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
            if (width == 0 || height == 0)
                return; // Prevent backend from blowing up

            GraphicsDevice.ResizeMainWindow(width, height);
        }

        public void Render()
        {
            var gd = GraphicsDevice;
            var sc = gd.MainSwapchain;

            using var cl = gd.ResourceFactory.CreateCommandList();

            cl.Begin();

            cl.SetFramebuffer(sc.Framebuffer);
            cl.ClearColorTarget(0, RgbaFloat.Blue);

            cl.End();

            gd.SubmitCommands(cl);

            if (!FastFrame)
                gd.SwapBuffers(sc);
        }
    }
}
