namespace Mercury.ParticleEngine.Modifiers {
    public class OpacityInterpolator2 : Modifier {
        public float InitialOpacity;
        public float FinalOpacity;

        protected internal override unsafe void Update(float elapsedSeconds, Particle* particle, int count) {
            var delta = FinalOpacity - InitialOpacity;

            while (count-- > 0) {
                particle->Opacity = (delta * particle->Age) + InitialOpacity;
                particle++;
            }
        }
    }
}