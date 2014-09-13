namespace Mercury.ParticleEngine
{
    public struct ColourRange
    {
        public ColourRange(Colour min, Colour max)
        {
            Min = min;
            Max = max;
        }

        public readonly Colour Min;
        public readonly Colour Max;

        static public implicit operator ColourRange(Colour value)
        {
            return new ColourRange(value, value);
        }
    }

    public struct Range<T>
    {
        public Range(T min, T max)
        {
            Min = min;
            Max = max;
        }

        public readonly T Min;
        public readonly T Max;

        static public implicit operator Range<T>(T value)
        {
            return new Range<T>(value, value);
        }
    }
}