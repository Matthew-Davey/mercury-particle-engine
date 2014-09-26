namespace Mercury.ParticleEngine.Renderers {
    using System;
    using System.Collections.Generic;
    using OpenTK;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;

    public class QuadRenderer : IDisposable {
        private readonly IReadOnlyDictionary<String, Int32> _textureIndexLookup;
        private readonly int _vertexBufferId;

        public QuadRenderer(IReadOnlyDictionary<String, Int32> textureIndexLookup) {
            if (textureIndexLookup == null)
                throw new ArgumentNullException("textureIndexLookup");

            _textureIndexLookup = textureIndexLookup;
        }

        public void Render(Emitter emitter, Matrix4 worldViewProjection) {
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.BindTexture(TextureTarget.Texture2D, _textureIndexLookup[emitter.TextureKey]);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref worldViewProjection);

            GL.Begin(PrimitiveType.Quads);

            unsafe {
                switch (emitter.RenderingOrder) {
                    case RenderingOrder.BackToFront: {
                        var count = 0;
                        var particle = (Particle*)emitter.Buffer.NativePointer + emitter.ActiveParticles -1;
                        while (count++ < emitter.ActiveParticles) {
                            RenderParticle(particle);
                            particle--;
                        }
                        break;
                    }
                    default:
                    case RenderingOrder.FrontToBack: {
                        var count = emitter.ActiveParticles;
                        var particle = (Particle*)emitter.Buffer.NativePointer;
                        while (count-- > 0) {
                            RenderParticle(particle);
                            particle++;
                        }
                        break;
                    }
                }
            }

            GL.End();
        }

        static unsafe void RenderParticle(Particle* particle) {
            GL.Color4(HslToRgb(particle->Colour));

            GL.TexCoord2(0, 0);
            GL.Vertex2(particle->Position[0] - (particle->Scale / 2f), particle->Position[1] - (particle->Scale / 2f));
            GL.TexCoord2(1, 0);
            GL.Vertex2(particle->Position[0] + (particle->Scale / 2f), particle->Position[1] - (particle->Scale / 2f));
            GL.TexCoord2(1, 1);
            GL.Vertex2(particle->Position[0] + (particle->Scale / 2f), particle->Position[1] + (particle->Scale / 2f));
            GL.TexCoord2(0, 1);
            GL.Vertex2(particle->Position[0] - (particle->Scale / 2f), particle->Position[1] + (particle->Scale / 2f));
        }

        static unsafe Color4 HslToRgb(float* hsl) {
            var rgb = HueToRgb(hsl[0] / 360f);
            float c = (1f - Math.Abs(2 * hsl[2] - 1f)) * hsl[1];

            var r = (rgb.R - 0.5f) * c + hsl[2];
            var g = (rgb.G - 0.5f) * c + hsl[2];
            var b = (rgb.B - 0.5f) * c + hsl[2];

            return new Color4(r, g, b, hsl[3]);
        }

        static Color4 HueToRgb(float hue) {
            var r = Math.Abs(hue * 6f - 3f) - 1f;
            var g = 2f - Math.Abs(hue * 6f - 2f);
            var b = 2f - Math.Abs(hue * 6f - 4f);

            return new Color4(r < 0f ? 0f : r > 1f ? 1f : r,
                              g < 0f ? 0f : g > 1f ? 1f : g,
                              b < 0f ? 0f : b > 1f ? 1f : b, 0f);
        }

        public void Dispose() {
            GL.DeleteBuffer(_vertexBufferId);
        }
    }
}