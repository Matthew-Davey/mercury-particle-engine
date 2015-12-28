namespace Mercury.ParticleEngine {
    using System;

    /// <summary>
    /// Defines a random number generator which uses the FastRand algorithm to generate random values.
    /// </summary>
    internal static class FastRand {
        static int _state = 1;

        static public void Seed(int seed) {
            if (seed < 1)
                throw new ArgumentOutOfRangeException(nameof(seed), "seed must be greater than zero");
            
            _state = seed;
        }
        
        /// <summary>
        /// Gets the next random integer value.
        /// </summary>
        /// <returns>A random positive integer.</returns>
        static public int NextInteger() {
            _state = 214013 * _state + 2531011;
            return (_state >> 16) & 0x7FFF;
        }

        /// <summary>
        /// Gets the next random integer value which is greater than zero and less than or equal to
        /// the specified maxmimum value.
        /// </summary>
        /// <param name="max">The maximum random integer value to return.</param>
        /// <returns>A random integer value between zero and the specified maximum value.</returns>
        static public int NextInteger(int max) => (int)(max * NextSingle());

        /// <summary>
        /// Gets the next random integer between the specified minimum and maximum values.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The inclusive maximum value.</param>
        static public int NextInteger(int min, int max) => (int)((max - min) * NextSingle()) + min;

        /// <summary>
        /// Gets the next random integer between the specified range of values.
        /// </summary>
        /// <param name="range">A range representing the inclusive minimum and maximum values.</param>
        /// <returns>A random integer between the specified minumum and maximum values.</returns>
        static public int NextInteger(Range range) => NextInteger(range.X, range.Y);

        /// <summary>
        /// Gets the next random single value.
        /// </summary>
        /// <returns>A random single value between 0 and 1.</returns>
        static public float NextSingle() => NextInteger() / (float)Int16.MaxValue;

        /// <summary>
        /// Gets the next random single value which is greater than zero and less than or equal to
        /// the specified maxmimum value.
        /// </summary>
        /// <param name="max">The maximum random single value to return.</param>
        /// <returns>A random single value between zero and the specified maximum value.</returns>
        static public float NextSingle(float max) => max * NextSingle();

        /// <summary>
        /// Gets the next random single value between the specified minimum and maximum values.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The inclusive maximum value.</param>
        /// <returns>A random single value between the specified minimum and maximum values.</returns>
        static public float NextSingle(float min, float max) => ((max - min) * NextSingle()) + min;

        /// <summary>
        /// Gets the next random single value between the specified range of values.
        /// </summary>
        /// <param name="range">A range representing the inclusive minimum and maximum values.</param>
        /// <returns>A random single value between the specified minimum and maximum values.</returns>
        static public float NextSingle(RangeF range) => NextSingle(range.X, range.Y);

        /// <summary>
        /// Gets the next random angle value.
        /// </summary>
        /// <returns>A random angle value.</returns>
        static public float NextAngle() => NextSingle((float)Math.PI * -1f, (float)Math.PI);

        static public unsafe void NextUnitVector(Vector* vector) {
            var angle = NextAngle();

            *vector = new Vector((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        static public unsafe void NextColour(Colour* colour, ColourRange range) {
            *colour = new Colour(NextSingle(range.Min._h, range.Max._h),
                                 NextSingle(range.Min._s, range.Max._s),
                                 NextSingle(range.Min._l, range.Max._l));
        }
    }
}