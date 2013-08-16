namespace Mercury.ParticleEngine
{
    public unsafe struct ParticleIterator
    {
        private readonly Particle* _buffer;
        private readonly int _size;
        private readonly int _from;
        public readonly int Count;
        private int _iteration;

        public readonly Particle* First;
        private readonly Particle* _last;

        public ParticleIterator(Particle* buffer, int size, int from, int count)
        {
            _buffer = buffer;
            _size = size;
            _from = from;
            Count = count;

            _iteration = 0;

            First = buffer + _from;
            _last = (buffer + _size) -1;
        }

        public int Remaining
        {
            get { return Count - _iteration; }
        }

        public bool MoveNext(Particle** particle)
        {
            if (++_iteration > (Count - 1))
                return false;

            if (++(*particle) > _last)
                (*particle) -= _size;

            return true;
        }

        public void Reset()
        {
            _iteration = 0;
        }
    }
}