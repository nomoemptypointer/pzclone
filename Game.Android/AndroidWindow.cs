using Game.Common.Windowing;
using Game.Graphics;
using Veldrid;
using static SDL3.SDL;
using SDL = SDL3.SDL;

namespace Game.Android
{
    internal class AndroidWindow : AbstractWindow
    {
        private GraphicsDevice _gd;
        public bool AndroidSurfaceReady { get; private set; }

        public AndroidWindow()
            : base(CreateSdlWindow())
        { }

        private static nint CreateSdlWindow()
        {
            if (!Init(InitFlags.Video))
            {
                throw new Exception($"SDL could not initialize: {SDL.GetError()}");
            }

            WindowFlags flags = WindowFlags.Vulkan;

            //switch (graphicsBackend)
            //{
            //    case GraphicsBackend.Vulkan:
            //        flags |= WindowFlags.Vulkan;
            //        break;
            //    case GraphicsBackend.OpenGL:
            //    case GraphicsBackend.OpenGLES:
            //        flags |= WindowFlags.OpenGL;
            //        break;
            //    case GraphicsBackend.Metal:
            //        flags |= WindowFlags.Metal;
            //        break;
            //    default:
            //        break;
            //}

            var sdlw = CreateWindow("", 1920, 1080, flags);
            SwapchainSourceExtensions.BaseSDL3Android = sdlw;
            return sdlw;
        }

        public override void Run(GraphicsDevice gd)
        {
            _gd = GraphicsRenderer.Singleton.GraphicsDevice;

            while (true)
            {
                while (PollEvent(out var e))
                {
                    if (e.Type == (uint)EventType.WindowShown ||
                        e.Type == (uint)EventType.WindowResized ||
                        e.Type == (uint)EventType.WindowPixelSizeChanged)
                    {
                        AndroidSurfaceReady = true;
                        GraphicsRenderer.Singleton.CreateGraphicsDevice();
                    }
                    if (e.Type == (uint)EventType.WindowResized)
                    {
                        int width = e.Window.Data1;
                        int height = e.Window.Data2;
                        GraphicsRenderer.Singleton.Resize((uint)width, (uint)height);
                    }
                }

                GraphicsRenderer.Singleton.RenderTest();

                _gd.SwapBuffers();
                _gd.WaitForIdle();
            }
        }
    }
}
