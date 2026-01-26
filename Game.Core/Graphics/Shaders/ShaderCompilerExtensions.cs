using Veldrid;
using Veldrid.SPIRV;

namespace Game.Core.Graphics.Shaders
{
    public static class ShaderCompilerExtensions
    {
        public static ShaderDescription CompileShader(ShaderStages stage, string glslSource, string fileName)
        {
            var compilation = SpirvCompilation.CompileGlslToSpirv(
                glslSource,
                fileName,
                stage,
                new GlslCompileOptions()
            );

            return new ShaderDescription(
                stage,
                compilation.SpirvBytes,
                "main"
            );
        }
    }
}
