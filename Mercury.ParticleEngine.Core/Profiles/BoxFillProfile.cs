namespace Mercury.ParticleEngine.Profiles {
    public class BoxFillProfile : Profile {
        public float Width;
        public float Height;

        public override unsafe void GetOffsetAndHeading(Coordinate* offset, Axis* heading) {
            *offset = new Coordinate(FastRand.NextSingle(Width * -0.5f, Width * 0.5f),
                                     FastRand.NextSingle(Height * -0.5f, Height * 0.5f));

            FastRand.NextUnitVector((Vector*)heading);
        }
    }
}