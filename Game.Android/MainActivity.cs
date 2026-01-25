using Android.Content.PM;
using Game.Common.Windowing;
using Game.Graphics;
using Org.Libsdl.App;
using Veldrid;
using static Android.Content.PM.ConfigChanges;
using static SDL3.SDL;
using SDL = SDL3.SDL;

namespace Game.Android
{
    [Activity(
    Label = "@string/app_name",
    MainLauncher = true,
    AlwaysRetainTaskState = true,
    LaunchMode = LaunchMode.SingleInstance,
    Exported = true,
    ConfigurationChanges =
        LayoutDirection | ConfigChanges.Locale | GrammaticalGender | FontScale |
        FontWeightAdjustment | ConfigChanges.Orientation | UiMode |
        ScreenLayout | ScreenSize | SmallestScreenSize |
        Keyboard | KeyboardHidden | Navigation
    )]

    public class MainActivity : SDLActivity
    {
        protected override string[] GetLibraries() => ["SDL3"];

        protected override void Main()
        {
            AndroidWindow aw = new();
            GraphicsRenderer graphicsRenderer = new(aw);
            aw.Run(graphicsRenderer.GraphicsDevice);
        }
    }
}