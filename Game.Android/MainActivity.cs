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
            if (!SDL.Init(SDL.InitFlags.Video))
            {
                //SDL.LogError(SDL.LogCategory.System, $"SDL could not initialize: {SDL.GetError()}");
                return;
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

            SDL.CreateWindow("", 800, 600, flags);

            AndroidWindow aw = new();
            GraphicsRenderer graphicsRenderer = new(aw);
            aw.Run(graphicsRenderer.GraphicsDevice);
        }
    }
}