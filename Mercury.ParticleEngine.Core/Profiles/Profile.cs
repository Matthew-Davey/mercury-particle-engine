namespace Mercury.ParticleEngine.Profiles
{
    using System;

    public abstract class Profile
    {
        protected Profile()
        {
            Random = new Random();
        }

        protected Random Random { get; private set; }

        public abstract unsafe void GetOffsetAndHeading(float* offset, float* heading);

        static public Profile Point()
        {
            return new PointProfile();
        }

        static public Profile Ring(float radius, bool radiate)
        {
            return new RingProfile
            {
                Radius = radius,
                Radiate = radiate
            };
        }
    }
}
