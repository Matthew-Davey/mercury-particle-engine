namespace Mercury.ParticleEngine.Modifiers {
    public class LinearGravityModifier : Modifier {
        public LinearGravityModifier(Axis direction, float strength) {
            Direction = direction;
            Strength = strength;
        }

        public LinearGravityModifier(Vector vector)
            : this(vector.Axis, vector.Magnitude) {
        }

        public Axis Direction { get; set; }
        public float Strength { get; set; }

        protected internal override unsafe void Update(float elapsedSeconds, Particle* particle, int count) {
            var vector = Direction * (Strength * elapsedSeconds);

            while (count-- > 0) {
                particle->Velocity[0] += vector._x * particle->Mass;
                particle->Velocity[1] += vector._y * particle->Mass;

                particle++;
            }
        }
    }
}