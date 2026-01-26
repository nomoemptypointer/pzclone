using System.Reflection;

namespace Game.Graphics.Shaders
{
    public static class EmbeddedShaderAssets
    {
        private const string BaseNamespace = "Game.Core.Graphics.GLSL.";

        /// <summary>
        /// Loads a shader embedded resource by file name (e.g., "DebugText.vert").
        /// </summary>
        /// <param name="file">The shader file name, including extension.</param>
        /// <returns>The shader source code.</returns>
        public static string Load(string file)
        {
            var asm = Assembly.GetExecutingAssembly();
            var fullName = BaseNamespace + file;
            var names = asm.GetManifestResourceNames();

            var resourceName = names.FirstOrDefault(n => n.Equals(fullName, StringComparison.OrdinalIgnoreCase));
            if (resourceName == null)
                throw new InvalidOperationException($"Shader '{file}' not found. Tried '{fullName}'");

            using var stream = asm.GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException($"Shader '{file}' stream is null.");

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
