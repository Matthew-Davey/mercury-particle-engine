namespace Mercury.ParticleEngine.Modifiers
{
    public class DragModifier : Modifier
    {
        public float DragCoefficient = 0.47f;
        public float Density = .5f;

        protected internal unsafe override void Update(float elapsedSeconds, Particle* particle, int count)
        {
            while (count-- > 0)
            {
                particle->Velocity[0] += (particle->Velocity[0] * -DragCoefficient * Density * particle->Mass * elapsedSeconds);
                particle->Velocity[1] += (particle->Velocity[1] * -DragCoefficient * Density * particle->Mass * elapsedSeconds);

                particle++;
            }
        }
    }
}