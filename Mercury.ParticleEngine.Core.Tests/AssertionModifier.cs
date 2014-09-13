namespace Mercury.ParticleEngine.Modifiers {
    using System;
    using FluentAssertions;

    internal class AssertionModifier : Modifier {
        readonly Predicate<Particle> _predicate;

        public AssertionModifier(Predicate<Particle> predicate) {
            _predicate = predicate;
        }

        protected internal override unsafe void Update(float elapsedSeconds, Particle* particle, int count) {
            while (count-- > 0) {
                _predicate(*particle).Should().BeTrue();

                particle++;
            }
        }
    }
}