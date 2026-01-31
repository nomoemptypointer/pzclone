using Veldrid;
using Veldrid.SPIRV;

namespace Game.Core.Graphics.Shaders
{
    public abstract class ShaderAbstract : IDisposable
    {
        public string Name { get; protected set; }
        public Shader VertexShader { get; protected set; }
        public Shader FragmentShader { get; protected set; }
        public VertexLayoutDescription Layout { get; protected set; }

        protected ShaderAbstract(ResourceFactory factory, string name)
        {
            Name = name;

            var vertSrc = LoadEmbeddedShaderSource($"GLSL.{name}.vert");
            var fragSrc = LoadEmbeddedShaderSource($"GLSL.{name}.frag");

            var vertSpirv = SpirvCompilation.CompileGlslToSpirv(
                vertSrc,
                name + ".vert",
                ShaderStages.Vertex,
                new GlslCompileOptions()
            ).SpirvBytes;

            var fragSpirv = SpirvCompilation.CompileGlslToSpirv(
                fragSrc,
                name + ".frag",
                ShaderStages.Fragment,
                new GlslCompileOptions()
            ).SpirvBytes;

            var shaders = factory.CreateFromSpirv(
                new ShaderDescription(ShaderStages.Vertex, vertSpirv, "main"),
                new ShaderDescription(ShaderStages.Fragment, fragSpirv, "main")
            );

            VertexShader = shaders[0];
            FragmentShader = shaders[1];
        }

        public virtual void Dispose()
        {
            VertexShader?.Dispose();
            FragmentShader?.Dispose();
            GC.SuppressFinalize(this);
        }

        private string LoadEmbeddedShaderSource(string resourceName)
        {
            var assembly = GetType().Assembly;
            var fullResourceName = $"{GetType().Namespace}.{resourceName}";

            using Stream? stream = assembly.GetManifestResourceStream(fullResourceName)
                ?? throw new InvalidOperationException(
                    $"Shader source not found: '{resourceName}'. Tried full resource name: '{fullResourceName}'");

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
