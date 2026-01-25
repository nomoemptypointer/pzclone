using Veldrid;
using static SDL3.SDL;

namespace Game.Common.Windowing
{
    public static class SwapchainSourceExtensions
    {
        public static nint BaseSDL3Android { get; set; } = nint.Zero; // TODO: Get rid of this workaround

        /// <summary>
        /// Creates a Veldrid SwapchainSource from an SDL3 window.
        /// Note: currently only Windows (Win32) and Android is implemented.
        /// </summary>
        public static SwapchainSource CreateSDL(nint sdlWindow)
        {
            uint props = 0;
            if (OperatingSystem.IsAndroid()) // TODO: Get rid of this workaround
                props = GetWindowProperties(BaseSDL3Android);
            else
                props = GetWindowProperties(sdlWindow);
            
            if (OperatingSystem.IsWindows()) // TODO: OperatingSystem -> Pre-Compiler macro step
            {
                nint hwnd = GetPointerProperty(props, Props.WindowWin32HWNDPointer, nint.Zero);
                nint hinstance = GetPointerProperty(props, Props.WindowWin32InstancePointer, nint.Zero);

                if (hwnd == nint.Zero || hinstance == nint.Zero)
                    throw new Exception("Some required SDL Window Properies are null");

                return SwapchainSource.CreateWin32(hwnd, hinstance);
            }
            if (OperatingSystem.IsAndroid())
            {
                nint windowPtr = GetPointerProperty(props, Props.WindowAndroidWindowPointer, nint.Zero); // is always null
                nint surfacePtr = GetPointerProperty(props, Props.WindowAndroidSurfacePointer, nint.Zero); // is always null

                if (windowPtr == nint.Zero || surfacePtr == nint.Zero)
                    throw new Exception("Some required SDL Window Properies are null");

                return SwapchainSource.CreateAndroidSurface(surfacePtr, windowPtr);
            }

            throw new PlatformNotSupportedException("Unsupported platform.");
        }
    }
}
