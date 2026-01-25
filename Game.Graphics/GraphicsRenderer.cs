using Game.Common.Windowing;
using Veldrid;

namespace Game.Graphics
{
    public class GraphicsRenderer
    {
        public static GraphicsRenderer Singleton { get; private set; }
        public GraphicsDevice GraphicsDevice { get; private set; }
        public AbstractWindow Window { get; private set; }

        private static readonly Random _rng = new();

        public GraphicsRenderer(AbstractWindow window, GraphicsBackend backend = GraphicsBackend.Vulkan)
        {
            if (Singleton == null)
                Singleton = this;
            else
                throw new Exception();

            Window = window;
            //CreateGraphicsDevice();
        }

        public void CreateGraphicsDevice(bool recreate = false)
        {
            if (recreate)
                GraphicsDevice.Dispose();
            // TODO: Check if gd is disposed and throw exception 

            var options = new GraphicsDeviceOptions(
                debug: true,
                swapchainDepthFormat: null, // no depth buffer by default
                syncToVerticalBlank: true, // bc my ears hurt from coil whine
                resourceBindingModel: ResourceBindingModel.Improved
            );

            if (Window.BaseSDL3 == 0)
                throw new Exception("Window handle is null (CreateGraphicsDevice)"); // never gets fired because window exists

            var source = SwapchainSourceExtensions.CreateSDL(Window.BaseSDL3); // passed Window.BaseSDL3 here exists but inside the method it is null/zero

            var swapchainDesc = new SwapchainDescription(
                source,
                1920,
                1080,
                null,
                true // vsync
            );

            GraphicsDevice = GraphicsDevice.CreateVulkan(
                options,
                swapchainDesc
            );
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

        public void RenderTest()
        {
            var gd = GraphicsDevice;
            var sc = gd.MainSwapchain;

            using var cl = gd.ResourceFactory.CreateCommandList();

            // Random color each frame
            var randomColor = new RgbaFloat(
                (float)_rng.NextDouble(),
                (float)_rng.NextDouble(),
                (float)_rng.NextDouble(),
                1f
            );

            cl.Begin();

            cl.SetFramebuffer(sc.Framebuffer);
            cl.ClearColorTarget(0, randomColor);

            cl.End();

            gd.SubmitCommands(cl);
            gd.SwapBuffers(sc);
        }
    }
}
