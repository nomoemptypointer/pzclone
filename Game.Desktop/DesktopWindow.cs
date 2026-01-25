using Game.Common.Windowing;
using Game.Graphics;
using SDL3;
using Veldrid;
using static SDL3.SDL;

namespace Game.Desktop
{
    public class DesktopWindow : AbstractWindow
    {
        private GraphicsDevice _gd;

        public DesktopWindow(GraphicsBackend graphicsBackend)
                        : base(CreateSdlWindow(graphicsBackend))
        { }
        private static nint CreateSdlWindow(GraphicsBackend graphicsBackend)
        {
            WindowFlags flags = WindowFlags.Hidden | WindowFlags.Resizable;

            switch (graphicsBackend)
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
                throw new Exception($"SDL could not initialize: {SDL.GetError()}");
            }

            var window = CreateWindow(
                "",
                1920,
                1080,
                flags
            );

            ShowWindow(window);
            return window;
        }

        public override void Run(GraphicsDevice gd)
        {
            if (_gd == null) // TODO: This might be slow
                GraphicsRenderer.Singleton.CreateGraphicsDevice();

            _gd = GraphicsRenderer.Singleton.GraphicsDevice;

            while (true)
            {
                while (PollEvent(out var e))
                {
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
