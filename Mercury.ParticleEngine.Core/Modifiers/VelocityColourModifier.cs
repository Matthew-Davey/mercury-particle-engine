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

                if (velocity2 >= velocityThreshold2) {
                    particle->Colour[0] = VelocityColour._h;
                    particle->Colour[1] = VelocityColour._s;
                    particle->Colour[2] = VelocityColour._l;
                }
                else {
                    var t = (float)Math.Sqrt(velocity2) / VelocityThreshold;

                    particle->Colour[0] = ((VelocityColour._h - StationaryColour._h) * t) + StationaryColour._h;
                    particle->Colour[1] = ((VelocityColour._s - StationaryColour._s) * t) + StationaryColour._s;
                    particle->Colour[2] = ((VelocityColour._l - StationaryColour._l) * t) + StationaryColour._l;
                }

                particle++;
            }
        }
    }
}