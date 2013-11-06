namespace Mercury.ParticleEngine
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using SharpDX;
    using SharpDX.Direct3D9;
    using SharpDX.Windows;
    using Mercury.ParticleEngine.Modifiers;
    using Mercury.ParticleEngine.Profiles;
    using Mercury.ParticleEngine.Renderers;
    using System.Threading.Tasks;
    using System.Windows.Input;

    static class Program
    {
        [STAThread]
        static void Main()
        {
            var worldSize = new Size2(1024, 768);
            var renderSize = new Size2(1920, 1080);
            const bool windowed = false;

            const int numParticles = 1000000;
            const int numEmitters = 5;
            const int budget = numParticles / numEmitters;

            var form = new RenderForm("Mercury Particle Engine - SharpDX.Direct3D9 Sample")
            {
                Size = new System.Drawing.Size(renderSize.Width, renderSize.Height)
            };

            var direct3d = new Direct3D();
            var device = new Device(direct3d, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(renderSize.Width, renderSize.Height) { PresentationInterval = PresentInterval.Immediate, Windowed = windowed });

            var view = new Matrix(
                1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, -1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, -1.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);
            var proj = Matrix.OrthoOffCenterLH(worldSize.Width * -0.5f, worldSize.Width * 0.5f, worldSize.Height * 0.5f, worldSize.Height * -0.5f, 0f, 1f);
            var wvp = Matrix.Identity * view * proj;

            var emitters = new Emitter[numEmitters];

            for (int i = 0; i < numEmitters; i++)
            {
                emitters[i] = new Emitter(budget, TimeSpan.FromSeconds(600), Profile.BoxFill(worldSize.Width, worldSize.Height))
                {
                    Parameters = new ReleaseParameters
                    {
                        Colour = new Colour(220f, 0.7f, 0.1f),
                        Opacity = 1f,
                        Quantity = budget,
                        Speed = 0f,
                        Scale = 1f,
                        Rotation = 0f,
                        Mass = new RangeF(8f, 12f)
                    },
                    BlendMode = BlendMode.Add,
                    ReclaimInterval = 600f
                };

                emitters[i].Modifiers.Add(new DragModifier
                {
                    DragCoefficient = .47f,
                    Density         = .15f
                }, 1f / 15f, 1f / 15f);
                emitters[i].Modifiers.Add(new VortexModifier
                {
                    Position = Coordinate.Origin,
                    Mass = 200f,
                    MaxSpeed = 1000f
                }, 1f / 30f, 1f / 30f);
                emitters[i].Modifiers.Add(new VelocityHueModifier
                {
                    StationaryHue = 220f,
                    VelocityHue = 300f,
                    VelocityThreshold = 800f
                }, 1f / 15f);
                emitters[i].Modifiers.Add(new ContainerModifier
                        {
                            RestitutionCoefficient = 0.75f,
                            Position = Coordinate.Origin,
                            Width    = worldSize.Width,
                            Height   = worldSize.Height
                        }, 1f / 30f, 1f / 60f);
                emitters[i].Modifiers.Add(new MoveModifier(), 1f / 60f);
            };

            var renderer = new PointSpriteRenderer(device, budget)
            {
                //EnableFastFade = true
            };

            var texture = Texture.FromFile(device, "Pixel.dds");

            var fontDescription = new FontDescription
            {
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

            foreach (var emitter in emitters)
            {
                emitter.Trigger(Coordinate.Origin);
            }

            float updateTime = 0f;

            RenderLoop.Run(form, () =>
                {
                    // ReSharper disable AccessToDisposedClosure
                    var frameTime = ((float)totalTimer.Elapsed.TotalSeconds) - totalTime;
                    totalTime = (float)totalTimer.Elapsed.TotalSeconds;

                    var mousePosition = form.PointToClient(RenderForm.MousePosition);

                    Task.WaitAll(
                        Task.Factory.StartNew(() =>
                        {
                            var mouseVector = new Vector3(mousePosition.X, mousePosition.Y, 0f);
                            var unprojected = Vector3.Unproject(mouseVector, 0, 0, renderSize.Width, renderSize.Height, 0f, 1f, wvp);

                            Parallel.ForEach(emitters, emitter => ((VortexModifier)emitter.Modifiers.ElementAt(1)).Position = new Coordinate(unprojected.X, unprojected.Y));

                            updateTimer.Restart();
                            Parallel.ForEach(emitters, emitter => emitter.Update(frameTime));
                            updateTimer.Stop();
                            updateTime = (float)updateTimer.Elapsed.TotalSeconds;
                        }),
                        Task.Factory.StartNew(() =>
                        {
                            device.Clear(ClearFlags.Target, Color.Black, 1f, 0);
                            device.BeginScene();

                            renderTimer.Restart();
                            for (int i = 0; i < numEmitters; i++)
                            {
                                renderer.Render(emitters[i], wvp, texture);
                            }
                            renderTimer.Stop();
                            var renderTime = (float)renderTimer.Elapsed.TotalSeconds;

                            font.DrawText(null, String.Format("Time:        {0}", totalTimer.Elapsed), 0, 0, Color.White);
                            font.DrawText(null, String.Format("Particles:   {0:n0}", emitters[0].ActiveParticles * numEmitters), 0, 16, Color.White);
                            font.DrawText(null, String.Format("Update:      {0:n4} ({1,8:P2})", updateTime, updateTime / 0.01666666f), 0, 32, Color.White);
                            font.DrawText(null, String.Format("Render:      {0:n4} ({1,8:P2})", renderTime, renderTime / 0.01666666f), 0, 48, Color.White);

                            device.EndScene();
                            device.Present();
                        })
                    );

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
