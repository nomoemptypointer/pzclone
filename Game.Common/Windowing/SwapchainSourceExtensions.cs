using SDL3;
using System.Runtime.InteropServices;
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
            if (OperatingSystem.IsWindows())
            {
                var props = GetWindowProperties(sdlWindow);

                nint hwnd = GetPointerProperty(props, Props.WindowWin32HWNDPointer, nint.Zero);
                nint hinstance = GetPointerProperty(props, Props.WindowWin32InstancePointer, nint.Zero);

                if (hwnd == nint.Zero)
                    throw new Exception("SDL HWND is null");

                return SwapchainSource.CreateWin32(hwnd, hinstance);
            }
            if (OperatingSystem.IsAndroid())
            {
                var props = GetWindowProperties(sdlWindow);

                nint windowPtr = GetPointerProperty(props, Props.WindowAndroidWindowPointer, nint.Zero); // is always null
                nint surfacePtr = GetPointerProperty(props, Props.WindowAndroidSurfacePointer, nint.Zero); // is always null

                if (windowPtr == nint.Zero)
                    throw new Exception("SDL windowPtr is null");

                return SwapchainSource.CreateAndroidSurface(surfacePtr, windowPtr);
            }

            throw new PlatformNotSupportedException("Unsupported platform.");
        }
    }
}
