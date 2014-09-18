namespace Mercury.ParticleEngine.Profiles {
    using System;

    public class SprayProfile : Profile {
        public Axis Direction;
        public float Spread;

        public override unsafe void GetOffsetAndHeading(Coordinate* offset, Axis* heading) {
            var angle = Direction.Map((x, y) => (float)Math.Atan2(y, x));

            angle = FastRand.NextSingle(angle - (Spread / 2f), angle + (Spread / 2f));

            *offset = Coordinate.Origin;
            *heading = new Axis((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
    }
}