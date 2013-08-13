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
            var device = new Device(direct3d, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing | CreateFlags.EnablePresentStatistics, new PresentParameters(form.ClientSize.Width, form.ClientSize.Height));

            var view = new Matrix(
                1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, -1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, -1.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);
            var proj = Matrix.OrthoOffCenterLH(form.ClientSize.Width * -0.5f, form.ClientSize.Width * 0.5f, form.ClientSize.Height * 0.5f, form.ClientSize.Height * -0.5f, 0f, 100f);
            var wvp = Matrix.Identity * view * proj;

            var emitter1 = new Emitter(5000, 2.0f, Profile.Point())
            {
                Parameters = new ReleaseParameters
                {
                    Colour   = new ColourRange(new RangeF(0f, 0.5f), new RangeF(0.3f, 0.7f), new RangeF(0.8f, 1f)),
                    Opacity  = new RangeF(1f, 1f),
                    Quantity = new Range(40, 40),
                    Speed    = new RangeF(0f, 2f),
                    Scale    = new RangeF(32f, 32f),
                    Rotation = new RangeF(-3.14157f, 3.14157f)
                }
            };
            emitter1.Modifiers.Add(new OpacityFastFadeModifier());
            emitter1.Modifiers.Add(new DampingModifier { DampingCoefficient = 0.5f });

            var emitter2 = new Emitter(5000, 2.0f, Profile.Point())
            {
                Parameters = new ReleaseParameters
                {
                    Colour = new ColourRange(new RangeF(1f, 1f), new RangeF(0f, 0.3f), new RangeF(0f, 0.5f)),
                    Opacity = new RangeF(1f, 1f),
                    Quantity = new Range(40, 40),
                    Speed = new RangeF(0f, 2f),
                    Scale = new RangeF(32f, 32f),
                    Rotation = new RangeF(-3.14157f, 3.14157f)
                }
            };
            emitter2.Modifiers.Add(new OpacityFastFadeModifier());
            emitter2.Modifiers.Add(new DampingModifier { DampingCoefficient = 0.5f });

            var renderer = new PointSpriteRenderer(device, 5000);

            var texture = Texture.FromFile(device, "Particle.dds");

            var stopwatch = Stopwatch.StartNew();
            var totalTime = 0f;

            RenderLoop.Run(form, () =>
                {
                    var elapsedTime = (float)stopwatch.Elapsed.TotalSeconds;

                    stopwatch.Reset();
                    stopwatch.Start();

                    if (form.Focused)
                    {
                        totalTime += elapsedTime;

                        emitter1.Trigger((float)Math.Sin(totalTime) * 350f, (float)Math.Cos(totalTime * 3f) * 200f);
                        emitter2.Trigger((float)Math.Sin(totalTime) * -350f, (float)Math.Cos(totalTime * 3f) * -200f);
                        
                        emitter1.Update(elapsedTime);
                        emitter2.Update(elapsedTime);
                    }

// ReSharper disable AccessToDisposedClosure
                    device.Clear(ClearFlags.Target, Color.Black, 1f, 0);
                    device.BeginScene();

                    renderer.Render(emitter1, wvp, texture);
                    renderer.Render(emitter2, wvp, texture);

                    device.EndScene();
                    device.Present();

                    System.Threading.Thread.Sleep(15);
// ReSharper restore AccessToDisposedClosure
                });

            device.Dispose();
            direct3d.Dispose();
        }
    }
}
