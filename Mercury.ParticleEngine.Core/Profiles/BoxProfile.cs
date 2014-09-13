namespace Mercury.ParticleEngine.Profiles {
    public class BoxProfile : Profile {
        public float Width;
        public float Height;

        public override unsafe void GetOffsetAndHeading(Coordinate* offset, Axis* heading) {
            switch (FastRand.NextInteger(3)) {
                case 0: { // Left
                        *offset = new Coordinate(Width * -0.5f, FastRand.NextSingle(Height * -0.5f, Height * 0.5f));
                        break;
                    }
                case 1: { // Top
                        *offset = new Coordinate(FastRand.NextSingle(Width * -0.5f, Width * 0.5f), Height * -0.5f);
                        break;
                    }
                case 2: { // Right
                        *offset = new Coordinate(Width * 0.5f, FastRand.NextSingle(Height * -0.5f, Height * 0.5f));
                        break;
                    }
                case 3: { // Bottom
                        *offset = new Coordinate(FastRand.NextSingle(Width * -0.5f, Width * 0.5f), Height * 0.5f);
                        break;
                    }
            }

            FastRand.NextUnitVector((Vector*)heading);
        }
    }
}