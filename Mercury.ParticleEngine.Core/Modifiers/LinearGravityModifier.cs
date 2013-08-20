namespace Mercury.ParticleEngine.Modifiers
{
    public class LinearGravityModifier : Modifier
    {
        public LinearGravityModifier(Axis direction, float strength)
        {
            Direction = direction;
            Strength = strength;
        }

        public LinearGravityModifier(Vector vector)
            : this(vector.Axis, vector.Magnitude)
        {
        }

        public Axis Direction { get; set; }
        public float Strength { get; set; }

        protected internal override unsafe void Update(float elapsedSeconds, ref ParticleIterator iterator)
        {
            var vector = Direction * (Strength * elapsedSeconds);

            var particle = iterator.First;

            do
            {
                particle->Velocity[0] *= vector._x;
                particle->Velocity[1] *= vector._y;
            }
            while (iterator.MoveNext(&particle));
        }
    }
}