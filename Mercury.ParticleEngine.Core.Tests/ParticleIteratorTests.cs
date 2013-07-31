namespace Mercury.ParticleEngine
{
    using System;
    using Xunit;
    using FluentAssertions;

    public class ParticleIteratorTests
    {
        public class CountProperty
        {
            [Fact]
            public void ReturnsNumberOfIterations()
            {
                var particles = new Particle[100];

                unsafe
                {
                    fixed (Particle* particle = &particles[0])
                    {
                        var subject = new ParticleIterator(particle, 100, 0, 100);

                        subject.Count.Should().Be(100);
                    }
                }
            }
        }

        public class RemainingProperty
        {
            [Fact]
            public void TracksIterations()
            {
                var particles = new Particle[100];

                unsafe
                {
                    fixed (Particle* particle = &particles[0])
                    {
                        var subject = new ParticleIterator(particle, 100, 0, 100);

                        var iteration = 0;
                        var ptr = subject.First;

                        do
                        {
                            subject.Remaining.Should().Be(100 - iteration);
                            iteration++;
                        }
                        while (subject.MoveNext(&ptr));
                    }
                }
            }
        }

        public class MoveNextMethod
        {
            [Fact]
            public void WhenThereAreRemainingIterations_ReturnsTrue()
            {
                var particles = new Particle[2];

                unsafe
                {
                    fixed (Particle* particle = &particles[0])
                    {
                        var subject = new ParticleIterator(particle, 2, 0, 2);

                        var ptr = subject.First;
                        subject.MoveNext(&ptr).Should().BeTrue();
                    }
                }
            }

            [Fact]
            public void WhenThereAreRemainingIterations_PointsPointerToNextParticle()
            {
                var particles = new Particle[2];

                unsafe
                {
                    fixed (Particle* particle = &particles[0])
                    {
                        var subject = new ParticleIterator(particle, 2, 0, 2);

                        var ptr = subject.First;
                        subject.MoveNext(&ptr);

                        ((IntPtr)ptr).Should().Be((IntPtr)(particle + 1));
                    }
                }
            }

            [Fact]
            public void WhenThereAreNoRemainingIterations_ReturnsFalse()
            {
                var particles = new Particle[2];

                unsafe
                {
                    fixed (Particle* particle = &particles[0])
                    {
                        var subject = new ParticleIterator(particle, 2, 0, 2);

                        var ptr = subject.First;
                        subject.MoveNext(&ptr);
                        subject.MoveNext(&ptr).Should().BeFalse();
                    }
                }
            }
        }

        public class ResetMethod
        {
            [Fact]
            public void ResetsRemainingCount()
            {
                var particles = new Particle[100];

                unsafe
                {
                    fixed (Particle* particle = &particles[0])
                    {
                        var subject = new ParticleIterator(particle, 100, 0, 100);

                        var ptr = subject.First;
                        do { }
                        while (subject.MoveNext(&ptr));

                        subject.Remaining.Should().Be(0);
                        subject.Reset();
                        subject.Remaining.Should().Be(100);
                    }
                }
            }
        }
    }
}