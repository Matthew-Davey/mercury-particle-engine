namespace Mercury.ParticleEngine.Modifiers {
    using Xunit;
    using FluentAssertions;

    public class ColourInterpolator2Tests {
        public class UpdateMethod {
            [Fact]
            public void WhenParticleLifeIsZero_SetsInitialColour() {
                var particle = new Particle {
                    Age = 0.0f
                };

                var subject = new ColourInterpolator2 {
                    InitialColour = new Colour(1f, 0f, 0f),
                    FinalColour = new Colour(0f, 0f, 1f)
                };

                unsafe {
                    subject.Update(0.01666f, &particle, 1);

                    particle.Colour[0].Should().BeApproximately(1f, 0.000001f);
                    particle.Colour[1].Should().BeApproximately(0f, 0.000001f);
                    particle.Colour[2].Should().BeApproximately(0f, 0.000001f);
                }
            }

            [Fact]
            public void WhenParticleLifeIsOne_SetsFinalColour() {
                var particle = new Particle {
                    Age = 1.0f
                };

                var subject = new ColourInterpolator2 {
                    InitialColour = new Colour(1f, 0f, 0f),
                    FinalColour = new Colour(0f, 0f, 1f)
                };

                unsafe {
                    subject.Update(0.01666f, &particle, 1);

                    particle.Colour[0].Should().BeApproximately(0f, 0.000001f);
                    particle.Colour[1].Should().BeApproximately(0f, 0.000001f);
                    particle.Colour[2].Should().BeApproximately(1f, 0.000001f);
                }
            }

            [Fact]
            public void WhenParticleLifeIsPointFive_SetsColourBetweenInitialAndFinal() {
                var particle = new Particle {
                    Age = 0.5f
                };

                var subject = new ColourInterpolator2 {
                    InitialColour = new Colour(1f, 0f, 0f),
                    FinalColour = new Colour(0f, 0f, 1f)
                };

                unsafe {
                    subject.Update(0.01666f, &particle, 1);

                    particle.Colour[0].Should().BeApproximately(0.5f, 0.000001f);
                    particle.Colour[1].Should().BeApproximately(0f, 0.000001f);
                    particle.Colour[2].Should().BeApproximately(0.5f, 0.000001f);
                }
            }

            [Fact]
            public void IteratesOverEachParticle() {
                var buffer = new Particle[100];
                
                for (int i = 0; i < buffer.Length; i++)
                    buffer[i].Age = 1.0f;

                var subject = new ColourInterpolator2 {
                    InitialColour = new Colour(1f, 0f, 0f),
                    FinalColour = new Colour(0f, 0f, 1f)
                };

                unsafe {
                    fixed (Particle* particle = &buffer[0]) {
                        subject.Update(0.1666666f, particle, buffer.Length);

                        for (int i = 0; i < buffer.Length; i++) {
                            particle[i].Colour[2].Should().BeApproximately(1f, 0.000001f);
                        }
                    }
                }
            }
        }
    }
}