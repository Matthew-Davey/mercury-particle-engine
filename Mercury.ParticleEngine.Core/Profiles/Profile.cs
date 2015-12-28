namespace Mercury.ParticleEngine.Profiles {
    public abstract class Profile {
        public abstract unsafe void GetOffsetAndHeading(Coordinate* offset, Axis* heading);

        static public Profile Point() => new PointProfile();

        static public Profile Ring(float radius, bool radiate) =>
            new RingProfile {
                Radius = radius,
                Radiate = radiate
            };

        static public Profile Box(float width, float height) =>
            new BoxProfile {
                Width = width,
                Height = height
            };

        static public Profile BoxFill(float width, float height) =>
            new BoxFillProfile {
                Width = width,
                Height = height
            };

        static public Profile Circle(float radius, bool radiate) =>
            new CircleProfile {
                Radius = radius,
                Radiate = radiate
            };

        static public Profile Spray(Axis direction, float spread) =>
            new SprayProfile {
                Direction = direction,
                Spread = spread
            };
    }
}
