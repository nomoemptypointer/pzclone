namespace Game.Graphics.Shaders
{
    public readonly struct ShaderDefinition
    {
        const string Prefix = "Game.Graphics.Shaders.GLSL.";

        public readonly string VertexSource;
        public readonly string FragmentSource;
        public readonly string VertexName;
        public readonly string FragmentName;

        public ShaderDefinition(string name)
        {
            VertexName = Prefix + name + ".vert";
            FragmentName = Prefix + name + ".frag";

            VertexSource = EmbeddedShaderAssets.Load(VertexName);
            FragmentSource = EmbeddedShaderAssets.Load(FragmentName);
        }
    }

    public enum ShaderId
    {
        DebugText,

        Count // always last
    }
}
