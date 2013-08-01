namespace Mercury.ParticleEngine.Profiles
{
    using System;

    public class RingProfile : Profile
    {
        public float Radius { get; set; }
        public bool Radiate { get; set; }

        public override unsafe void GetOffsetAndHeading(float* offset, float* heading)
        {
            var angle = (float)((Math.PI * 2d) * Random.NextDouble());

            offset[0] = (float)Math.Cos(angle) * Radius;
            offset[1] = (float)Math.Sin(angle) * Radius;

            if (Radiate)
            {
                heading[0] = offset[0] / Radius;
                heading[1] = offset[1] / Radius;
            }
            else
            {
                angle = (float)((Math.PI * 2d) * Random.NextDouble());

                heading[0] = (float)Math.Cos(angle) * Radius;
                heading[1] = (float)Math.Sin(angle) * Radius;
            }
        }
    }
}