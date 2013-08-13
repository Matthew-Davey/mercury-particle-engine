namespace Mercury.ParticleEngine.Profiles
{
    using System;

    public class PointProfile : Profile
    {
        public override unsafe void GetOffsetAndHeading(float* offset, Axis* heading)
        {
            offset[0] = 0f;
            offset[1] = 0f;

            FastRand.NextUnitVector((float*)heading);
        }
    }
}