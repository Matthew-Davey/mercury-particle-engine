namespace Mercury.ParticleEngine {
    using System;
    using Xunit;
    using FluentAssertions;
    using Mercury.ParticleEngine.Modifiers;
    using Mercury.ParticleEngine.Profiles;

    public class EmitterTests {
        public class UpdateMethod {
            [Fact]
            public void WhenThereAreParticlesToExpire_DecreasesActiveParticleCount() {
                var subject = new Emitter(100, TimeSpan.FromSeconds(1), Profile.Point()) {
                    Parameters = new ReleaseParameters {
                        Quantity = 1
                    }
                };

                subject.Trigger(new Coordinate(0f, 0f));
                subject.ActiveParticles.Should().Be(1);

                subject.Update(2f);
                subject.ActiveParticles.Should().Be(0);
            }

            [Fact]
            public void WhenThereAreParticlesToExpire_DoesNotPassExpiredParticlesToModifiers() {
                var subject = new Emitter(100, TimeSpan.FromSeconds(1), Profile.Point()) {
                    Parameters = new ReleaseParameters {
                        Quantity = 1
                    }
                };

                subject.Modifiers.Add(new AssertionModifier(particle => particle.Age <= 1f));

                subject.Trigger(new Coordinate(0f, 0f));
                subject.Update(0.5f);
                subject.Trigger(new Coordinate(0f, 0f));
                subject.Update(0.5f);
                subject.Trigger(new Coordinate(0f, 0f));
                subject.Update(0.5f);
            }

            [Fact]
            public void WhenThereAreNoActiveParticles_GracefullyDoesNothing() {
                var subject = new Emitter(100, TimeSpan.FromSeconds(1), Profile.Point());

                subject.Update(0.5f);

                subject.ActiveParticles.Should().Be(0);
            }
        }

        public class TriggerMethod {
            [Fact]
            public void WhenEnoughHeadroom_IncreasesActiveParticlesCountByReleaseQuantity() {
                var subject = new Emitter(100, TimeSpan.FromSeconds(1), Profile.Point()) {
                    Parameters = new ReleaseParameters {
                        Quantity = 10
                    }
                };

                subject.ActiveParticles.Should().Be(0);
                subject.Trigger(new Coordinate(0f, 0f));
                subject.ActiveParticles.Should().Be(10);
            }

            [Fact]
            public void WhenNotEnoughHeadroom_IncreasesActiveParticlesCountByRemainingParticles() {
                var subject = new Emitter(15, TimeSpan.FromSeconds(1), Profile.Point()) {
                    Parameters = new ReleaseParameters {
                        Quantity = 10
                    }
                };

                subject.Trigger(new Coordinate(0f, 0f));
                subject.ActiveParticles.Should().Be(10);
                subject.Trigger(new Coordinate(0f, 0f));
                subject.ActiveParticles.Should().Be(15);
            }

            [Fact]
            public void WhenNoRemainingParticles_DoesNotIncreaseActiveParticlesCount() {
                var subject = new Emitter(10, TimeSpan.FromSeconds(1), Profile.Point()) {
                    Parameters = new ReleaseParameters {
                        Quantity = 10
                    }
                };

                subject.Trigger(new Coordinate(0f, 0f));
                subject.ActiveParticles.Should().Be(10);
                subject.Trigger(new Coordinate(0f, 0f));
                subject.ActiveParticles.Should().Be(10);
            }
        }

        public class DisposeMethod {
            [Fact]
            public void IsIdempotent() {
                var subject = new Emitter(10, TimeSpan.FromSeconds(1), Profile.Point());

                subject.Dispose();
                subject.Dispose();
            }
        }
    }
}