using Game.Common.Windowing;
using Game.Graphics;
using SDL3;
using Veldrid;
using static SDL3.SDL;

namespace Game.Desktop
{
    public class DesktopWindow : AbstractWindow
    {
        private bool shownInit;

        public DesktopWindow()
        {
            BaseSDL3 = CreateSdlWindow();
        }

        private nint CreateSdlWindow(GraphicsBackend graphicsBackend = GraphicsBackend.Vulkan)
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
                throw new Exception($"SDL could not initialize: {SDL.GetError()}");

            var window = CreateWindow(
                "",
                1920,
                1080,
                flags
            );

            return window;
        }

        public override void Tick(GraphicsDevice gd)
        {
            while (!Core.Game.Instance.Initialized)
                Thread.Sleep(10);
            if (!shownInit)
                Show();

            while (PollEvent(out var e))
            {
                if (e.Type == (uint)EventType.WindowResized)
                {
                    int width = e.Window.Data1;
                    int height = e.Window.Data2;
                    GraphicsRenderer.Singleton.Resize((uint)width, (uint)height);
                }
            }
        }

        public override void Show()
        {
            shownInit = true;
            ShowWindow(BaseSDL3);
        }
    }
}
