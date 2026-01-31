using Game.Core.ECS;
using Game.Core.Graphics.Shaders;
using System.Numerics;
using Veldrid;

namespace Game.Core.Graphics.Components
{
    public class Text2D : EcsComponent, IRenderItem
    {
        public string Text { get; set; } = "Hello World!";
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Vector2 Scale { get; set; } = Vector2.One;
        public RgbaFloat Color { get; set; } = RgbaFloat.White;

        private GraphicsSystem _gs;
        private TextShader _shader;
        private DeviceBuffer _vertexBuffer;
        private DeviceBuffer _indexBuffer;
        private Texture _fontTexture;
        private Pipeline _pipeline;
        private ResourceSet _fontResourceSet;
        private int _vertexCount;
        private int _indexCount;

        protected override void Attached(SystemRegistry registry)
        {
            _gs = registry.GetSystem<GraphicsSystem>();
            _shader = _gs.ShaderManager.Get<TextShader>();

            var factory = _gs.GraphicsDevice.ResourceFactory;

            // Create uniform buffers
            var projectionBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            var worldBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

            // Resource layouts
            var projViewLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("Projection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("World", ResourceKind.UniformBuffer, ShaderStages.Vertex)
                )
            );

            var textureLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("SourceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("SourceSampler", ResourceKind.Sampler, ShaderStages.Fragment)
                )
            );

            // Resource set for shader
            _fontResourceSet = factory.CreateResourceSet(new ResourceSetDescription(
                projViewLayout,
                projectionBuffer,
                worldBuffer
            ));

            // Pipeline description
            var pipelineDesc = new GraphicsPipelineDescription
            {
                BlendState = BlendStateDescription.SingleAlphaBlend,
                DepthStencilState = new DepthStencilStateDescription(
                    depthTestEnabled: true,
                    depthWriteEnabled: true,
                    comparisonKind: ComparisonKind.LessEqual
                ),
                RasterizerState = new RasterizerStateDescription(
                    cullMode: FaceCullMode.Back,
                    fillMode: PolygonFillMode.Solid,
                    frontFace: FrontFace.Clockwise,
                    depthClipEnabled: true,
                    scissorTestEnabled: false
                ),
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                ShaderSet = new ShaderSetDescription(
                    vertexLayouts: [_shader.Layout],
                    shaders: [_shader.VertexShader, _shader.FragmentShader]
                ),
                ResourceLayouts = [projViewLayout, textureLayout],
                Outputs = _gs.GraphicsDevice.SwapchainFramebuffer.OutputDescription
            };

            _pipeline = factory.CreateGraphicsPipeline(pipelineDesc);

            // TODO: Generate vertex/index buffers based on Text
            // Placeholder: empty buffers for now
            _vertexBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.VertexBuffer));
            _indexBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.IndexBuffer));
            _vertexCount = 0;
            _indexCount = 0;
        }

        protected override void OnDisabled() { }
        protected override void OnEnabled() { }

        protected override void Removed(SystemRegistry registry)
        {
            _vertexBuffer?.Dispose();
            _indexBuffer?.Dispose();
            _fontTexture?.Dispose();
            _pipeline?.Dispose();
            _fontResourceSet?.Dispose();
        }

        public void Render(CommandList cl)
        {
            if (_pipeline == null || _vertexBuffer == null || _indexBuffer == null || _indexCount == 0)
                return;

            cl.SetPipeline(_pipeline);
            cl.SetVertexBuffer(0, _vertexBuffer);
            cl.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
            cl.SetGraphicsResourceSet(0, _fontResourceSet);
            cl.DrawIndexed((uint)_indexCount, 1, 0, 0, 0);
        }
    }
}
