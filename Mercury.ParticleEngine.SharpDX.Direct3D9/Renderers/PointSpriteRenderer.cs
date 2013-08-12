namespace Mercury.ParticleEngine.Renderers
{
    using System;
    using SharpDX;
    using SharpDX.Direct3D9;

    public class PointSpriteRenderer : IDisposable
    {
        private readonly Device _device;
        private readonly VertexBuffer _vertexBuffer;
        private readonly Emitter _emitter;
        private readonly VertexDeclaration _vertexDeclaration;
        private readonly Effect _effect;

        public PointSpriteRenderer(Device device, Emitter emitter)
        {
            if (device == null)
                throw new ArgumentNullException("device");

            if (emitter == null)
                throw new ArgumentNullException("emitter");

            _device = device;
            _emitter = emitter;
            _vertexBuffer = new VertexBuffer(_device, _emitter.Buffer.SizeInBytes, Usage.Dynamic | Usage.Points | Usage.WriteOnly, VertexFormat.None, Pool.Default);

            var vertexElements = new[]
            {
                new VertexElement(0,  4, DeclarationType.Float1, DeclarationMethod.Default, DeclarationUsage.Color, 1),
                new VertexElement(0,  8, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                new VertexElement(0, 24, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                new VertexElement(0, 40, DeclarationType.Float1, DeclarationMethod.Default, DeclarationUsage.PointSize, 0),
                new VertexElement(0, 44, DeclarationType.Float1, DeclarationMethod.Default, DeclarationUsage.Color, 2),
                VertexElement.VertexDeclarationEnd
            };

            _effect = Effect.FromString(device, Resources.PointSpriteShader, ShaderFlags.PartialPrecision);

            _vertexDeclaration = new VertexDeclaration(device, vertexElements);
        }

        public void Render(Matrix worldViewProjection, Texture texture)
        {
            var technique = _effect.GetTechnique(0);

            _effect.SetValue("WVPMatrix", worldViewProjection);
            _effect.SetTexture(_effect.GetParameter(null, "SpriteTexture"), texture);

            var dataStream = _vertexBuffer.Lock(0, 0, LockFlags.NoDirtyUpdate);
            Utilities.CopyMemory(dataStream.DataPointer, _emitter.Buffer.NativePointer, _emitter.Buffer.SizeInBytes);
            _vertexBuffer.Unlock();

            _device.SetRenderState(RenderState.PointSpriteEnable, true);
            _device.SetRenderState(RenderState.PointSizeMin, 0.0f);
            _device.SetRenderState(RenderState.PointSizeMax, 512.0f);
            _device.SetRenderState(RenderState.AlphaBlendEnable, true);
            _device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            _device.SetRenderState(RenderState.DestinationBlend, Blend.One);

            _device.SetRenderState(RenderState.ZWriteEnable, false);
            
            _effect.Technique = technique;
            _effect.Begin();
            _effect.BeginPass(0);

            _device.SetStreamSource(0, _vertexBuffer, 0, Particle.SizeInBytes);
            _device.VertexDeclaration = _vertexDeclaration;
            _device.DrawPrimitives(PrimitiveType.PointList, 0, _emitter.Buffer.Size);

            _effect.EndPass();
            _effect.End();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                _vertexBuffer.Dispose();
                _effect.Dispose();
            }
        }

        ~PointSpriteRenderer()
        {
            Dispose(false);
        }
    }
}