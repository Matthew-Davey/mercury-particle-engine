namespace Mercury.ParticleEngine
{
    using System;
    using System.Runtime.Serialization;
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

                colour.R.Should<Single>().Be(1f);
                colour.G.Should<Single>().Be(1f);
                colour.B.Should<Single>().Be(1f);
            }
        }

        public class ParseMethod
        {
            [Fact]
            [Trait("Type", "Colour")]
            public void WhenGivenInvalidString_ThrowsFormatException()
            {
                Action invocation = () => Colour.Parse("invalid");

                invocation.ShouldThrow<FormatException>();
            }

            [Fact]
            [Trait("Type", "Colour")]
            public void WhenGivenValidString_ReturnsNewColour()
            {
                const String hex = "#FFFFFF";

                var actual = Colour.Parse(hex);

                actual.R.Should<Single>().Be(1f);
                actual.G.Should<Single>().Be(1f);
                actual.B.Should<Single>().Be(1f);
            }
        }

        public class EqualsColourMethod
        {
            [Fact]
            [Trait("Type", "Colour")]
            public void WhenGivenEqualValues_ReturnsTrue()
            {
                var x = Colour.Parse("#FFFFFF");
                var y = Colour.Parse("#FFFFFF");

                x.Equals(y).Should().BeTrue();
            }

            [Fact]
            [Trait("Type", "Colour")]
            public void WhenGivenDifferentValues_ReturnsFalse()
            {
                var x = Colour.Parse("#000000");
                var y = Colour.Parse("#FFFFFF");

                x.Equals(y).Should().BeFalse();
            }
        }
        
        public class EqualsObjectMethod
        {
            [Fact]
            [Trait("Type", "Colour")]
            public void WhenGivenNull_ReturnsFalse()
            {
                var colour = new Colour(1f, 1f, 1f);

                colour.Equals(null).Should().BeFalse();
            }

            [Fact]
            [Trait("Type", "Colour")]
            public void WhenGivenEqualColour_ReturnsTrue()
            {
                var x = Colour.Parse("#FFFFFF");

                Object y = Colour.Parse("#FFFFFF");

                x.Equals(y).Should().BeTrue();
            }

            [Fact]
            [Trait("Type", "Colour")]
            public void WhenGivenDifferentColour_ReturnsFalse()
            {
                var x = Colour.Parse("#FFFFFF");

                Object y = Colour.Parse("#000000");

                x.Equals(y).Should().BeFalse();
            }

            [Fact]
            [Trait("Type", "Colour")]
            public void WhenGivenObjectOfAntotherType_ReturnsFalse()
            {
                var colour = new Colour(1f, 1f, 1f);

                colour.Equals(DateTime.Now).Should().BeFalse();
            }
        }

        public class GetHashCodeMethod
        {
            [Fact]
            [Trait("Type", "Colour")]
            public void WhenObjectsAreDifferent_YieldsDifferentHashCodes()
            {
                var x = new Colour(0f, 0f, 0f);
                var y = new Colour(1f, 1f, 1f);

                x.GetHashCode().Should().NotBe(y.GetHashCode());
            }

            [Fact]
            [Trait("Type", "Colour")]
            public void WhenObjectsAreSame_YieldsIdenticalHashCodes()
            {
                var x = new Colour(0.5f, 0.5f, 0.5f);
                var y = new Colour(0.5f, 0.5f, 0.5f);

                x.GetHashCode().Should().Be(y.GetHashCode());
            }
        }

        public class ToStringMethod
        {
            [Theory]
            [Trait("Type", "Colour")]
            [InlineData(0f, 0f, 0f, "#000000")]
            [InlineData(1f, 1f, 1f, "#FFFFFF")]
            [InlineData(0.5f, 0.5f, 0.5f, "#808080")]
            public void ReturnsCorrectValue(Single r, Single g, Single b, String expected)
            {
                var colour = new Colour(r, g, b);

                colour.ToString().Should().Be(expected);
            }
        }
    }
}