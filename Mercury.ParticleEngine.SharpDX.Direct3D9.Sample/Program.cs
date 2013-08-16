namespace Mercury.ParticleEngine
{
    using System;
    using System.Diagnostics;
    using SharpDX;
    using SharpDX.Direct3D9;
    using SharpDX.Windows;
    using Mercury.ParticleEngine.Modifiers;
    using Mercury.ParticleEngine.Profiles;
    using Mercury.ParticleEngine.Renderers;

    static class Program
    {
        [STAThread]
        static void Main()
        {
            var form = new RenderForm("Mercury Particle Engine - SharpDX.Direct3D9 Sample");

            var direct3d = new Direct3D();
            var device = new Device(direct3d, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing | CreateFlags.EnablePresentStatistics, new PresentParameters(form.ClientSize.Width, form.ClientSize.Height) { PresentationInterval = PresentInterval.Default });

            var view = new Matrix(
                1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, -1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, -1.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);
            var proj = Matrix.OrthoOffCenterLH(form.ClientSize.Width * -0.5f, form.ClientSize.Width * 0.5f, form.ClientSize.Height * 0.5f, form.ClientSize.Height * -0.5f, 0f, 100f);
            var wvp = Matrix.Identity * view * proj;

            var emitter1 = new Emitter(500000, 2.0f, Profile.Point())
            {
                Parameters = new ReleaseParameters
                {
                    Colour   = new ColourRange(new RangeF(0f, 0.5f), new RangeF(0.3f, 0.7f), new RangeF(0.8f, 1f)),
                    Opacity  = 1f,
                    Quantity = 1,
                    Speed    = new RangeF(0f, 3f),
                    Scale    = 32f,
                    Rotation = 0f
                }
            };
            emitter1.Modifiers.Add(new DampingModifier { DampingCoefficient = 1f });

            var emitter2 = new Emitter(500000, 2.0f, Profile.Point())
            {
                Parameters = new ReleaseParameters
                {
                    Colour = new ColourRange(1f, new RangeF(0f, 0.3f), new RangeF(0f, 0.5f)),
                    Opacity = 1f,
                    Quantity = 1,
                    Speed = new RangeF(0f, 3f),
                    Scale = 32f,
                    Rotation = 0f
                }
            };
            emitter2.Modifiers.Add(new DampingModifier { DampingCoefficient = 1f });

            var renderer = new PointSpriteRenderer(device, 500000)
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

            RenderLoop.Run(form, () =>
                {
                    var frameTime = ((float)totalTimer.Elapsed.TotalSeconds) - totalTime;
                    totalTime = (float)totalTimer.Elapsed.TotalSeconds;

                    updateTimer.Restart();
                    emitter1.Trigger((float)Math.Sin(totalTime) * 300f, (float)Math.Cos(totalTime * 3f) * 200f);
                    emitter2.Trigger((float)Math.Sin(totalTime) * -300f, (float)Math.Cos(totalTime * 3f) * -200f);

                    emitter1.Update(frameTime);
                    emitter2.Update(frameTime);
                    updateTimer.Stop();
                    var updateTime = (float)updateTimer.Elapsed.TotalSeconds;

// ReSharper disable AccessToDisposedClosure
                    device.Clear(ClearFlags.Target, Color.Black, 1f, 0);
                    device.BeginScene();

                    renderTimer.Restart();
                    renderer.Render(emitter1, wvp, texture);
                    renderer.Render(emitter2, wvp, texture);
                    renderTimer.Stop();
                    var renderTime = (float)renderTimer.Elapsed.TotalSeconds;

                    font.DrawText(null, String.Format("Time:        {0}", totalTimer.Elapsed), 0, 0, Color.White);
                    font.DrawText(null, String.Format("Particles:   {0:n0}", emitter1.ActiveParticles + emitter2.ActiveParticles), 0, 16, Color.White);
                    font.DrawText(null, String.Format("Update:      {0:n4} ({1,8:P2})", updateTime, updateTime / 0.01666666f), 0, 32, Color.White);
                    font.DrawText(null, String.Format("Render:      {0:n4} ({1,8:P2})", renderTime, renderTime / 0.01666666f), 0, 48, Color.White);

                    device.EndScene();
                    device.Present();

                    if (adjustmentTimer.ElapsedMilliseconds > 500)
                    {
                        if ((updateTime / 0.01666666f) + (renderTime / 0.01666666f) > 1f)
                        {
                            emitter1.Parameters.Quantity = emitter1.Parameters.Quantity.X - 1;
                            emitter2.Parameters.Quantity = emitter2.Parameters.Quantity.X - 1;
                        }
                        else if ((updateTime / 0.01666666f) + (renderTime / 0.01666666f) < 0.5f)
                        {
                            emitter1.Parameters.Quantity = emitter1.Parameters.Quantity.X + 10;
                            emitter2.Parameters.Quantity = emitter2.Parameters.Quantity.X + 10;
                        }
                        else if ((updateTime / 0.01666666f) + (renderTime / 0.01666666f) < 0.75f)
                        {
                            emitter1.Parameters.Quantity = emitter1.Parameters.Quantity.X + 5;
                            emitter2.Parameters.Quantity = emitter2.Parameters.Quantity.X + 5;
                        }
                        else if ((updateTime / 0.01666666f) + (renderTime / 0.01666666f) < 0.995f)
                        {
                            emitter1.Parameters.Quantity = emitter1.Parameters.Quantity.X + 1;
                            emitter2.Parameters.Quantity = emitter2.Parameters.Quantity.X + 1;
                        }

                        adjustmentTimer.Restart();
                    }

                    form.Text = emitter1.Parameters.Quantity.ToString();
// ReSharper restore AccessToDisposedClosure
                });

            device.Dispose();
            direct3d.Dispose();
        }
    }
}
