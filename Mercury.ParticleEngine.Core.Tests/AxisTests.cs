namespace Mercury.ParticleEngine {
    using System;
    using FluentAssertions;
    using Xunit;

    public class AxisTests {
        public class Constructor {
            [Fact, Trait("Type", "Axis")]
            public void WhenGivenAxisValues_ReturnsInitializedAxis() {
                var axis = new Axis(1f, 0f);

                axis.Match((x, y) => {
                    x.Should().Be(1f);
                    y.Should().Be(0f);
                });
            }

            [Fact, Trait("Type", "Axis")]
            public void WhenGivenAngle_ReturnsInitializedAxis() {
                var axis = new Axis((float)Math.PI / 2f);

                axis.Match((x, y) => {
                    x.Should().BeApproximately(0f, 0.0000001f);
                    y.Should().Be(1f);
                });
            }
        }

        public class MultiplyMethod {
            [Fact, Trait("Type", "Axis")]
            public void WhenMultiplied_ReturnsVectorOfMagnitude() {
                var result = Axis.Left.Multiply(2f);

                result.Magnitude.Should().Be(2f);
            }
        }

        public class UnsafeCopyToMethod {
            [Fact, Trait("Type", "Axis")]
            public void WhenGivenBuffer_CopiesAxisValues() {
                var result = new float[2];

                unsafe {
                    fixed (float* p = result) {
                        Axis.Right.CopyTo(p);

                        p[0].Should().Be(1f);
                        p[1].Should().Be(0f);
                    }
                }
            }
        }

        public class DestructureMethod {
            [Fact, Trait("Type", "Axis")]
            public void CopiesAxisValues() {
                float x, y;
                Axis.Right.Destructure(out x, out y);

                x.Should().Be(1f);
                y.Should().Be(0f);
            }
        }

        public class MatchMethod {
            [Fact, Trait("Type", "Axis")]
            public void WhenGivenCallback_CallsCallbackWithAxisValues() {
                Axis.Right.Match((x, y) => {
                    x.Should().Be(1f);
                    y.Should().Be(0f);
                });
            }

            [Fact, Trait("Type", "Axis")]
            public void WhenGivenNullCallback_ThrowsArgumentNullException() {
                Action action = () => Axis.Right.Match(null);
                action.ShouldThrow<ArgumentNullException>();
            }
        }

        public class MapMethod {
            [Fact, Trait("Type", "Axis")]
            public void WhenGivenMapFunction_CallsMapFunctionWithAxisValues() {
                var result = Axis.Right.Map((x, y) => {
                    x.Should().Be(1f);
                    y.Should().Be(0f);
                    return -1f;
                });

                result.Should().Be(-1f);
            }

            [Fact, Trait("Type", "Axis")]
            public void WhenGivenNullMapFunction_ThrowsArgumentNullException() {
                Action action = () => Axis.Right.Map<object>(null);
                action.ShouldThrow<ArgumentNullException>();
            }
        }

        public class EqualsObjectMethod {
            [Fact, Trait("Type", "Axis")]
            public void WhenGivenNull_ReturnsFalse() {
                var axis = new Axis(1f, 0f);

                axis.Equals(null).Should().BeFalse();
            }

            [Fact, Trait("Type", "Axis")]
            public void WhenGivenEqualAxis_ReturnsTrue() {
                var x = new Axis(1f, 0f);

                Object y = new Axis(1f, 0f);

                x.Equals(y).Should().BeTrue();
            }

            [Fact, Trait("Type", "Axis")]
            public void WhenGivenDifferentAxis_ReturnsFalse() {
                var x = new Axis(1f, 0f);

                Object y = new Axis(0f, 1f);

                x.Equals(y).Should().BeFalse();
            }

            [Fact, Trait("Type", "Axis")]
            public void WhenGivenObjectOfAntotherType_ReturnsFalse() {
                var axis = new Axis(1f, 0f);

                // ReSharper disable SuspiciousTypeConversion.Global
                axis.Equals(DateTime.Now).Should().BeFalse();
                // ReSharper restore SuspiciousTypeConversion.Global
            }
        }

        public class GetHashCodeMethod
        {
            [Fact, Trait("Type", "Axis")]
            public void WhenObjectsAreDifferent_YieldsDifferentHashCodes() {
                var x = new Axis(0f, 1f);
                var y = new Axis(1f, 0f);

                x.GetHashCode().Should().NotBe(y.GetHashCode());
            }

            [Fact, Trait("Type", "Axis")]
            public void WhenObjectsAreSame_YieldsIdenticalHashCodes() {
                var x = new Axis(0f, 1f);
                var y = new Axis(0f, 1f);

                x.GetHashCode().Should().Be(y.GetHashCode());
            }
        }
    }
}
