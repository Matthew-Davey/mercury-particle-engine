namespace Mercury.ParticleEngine
{
    using System;
    using FluentAssertions;
    using Xunit;
    using Xunit.Extensions;

    public class ColourTests
    {
        public class Constructor
        {
            [Fact]
            [Trait("Type", "Colour")]
            public void WhenGivenValues_ReturnsInitializedColour()
            {
                var colour = new Colour(1f, 1f, 1f);

                colour.H.Should<float>().Be(1f);
                colour.S.Should<float>().Be(1f);
                colour.L.Should<float>().Be(1f);
            }
        }

        public class EqualsColourMethod
        {
            [Fact]
            [Trait("Type", "Colour")]
            public void WhenGivenEqualValues_ReturnsTrue()
            {
                var x = new Colour(360f, 1f, 1f);
                var y = new Colour(360f, 1f, 1f);

                x.Equals(y).Should().BeTrue();
            }

            [Fact]
            [Trait("Type", "Colour")]
            public void WhenGivenDifferentValues_ReturnsFalse()
            {
                var x = new Colour(0f, 1f, 0f);
                var y = new Colour(360f, 1f, 1f);

                x.Equals(y).Should().BeFalse();
            }
        }
        
        public class EqualsObjectMethod
        {
            [Fact]
            [Trait("Type", "Colour")]
            public void WhenGivenNull_ReturnsFalse()
            {
                var colour = new Colour(360f, 1f, 1f);

                colour.Equals(null).Should().BeFalse();
            }

            [Fact]
            [Trait("Type", "Colour")]
            public void WhenGivenEqualColour_ReturnsTrue()
            {
                var x = new Colour(360f, 1f, 1f);

                Object y = new Colour(360f, 1f, 1f);

                x.Equals(y).Should().BeTrue();
            }

            [Fact]
            [Trait("Type", "Colour")]
            public void WhenGivenDifferentColour_ReturnsFalse()
            {
                var x = new Colour(360f, 1f, 1f);

                Object y = new Colour(0f, 1f, 0f);

                x.Equals(y).Should().BeFalse();
            }

            [Fact]
            [Trait("Type", "Colour")]
            public void WhenGivenObjectOfAntotherType_ReturnsFalse()
            {
                var colour = new Colour(360f, 1f, 1f);

// ReSharper disable SuspiciousTypeConversion.Global
                colour.Equals(DateTime.Now).Should().BeFalse();
// ReSharper restore SuspiciousTypeConversion.Global
            }
        }

        public class GetHashCodeMethod
        {
            [Fact]
            [Trait("Type", "Colour")]
            public void WhenObjectsAreDifferent_YieldsDifferentHashCodes()
            {
                var x = new Colour(0f, 1f, 0f);
                var y = new Colour(360f, 1f, 1f);

                x.GetHashCode().Should().NotBe(y.GetHashCode());
            }

            [Fact]
            [Trait("Type", "Colour")]
            public void WhenObjectsAreSame_YieldsIdenticalHashCodes()
            {
                var x = new Colour(180f, 0.5f, 0.5f);
                var y = new Colour(180f, 0.5f, 0.5f);

                x.GetHashCode().Should().Be(y.GetHashCode());
            }
        }

        public class ToStringMethod
        {
            [Theory]
            [Trait("Type", "Colour")]
            [InlineData(0f, 0f, 0f, "0°,0%,0%")]
            [InlineData(360f, 1f, 1f, "360°,100%,100%")]
            [InlineData(180f, 0.5f, 0.5f, "180°,50%,50%")]
            public void ReturnsCorrectValue(float h, float s, float l, String expected)
            {
                var colour = new Colour(h, s, l);

                colour.ToString().Should().Be(expected);
            }
        }
    }
}