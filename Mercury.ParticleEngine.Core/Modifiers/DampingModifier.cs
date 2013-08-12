namespace Mercury.ParticleEngine.Modifiers
{
    public class DampingModifier : Modifier
    {
        public float DampingCoefficient;

        protected internal unsafe override void Update(float elapsedSeconds, ref ParticleIterator iterator)
        {
            var particle = iterator.First;
            var deltaDampingCoefficient = 1f - (DampingCoefficient * elapsedSeconds);

            do
            {
                particle->Velocity[0] *= deltaDampingCoefficient;
                particle->Velocity[1] *= deltaDampingCoefficient;
            }
            while (iterator.MoveNext(&particle));
        }
    }
}