namespace Game.Graphics.Shaders
{
    public readonly struct ShaderDefinition
    {
        public readonly string VertexSource;
        public readonly string FragmentSource;
        public readonly string VertexName;
        public readonly string FragmentName;

        public ShaderDefinition(string name)
        {
            VertexName = name + ".vert";
            FragmentName = name + ".frag";

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
