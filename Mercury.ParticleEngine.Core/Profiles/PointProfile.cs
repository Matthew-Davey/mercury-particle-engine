namespace Mercury.ParticleEngine.Profiles
{
    using System;

    public class PointProfile : Profile
    {
        public override unsafe void GetOffsetAndHeading(float* offset, float* heading)
        {
            var angle = (float)((Math.PI * 2d) * Random.NextDouble());

            offset[0] = 0f;
            offset[1] = 0f;

            heading[0] = (float)Math.Cos(angle);
            heading[1] = (float)Math.Sin(angle);
        }
    }
}