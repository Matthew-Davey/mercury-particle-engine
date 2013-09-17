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
            const int width = 320;
            const int height = 240;
            const bool windowed = true;

            const int budget = 100000;

            const int numEmitters = 5;

            var form = new RenderForm("Mercury Particle Engine - SharpDX.Direct3D9 Sample")
            {
                Size = new System.Drawing.Size(width, height)
            };

            var direct3d = new Direct3D();
            var device = new Device(direct3d, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing | CreateFlags.EnablePresentStatistics, new PresentParameters(width, height) { PresentationInterval = PresentInterval.Immediate, Windowed = windowed});

            var view = new Matrix(
                1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, -1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, -1.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);
            var proj = Matrix.OrthoOffCenterLH(width * -0.5f, width * 0.5f, height * 0.5f, height * -0.5f, 0f, 100f);
            var wvp = Matrix.Identity * view * proj;

            var releaseParameters = new ReleaseParameters
                {
                    Colour   = new ColourRange(new RangeF(.5f, 1f), new RangeF(.5f, 1f), new RangeF(.5f, 1f)),
                    Opacity  = 1f,
                    Quantity = 1,
                    Speed    = new RangeF(0f, 200f),
                    Scale    = 1f,
                    Rotation = 0f
                };

            var modifiers = new List<Modifier>
            {
                new RadialGravityModifier
                {
                    Position = Coordinate.Origin,
                    Radius = 1500f,
                    Strength = 250f
                },
                new ColourInterpolator2
                {
                    //InitialColour = new Colour(0f, 0f, 0f),
                    InitialColour = new Colour(.8f, .3f, 0f),
                    FinalColour = new Colour(.5f, .1f, 1f)
                },
                new DampingModifier
                {
                    DampingCoefficient = .25f
                }
            };

            var emitters = new Emitter[numEmitters];

            for (int i = 0; i < numEmitters; i++)
            {
                emitters[i] = new Emitter(budget, TimeSpan.FromSeconds(2), Profile.Point())
                {
                    Parameters = releaseParameters,
                    BlendMode = BlendMode.Add,
                    Modifiers = modifiers
                };
            }

            var renderer = new PointSpriteRenderer(device, budget)
            {
                //EnableFastFade = true
            };

            var texture = Texture.FromFile(device, "Pixel.dds");

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

                    modifiers.OfType<RadialGravityModifier>().Single().Position = new Coordinate(
                        (float)Math.Sin(totalTime * 2f) * (width * 0.1f),
                        (float)Math.Cos(totalTime * 3f) * (height * 0.1f)
                    );

                    updateTimer.Restart();

                    for (int i = 0; i < numEmitters; i++)
                    {
                        emitters[i].Trigger(new Coordinate((float)Math.Sin(totalTime + i) * (width * 0.4f), (float)Math.Cos(((totalTime * 0.5f) + i ) * 4f) * (height * 0.4f)));
                    }

                    Parallel.ForEach(emitters, emitter => emitter.Update(frameTime));
                    updateTimer.Stop();
                    var updateTime = (float)updateTimer.Elapsed.TotalSeconds;

// ReSharper disable AccessToDisposedClosure
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

                    timings.Add((updateTime + renderTime) / 0.01666666f);

                    if (adjustmentTimer.ElapsedMilliseconds > 200)
                    {
                        var averageTime = timings.Average();
                        timings.Clear();

                        if (averageTime > 1f)
                        {
                            releaseParameters.Quantity = Math.Max(releaseParameters.Quantity.X - 1, 0);
                        }
                        else if (averageTime < 0.75f)
                        {
                            releaseParameters.Quantity = releaseParameters.Quantity.X + 30;
                        }
                        else if (averageTime < 0.85f)
                        {
                            releaseParameters.Quantity = releaseParameters.Quantity.X + 10;
                        }
                        else if (averageTime < 0.95f)
                        {
                            releaseParameters.Quantity = releaseParameters.Quantity.X + 1;
                        }

                        adjustmentTimer.Restart();
                    }
// ReSharper restore AccessToDisposedClosure
                });

            font.Dispose();
            device.Dispose();
            direct3d.Dispose();
        }
    }
}
