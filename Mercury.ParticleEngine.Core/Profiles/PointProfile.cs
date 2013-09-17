namespace Mercury.ParticleEngine.Profiles
{
    public class PointProfile : Profile
    {
        public override unsafe void GetOffsetAndHeading(Coordinate* offset, Axis* heading)
        {
            *offset = Coordinate.Origin;

            FastRand.NextUnitVector((float*)heading);
        }
    }
}