using Game.Common.Windowing;
using Game.Desktop;
using Veldrid;

namespace Game.Graphics
{
    public class GraphicsRenderer
    {
        public GraphicsDevice GraphicsDevice { get; private set; }
        public Swapchain MainSwapchain { get; private set; }

        public GraphicsRenderer(GraphicsBackend backend = GraphicsBackend.Vulkan, uint width = 1280, uint height = 720)
        {
            var options = new GraphicsDeviceOptions(
                debug: true,
                swapchainDepthFormat: null, // no depth buffer by default
                syncToVerticalBlank: true, // bc my ears hurt from coil whine
                resourceBindingModel: ResourceBindingModel.Improved
            );

            GraphicsDevice = GraphicsDevice.CreateVulkan(options);
            MainSwapchain = GraphicsDevice.MainSwapchain;
        }

        public void BindSdlWindow(nint sdlWindow) // This is dumb
        {
            var source = SwapchainSourceExtensions.CreateSDL(sdlWindow);

            var scDesc = new SwapchainDescription(
                source,
                1920, // TODO: This shouldn't be like this
                1080,
                null,
                true
            );

            MainSwapchain = GraphicsDevice.ResourceFactory.CreateSwapchain(scDesc);
        }

        /// <summary>
        /// Presents the current frame.
        /// </summary>
        public void Present()
        {
            GraphicsDevice.SwapBuffers(MainSwapchain);
        }

        /// <summary>
        /// Resizes the main swapchain.
        /// </summary>
        public void Resize(uint width, uint height)
        {
            GraphicsDevice.ResizeMainWindow(width, height);
        }

        public static void RenderTest(GraphicsRenderer renderer) // This is dumb
        {
            var gd = renderer.GraphicsDevice;
            var sc = renderer.MainSwapchain;

            using var cl = gd.ResourceFactory.CreateCommandList();

            cl.Begin();

            cl.SetFramebuffer(sc.Framebuffer);
            cl.ClearColorTarget(0, RgbaFloat.CornflowerBlue);

            cl.End();

            gd.SubmitCommands(cl);
            renderer.Present();
        }
    }
}
