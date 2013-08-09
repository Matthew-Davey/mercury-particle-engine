namespace Mercury.ParticleEngine.Modifiers
{
    public unsafe sealed class OpacityFastFadeModifier : Modifier
    {
        protected internal override void Update(float elapsedSeconds, ref ParticleIterator iterator)
        {
            var particle = iterator.First;

            do
            {
                particle->Opacity = 1.0f - particle->Age;
            }
            while (iterator.MoveNext(&particle));
        }
    }
}