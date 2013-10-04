namespace Mercury.ParticleEngine.Modifiers
{
    public class MoveModifier : Modifier
    {
        protected internal override unsafe void Update(float elapsedSeconds, Particle* particle, int count)
        {
            while (count-- > 0)
            {
                particle->Position[0] += particle->Velocity[0] * elapsedSeconds;
                particle->Position[1] += particle->Velocity[1] * elapsedSeconds;

                particle++;
            }
        }
    }
}