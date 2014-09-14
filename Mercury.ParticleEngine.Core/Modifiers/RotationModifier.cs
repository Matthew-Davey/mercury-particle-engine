namespace Mercury.ParticleEngine.Modifiers {
    using System;

    public class RotationModifier : Modifier {
        public float RotationRate;

        protected internal override unsafe void Update(float elapsedSeconds, Particle* particle, int count) {
            var rotationRateDelta = RotationRate * elapsedSeconds;

            while (count-- > 0) {
                particle->Rotation += rotationRateDelta;
                particle++;
            }
        }
    }
}