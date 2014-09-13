namespace Mercury.ParticleEngine.Profiles {
    using System;
    using Xunit;
    using FluentAssertions;

    public class PointProfileTests {
        public class GetOffsetAndHeadingMethod {
            [Fact]
            public void ReturnsZeroOffset() {
                var subject = new PointProfile();
                var values = new float[4];

                unsafe {
                    fixed (float* offset = &values[0])
                    fixed (float* heading = &values[2]) {
                        subject.GetOffsetAndHeading((Coordinate*)offset, (Axis*)heading);

                        offset[0].Should().Be(0f);
                        offset[1].Should().Be(0f);
                    }
                }
            }

            [Fact]
            public void ReturnsHeadingAsUnitVector() {
                var subject = new PointProfile();
                var values = new float[4];

                unsafe {
                    fixed (float* offset = &values[0])
                    fixed (float* heading = &values[2]) {
                        subject.GetOffsetAndHeading((Coordinate*)offset, (Axis*)heading);

                        var length = Math.Sqrt((heading[0] * heading[0]) + (heading[1] * heading[1]));
                        length.Should().BeApproximately(1f, 0.000001);
                    }
                }
            }
        }
    }
}