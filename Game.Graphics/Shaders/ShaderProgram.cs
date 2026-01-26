using Veldrid;
using Veldrid.SPIRV;

namespace Game.Graphics.Shaders
{
    public sealed class ShaderProgram : IDisposable
    {
        public Shader Vertex { get; }
        public Shader Fragment { get; }

        public Shader[] Shaders => [Vertex, Fragment];

        public ShaderProgram(Shader vertex, Shader fragment)
        {
            Vertex = vertex;
            Fragment = fragment;
        }

        public void Dispose()
        {
            //Vertex.Dispose();
            //Fragment.Dispose();
        }
    }
}
