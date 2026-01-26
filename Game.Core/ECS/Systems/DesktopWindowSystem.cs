using Game.Core.ECS;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Game.Core.ECS.Systems
{
    public class DesktopWindowSystem : EcsSystem, IWindow
    {
        public Sdl2Window Base { get; private set; }

        public int Width => Base.Width;

        public int Height => Base.Height;

        public bool Visible { get => Base.Visible; set => Base.Visible = value; }

        public bool Exists => Base.Exists;

        private bool shownInit;

        public event Action Resized;

        public DesktopWindowSystem()
        {
            Base = CreateSdlWindow();
            Base.Resized += () =>
            {
                Resized?.Invoke();
            };
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

        protected override void UpdateCore(double deltaTime)
        {
            while (!Core.GameClient.Instance.Running)
                Thread.Sleep(10);
            if (!shownInit)
                ShowOnceInitialized();
        }

        private void ShowOnceInitialized()
        {
            shownInit = true;
            Base.Visible = true;
            //Base.WindowState = WindowState.BorderlessFullScreen;
        }

        public void PumpEvents()
        {
            throw new NotImplementedException();
        }

        public void Show()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }
    }
}
