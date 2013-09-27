namespace Mercury.ParticleEngine
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// An immutable data structure representing a 24bit colour composed of separate red, green and blue channels.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Colour : IEquatable<Colour>
    {
        /// <summary>
        /// Gets the value of the hue channel.
        /// </summary>
        public readonly float H;

        /// <summary>
        /// Gets the value of the saturation channel.
        /// </summary>
        public readonly float S;
        
        /// <summary>
        /// Gets the value of the lightness channel.
        /// </summary>
        public readonly float L;

        /// <summary>
        /// Initializes a new instance of the <see cref="Colour"/> struct.
        /// </summary>
        /// <param name="h">The value of the hue channel.</param>
        /// <param name="s">The value of the saturation channel.</param>
        /// <param name="l">The value of the lightness channel.</param>
        public Colour(float h, float s, float l)
        {
            H = h;
            S = s;
            L = l;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Colour)
                return Equals((Colour)obj);

            return base.Equals(obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Colour"/> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="Colour"/> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="Colour"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Colour value)
        {
            return H.Equals(value.H) &&
                   S.Equals(value.S) &&
                   L.Equals(value.L);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return H.GetHashCode() ^
                   S.GetHashCode() ^
                   L.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{0}°,{1:P0},{2:P0}", H, S, L);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="x">The lvalue.</param>
        /// <param name="y">The rvalue.</param>
        /// <returns>
        ///     <c>true</c> if the lvalue <see cref="Colour"/> is equal to the rvalue; otherwise, <c>false</c>.
        /// </returns>
        static public bool operator ==(Colour x, Colour y)
        {
            return x.Equals(y);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="x">The lvalue.</param>
        /// <param name="y">The rvalue.</param>
        /// <returns>
        ///     <c>true</c> if the lvalue <see cref="Colour"/> is not equal to the rvalue; otherwise, <c>false</c>.
        /// </returns>
        static public bool operator !=(Colour x, Colour y)
        {
            return !x.Equals(y);
        }
    }
}