namespace Mercury.ParticleEngine.Profiles {
    public class CircleProfile : Profile {
        public float Radius;
        public bool Radiate;

        public override unsafe void GetOffsetAndHeading(Coordinate* offset, Axis* heading) {
            var dist = FastRand.NextSingle(0f, Radius);

            FastRand.NextUnitVector((Vector*)heading);

            *offset = new Coordinate(heading->_x * dist, heading->_y * dist);

            if (!Radiate)
                FastRand.NextUnitVector((Vector*)heading);
        }
    }
}