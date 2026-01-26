using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Game.Desktop
{
    public class DesktopWindow
    {
        public Sdl2Window Base { get; private set; }

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

        public void Tick()
        {
            while (!Core.GameClient.Instance.Initialized)
                Thread.Sleep(10);
            if (!shownInit)
                ShowOnceInitialized();

            Base.PumpEvents();
        }

        private void ShowOnceInitialized()
        {
            shownInit = true;
            Base.Visible = true;
            Base.WindowState = WindowState.BorderlessFullScreen;
        }
    }
}
