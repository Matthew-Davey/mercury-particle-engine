namespace Mercury.ParticleEngine.Modifiers
{
    /// <summary>
    /// Defines a modifier which interpolates the colour of a particle over the course of its lifetime.
    /// </summary>
    public sealed class ColourInterpolator2 : Modifier
    {
        /// <summary>
        /// Gets or sets the initial colour of particles when they are released.
        /// </summary>
        public Colour InitialColour;

        /// <summary>
        /// Gets or sets the final colour of particles when they are retired.
        /// </summary>
        public Colour FinalColour;

        protected internal override unsafe void Update(float elapsedseconds, Particle* particle, int count)
        {
            var delta = FinalColour - InitialColour;

            while (count-- > 0)
            {
                particle->Colour[0] = (InitialColour.R + (delta.R * particle->Age));
                particle->Colour[1] = (InitialColour.G + (delta.G * particle->Age));
                particle->Colour[2] = (InitialColour.B + (delta.B * particle->Age));

                particle++;
            }
        }
    }
}