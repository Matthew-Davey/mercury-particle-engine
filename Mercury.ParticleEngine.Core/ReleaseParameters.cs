namespace Mercury.ParticleEngine
{
    public class ReleaseParameters
    {
        public ReleaseParameters()
        {
            Quantity = Range.Parse("[1,1]");
            Speed    = RangeF.Parse("[-1.0,1.0]");
        }

        public Range Quantity;
        public RangeF Speed;
    }
}