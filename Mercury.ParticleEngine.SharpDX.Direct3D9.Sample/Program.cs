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

            var smokeEmitter = new Emitter(2000, TimeSpan.FromSeconds(3), Profile.Point()) {
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
                RenderingOrder = RenderingOrder.BackToFront,
                TextureKey = "Cloud",
                Modifiers = new ModifierCollection {
                    new DragModifier {
                        Frequency = 10f,
                        DragCoefficient = 0.47f,
                        Density = 0.125f
                    },
                    new ScaleInterpolator2 {
                        Frequency = 60f,
                        InitialScale = 32f,
                        FinalScale = 256f
                    },
                    new RotationModifier {
                        Frequency = 15f,
                        RotationRate = 1f
                    },
                    new OpacityInterpolator2 {
                        Frequency = 25f,
                        InitialOpacity = 0.3f,
                        FinalOpacity = 0.0f
                    }
                },
            };

            var sparkEmitter = new Emitter(2000, TimeSpan.FromSeconds(2), Profile.Point()) {
                Parameters = new ReleaseParameters {
                    Colour = new Colour(50f, 0.8f, 0.5f),
                    Opacity = 1f,
                    Quantity = 10,
                    Speed = new RangeF(0f, 100f),
                    Scale = 64f,
                    Mass = new RangeF(8f, 12f)
                },
                ReclaimFrequency = 5f,
                BlendMode = BlendMode.Add,
                RenderingOrder = RenderingOrder.FrontToBack,
                TextureKey = "Particle",
                Modifiers = new ModifierCollection {
                    new LinearGravityModifier(Axis.Down, 30f) {
                        Frequency = 15f
                    },
                    new OpacityFastFadeModifier() {
                        Frequency = 10f
                    }
                }
            };

            var ringEmitter = new Emitter(2000, TimeSpan.FromSeconds(3), Profile.Spray(Axis.Up, 0.5f)) {
                Parameters = new ReleaseParameters {
                    Colour = new ColourRange(new Colour(210f, 0.5f, 0.6f), new Colour(230f, 0.7f, 0.8f)),
                    Opacity = 1f,
                    Quantity = 5,
                    Speed = new RangeF(300f, 700f),
                    Scale = 64f,
                    Mass = new RangeF(4f, 12f),
                },
                ReclaimFrequency = 5f,
                BlendMode = BlendMode.Alpha,
                RenderingOrder = RenderingOrder.FrontToBack,
                TextureKey = "Ring",
                Modifiers = new ModifierCollection {
                    new LinearGravityModifier(Axis.Down, 100f) {
                        Frequency = 20f
                    },
                    new OpacityFastFadeModifier() {
                        Frequency = 10f,
                    },
                    new ContainerModifier {
                        Frequency = 15f,
                        Width = worldSize.Width,
                        Height = worldSize.Height,
                        Position = new Coordinate(worldSize.Width / 2f, worldSize.Height / 2f),
                        RestitutionCoefficient = 0.75f
                    }
                }
            };

            var currentEmitter = smokeEmitter;

            var textureLookup = new Dictionary<String, Texture> {
                { "Particle", Texture.FromFile(device, "Particle.dds") },
                { "Pixel",    Texture.FromFile(device, "Pixel.dds") },
                { "Cloud",    Texture.FromFile(device, "Cloud001.png") },
                { "Ring",      Texture.FromFile(device, "Ring001.png") }
            };

            var renderer = new PointSpriteRenderer(device, 2000, textureLookup) {
                //EnableFastFade = true
            };

            var fontDescription = new FontDescription {
                Height         = 14,
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

                if (Keyboard.IsKeyDown(Key.D1))
                    currentEmitter = smokeEmitter;

                if (Keyboard.IsKeyDown(Key.D2))
                    currentEmitter = sparkEmitter;

                if (Keyboard.IsKeyDown(Key.D3))
                    currentEmitter = ringEmitter;

                if (RenderForm.MouseButtons.HasFlag(System.Windows.Forms.MouseButtons.Left)) {
                    currentEmitter.Trigger(new Coordinate(unprojected.X, unprojected.Y));
                }

                updateTimer.Restart();
                smokeEmitter.Update(frameTime);
                sparkEmitter.Update(frameTime);
                ringEmitter.Update(frameTime);
                updateTimer.Stop();
                updateTime = (float)updateTimer.Elapsed.TotalSeconds;

                device.Clear(ClearFlags.Target, Color.Black, 1f, 0);
                device.BeginScene();

                renderTimer.Restart();
                renderer.Render(smokeEmitter, wvp);
                renderer.Render(sparkEmitter, wvp);
                renderer.Render(ringEmitter, wvp);
                renderTimer.Stop();
                var renderTime = (float)renderTimer.Elapsed.TotalSeconds;

                font.DrawText(null, "1 - Smoke, 2 - Sparks, 3 - Rings", 0, 0, Color.White);
                font.DrawText(null, String.Format("Time:        {0}", totalTimer.Elapsed), 0, 32, Color.White);
                font.DrawText(null, String.Format("Particles:   {0:n0}", currentEmitter.ActiveParticles), 0, 48, Color.White);
                font.DrawText(null, String.Format("Update:      {0:n4} ({1,8:P2})", updateTime, updateTime / 0.01666666f), 0, 64, Color.White);
                font.DrawText(null, String.Format("Render:      {0:n4} ({1,8:P2})", renderTime, renderTime / 0.01666666f), 0, 80, Color.White);

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
