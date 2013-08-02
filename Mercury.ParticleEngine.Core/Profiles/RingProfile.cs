namespace Mercury.ParticleEngine.Profiles
{
    using System;

    public class RingProfile : Profile
    {
        public float Radius { get; set; }
        public bool Radiate { get; set; }

        public override unsafe void GetOffsetAndHeading(float* offset, float* heading)
        {
            Randu.NextAngle(offset);

            if (Radiate)
            {
                heading[0] = offset[0];
                heading[1] = offset[1];
            }
            else
            {
                Randu.NextAngle(heading);
            }

            offset[0] *= Radius;
            offset[1] *= Radius;
        }
    }
}