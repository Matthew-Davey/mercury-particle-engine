namespace Mercury.ParticleEngine {
    using System;
    using Mercury.ParticleEngine.Modifiers;
    using Mercury.ParticleEngine.Profiles;

    public unsafe class Emitter : IDisposable {
        public Emitter(int capacity, TimeSpan term, Profile profile) {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            _term = (float)term.TotalSeconds;

            Buffer                    = new ParticleBuffer(capacity);
            Profile                   = profile;
            Modifiers                 = new Modifier[0];
            ModifierExecutionStrategy = ModifierExecutionStrategy.Serial;
            Parameters                = new ReleaseParameters();
            ReclaimFrequency          = 60f;
        }

        private readonly float _term;
        private float _totalSeconds;

        internal readonly ParticleBuffer Buffer;

        public int ActiveParticles => Buffer.Count;

        public Modifier[] Modifiers { get; set; }
        public ModifierExecutionStrategy ModifierExecutionStrategy { get; set; }
        public Profile Profile { get; private set; }
        public ReleaseParameters Parameters { get; set; }
        public BlendMode BlendMode { get; set; }
        public RenderingOrder RenderingOrder { get; set; }
        public String TextureKey { get; set; }

        public float ReclaimFrequency { get; set; }
        private float _secondsSinceLastReclaim;

        private void ReclaimExpiredParticles() {
            var particle = (Particle*)Buffer.NativePointer;
            var count = Buffer.Count;

            var expired = 0;

            while (count-- > 0) {
                if ((_totalSeconds - particle->Inception) < _term)
                    break;

                expired++;
                particle++;
            }

            Buffer.Reclaim(expired);
        }

        public void Update(float elapsedSeconds) {
            _totalSeconds += elapsedSeconds;
            _secondsSinceLastReclaim += elapsedSeconds;

            if (Buffer.Count == 0)
                return;

            if (_secondsSinceLastReclaim > (1f / ReclaimFrequency)) {
                ReclaimExpiredParticles();
                _secondsSinceLastReclaim -= (1f / ReclaimFrequency);
            }

            if (Buffer.Count > 0) {
                var particle = (Particle*)Buffer.NativePointer;
                var count = Buffer.Count;

                while (count-- > 0) {
                    particle->Age = (_totalSeconds - particle->Inception) / _term;

                    particle->Position[0] += particle->Velocity[0] * elapsedSeconds;
                    particle->Position[1] += particle->Velocity[1] * elapsedSeconds;

                    particle++;
                }

                ModifierExecutionStrategy.ExecuteModifiers(Modifiers, elapsedSeconds, (Particle*)Buffer.NativePointer, Buffer.Count);
            }
        }

        public void Trigger(Coordinate position) {
            var numToRelease = FastRand.NextInteger(Parameters.Quantity);

            Release(position, numToRelease);
        }

        public void Trigger(LineSegment line) {
            var numToRelease = FastRand.NextInteger(Parameters.Quantity);
            var lineVector = line.ToVector();

            for (var i = 0; i < numToRelease; i++) {
                var offset = lineVector * FastRand.NextSingle();
                Release(line.Origin.Translate(offset), 1);
            }
        }

        private void Release(Coordinate position, int numToRelease) {
            Particle* particle;
            var count = Buffer.Release(numToRelease, out particle);

            while (count-- > 0) {
                Profile.GetOffsetAndHeading((Coordinate*)particle->Position, (Axis*)particle->Velocity);

                particle->Age = 0f;
                particle->Inception = _totalSeconds;

                particle->Position[0] += position._x;
                particle->Position[1] += position._y;

                var speed = FastRand.NextSingle(Parameters.Speed);

                particle->Velocity[0] *= speed;
                particle->Velocity[1] *= speed;

                FastRand.NextColour((Colour*)particle->Colour, Parameters.Colour);

                particle->Opacity  = FastRand.NextSingle(Parameters.Opacity);
                particle->Scale    = FastRand.NextSingle(Parameters.Scale);
                particle->Rotation = FastRand.NextSingle(Parameters.Rotation);
                particle->Mass     = FastRand.NextSingle(Parameters.Mass);

                particle++;
            }
        }

        public void Dispose() {
            Buffer.Dispose();
            GC.SuppressFinalize(this);
        }

        ~Emitter() {
            Dispose();
        }
    }
}
