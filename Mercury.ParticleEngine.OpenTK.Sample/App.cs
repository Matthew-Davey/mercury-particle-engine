namespace Mercury.ParticleEngine {
    using System;
    using System.Collections.Generic;
    using Mercury.ParticleEngine.Profiles;
    using Mercury.ParticleEngine.Modifiers;
    using Mercury.ParticleEngine.Renderers;
    using OpenTK;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;

    public class App : GameWindow {
        static void Main() {
            var app = new App();
            app.Run(60L);
        }

        ParticleEffect _effect;
        QuadRenderer _renderer;
        Coordinate _mousePosition;
        Coordinate _previousMousePosition;

        public App() {
            Title = "OpenTK Sample";
            WindowBorder = WindowBorder.Fixed;
            ClientSize = new System.Drawing.Size(1024, 768);
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            _effect = new ParticleEffect {
                Emitters = new[] {
                    new Emitter(2000, TimeSpan.FromSeconds(3), Profile.Point()) {
                        Parameters = new ReleaseParameters {
                            Colour   = new Colour(0f, 0f, 0.6f),
                            Opacity  = 1f,
                            Quantity = 5,
                            Speed    = new RangeF(0f, 100f),
                            Scale    = 32f,
                            Rotation = new RangeF((float)-Math.PI, (float)Math.PI),
                            Mass     = new RangeF(8f, 12f)
                        },
                        ReclaimFrequency = 5f,
                        BlendMode = BlendMode.Alpha,
                        RenderingOrder = RenderingOrder.BackToFront,
                        TextureKey = "Cloud",
                        Modifiers = new Modifier[] {
                            new DragModifier {
                                Frequency       = 10f,
                                DragCoefficient = 0.47f,
                                Density         = 0.125f
                            },
                            new ScaleInterpolator2 {
                                Frequency       = 60f,
                                InitialScale    = 32f,
                                FinalScale      = 256f
                            },
                            new RotationModifier {
                                Frequency       = 15f,
                                RotationRate    = 1f
                            },
                            new OpacityInterpolator2 {
                                Frequency       = 25f,
                                InitialOpacity  = 0.3f,
                                FinalOpacity    = 0.0f
                            }
                        },
                    }
                }
            };

            GL.ClearColor(Color4.Black);
            GL.Ortho(0, 1024, 768, 0, 0, 1);
            GL.Viewport(0, 0, 1024, 768);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            int textureHandle;
            GL.GenTextures(1, out textureHandle);
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);

            var bitmap = new System.Drawing.Bitmap("Cloud001.png");
            var data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bitmap.UnlockBits(data);
            bitmap.Dispose();

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            var textureLookup = new Dictionary<String, Int32> {
                { "Cloud", textureHandle }
            };

            _renderer = new QuadRenderer(textureLookup);
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);

            _previousMousePosition = _mousePosition;
            _mousePosition = new Coordinate(Mouse.X, Mouse.Y);

            if (System.Windows.Forms.Form.MouseButtons.HasFlag(System.Windows.Forms.MouseButtons.Left)) {
                _effect.Trigger(new LineSegment(_previousMousePosition, _mousePosition));
            }

            _effect.Update((float)e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _renderer.Render(_effect.Emitters[0], Matrix4.Identity);

            GL.Flush();

            SwapBuffers();
        }
    }
}