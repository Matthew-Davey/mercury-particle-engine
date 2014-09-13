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
                    particle->Colour[0] = VelocityColour.H;
                    particle->Colour[1] = VelocityColour.S;
                    particle->Colour[2] = VelocityColour.L;
                }
                else {
                    var t = (float)Math.Sqrt(velocity2) / VelocityThreshold;

                    particle->Colour[0] = ((VelocityColour.H - StationaryColour.H) * t) + StationaryColour.H;
                    particle->Colour[1] = ((VelocityColour.S - StationaryColour.S) * t) + StationaryColour.S;
                    particle->Colour[2] = ((VelocityColour.L - StationaryColour.L) * t) + StationaryColour.L;
                }

                particle++;
            }
        }
    }
}