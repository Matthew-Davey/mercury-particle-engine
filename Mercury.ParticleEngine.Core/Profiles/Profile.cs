namespace Mercury.ParticleEngine.Profiles
{
    public abstract class Profile
    {
        protected Profile()
        {
        }

        public abstract unsafe void GetOffsetAndHeading(float* offset, Axis* heading);

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
