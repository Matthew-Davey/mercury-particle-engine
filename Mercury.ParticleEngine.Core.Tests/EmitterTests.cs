namespace Mercury.ParticleEngine
{
    using Xunit;
    using FluentAssertions;
    using Mercury.ParticleEngine.Modifiers;
    using Mercury.ParticleEngine.Profiles;

    public class EmitterTests
    {
        public class UpdateMethod
        {
            [Fact]
            public void WhenThereAreParticlesToExpire_DecreasesActiveParticleCount()
            {
                var subject = new Emitter(100, 1f, Profile.Point())
                {
                    ReleaseQuantity = 1
                };

                subject.Trigger(0f, 0f);
                subject.ActiveParticles.Should().Be(1);

                subject.Update(2f);
                subject.ActiveParticles.Should().Be(0);
            }

            [Fact]
            public void WhenThereAreActiveParticles_UpdatesAgeOfParticles()
            {
                var subject = new Emitter(100, 1f, Profile.Point())
                {
                    ReleaseQuantity = 100
                };

                subject.Modifiers.Add(new AssertionModifier(particle => particle.Age > 0f));

                subject.Trigger(0f, 0f);
                subject.Update(.1f);
            }

            [Fact]
            public unsafe void WhenThereAreActiveParticles_ZerosParticleVelocity()
            {
                var subject = new Emitter(100, 1f, Profile.Point())
                {
                    ReleaseQuantity = 100
                };

                subject.Modifiers.Add(new AssertionModifier(particle =>
                        particle.Velocity[0] < 0.000001f &&
                        particle.Velocity[1] < 0.000001f));

                subject.Trigger(0f, 0f);
                subject.Update(.1f);
            }

            [Fact]
            public void WhenThereAreParticlesToExpire_DoesNotPassExpiredParticlesToModifiers()
            {
                var subject = new Emitter(100, 1f, Profile.Point())
                {
                    ReleaseQuantity = 50
                };

                subject.Modifiers.Add(new AssertionModifier(particle => particle.Age <= 1f));

                subject.Trigger(0f, 0f);
                subject.Update(0.5f);
                subject.Trigger(0f, 0f);
                subject.Update(0.5f);
                subject.Trigger(0f, 0f);
                subject.Update(0.5f);
            }
        }

        public class TriggerMethod
        {
            [Fact]
            public void WhenEnoughHeadroom_IncreasesActiveParticlesCountByReleaseQuantity()
            {
                var subject = new Emitter(100, 1f, Profile.Point())
                {
                    ReleaseQuantity = 10
                };

                subject.ActiveParticles.Should().Be(0);
                subject.Trigger(0f, 0f);
                subject.ActiveParticles.Should().Be(10);
            }

            [Fact]
            public void WhenNotEnoughHeadroom_IncreasesActiveParticlesCountByRemainingParticles()
            {
                var subject = new Emitter(15, 1f, Profile.Point())
                {
                    ReleaseQuantity = 10
                };

                subject.Trigger(0f, 0f);
                subject.ActiveParticles.Should().Be(10);
                subject.Trigger(0f, 0f);
                subject.ActiveParticles.Should().Be(15);
            }

            [Fact]
            public void WhenNoRemainingParticles_DoesNotIncreaseActiveParticlesCount()
            {
                var subject = new Emitter(10, 1f, Profile.Point())
                {
                    ReleaseQuantity = 10
                };

                subject.Trigger(0f, 0f);
                subject.ActiveParticles.Should().Be(10);
                subject.Trigger(0f, 0f);
                subject.ActiveParticles.Should().Be(10);
            }
        }

        public class DisposeMethod
        {
            [Fact]
            public void IsIdempotent()
            {
                var subject = new Emitter(10, 1f, Profile.Point());

                subject.Dispose();
                subject.Dispose();
            }
        }
    }
}