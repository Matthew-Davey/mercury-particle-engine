namespace Mercury.ParticleEngine {
    using System;
    using Xunit;
    using FluentAssertions;

    public class ParticleBufferTests {
        public class AvailableProperty
        {
            [Fact]
            public void WhenNoParticlesReleased_ReturnsBufferSize() {
                var subject = new ParticleBuffer(100);

                subject.Available.Should().Be(100);
            }

            [Fact]
            public void WhenSomeParticlesReleased_ReturnsAvailableCount() {
                var subject = new ParticleBuffer(100);

                unsafe {
                    Particle* particle;
                    subject.Release(10, out particle);
                }

                subject.Available.Should().Be(90);
            }

            [Fact]
            public void WhenAllParticlesReleased_ReturnsZero() {
                var subject = new ParticleBuffer(100);

                unsafe {
                    Particle* particle;
                    subject.Release(100, out particle);
                }

                subject.Available.Should().Be(0);
            }
        }

        public class CountProperty {
            [Fact]
            public void WhenNoParticlesReleased_ReturnsZero() {
                var subject = new ParticleBuffer(100);
                subject.Count.Should().Be(0);
            }

            [Fact]
            public void WhenSomeParticlesReleased_ReturnsCount() {
                var subject = new ParticleBuffer(100);
                
                unsafe {
                    Particle* particle;
                    subject.Release(10, out particle);
                }
                
                subject.Count.Should().Be(10);
            }

            [Fact]
            public void WhenAllParticlesReleased_ReturnsZero() {
                var subject = new ParticleBuffer(100);

                unsafe {
                    Particle* particle;
                    subject.Release(100, out particle);
                }

                subject.Count.Should().Be(100);
            }
        }

        public class ReleaseMethod
        {
            [Fact]
            public void WhenPassedReasonableQuantity_ReturnsNumberReleased() {
                var subject = new ParticleBuffer(100);

                unsafe {
                    Particle* particle;
                    var count = subject.Release(50, out particle);
                    
                    count.Should().Be(50);
                }
            }

            [Fact]
            public void WhenPassedImpossibleQuantity_ReturnsNumberActuallyReleased() {
                var subject = new ParticleBuffer(100);

                unsafe {
                    Particle* particle;
                    var count = subject.Release(200, out particle);
                    count.Should().Be(100);
                }
            }
        }

        public class ReclaimMethod {
            [Fact]
            public void WhenPassedReasonableNumber_ReclaimsParticles() {
                var subject = new ParticleBuffer(100);
                
                unsafe {
                    Particle* particle;
                    subject.Release(100, out particle);
                }

                subject.Count.Should().Be(100);

                subject.Reclaim(50);

                subject.Count.Should().Be(50);
            }
        }

        public class CopyToMethod {
            [Fact]
            public void WhenBufferIsSequential_CopiesParticlesInOrder() {
                unsafe {
                    var subject = new ParticleBuffer(10);
                    Particle* particle;
                    var count = subject.Release(5, out particle);

                    do {
                        particle->Age = 1f;
                        particle++;
                    }
                    while (count-- > 0);

                    var destination = new Particle[10];

                    fixed (Particle* buffer = destination) {
                        subject.CopyTo((IntPtr)buffer);
                    }

                    destination[0].Age.Should().BeApproximately(1f, 0.0001f);
                    destination[1].Age.Should().BeApproximately(1f, 0.0001f);
                    destination[2].Age.Should().BeApproximately(1f, 0.0001f);
                    destination[3].Age.Should().BeApproximately(1f, 0.0001f);
                    destination[4].Age.Should().BeApproximately(1f, 0.0001f);
                }
            }
        }

        public class DisposeMethod {
            [Fact]
            public void IsIdempotent() {
                var subject = new ParticleBuffer(100);
                subject.Dispose();
                subject.Dispose();
            }
        }
    }
}