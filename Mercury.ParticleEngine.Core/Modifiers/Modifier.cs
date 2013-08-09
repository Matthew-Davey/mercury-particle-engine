namespace Mercury.ParticleEngine.Modifiers
{
    public abstract class Modifier
    {
        protected internal abstract void Update(float elapsedSeconds, ref ParticleIterator iterator);
    }
}