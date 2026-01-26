using Veldrid;

namespace Game.Common.Windowing
{
    public abstract class AbstractWindow
    {
        public static AbstractWindow Singleton { get; internal set; }
        public nint BaseSDL3 { get; set; } = nint.Zero;

        protected AbstractWindow()
        {
            if (Singleton != null)
                throw new Exception("Singleton already exists, there must be only one window instance.");

            Singleton = this;
        }

        public virtual void Tick(GraphicsDevice gd)
        {

        }

        public abstract void Show();
    }
}
