namespace Mercury.ParticleEngine.Renderers {
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using SharpDX;
    using SharpDX.Direct3D9;

    public class PointSpriteRenderer : IDisposable {
        private readonly Device _device;
        private readonly int _size;
        private readonly IReadOnlyDictionary<String, Texture> _textureLookup;
        private readonly VertexBuffer _vertexBuffer;
        private readonly VertexDeclaration _vertexDeclaration;
        private readonly Effect _effect;
        private readonly EffectHandle _technique;
        private readonly EffectHandle _matrixParameter;
        private readonly EffectHandle _textureParameter;

        private bool _enableFastFade;
        public bool EnableFastFade {
            get { return _enableFastFade; }
            set {
                if(value != _enableFastFade) {
                    _enableFastFade = value;
                    _effect.SetValue("FastFade", _enableFastFade);
                }
            }
        }

        public PointSpriteRenderer(Device device, int size, IReadOnlyDictionary<String, Texture> textureLookup) {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            if (textureLookup == null)
                throw new ArgumentNullException(nameof(textureLookup));

            _device = device;
            _size = size;
            _textureLookup = textureLookup;
            _vertexBuffer = new VertexBuffer(_device, _size * Particle.SizeInBytes, Usage.Dynamic | Usage.Points | Usage.WriteOnly, VertexFormat.None, Pool.Default);

            var vertexElements = new[] {
                new VertexElement(0, (short)Marshal.OffsetOf(typeof(Particle), "Age"),      DeclarationType.Float1, DeclarationMethod.Default, DeclarationUsage.Color, 1),
                new VertexElement(0, (short)Marshal.OffsetOf(typeof(Particle), "Position"), DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                new VertexElement(0, (short)Marshal.OffsetOf(typeof(Particle), "Colour"),   DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                new VertexElement(0, (short)Marshal.OffsetOf(typeof(Particle), "Scale"),    DeclarationType.Float1, DeclarationMethod.Default, DeclarationUsage.PointSize, 0),
                new VertexElement(0, (short)Marshal.OffsetOf(typeof(Particle), "Rotation"), DeclarationType.Float1, DeclarationMethod.Default, DeclarationUsage.Color, 2),
                VertexElement.VertexDeclarationEnd
            };

            _effect = Effect.FromString(device, Resources.PointSpriteShader, ShaderFlags.PartialPrecision);
            _technique = _effect.GetTechnique(0);
            _matrixParameter = _effect.GetParameter(null, "WVPMatrix");
            _textureParameter = _effect.GetParameter(null, "SpriteTexture");
            _vertexDeclaration = new VertexDeclaration(device, vertexElements);
        }

        public void Render(ParticleEffect effect, Matrix worldViewProjection) {
            for (var i = 0; i < effect.Emitters.Length; i++) {
                Render(effect.Emitters[i], worldViewProjection);
            }
        }

        internal void Render(Emitter emitter, Matrix worldViewProjection) {
            if (emitter.ActiveParticles == 0)
                return;

            if (emitter.ActiveParticles > _size)
                throw new Exception("Cannot render this emitter, vertex buffer not big enough");

            _effect.SetValue(_matrixParameter, worldViewProjection);
            _effect.SetTexture(_textureParameter, _textureLookup[emitter.TextureKey]);

            var vertexDataPointer = _vertexBuffer.LockToPointer(0, emitter.Buffer.ActiveSizeInBytes, LockFlags.Discard);
            
            switch (emitter.RenderingOrder) {
                case RenderingOrder.FrontToBack: {
                    emitter.Buffer.CopyTo(vertexDataPointer);
                    break;
                }
                case RenderingOrder.BackToFront: {
                    emitter.Buffer.CopyToReverse(vertexDataPointer);
                    break;
                }
            }

            _vertexBuffer.Unlock();

            SetupBlend(emitter.BlendMode);

            _effect.Technique = _technique;
            _effect.Begin(FX.DoNotSaveState);
            _effect.BeginPass(0);

            _device.SetStreamSource(0, _vertexBuffer, 0, Particle.SizeInBytes);
            _device.VertexDeclaration = _vertexDeclaration;
            _device.DrawPrimitives(PrimitiveType.PointList, 0, emitter.ActiveParticles);

            _effect.EndPass();
            _effect.End();
        }

        private void SetupBlend(BlendMode blendMode) {
            switch (blendMode) {
                case BlendMode.Alpha:
                    _device.SetRenderState(RenderState.BlendOperation,        BlendOperation.Add);
                    _device.SetRenderState(RenderState.BlendOperationAlpha,   BlendOperation.Add);
                    _device.SetRenderState(RenderState.SourceBlendAlpha,      Blend.SourceAlpha);
                    _device.SetRenderState(RenderState.DestinationBlendAlpha, Blend.InverseSourceAlpha);
                    _device.SetRenderState(RenderState.SourceBlend,           Blend.SourceAlpha);
                    _device.SetRenderState(RenderState.DestinationBlend,      Blend.InverseSourceAlpha);
                    return;
                case BlendMode.Add:
                    _device.SetRenderState(RenderState.BlendOperation,        BlendOperation.Add);
                    _device.SetRenderState(RenderState.BlendOperationAlpha,   BlendOperation.Add);
                    _device.SetRenderState(RenderState.SourceBlendAlpha,      Blend.SourceAlpha);
                    _device.SetRenderState(RenderState.DestinationBlendAlpha, Blend.InverseSourceAlpha);
                    _device.SetRenderState(RenderState.SourceBlend,           Blend.SourceAlpha);
                    _device.SetRenderState(RenderState.DestinationBlend,      Blend.One);
                    return;
                case BlendMode.Subtract:
                    _device.SetRenderState(RenderState.BlendOperation,        BlendOperation.ReverseSubtract);
                    _device.SetRenderState(RenderState.BlendOperationAlpha,   BlendOperation.Add);
                    _device.SetRenderState(RenderState.SourceBlendAlpha,      Blend.SourceAlpha);
                    _device.SetRenderState(RenderState.DestinationBlendAlpha, Blend.InverseSourceAlpha);
                    _device.SetRenderState(RenderState.SourceBlend,           Blend.SourceAlpha);
                    _device.SetRenderState(RenderState.DestinationBlend,      Blend.One);
                    return;
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing) {
            if (disposing) {
                _vertexBuffer.Dispose();
                _vertexDeclaration.Dispose();
                _matrixParameter.Dispose();
                _textureParameter.Dispose();
                _technique.Dispose();
                _effect.Dispose();
            }
        }

        ~PointSpriteRenderer() {
            Dispose(false);
        }
    }
}