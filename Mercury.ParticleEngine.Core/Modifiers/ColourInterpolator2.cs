namespace Mercury.ParticleEngine.Modifiers
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Defines a modifier which interpolates the colour of a particle over the course of its lifetime.
    /// </summary>
    public sealed class ColourInterpolator2 : Modifier
    {
        /// <summary>
        /// Gets or sets the initial colour of particles when they are released.
        /// </summary>
        public Colour InitialColour { get; set; }

        /// <summary>
        /// Gets or sets the final colour of particles when they are retired.
        /// </summary>
        public Colour FinalColour { get; set; }

        /// <summary>
        /// Processes active particles.
        /// </summary>
        /// <param name="elapsedseconds">Elapsed time in whole and fractional seconds.</param>
        /// <param name="iterator">A particle iterator object.</param>
        protected internal override unsafe void Update(float elapsedseconds, ref ParticleIterator iterator)
        {
            var initialColour = this.InitialColour;
            var delta = this.FinalColour - initialColour;
            
            var particle = iterator.First;

            do
            {
                particle->Colour[0] = (initialColour.R + (delta.R * particle->Age));
                particle->Colour[1] = (initialColour.G + (delta.G * particle->Age));
                particle->Colour[2] = (initialColour.B + (delta.B * particle->Age));
            }
            while (iterator.MoveNext(&particle));
        }
    }
}