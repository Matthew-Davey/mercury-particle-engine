namespace Mercury.ParticleEngine.Modifiers
{
    public abstract class Modifier
    {
        internal abstract void Update(ref ParticleIterator iterator);
    }
}