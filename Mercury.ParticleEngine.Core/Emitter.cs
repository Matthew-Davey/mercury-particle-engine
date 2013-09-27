namespace Mercury.ParticleEngine
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Mercury.ParticleEngine.Modifiers;
    using Mercury.ParticleEngine.Profiles;

    public unsafe class Emitter : IDisposable
    {
        private readonly float _term;
        private float _totalSeconds;

        public Emitter(int capacity, TimeSpan term, Profile profile)
        {
            _term = (float)term.TotalSeconds;

            Buffer = new ParticleBuffer(capacity);
            Profile = profile;
            Modifiers = new List<Modifier>();
            Parameters = new ReleaseParameters();
        }

        internal ParticleBuffer Buffer { get; private set; }

        public int ActiveParticles
        {
            get { return Buffer.Count; }
        }

        public IList<Modifier> Modifiers { get; set; }
        public Profile Profile { get; private set; }
        public ReleaseParameters Parameters { get; set; }
        public BlendMode BlendMode { get; set; }

        private void ReclaimExpiredParticles()
        {
            Particle* particle;
            var count = Buffer.Iter(out particle);

            var expired = 0;
            
            while (count-- > 0)
            {
                if ((_totalSeconds - particle->Inception) < _term)
                    break;
                
                expired++;
                particle++;
            }

            Buffer.Reclaim(expired);
        }

        public void Update(float elapsedSeconds)
        {
            _totalSeconds += elapsedSeconds;

            if (Buffer.Count == 0)
                return;

            ReclaimExpiredParticles();

            var particle = (Particle*)0;
            var count = Buffer.Iter(out particle);

            while (count-- > 0)
            {
                particle->Age = (_totalSeconds - particle->Inception) / _term;

                particle->Position[0] += particle->Velocity[0] * elapsedSeconds;
                particle->Position[1] += particle->Velocity[1] * elapsedSeconds;

                particle++;
            }

            if (Buffer.Count > 0)
            {
                count = Buffer.Iter(out particle);

                Parallel.ForEach(Modifiers, modifier => modifier.Update(elapsedSeconds, particle, count));
            }
        }

        public void Trigger(Coordinate position)
        {
            var numToRelease = FastRand.NextInteger(Parameters.Quantity);

            Particle* particle;
            var count = Buffer.Release(numToRelease, out particle);

            while (count-- > 0)
            {
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

        public void Dispose()
        {
            Buffer.Dispose();
            GC.SuppressFinalize(this);
        }

        ~Emitter()
        {
            Dispose();
        }
    }
}