namespace Mercury.ParticleEngine
{
    public struct ColourRange
    {
        public ColourRange(RangeF r, RangeF g, RangeF b)
        {
            R = r;
            G = g;
            B = b;
        }

        public readonly RangeF R;
        public readonly RangeF G;
        public readonly RangeF B;
    }
}