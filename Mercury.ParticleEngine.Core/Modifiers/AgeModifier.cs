namespace Mercury.ParticleEngine.Modifiers
{
    using System;

    public class AgeModifier : Modifier
    {
        public AgeModifier(TimeSpan term)
        {
            _term = (float)term.TotalSeconds;
        }

        private float _term;
        private float _totalSeconds;

        protected internal override unsafe void Update(float elapsedSeconds, Particle* particle, int count)
        {
            _totalSeconds += elapsedSeconds;

            while (count-- > 0)
            {
                particle->Age = (_totalSeconds - particle->Inception) / _term;

                particle++;
            }
        }
    }
}