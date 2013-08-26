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

    static class Program
    {
        [STAThread]
        static void Main()
        {
            const int width = 1024;
            const int height = 768;
            const bool windowed = true;

            var form = new RenderForm("Mercury Particle Engine - SharpDX.Direct3D9 Sample")
            {
                Size = new System.Drawing.Size(width, height)
            };

            var direct3d = new Direct3D();
            var device = new Device(direct3d, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing | CreateFlags.EnablePresentStatistics, new PresentParameters(width, height) { PresentationInterval = PresentInterval.Default, Windowed = windowed});

            var view = new Matrix(
                1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, -1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, -1.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);
            var proj = Matrix.OrthoOffCenterLH(width * -0.5f, width * 0.5f, height * 0.5f, height * -0.5f, 0f, 100f);
            var wvp = Matrix.Identity * view * proj;

            var emitter1 = new Emitter(1000000, 2.0f, Profile.Point())
            {
                Parameters = new ReleaseParameters
                {
                    Colour   = new ColourRange(new RangeF(0f, 0.5f), new RangeF(0.3f, 0.7f), new RangeF(0.8f, 1f)),
                    Opacity  = 1f,
                    Quantity = 1,
                    Speed    = new RangeF(0f, 2f),
                    Scale    = 32f,
                    Rotation = 0f
                }
            };
            //emitter1.Modifiers.Add(new DampingModifier { DampingCoefficient = 1f });
            emitter1.Modifiers.Add(new RadialGravityModifier { Position = Coordinate.Origin, Radius = 1500f, Strength = 1f });

            var emitter2 = new Emitter(1000000, 2.0f, Profile.Point())
            {
                Parameters = new ReleaseParameters
                {
                    Colour = new ColourRange(1f, new RangeF(0f, 0.3f), new RangeF(0f, 0.5f)),
                    Opacity = 1f,
                    Quantity = 1,
                    Speed = new RangeF(0f, 2f),
                    Scale = 32f,
                    Rotation = 0f
                }
            };
            //emitter2.Modifiers.Add(new DampingModifier { DampingCoefficient = 1f });
            emitter2.Modifiers.Add(new RadialGravityModifier { Position = Coordinate.Origin, Radius = 1500f, Strength = 1f });

            var emitter3 = new Emitter(1000000, 2.0f, Profile.Point())
            {
                Parameters = new ReleaseParameters
                {
                    Colour = new ColourRange(new RangeF(0f, 0.5f), new RangeF(0.5f, 1f), new RangeF(0f, 0.5f)),
                    Opacity = 1f,
                    Quantity = 1,
                    Speed = new RangeF(0f, 2f),
                    Scale = 32f,
                    Rotation = 0f
                }
            };
            //emitter3.Modifiers.Add(new DampingModifier { DampingCoefficient = 1f });
            emitter3.Modifiers.Add(new RadialGravityModifier { Position = Coordinate.Origin, Radius = 1500f, Strength = 1f });

            var renderer = new PointSpriteRenderer(device, 1000000)
            {
                EnableFastFade = true
            };

            var texture = Texture.FromFile(device, "Particle.dds");

            var fontDescription = new FontDescription
            {
                Height = 16,
                FaceName = "Consolas",
                PitchAndFamily = FontPitchAndFamily.Mono,
                Quality = FontQuality.Draft
            };

            var font = new Font(device, fontDescription);

            var totalTimer = Stopwatch.StartNew();
            var updateTimer = new Stopwatch();
            var renderTimer = new Stopwatch();
            var adjustmentTimer = Stopwatch.StartNew();

            var totalTime = 0f;

            var timings = new List<float>();

            RenderLoop.Run(form, () =>
                {
                    var frameTime = ((float)totalTimer.Elapsed.TotalSeconds) - totalTime;
                    totalTime = (float)totalTimer.Elapsed.TotalSeconds;

                    updateTimer.Restart();
                    emitter1.Trigger(new Coordinate((float)Math.Sin(totalTime) * (width * 0.4f), (float)Math.Cos(totalTime * 3f) * (height * 0.4f)));
                    emitter2.Trigger(new Coordinate((float)Math.Sin(totalTime) * (width * -0.4f), (float)Math.Cos(totalTime * 3f) * (height * -0.4f)));
                    emitter3.Trigger(new Coordinate((float)Math.Sin(totalTime + 1.570795) * (width * 0.4f), (float)Math.Sin(totalTime * 2f) * height * 0.4f));

                    Parallel.ForEach(new[] { emitter1, emitter2, emitter3 }, emitter => emitter.Update(frameTime));
                    //emitter1.Update(frameTime);
                    //emitter2.Update(frameTime);
                    updateTimer.Stop();
                    var updateTime = (float)updateTimer.Elapsed.TotalSeconds;

// ReSharper disable AccessToDisposedClosure
                    device.Clear(ClearFlags.Target, Color.Black, 1f, 0);
                    device.BeginScene();

                    renderTimer.Restart();
                    renderer.Render(emitter3, wvp, texture);
                    renderer.Render(emitter1, wvp, texture);
                    renderer.Render(emitter2, wvp, texture);
                    renderTimer.Stop();
                    var renderTime = (float)renderTimer.Elapsed.TotalSeconds;

                    font.DrawText(null, String.Format("Time:        {0}", totalTimer.Elapsed), 0, 0, Color.White);
                    font.DrawText(null, String.Format("Particles:   {0:n0}", emitter1.ActiveParticles + emitter2.ActiveParticles + emitter3.ActiveParticles), 0, 16, Color.White);
                    font.DrawText(null, String.Format("Update:      {0:n4} ({1,8:P2})", updateTime, updateTime / 0.01666666f), 0, 32, Color.White);
                    font.DrawText(null, String.Format("Render:      {0:n4} ({1,8:P2})", renderTime, renderTime / 0.01666666f), 0, 48, Color.White);

                    device.EndScene();
                    device.Present();

                    timings.Add((updateTime + renderTime) / 0.01666666f);

                    if (adjustmentTimer.ElapsedMilliseconds > 200)
                    {
                        var averageTime = timings.Average();
                        timings.Clear();

                        if (averageTime > 1f)
                        {
                            emitter1.Parameters.Quantity = emitter1.Parameters.Quantity.X - 1;
                            emitter2.Parameters.Quantity = emitter2.Parameters.Quantity.X - 1;
                            emitter3.Parameters.Quantity = emitter3.Parameters.Quantity.X - 1;
                        }
                        else if (averageTime < 0.75f)
                        {
                            emitter1.Parameters.Quantity = emitter1.Parameters.Quantity.X + 30;
                            emitter2.Parameters.Quantity = emitter2.Parameters.Quantity.X + 30;
                            emitter3.Parameters.Quantity = emitter3.Parameters.Quantity.X + 30;
                        }
                        else if (averageTime < 0.85f)
                        {
                            emitter1.Parameters.Quantity = emitter1.Parameters.Quantity.X + 10;
                            emitter2.Parameters.Quantity = emitter2.Parameters.Quantity.X + 10;
                            emitter3.Parameters.Quantity = emitter3.Parameters.Quantity.X + 10;
                        }
                        else if (averageTime < 0.95f)
                        {
                            emitter1.Parameters.Quantity = emitter1.Parameters.Quantity.X + 1;
                            emitter2.Parameters.Quantity = emitter2.Parameters.Quantity.X + 1;
                            emitter3.Parameters.Quantity = emitter3.Parameters.Quantity.X + 1;
                        }

                        adjustmentTimer.Restart();
                    }
// ReSharper restore AccessToDisposedClosure
                });

            device.Dispose();
            direct3d.Dispose();
        }
    }
}
