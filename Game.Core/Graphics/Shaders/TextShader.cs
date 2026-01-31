using Veldrid;

namespace Game.Core.Graphics.Shaders
{
    public class TextShader : ShaderAbstract
    {
        public TextShader(ResourceFactory factory) : base(factory, "Text")
        {
            Layout = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float2),
                new VertexElementDescription("TexCoords", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                new VertexElementDescription("Color", VertexElementSemantic.Color, VertexElementFormat.Float4)
            );
        }
    }
}
