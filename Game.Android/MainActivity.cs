using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Game.Core;
using Game.Graphics;

namespace Game.Android
{
    [Activity(
        Label = "@string/app_name",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize
    )]
    public class MainActivity : Activity
    {
        internal GraphicsRenderer _renderer;
        private VeldridSurfaceView _surfaceView;
        private Core.GameClient _game;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestedOrientation = ScreenOrientation.Landscape;

            var decorView = Window.DecorView;
            decorView.SystemUiFlags =
                SystemUiFlags.ImmersiveSticky |
                SystemUiFlags.HideNavigation |
                SystemUiFlags.Fullscreen |
                SystemUiFlags.LayoutStable |
                SystemUiFlags.LayoutHideNavigation |
                SystemUiFlags.LayoutFullscreen;

            _renderer = new GraphicsRenderer();

            _surfaceView = new VeldridSurfaceView(this, _renderer);
            SetContentView(_surfaceView);

            _game = new Core.GameClient();
            _game.Initialize();

            _game.SetRenderAction(() =>
            {
                _renderer?.Render();
            });

            _game.TargetFramesPerSecond = double.MaxValue;

            _surfaceView.OnGraphicsDeviceCreated += StartGameLoop;
        }

        private void StartGameLoop()
        {
            _surfaceView.StartRenderingLoop(() =>
            {
                _game.Run();
            });
        }

        protected override void OnPause()
        {
            base.OnPause();
            _surfaceView?.PauseRenderingLoop();
        }

        protected override void OnResume()
        {
            base.OnResume();
            //_surfaceView?.ResumeRenderingLoop();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _renderer.GraphicsDevice?.Dispose();
        }
    }
}
