﻿namespace Mercury.ParticleEngine
{
    public class ReleaseParameters
    {
        public ReleaseParameters()
        {
            Quantity = Range.Parse("[1,1]");
            Speed    = RangeF.Parse("[-1.0,1.0]");
            Colour   = new ColourRange(RangeF.Parse("[0.0,1.0]"), RangeF.Parse("[0.0,1.0]"), RangeF.Parse("[0.0,1.0]"));
            Opacity  = RangeF.Parse("[0.0,1.0]");
            Scale    = RangeF.Parse("[1.0,10.0]");
            Rotation = RangeF.Parse("[-3.14159,3.14159]");
        }

        public Range Quantity;
        public RangeF Speed;
        public ColourRange Colour;
        public RangeF Opacity;
        public RangeF Scale;
        public RangeF Rotation;
    }
}