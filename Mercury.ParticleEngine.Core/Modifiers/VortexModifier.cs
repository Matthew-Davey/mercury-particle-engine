namespace Mercury.ParticleEngine.Modifiers
{
    using System;

    public class VortexModifier : Modifier
    {
        public Coordinate Position;
        public float Mass;

        protected internal override unsafe void Update(float elapsedSeconds, Particle* particle, int count)
        {
            while (count-- > 0)
            {
                double distx = Position._x - particle->Position[0];
                double disty = Position._y - particle->Position[1];

                var distance = Math.Sqrt((distx * distx) + (disty * disty));

                var m = (10000d * Mass * particle->Mass) / (distance * distance);

                m = Math.Min(m, 1500d);

                distx /= distance;
                disty /= distance;

                distx *= m * elapsedSeconds;
                disty *= m * elapsedSeconds;

                particle->Velocity[0] += (float)distx;
                particle->Velocity[1] += (float)disty;

                particle++;
            }
        }
    }
}