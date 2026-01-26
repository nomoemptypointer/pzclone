using Game.Common.Windowing;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Game.Desktop
{
    public class DesktopWindow : AbstractWindow
    {
        private bool shownInit;

        public DesktopWindow()
        {
            Base = CreateSdlWindow();
        }

        private static Sdl2Window CreateSdlWindow()
        {
            WindowCreateInfo windowCI = new()
            {
                X = 100,
                Y = 100,
                WindowWidth = 960,
                WindowHeight = 540,
                WindowTitle = "",
                WindowInitialState = WindowState.Hidden
            };
            return VeldridStartup.CreateWindow(ref windowCI);
        }

        public override void Tick(GraphicsDevice gd)
        {
            while (!Core.Game.Instance.Initialized)
                Thread.Sleep(10);
            if (!shownInit)
                Show();

            Base.PumpEvents();
        }

        public override void Show()
        {
            shownInit = true;
            Base.Visible = true;
        }
    }
}
