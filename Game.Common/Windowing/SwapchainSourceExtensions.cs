using SDL3;
using static SDL3.SDL;

namespace Game.Desktop
{
    public static class SwapchainSourceExtensions
    {
        /// <summary>
        /// Creates a Veldrid SwapchainSource from an SDL3 window.
        /// Note: currently only Windows (Win32) is implemented.
        /// </summary>
        public static Veldrid.SwapchainSource CreateSDL(nint sdlWindow)
        {
            var props = GetWindowProperties(sdlWindow);

            // TODO: Implement Linux and MacOS
            nint hwnd = GetPointerProperty(props, "SDL_PROP_WINDOW_WIN32_HWND_POINTER", nint.Zero);
            nint hinstance = GetPointerProperty(props, "SDL_PROP_WINDOW_WIN32_INSTANCE_POINTER", nint.Zero);


            return Veldrid.SwapchainSource.CreateWin32(hwnd, hinstance);
        }
    }
}
