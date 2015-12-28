namespace Mercury.ParticleEngine.Modifiers {
    using System;

    public abstract class Modifier {
        const float DefaultModifierFrequency = 60.0f;

        private float _frequency;
        private float _cycleTime;
        private int _particlesUpdatedThisCycle;

        protected Modifier() {
            Frequency = DefaultModifierFrequency;
        }

        public float Frequency {
            get { return _frequency; }
            set {
                if (value > 0.0f == false)
                    throw new ArgumentOutOfRangeException(nameof(value), "Frequency must be greater than zero.");

                _frequency = value;
                _cycleTime = 1f / _frequency;
            }
        }

        internal unsafe void InternalUpdate(float elapsedSeconds, Particle* buffer, int count) {
            var particlesRemaining = count - _particlesUpdatedThisCycle;
            var particlesToUpdate = Math.Min(particlesRemaining, (int)Math.Ceiling((elapsedSeconds / _cycleTime) * count));

            if (particlesToUpdate > 0) {
                Update(_cycleTime, buffer + _particlesUpdatedThisCycle, particlesToUpdate);

                _particlesUpdatedThisCycle += particlesToUpdate;
            }

            if (_particlesUpdatedThisCycle >= count)
                _particlesUpdatedThisCycle = 0;
        }

        protected internal abstract unsafe void Update(float elapsedSeconds, Particle* particle, int count);
    }
}