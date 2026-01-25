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

        public override void Run(GraphicsDevice gd)
        {
            _gd = gd;

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
