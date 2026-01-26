using Veldrid;

namespace Game.Graphics.Shaders
{
    public sealed class ShaderProgram(Shader[] shaders) : IDisposable
    {
        public Shader[] Shaders { get; } = shaders;

        public void Dispose()
        {
            foreach(var shader in Shaders)
                shader.Dispose();
        }
    }
}
