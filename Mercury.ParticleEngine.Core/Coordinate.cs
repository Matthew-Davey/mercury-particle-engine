namespace Mercury.ParticleEngine {
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    /// <summary>
    /// An immutable data structure encapsulating a 3D Cartesian coordinate.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Coordinate : IEquatable<Coordinate> {
        internal readonly float _x;
        internal readonly float _y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Coordinate"/> structure.
        /// </summary>
        /// <param name="x">The value of the point on the first number line (the abscissa).</param>
        /// <param name="y">The value of the point on the second number line (the ordinate).</param>
        public Coordinate(float x, float y) {
            _x = x;
            _y = y;
        }

        /// <summary>
        /// Gets the cartesian origin O.
        /// </summary>
        static public Coordinate Origin => new Coordinate(0f, 0f);

        public Coordinate Add(Coordinate other) {
            var x = _x + other._x;
            var y = _y + other._y;

            return new Coordinate(x, y);
        }

        public Coordinate Subtract(Coordinate other) {
            var x = _x - other._x;
            var y = _y - other._y;

            return new Coordinate(x, y);
        }

        /// <summary>
        /// Translates the coordinate by the specified vector, using the current instance as the
        /// origin of the translation.
        /// </summary>
        /// <param name="vector">The vector to translate by.</param>
        /// <returns>A <see cref="Coordinate"/> representing the current instance translated by the
        /// specified vector.</returns>
        public Coordinate Translate(Vector vector) {
            var x = _x + vector._x;
            var y = _y + vector._y;

            return new Coordinate(x, y);
        }

        /// <summary>
        /// Copies the X and Y components of the coordinate to the specified memory location.
        /// </summary>
        /// <param name="destination">The memory location to copy the coordinate to.</param>
        public unsafe void CopyTo(float* destination) {
            destination[0] = _x;
            destination[1] = _y;
        }

        /// <summary>
        /// Destructures the coordinate, exposing the individual X and Y components.
        /// </summary>
        public void Destructure(out float x, out float y) {
            x = _x;
            y = _y;
        }

        /// <summary>
        /// Exposes the individual X and Y components of the coordinate to the specified matching function.
        /// </summary>
        /// <param name="callback">The function which matches the individual X and Y components.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the value passed to the <paramref name="callback"/> parameter is <c>null</c>.
        /// </exception>
        public void Match(Action<float, float> callback) {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(_x, _y);
        }

        /// <summary>
        /// Exposes the individual X and Y components of the coordinate to the specified mapping function and returns the
        /// result;
        /// </summary>
        /// <typeparam name="T">The type being mapped to.</typeparam>
        /// <param name="map">
        /// A function which maps the X and Y values to an instance of <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        /// The result of the <paramref name="map"/> function when passed the individual X and Y components.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the value passed to the <paramref name="map"/> parameter is <c>null</c>.
        /// </exception>
        public T Map<T>(Func<float, float, T> map) {
            if (map == null)
                throw new ArgumentNullException(nameof(map));

            return map(_x, _y);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) {
            if (obj is Coordinate)
                return Equals((Coordinate)obj);

            return base.Equals(obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Coordinate other) => _x.Equals(other._x) &&
                                                _y.Equals(other._y);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => _x.GetHashCode() ^
                                             _y.GetHashCode();

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString() => ToString("g", CultureInfo.InvariantCulture);

        /// <summary>
        /// Formats the value of the current instance using the specified format.
        /// </summary>
        /// <param name="format">The format to use.
        /// -or- A null reference (Nothing in Visual Basic) to use the default format defined for the type of the System.IFormattable implementation.</param>
        /// <param name="formatProvider">The provider to use to format the value.
        /// -or- A null reference (Nothing in Visual Basic) to obtain the numeric format information from the current locale setting of the operating system.</param>
        /// <returns>The value of the current instance in the specified format.</returns>
        public string ToString(string format, IFormatProvider formatProvider) {
            if (formatProvider != null) {
                var formatter = formatProvider.GetFormat(GetType()) as ICustomFormatter;

                if (formatter != null)
                    return formatter.Format(format, this, formatProvider);
            }

            switch (format.ToLowerInvariant()) {
                case "x": return _x.ToString("F4");
                case "y": return _y.ToString("F4");
                default: return $"({_x:F4}, {_y:F4})";
            }
        }

        static public Coordinate operator +(Coordinate a, Coordinate b) => a.Add(b);
        static public Coordinate operator -(Coordinate a, Coordinate b) => a.Subtract(b);

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="coord">The first operand.</param>
        /// <param name="vector">The second operand.</param>
        /// <returns>A <see cref="Coordinate"/> value representing the the <paramref name="coord"/>
        /// value translated by the <paramref name="vector"/> value.</returns>
        static public Coordinate operator +(Coordinate coord, Vector vector) => coord.Translate(vector);
    }
}