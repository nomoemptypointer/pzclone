using Veldrid;
using Veldrid.SPIRV;

namespace Game.Graphics.Shaders
{
    public sealed class ShaderManager : IDisposable
    {
        private readonly ResourceFactory _factory;
        private readonly ShaderProgram[] _programs;

        public ShaderManager(ResourceFactory factory)
        {
            _factory = factory;
            _programs = new ShaderProgram[(int)ShaderId.Count];
            LoadAllInternal();
        }

        public ShaderProgram Load(
            ShaderId id,
            string vertexSource,
            string fragmentSource,
            string vertexFile = "vertex.glsl",
            string fragmentFile = "fragment.glsl")
        {
            ref ShaderProgram slot = ref _programs[(int)id];
            if (slot != null)
                return slot;

            var vsDesc = ShaderCompilerExtensions.CompileShader(
                ShaderStages.Vertex,
                vertexSource,
                vertexFile);

            var fsDesc = ShaderCompilerExtensions.CompileShader(
                ShaderStages.Fragment,
                fragmentSource,
                fragmentFile);

            Shader[] shaderPack = _factory.CreateFromSpirv(vsDesc, fsDesc);

            slot = new ShaderProgram(shaderPack);

            return slot;
        }

        public void LoadAllInternal()
        {
            for (int i = 0; i < (int)ShaderId.Count; i++)
            {
                var def = ShaderDefinitions.All[i];

                Load(
                    (ShaderId)i,
                    def.VertexSource,
                    def.FragmentSource,
                    def.VertexName,
                    def.FragmentName
                );
            }
        }

        public ShaderProgram Get(ShaderId id)
        {
            var program = _programs[(int)id];
            return program == null ? throw new InvalidOperationException($"Shader '{id}' not loaded.") : program;
        }

        public void Dispose()
        {
            for (int i = 0; i < _programs.Length; i++)
            {
                _programs[i]?.Dispose();
                _programs[i] = null!;
            }
        }
    }
}
