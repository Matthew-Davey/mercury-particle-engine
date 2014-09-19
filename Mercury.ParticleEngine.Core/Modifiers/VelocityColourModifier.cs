namespace Mercury.ParticleEngine.Modifiers {
    using System;

    public class VelocityColourModifier : Modifier {
        public Colour StationaryColour;
        public Colour VelocityColour;
        public float VelocityThreshold;

        protected internal override unsafe void Update(float elapsedSeconds, Particle* particle, int count) {
            var velocityThreshold2 = VelocityThreshold * VelocityThreshold;

            while (count-- > 0) {
                var velocity2 = ((particle->Velocity[0] * particle->Velocity[0]) + (particle->Velocity[1] * particle->Velocity[1]));
                var deltaColour = VelocityColour - StationaryColour;

                if (velocity2 >= velocityThreshold2) {
                    VelocityColour.CopyTo(particle->Colour);
                }
                else {
                    var t = (float)Math.Sqrt(velocity2) / VelocityThreshold;

                    particle->Colour[0] = (deltaColour._h * t) + StationaryColour._h;
                    particle->Colour[1] = (deltaColour._s * t) + StationaryColour._s;
                    particle->Colour[2] = (deltaColour._l * t) + StationaryColour._l;
                }

                particle++;
            }
        }
    }
}