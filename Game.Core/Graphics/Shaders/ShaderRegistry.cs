using System;
using System.Collections.Generic;
using Veldrid;

namespace Game.Core.Graphics.Shaders
{
    public sealed class ShaderRegistry : IDisposable
    {
        private readonly ResourceFactory _factory;
        private readonly List<ShaderAbstract> _shaders = new();

        public ShaderRegistry(ResourceFactory factory)
        {
            _factory = factory;
            LoadAllInternal();
        }

        /// <summary>
        /// Register a ShaderAbstract instance.
        /// </summary>
        public void Register(ShaderAbstract shader)
        {
            if (_shaders.Contains(shader))
                throw new InvalidOperationException($"Shader '{shader.Name}' is already registered.");

            _shaders.Add(shader);
        }

        /// <summary>
        /// Get the first shader of a specific concrete type.
        /// </summary>
        public TShader Get<TShader>() where TShader : ShaderAbstract
        {
            foreach (var shader in _shaders)
            {
                if (shader is TShader typedShader)
                    return typedShader;
            }

            throw new InvalidOperationException($"Shader of type {typeof(TShader).Name} not found.");
        }

        /// <summary>
        /// Enumerate all registered shaders.
        /// </summary>
        public IEnumerable<ShaderAbstract> GetAllShaders() => _shaders;

        /// <summary>
        /// Create and register all known shaders here.
        /// </summary>
        private void LoadAllInternal()
        {
            Register(new TextShader(_factory));
        }

        public void Dispose()
        {
            foreach (var shader in _shaders)
                shader.Dispose();

            _shaders.Clear();
        }
    }
}
