namespace Mercury.ParticleEngine {
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// An immutable data structure representing a 24bit colour composed of separate hue, saturation and lightness channels.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Colour : IEquatable<Colour> {
        /// <summary>
        /// Gets the value of the hue channel.
        /// </summary>
        internal readonly float _h;

        /// <summary>
        /// Gets the value of the saturation channel.
        /// </summary>
        internal readonly float _s;
        
        /// <summary>
        /// Gets the value of the lightness channel.
        /// </summary>
        internal readonly float _l;

        /// <summary>
        /// Initializes a new instance of the <see cref="Colour"/> structure.
        /// </summary>
        /// <param name="h">The value of the hue channel.</param>
        /// <param name="s">The value of the saturation channel.</param>
        /// <param name="l">The value of the lightness channel.</param>
        public Colour(float h, float s, float l) {
            _h = h;
            _s = s;
            _l = l;
        }

        /// <summary>
        /// Copies the individual channels of the colour to the specified memory location.
        /// </summary>
        /// <param name="destination">The memory location to copy the axis to.</param>
        public unsafe void CopyTo(float* destination) {
            destination[0] = _h;
            destination[1] = _s;
            destination[2] = _l;
        }

        /// <summary>
        /// Destructures the colour, exposing the individual channels.
        /// </summary>
        public void Destructure(out float h, out float s, out float l) {
            h = _h;
            s = _s;
            l = _l;
        }

        /// <summary>
        /// Exposes the individual channels of the colour to the specified matching function.
        /// </summary>
        /// <param name="callback">The function which matches the individual channels of the colour.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the value passed to the <paramref name="callback"/> parameter is <c>null</c>.
        /// </exception>
        public void Match(Action<float, float, float> callback) {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(_h, _s, _l);
        }

        /// <summary>
        /// Exposes the individual channels of the colour to the specified mapping function and returns the
        /// result;
        /// </summary>
        /// <typeparam name="T">The type being mapped to.</typeparam>
        /// <param name="map">
        /// A function which maps the colour channels to an instance of <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        /// The result of the <paramref name="map"/> function when passed the individual X and Y components.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the value passed to the <paramref name="map"/> parameter is <c>null</c>.
        /// </exception>
        public T Map<T>(Func<float, float, float, T> map)
        {
            if (map == null)
                throw new ArgumentNullException(nameof(map));

            return map(_h, _s, _l);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) {
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
        public bool Equals(Colour value) => _h.Equals(value._h) &&
                                            _s.Equals(value._s) &&
                                            _l.Equals(value._l);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => _h.GetHashCode() ^
                                             _s.GetHashCode() ^
                                             _l.GetHashCode();

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString() => $"{_h}°,{_s:P0},{_l:P0}";

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="x">The lvalue.</param>
        /// <param name="y">The rvalue.</param>
        /// <returns>
        ///     <c>true</c> if the lvalue <see cref="Colour"/> is equal to the rvalue; otherwise, <c>false</c>.
        /// </returns>
        static public bool operator ==(Colour x, Colour y) => x.Equals(y);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="x">The lvalue.</param>
        /// <param name="y">The rvalue.</param>
        /// <returns>
        ///     <c>true</c> if the lvalue <see cref="Colour"/> is not equal to the rvalue; otherwise, <c>false</c>.
        /// </returns>
        static public bool operator !=(Colour x, Colour y) => !x.Equals(y);

        static public Colour operator -(Colour a, Colour b) => new Colour(a._h - b._h, a._s - b._s, a._l - b._l);
    }
}