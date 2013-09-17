namespace Mercury.ParticleEngine.Modifiers
{
    public abstract class Modifier
    {
        protected internal abstract unsafe void Update(float elapsedSeconds, Particle* particle, int count);
    }
}