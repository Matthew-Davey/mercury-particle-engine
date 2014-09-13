namespace Mercury.ParticleEngine.Modifiers {
    using System;

    public class VelocityHueModifier : Modifier {
        public float StationaryHue;
        public float VelocityHue;
        public float VelocityThreshold;

        protected internal override unsafe void Update(float elapsedSeconds, Particle* particle, int count) {
            var velocityThreshold2 = VelocityThreshold * VelocityThreshold;

            while (count-- > 0) {
                var velocity2 = ((particle->Velocity[0] * particle->Velocity[0]) + (particle->Velocity[1] * particle->Velocity[1]));

                if (velocity2 >= velocityThreshold2) {
                    particle->Colour[0] = VelocityHue;
                }
                else {
                    var t = (float)Math.Sqrt(velocity2) / VelocityThreshold;
                    particle->Colour[0] = ((VelocityHue - StationaryHue) * t) + StationaryHue;
                }

                particle++;
            }
        }
    }
}