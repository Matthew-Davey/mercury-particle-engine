namespace Mercury.ParticleEngine.Profiles
{
    using System;
    using Xunit;
    using FluentAssertions;

    public class RingProfileTests
    {
        public class GetOffsetAndHeadingMethod
        {
            [Fact]
            public void ReturnsOffsetEqualToRadius()
            {
                var subject = new RingProfile
                {
                    Radius = 10f
                };
                var values = new float[4];

                unsafe
                {
                    fixed (float* offset = &values[0])
                    fixed (float* heading = &values[2])
                    {
                        subject.GetOffsetAndHeading(offset, heading);

                        var length = Math.Sqrt((heading[0] * heading[0]) + (heading[1] * heading[1]));
                        length.Should().BeApproximately(10f, 0.000001f);
                    }
                }
            }

            [Fact]
            public void WhenRadiateIsTrue_HeadingIsEqualToNormalizedOffset()
            {
                var subject = new RingProfile
                {
                    Radius = 10f,
                    Radiate = true
                };
                var values = new float[4];

                unsafe
                {
                    fixed (float* offset = &values[0])
                    fixed (float* heading = &values[2])
                    {
                        subject.GetOffsetAndHeading(offset, heading);

                        heading[0].Should().BeApproximately(offset[0] / 10f, 0.000001f);
                        heading[1].Should().BeApproximately(offset[1] / 10f, 0.000001f);
                    }
                }
            }
        }
    }
}