namespace Mercury.ParticleEngine
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Defines a data structure representing a Euclidean vector facing a particular direction,
    /// including a magnitude value.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector
    {
        public Vector(Axis axis, Single magnitude)
        {
            _x = axis._x * magnitude;
            _y = axis._y * magnitude;
        }

        internal readonly Single _x;
        internal readonly Single _y;

        /// <summary>
        /// Gets the length or magnitude of the Euclidean vector.
        /// </summary>
        public Single Magnitude
        {
            get { return(float)Math.Sqrt((_x * _x) + (_y * _y)); }
        }

        /// <summary>
        /// Gets the axis in which the vector is facing.
        /// </summary>
        /// <returns>A <see cref="Axis"/> value representing the direction the vector is facing.</returns>
        public Axis Axis
        {
            get { return new Axis(_x, _y); }
        }
    }
}