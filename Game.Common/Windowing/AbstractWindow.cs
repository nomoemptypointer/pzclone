namespace Game.Common.Windowing
{
    public abstract class AbstractWindow
    {
        public static AbstractWindow Singleton { get; private set; }

        public AbstractWindow()
        {
            if (Singleton == null)
            {
                Singleton = this;
            }
            else
            {
                throw new Exception("Singleton already exists, there must be only one window instance.");
            }
        }

        public abstract void Run();
    }
}
