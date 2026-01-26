using Veldrid;
using Veldrid.Sdl2;

namespace Game.Core
{
    public interface IWindow
    {
        int Width { get; }
        int Height { get; }
        bool Visible { get; set; }
        bool Exists { get; }
        Sdl2Window Base { get; }

        event Action Resized;

        void PumpEvents();
        void Show();
        void Close();
    }
}
