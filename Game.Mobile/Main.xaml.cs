using Veldrid;
using Veldrid.Maui.Controls;

namespace Game.Mobile
{
    public partial class Main : ContentPage
    {
        public Main()
        {
            InitializeComponent();

            var d = new GettingStartedDrawable();

            var platformView = new VeldridView
            {
                AutoReDraw = true,
                Backend = GraphicsBackend.Vulkan,
                Drawable = d
            };

            veldridContainer.Children.Add(platformView);
        }
    }
}
