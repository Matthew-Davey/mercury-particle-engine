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
            var worldSize = new DrawingPoint(1024, 768);
            var renderSize = new DrawingPoint(1280, 720);
            const bool windowed = true;

            const int budget = 200000;

            const int numEmitters = 5;

            var form = new RenderForm("Mercury Particle Engine - SharpDX.Direct3D9 Sample")
            {
                Size = new System.Drawing.Size(renderSize.X, renderSize.Y)
            };

            var direct3d = new Direct3D();
            var device = new Device(direct3d, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(renderSize.X, renderSize.Y) { PresentationInterval = PresentInterval.Default, Windowed = windowed });

            var view = new Matrix(
                1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, -1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, -1.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);
            var proj = Matrix.OrthoOffCenterLH(worldSize.X * -0.5f, worldSize.X * 0.5f, worldSize.Y * 0.5f, worldSize.Y * -0.5f, 0f, 1f);
            var wvp = Matrix.Identity * view * proj;

            var releaseParameters = new ReleaseParameters
                {
                    Colour   = new Colour(220f, 0.7f, 0.3f),
                    Opacity  = 1f,
                    Quantity = budget,
                    Speed    = new RangeF(0f, 200f),
                    Scale    = 1f,
                    Rotation = 0f,
                    Mass     = new RangeF(8f, 12f)
                };

            var modifiers = new List<Modifier>
            {
                new VortexModifier
                {
                    Position = Coordinate.Origin,
                    Mass     = 200f,
                    MaxSpeed = 1000f
                },
                //new ColourInterpolator2
                //{
                //    //InitialColour = new Colour(0f, 0f, 0f),
                //    InitialColour   = new Colour(280f, .5f, .5f),
                //    FinalColour     = new Colour(220f, .5f, .5f)
                //},
                //new VelocityColourModifier
                //{
                //    StationaryColour  = new Colour(220f, 0.7f, 0.5f),
                //    VelocityColour    = new Colour(300f, 0.7f, 0.5f),
                //    VelocityThreshold = 800f
                //},
                new VelocityHueModifier
                {
                    StationaryHue     = 220f,
                    VelocityHue       = 300f,
                    VelocityThreshold = 800f
                },
                new DragModifier
                {
                    DragCoefficient = .47f,
                    Density         = .15f
                },
                new ContainerModifier
                {
                    Position = Coordinate.Origin,
                    Width    = worldSize.X,
                    Height   = worldSize.Y
                },
                //new HueInterpolator2
                //{
                //    InitialHue = 80f,
                //    FinalHue   = 300f
                //},
                //new LinearGravityModifier(Axis.Down, 10f)
            };

            var emitters = new Emitter[numEmitters];

            for (int i = 0; i < numEmitters; i++)
            {
                emitters[i] = new Emitter(budget, TimeSpan.FromSeconds(600), Profile.BoxFill(worldSize.X, worldSize.Y))
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

            emitters[0].Trigger(Coordinate.Origin);
            emitters[1].Trigger(Coordinate.Origin);
            emitters[2].Trigger(Coordinate.Origin);
            emitters[3].Trigger(Coordinate.Origin);
            emitters[4].Trigger(Coordinate.Origin);

            RenderLoop.Run(form, () =>
                {
                    float updateTime;

                    var renderTask = Task.Factory.StartNew(() =>
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
                        //font.DrawText(null, String.Format("Update:      {0:n4} ({1,8:P2})", updateTime, updateTime / 0.01666666f), 0, 32, Color.White);
                        //font.DrawText(null, String.Format("Render:      {0:n4} ({1,8:P2})", renderTime, renderTime / 0.01666666f), 0, 48, Color.White);

                        device.EndScene();
                        device.Present();
                    });

                    var frameTime = ((float)totalTimer.Elapsed.TotalSeconds) - totalTime;
                    totalTime = (float)totalTimer.Elapsed.TotalSeconds;

                    var mousePosition = form.PointToClient(RenderForm.MousePosition);

                    Vector3 mouseVector = new Vector3(mousePosition.X, mousePosition.Y, 0f);
                    var unprojected = Vector3.Unproject(mouseVector, 0, 0, renderSize.X, renderSize.Y, 0f, 1f, wvp);

                    modifiers.OfType<VortexModifier>().First().Position = new Coordinate(unprojected.X, unprojected.Y);

                    updateTimer.Restart();

                    //for (int i = 0; i < numEmitters; i++)
                    //{
                    //    emitters[i].Trigger(Coordinate.Origin);
                    //    emitters[i].Trigger(new Coordinate((float)Math.Sin(totalTime + i) * (worldSize.X * 0.4f), (float)Math.Cos(((totalTime * 0.5f) + i) * 4f) * (worldSize.Y * 0.4f)));
                    //}

                    Parallel.ForEach(emitters, emitter => emitter.Update(frameTime));
                    updateTimer.Stop();
                    updateTime = (float)updateTimer.Elapsed.TotalSeconds;

// ReSharper disable AccessToDisposedClosure
                    Task.WaitAll(renderTask);

                    if (Keyboard.IsKeyDown(Key.Escape))
                        Environment.Exit(0);

                    if (Keyboard.IsKeyDown(Key.Space))
                        modifiers.OfType<VortexModifier>().Single().Mass = -200f;
                    else
                        modifiers.OfType<VortexModifier>().Single().Mass = 200f;
// ReSharper restore AccessToDisposedClosure
                });

            font.Dispose();
            device.Dispose();
            direct3d.Dispose();
        }
    }
}
