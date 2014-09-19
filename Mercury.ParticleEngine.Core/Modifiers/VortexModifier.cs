namespace Mercury.ParticleEngine.Modifiers {
    using System;

    public class VortexModifier : Modifier {
        public Coordinate Position;
        public float Mass;
        public float MaxSpeed;

        protected internal override unsafe void Update(float elapsedSeconds, Particle* particle, int count) {
            while (count-- > 0) {
                var distx = Position._x - particle->Position[0];
                var disty = Position._y - particle->Position[1];

                var distance2 = (distx * distx) + (disty * disty);
                var distance = (float)Math.Sqrt(distance2);

                var m = (10000f * Mass * particle->Mass) / distance2;

                m = Math.Max(Math.Min(m, MaxSpeed), -MaxSpeed) * elapsedSeconds;

                distx = (distx / distance) * m;
                disty = (disty / distance) * m;

                particle->Velocity[0] += distx;
                particle->Velocity[1] += disty;

                particle++;
            }
        }
    }
}