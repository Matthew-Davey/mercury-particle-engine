namespace Mercury.ParticleEngine.Profiles
{
    public abstract class Profile
    {
        public abstract unsafe void GetOffsetAndHeading(Coordinate* offset, Axis* heading);

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

        static public Profile Box(float width, float height)
        {
            return new BoxProfile
            {
                Width = width,
                Height = height
            };
        }

        static public Profile BoxFill(float width, float height)
        {
            return new BoxFillProfile
            {
                Width = width,
                Height = height
            };
        }

        static public Profile Circle(float radius, bool radiate)
        {
            return new CircleProfile
            {
                Radius = radius,
                Radiate = radiate
            };
        }
    }
}
