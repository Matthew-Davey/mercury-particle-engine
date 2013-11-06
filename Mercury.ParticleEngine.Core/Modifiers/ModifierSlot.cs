namespace Mercury.ParticleEngine.Modifiers
{
    using Mercury.ParticleEngine.Modifiers;

    internal class ModifierSlot
    {
        public Modifier Modifier { get; set; }

        public float Interval { get; set; }

        public float Delay { get; set; }

        private float _secondsSinceLastUpdate;

        public ModifierSlot(Modifier modifier, float interval = 0f, float delay = 0f)
        {
            Modifier = modifier;
            Interval = interval;
            Delay = delay;
        }

        public unsafe void Update(float elapsedSeconds, Particle* particle, int count)
        {
            _secondsSinceLastUpdate += elapsedSeconds;

            if (_secondsSinceLastUpdate > Interval)
            {
                Modifier.Update(_secondsSinceLastUpdate, particle, count);

                _secondsSinceLastUpdate = 0f;
            }
        }
    }
}