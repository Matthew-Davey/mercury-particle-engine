namespace Mercury.ParticleEngine.Profiles
{
    using System;

    public class RingProfile : Profile
    {
        public float Radius { get; set; }
        public bool Radiate { get; set; }

        public override unsafe void GetOffsetAndHeading(float* offset, float* heading)
        {
            Randu.NextUnitVector(heading);

            offset[0] = heading[0] * Radius;
            offset[1] = heading[1] * Radius;

            if (!Radiate)
                Randu.NextUnitVector(heading);
        }
    }
}