namespace Mercury.ParticleEngine
{
    using System;
    using Xunit;
    using FluentAssertions;

    public class ParticleBufferTests
    {
        public class AvailableProperty
        {
            [Fact]
            public void WhenNoParticlesReleased_ReturnsBufferSize()
            {
                var subject = new ParticleBuffer(100);

                subject.Available.Should().Be(100);
            }

            [Fact]
            public void WhenSomeParticlesReleased_ReturnsAvailableCount()
            {
                var subject = new ParticleBuffer(100);
                subject.Release(10);
                subject.Available.Should().Be(90);
            }

            [Fact]
            public void WhenAllParticlesReleased_ReturnsZero()
            {
                var subject = new ParticleBuffer(100);
                subject.Release(100);

                subject.Available.Should().Be(0);
            }
        }

        public class CountProperty
        {
            [Fact]
            public void WhenNoParticlesReleased_ReturnsZero()
            {
                var subject = new ParticleBuffer(100);
                subject.Count.Should().Be(0);
            }

            [Fact]
            public void WhenSomeParticlesReleased_ReturnsCount()
            {
                var subject = new ParticleBuffer(100);
                subject.Release(10);
                subject.Count.Should().Be(10);
            }

            [Fact]
            public void WhenAllParticlesReleased_ReturnsZero()
            {
                var subject = new ParticleBuffer(100);
                subject.Release(100);

                subject.Count.Should().Be(100);
            }
        }

        public class ReleaseMethod
        {
            [Fact]
            public void WhenPassedReasonableQuantity_ReturnsIterator()
            {
                var subject = new ParticleBuffer(100);
                var iterator = subject.Release(50);

                iterator.Count.Should().Be(50);
            }

            [Fact]
            public void WhenPassedImpossibleQuantity_ReturnsPartialIterator()
            {
                var subject = new ParticleBuffer(100);
                var iterator = subject.Release(200);

                iterator.Count.Should().Be(100);
            }
        }

        public class ReclaimMethod
        {
            [Fact]
            public void WhenPassedReasonableNumber_ReclaimsParticles()
            {
                var subject = new ParticleBuffer(100);
                subject.Release(100);

                subject.Count.Should().Be(100);

                subject.Reclaim(50);

                subject.Count.Should().Be(50);
            }
        }

        public class GetIteratorMethod
        {
            [Fact]
            public void WhenThereAreNoActiveParticles_ReturnsNopIterator()
            {
                var subject = new ParticleBuffer(100);
                var iterator = subject.GetIterator();

                iterator.Count.Should().Be(0);
            }

            [Fact]
            public void WhenThereAreActiveParticles_ReturnsIteratorOverActiveParticles()
            {
                var subject = new ParticleBuffer(100);
                subject.Release(50);

                var iterator = subject.GetIterator();

                iterator.Count.Should().Be(50);
            }
        }

        public class DisposeMethod
        {
            [Fact]
            public void IsIdempotent()
            {
                var subject = new ParticleBuffer(100);
                subject.Dispose();
                subject.Dispose();
            }
        }
    }
}