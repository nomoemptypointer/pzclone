using Veldrid;
using Veldrid.Sdl2;

namespace Game.Common.Windowing
{
    public abstract class AbstractWindow
    {
        public static AbstractWindow Singleton { get; internal set; }
        public Sdl2Window Base { get; set; }

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
