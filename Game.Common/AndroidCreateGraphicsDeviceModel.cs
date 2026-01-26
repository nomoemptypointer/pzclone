using System;

namespace Game.Common
{
    public struct AndroidCreateGraphicsDeviceModel(IntPtr holderHandle, IntPtr jniEnvHandle, uint width, uint height)
    {
        /// <summary>
        /// Handle to the native Android Surface.
        /// </summary>
        public IntPtr HolderHandle { get; } = holderHandle;

        /// <summary>
        /// Handle to the JNI environment.
        /// </summary>
        public IntPtr JNIEnvHandle { get; } = jniEnvHandle;

        /// <summary>
        /// Width of the surface in pixels.
        /// </summary>
        public uint Width { get; } = width;

        /// <summary>
        /// Height of the surface in pixels.
        /// </summary>
        public uint Height { get; } = height;
    }
}
