namespace Mercury.ParticleEngine {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using SharpDX;
    using SharpDX.Direct3D9;
    using SharpDX.Windows;
    using Mercury.ParticleEngine.Modifiers;
    using Mercury.ParticleEngine.Profiles;
    using Mercury.ParticleEngine.Renderers;
    using System.Windows.Input;

    static class Program {
        [STAThread]
        static void Main() {
            var worldSize = new Size2(1024, 768);
            var renderSize = new Size2(1024, 768);
            const bool windowed = true;

            const int numParticles = 2000000;

            var form = new RenderForm("Mercury Particle Engine - SharpDX.Direct3D9 Sample") {
                Size = new System.Drawing.Size(renderSize.Width, renderSize.Height)
            };

            var direct3d = new Direct3D();
            var device = new Device(direct3d, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(renderSize.Width, renderSize.Height) { PresentationInterval = PresentInterval.One, Windowed = windowed });

            var view = new Matrix(
                1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, -1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, -1.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);
            var proj = Matrix.OrthoOffCenterLH(worldSize.Width * -0.5f, worldSize.Width * 0.5f, worldSize.Height * 0.5f, worldSize.Height * -0.5f, 0f, 1f);
            var wvp = Matrix.Identity * view * proj;

            var emitter = new Emitter(numParticles, TimeSpan.FromSeconds(3), Profile.Point()) {
                Parameters = new ReleaseParameters {
                    Colour = new Colour(0f, 0f, 0.6f),
                    Opacity = 1f,
                    Quantity = 5,
                    Speed = new RangeF(0f, 100f),
                    Scale = 32f,
                    Rotation = new RangeF((float)-Math.PI, (float)Math.PI),
                    Mass = new RangeF(8f, 12f)
                },
                ReclaimFrequency = 5f,
                BlendMode = BlendMode.Alpha,
                TextureKey = "Cloud",
                Modifiers = new ModifierCollection {
                    new DragModifier {
                        DragCoefficient = 0.47f,
                        Density = 0.125f
                    },
                    new ScaleInterpolator2 {
                        Frequency = 60f,
                        InitialScale = 32f,
                        FinalScale = 256f
                    },
                    new RotationModifier {
                        RotationRate = 1f
                    },
                    new OpacityInterpolator2 {
                        InitialOpacity = 0.3f,
                        FinalOpacity = 0.0f
                    }
                },
                ModifierExecutionStrategy = ModifierExecutionStrategy.Serial
            };

            var textureLookup = new Dictionary<String, Texture> {
                { "Particle", Texture.FromFile(device, "Particle.dds") },
                { "Pixel",    Texture.FromFile(device, "Pixel.dds") },
                { "Flame",    Texture.FromFile(device, "Flame.png") },
                { "Splash",   Texture.FromFile(device, "Splash.png") },
                { "Cloud",    Texture.FromFile(device, "Cloud001.png") }
            };

            var renderer = new PointSpriteRenderer(device, numParticles, textureLookup) {
                //EnableFastFade = true
            };

            var fontDescription = new FontDescription {
                Height         = 16,
                FaceName       = "Consolas",
                PitchAndFamily = FontPitchAndFamily.Mono,
                Quality        = FontQuality.Draft
            };

            var font = new Font(device, fontDescription);

            var totalTimer = Stopwatch.StartNew();
            var updateTimer = new Stopwatch();
            var renderTimer = new Stopwatch();

            var totalTime = 0f;

            float updateTime = 0f;

            RenderLoop.Run(form, () => {
                    // ReSharper disable AccessToDisposedClosure
                    var frameTime = ((float)totalTimer.Elapsed.TotalSeconds) - totalTime;
                    totalTime = (float)totalTimer.Elapsed.TotalSeconds;

                    var mousePosition = form.PointToClient(RenderForm.MousePosition);

                    var mouseVector = new Vector3(mousePosition.X, mousePosition.Y, 0f);
                    var unprojected = Vector3.Unproject(mouseVector, 0, 0, renderSize.Width, renderSize.Height, 0f, 1f, wvp);

                    if (RenderForm.MouseButtons.HasFlag(System.Windows.Forms.MouseButtons.Left)) {
                        emitter.Trigger(new Coordinate(unprojected.X, unprojected.Y));
                    }

                    updateTimer.Restart();
                    emitter.Update(frameTime);
                    updateTimer.Stop();
                    updateTime = (float)updateTimer.Elapsed.TotalSeconds;

                    device.Clear(ClearFlags.Target, Color.Black, 1f, 0);
                    device.BeginScene();

                    renderTimer.Restart();
                    renderer.Render(emitter, wvp);
                    renderTimer.Stop();
                    var renderTime = (float)renderTimer.Elapsed.TotalSeconds;

                    font.DrawText(null, String.Format("Time:        {0}", totalTimer.Elapsed), 0, 0, Color.White);
                    font.DrawText(null, String.Format("Particles:   {0:n0}", emitter.ActiveParticles), 0, 16, Color.White);
                    font.DrawText(null, String.Format("Update:      {0:n4} ({1,8:P2})", updateTime, updateTime / 0.01666666f), 0, 32, Color.White);
                    font.DrawText(null, String.Format("Render:      {0:n4} ({1,8:P2})", renderTime, renderTime / 0.01666666f), 0, 48, Color.White);

                    device.EndScene();
                    device.Present();

                    if (Keyboard.IsKeyDown(Key.Escape))
                        Environment.Exit(0);
// ReSharper restore AccessToDisposedClosure
                });

            form.Dispose();
            font.Dispose();
            device.Dispose();
            direct3d.Dispose();
        }
    }
}
