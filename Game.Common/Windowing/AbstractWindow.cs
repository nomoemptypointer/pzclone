using Veldrid;

namespace Game.Common.Windowing
{
    public abstract class AbstractWindow
    {
        public static AbstractWindow Singleton { get; internal set; }
        public nint BaseSDL3 { get; protected set; } = nint.Zero;

        protected AbstractWindow(nint baseSdl3)
        {
            if (baseSdl3 == 0)
                throw new ArgumentException("Invalid SDL window handle.");

            if (Singleton != null)
                throw new Exception("Singleton already exists, there must be only one window instance.");

            BaseSDL3 = baseSdl3;
            Singleton = this;
        }

        public abstract void Run(GraphicsDevice gd, Action perFrameAction);
    }
}
