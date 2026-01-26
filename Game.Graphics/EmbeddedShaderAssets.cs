using System.Reflection;

namespace Game.Graphics
{
    public class EmbeddedShaderAssets
    {
        private const string Base = "Game.Graphics.Shaders.GLSL.";

        public static string Load(string file)
        {
            var asm = Assembly.GetExecutingAssembly();
            using var stream = asm.GetManifestResourceStream(Base + file)
                ?? throw new InvalidOperationException($"Shader '{file}' not found.");

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
