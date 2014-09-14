namespace Mercury.ParticleEngine.Modifiers {
    public class ScaleInterpolator2 : Modifier {
        public float InitialScale;
        public float FinalScale;

        protected internal override unsafe void Update(float elapsedSeconds, Particle* particle, int count) {
            var delta = FinalScale - InitialScale;

            while (count-- > 0) {
                particle->Scale = (delta * particle->Age) + InitialScale;
                particle++;
            }
        }
    }
}