using Veldrid;

namespace Game.Core.Graphics
{
    public class RenderQueue
    {
        public List<IRenderItem> Items { get; internal set; } = []; // This will get much more complex later on so it's better to make this private sooner to avoid absolute brainfuck
        public void Add(IRenderItem item) => Items.Add(item);
        public void Clear() => Items.Clear();

        internal void RenderAll(CommandList cl)
        {
            foreach (var item in Items)
                item.Render(cl);
        }
    }
}
