namespace Mercury.ParticleEngine.Modifiers {
    /// <summary>
    /// Defines a modifier which interpolates the colour of a particle over the course of its lifetime.
    /// </summary>
    public sealed class ColourInterpolator2 : Modifier {
        /// <summary>
        /// Gets or sets the initial colour of particles when they are released.
        /// </summary>
        public Colour InitialColour;

        /// <summary>
        /// Gets or sets the final colour of particles when they are retired.
        /// </summary>
        public Colour FinalColour;

        protected internal override unsafe void Update(float elapsedseconds, Particle* particle, int count) {
            var delta = new Colour(FinalColour._h - InitialColour._h,
                                   FinalColour._s - InitialColour._s,
                                   FinalColour._l - InitialColour._l);

            while (count-- > 0) {
                particle->Colour[0] = (InitialColour._h + (delta._h * particle->Age));
                particle->Colour[1] = (InitialColour._s + (delta._s * particle->Age));
                particle->Colour[2] = (InitialColour._l + (delta._l * particle->Age));

                particle++;
            }
        }
    }
}