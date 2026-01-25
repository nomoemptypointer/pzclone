using SDL3;
using Veldrid;
using static SDL3.SDL;

namespace Game.Desktop
{
    public static class SwapchainSourceExtensions
    {
        /// <summary>
        /// Creates a Veldrid SwapchainSource from an SDL3 window.
        /// Note: currently only Windows (Win32) is implemented.
        /// </summary>
        public static SwapchainSource CreateSDL(nint sdlWindow)
        {
            var props = GetWindowProperties(sdlWindow);

            // TODO: Implement Linux and MacOS
            nint hwnd = GetPointerProperty(props, Props.WindowWin32HWNDPointer, nint.Zero);
            nint hinstance = GetPointerProperty(props, Props.WindowWin32InstancePointer, nint.Zero);

            if (hwnd == nint.Zero)
                throw new Exception($"SDL HWND is null");

            return SwapchainSource.CreateWin32(hwnd, hinstance);
        }
    }
}
