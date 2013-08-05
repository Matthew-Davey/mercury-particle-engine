namespace Mercury.ParticleEngine.Renderers
{
    using System;
    using SharpDX.Direct3D9;

    public class PointSpriteRenderer : IDisposable
    {
        private Device _device;
        private VertexBuffer _vertexBuffer;
        private Emitter _emitter;

        public PointSpriteRenderer(Device device, Emitter emitter)
        {
            if (device == null)
                throw new ArgumentNullException("device");

            if (emitter == null)
                throw new ArgumentNullException("emitter");

            _device = device;
            _emitter = emitter;
            _vertexBuffer = new VertexBuffer(_device, _emitter.Buffer.SizeInBytes, Usage.Dynamic | Usage.Points, VertexFormat.None, Pool.Managed);
        }

        public void Render()
        {
            var dataStream = _vertexBuffer.Lock(0, 0, LockFlags.Discard);
            dataStream.Write(_emitter.Buffer.NativePointer, 0, _emitter.Buffer.SizeInBytes);
            _vertexBuffer.Unlock();

            _device.SetRenderState(RenderState.PointSpriteEnable, true);
            _device.SetRenderState(RenderState.PointScaleEnable, true);
            _device.SetRenderState(RenderState.PointScaleC, 100);

            _device.SetStreamSource(0, _vertexBuffer, 0, Particle.SizeInBytes);
            _device.DrawPrimitives(PrimitiveType.PointList, 0, _emitter.Buffer.Size);
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
            }
        }

        ~PointSpriteRenderer()
        {
            Dispose(false);
        }
    }
}