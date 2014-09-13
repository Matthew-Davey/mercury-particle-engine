namespace Mercury.ParticleEngine.Modifiers {
    public class HueInterpolator2 : Modifier {
        public float InitialHue;
        public float FinalHue;

        protected internal override unsafe void Update(float elapsedSeconds, Particle* particle, int count) {
            var delta = FinalHue - InitialHue;

            while (count-- > 0) {
                particle->Colour[0] = (delta * particle->Age) + InitialHue;
                particle++;
            }
        }
    }
}