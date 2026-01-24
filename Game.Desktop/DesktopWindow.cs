using Game.Common.Windowing;
using Veldrid;
using static SDL3.SDL;

namespace Game.Desktop
{
    public class DesktopWindow : AbstractWindow
    {
        public nint BaseSDL3 { get; private set; }

        private GraphicsDevice _gd;

        public DesktopWindow(GraphicsDevice gd)
        {
            _gd = gd;

            WindowFlags flags = 0;

            switch (_gd.BackendType)
            {
                case GraphicsBackend.Vulkan:
                    flags |= WindowFlags.Vulkan;
                    break;
                case GraphicsBackend.OpenGL:
                case GraphicsBackend.OpenGLES:
                    flags |= WindowFlags.OpenGL;
                    break;
                case GraphicsBackend.Metal:
                    flags |= WindowFlags.Metal;
                    break;
                default:
                    break;
            }

            if (!Init(InitFlags.Video))
            {
                return; // TODO: Throw exception
            }

            BaseSDL3 = CreateWindow(
                "",
                1920,
                1080,
                flags
            );

            //Run();
        }

        public override void Run()
        {
            while (true)
            {
                while (PollEvent(out var e))
                {
                    switch (e)
                    {

                    }
                }

                //_gd.SwapBuffers();
                _gd.WaitForIdle();
            }
        }
    }
}
