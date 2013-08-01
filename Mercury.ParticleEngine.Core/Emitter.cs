namespace Mercury.ParticleEngine
{
    using System;
    using System.Collections.Generic;
    using Mercury.ParticleEngine.Modifiers;

    public unsafe class Emitter
    {
        private readonly float _term;
        private float _totalSeconds;

        public Emitter(int capacity, float term, EmitterShape shape)
            : this(capacity, TimeSpan.FromSeconds(term), shape)
        {
        }

        public Emitter(int capacity, TimeSpan term, EmitterShape shape)
        {
            _term = (float)term.TotalSeconds;

            Buffer = new ParticleBuffer(capacity);
            Shape = shape;
            Modifiers = new List<Modifier>();
        }

        internal ParticleBuffer Buffer { get; private set; }

        public int ActiveParticles
        {
            get { return Buffer.Count; }
        }

        public IList<Modifier> Modifiers { get; private set; }
        public EmitterShape Shape { get; private set; }
        public int ReleaseQuantity { get; set; }
        public float ReleaseSpeed { get; set; }

        public void Update(float elapsedSeconds)
        {
            _totalSeconds += elapsedSeconds;

            if (Buffer.Count == 0)
                return;

            var iterator = Buffer.GetIterator();
            var particle = iterator.First;

            do
            {
                particle->Age = (_totalSeconds - particle->Inception) / _term;

                if (particle->Age > 1f)
                    break;

                particle->Position[0] += particle->Velocity[0];
                particle->Position[1] += particle->Velocity[1];
            }
            while (iterator.MoveNext(&particle));

            if (iterator.Remaining > 0)
                Buffer.Reclaim(iterator.Remaining);

            if (Buffer.Count > 0)
            {
                iterator = Buffer.GetIterator();

                foreach (var modifier in Modifiers)
                {
                    modifier.Update(ref iterator);
                    iterator.Reset();
                }
            }
        }

        public void Trigger(float x, float y)
        {
            var iterator = Buffer.Release(ReleaseQuantity);
            var particle = iterator.First;

            do
            {
                Shape.GenerateOffsetAndHeading(particle->Position, particle->Velocity);

                particle->Age = 0f;
                particle->Inception = _totalSeconds;
                
                particle->Position[0] += x;
                particle->Position[1] += y;

                particle->Velocity[0] *= ReleaseSpeed;
                particle->Velocity[1] *= ReleaseSpeed;
            }
            while (iterator.MoveNext(&particle));
        }
    }
}