using Veldrid;
using Veldrid.SPIRV;

namespace Game.Graphics
{
    internal class Spirv
    {
        public static ShaderDescription CompileShader(ResourceFactory factory, ShaderStages stage, string glslSource, string fileName)
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
