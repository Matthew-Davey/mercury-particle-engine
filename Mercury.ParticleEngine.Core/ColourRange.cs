namespace Mercury.ParticleEngine {
    public struct ColourRange {
        public ColourRange(Colour min, Colour max) {
            Min = min;
            Max = max;
        }

        public readonly Colour Min;
        public readonly Colour Max;

        static public implicit operator ColourRange(Colour value) {
            return new ColourRange(value, value);
        }
    }
}