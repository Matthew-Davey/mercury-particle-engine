namespace Mercury.ParticleEngine.Profiles
{
    using System;

    public class RingProfile : Profile
    {
        public float Radius { get; set; }
        public bool Radiate { get; set; }

        public override unsafe void GetOffsetAndHeading(float* offset, Axis* heading)
        {
            FastRand.NextUnitVector((float*)heading);

            offset[0] = heading->_x * Radius;
            offset[1] = heading->_y * Radius;

            if (!Radiate)
                FastRand.NextUnitVector((float*)heading);
        }
    }
}