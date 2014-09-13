namespace Mercury.ParticleEngine.Profiles {
    public class RingProfile : Profile {
        public float Radius { get; set; }
        public bool Radiate { get; set; }

        public override unsafe void GetOffsetAndHeading(Coordinate* offset, Axis* heading) {
            FastRand.NextUnitVector((Vector*)heading);

            *offset = new Coordinate(heading->_x * Radius, heading->_y * Radius);

            if (!Radiate)
                FastRand.NextUnitVector((Vector*)heading);
        }
    }
}