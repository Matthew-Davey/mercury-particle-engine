namespace Mercury.ParticleEngine.Modifiers {
    public unsafe sealed class OpacityFastFadeModifier : Modifier {
        protected internal override void Update(float elapsedSeconds, Particle* particle, int count) {
            while (count-- > 0) {
                particle->Opacity = 1.0f - particle->Age;
                particle++;
            }
        }
    }
}