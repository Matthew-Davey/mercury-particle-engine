namespace Mercury.ParticleEngine.Modifiers
{
    public class DampingModifier : Modifier
    {
        public float DampingCoefficient;

        protected internal unsafe override void Update(float elapsedSeconds, Particle* particle, int count)
        {
            var deltaDampingCoefficient = 1f - (DampingCoefficient * elapsedSeconds);

            while (count-- > 0)
            {
                particle->Velocity[0] *= deltaDampingCoefficient;
                particle->Velocity[1] *= deltaDampingCoefficient;
                
                particle++;
            }
        }
    }
}