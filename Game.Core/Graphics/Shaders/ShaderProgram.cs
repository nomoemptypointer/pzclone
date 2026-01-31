using Veldrid;

namespace Game.Core.Graphics.Shaders
{
    public sealed class ShaderProgram(Shader[] shaders) : IDisposable // TODO: This class is retarded
    {
        public Shader[] Shaders { get; } = shaders;

        public void Dispose()
        {
            foreach(var shader in Shaders)
                shader.Dispose();
        }
    }
}
