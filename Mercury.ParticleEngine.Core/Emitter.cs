namespace Mercury.ParticleEngine
{
    using System;
    using System.Collections.Generic;
    using SharpDX;
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

                particle->Position.X += particle->Velocity.X;
                particle->Position.Y += particle->Velocity.Y;
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
            var position = new Vector2(x, y);
            var iterator = Buffer.Release(ReleaseQuantity);
            
            var particle = iterator.First;

            do
            {
                Vector2 offset, direction;
                Shape.GetOffsetAndDirection(out offset, out direction);

                particle->Age = 0f;
                particle->Inception = _totalSeconds;
                particle->Position = position + offset;
                particle->Velocity = direction * ReleaseSpeed;
            }
            while (iterator.MoveNext(&particle));
        }
    }
}