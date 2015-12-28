namespace Mercury.ParticleEngine {
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// An immutable data structure representing a directed fixed axis.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Axis : IEquatable<Axis> {
        internal readonly float _x;
        internal readonly float _y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Axis"/> structure.
        /// </summary>
        /// <param name="x">The X component of the unit vector representing the axis.</param>
        /// <param name="y">The Y component of the unit vector representing the axis.</param>
        public Axis(float x, float y) {
            var length = (float)Math.Sqrt((x * x) + (y * y));

            _x = x / length;
            _y = y / length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Axis"/> structure.
        /// </summary>
        /// <param name="angle">The angle of the axis in radians.</param>
        public Axis(float angle){
            var x = (float)Math.Cos(angle);
            var y = (float)Math.Sin(angle);

            var length = (float)Math.Sqrt((x * x) + (y * y));

            _x = x / length;
            _y = y / length;
        }

        /// <summary>
        /// Gets a directed axis which points to the left.
        /// </summary>
        static public Axis Left => new Axis(-1f, 0f);

        /// <summary>
        /// Gets a directed axis which points up.
        /// </summary>
        static public Axis Up => new Axis(0f, 1f);

        /// <summary>
        /// Gets a directed axis which points to the right.
        /// </summary>
        static public Axis Right => new Axis(1f, 0f);

        /// <summary>
        /// Gets a directed axis which points down.
        /// </summary>
        static public Axis Down => new Axis(0f, -1f);

        /// <summary>
        /// Multiplies the fixed axis by a magnitude value resulting in a directed vector.
        /// </summary>
        /// <param name="magnitude">The magnitude of the vector.</param>
        /// <returns>A directed vector.</returns>
        public Vector Multiply(float magnitude) => new Vector(this, magnitude);

        /// <summary>
        /// Copies the X and Y components of the axis to the specified memory location.
        /// </summary>
        /// <param name="destination">The memory location to copy the axis to.</param>
        public unsafe void CopyTo(float* destination) {
            destination[0] = _x;
            destination[1] = _y;
        }

        /// <summary>
        /// Destructures the axis, exposing the individual X and Y components.
        /// </summary>
        public void Destructure(out float x, out float y) {
            x = _x;
            y = _y;
        }

        /// <summary>
        /// Exposes the individual X and Y components of the axis to the specified matching function.
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
        /// Exposes the individual X and Y components of the axis to the specified mapping function and returns the
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
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Axis other) => _x.Equals(other._x) &&
                                          _y.Equals(other._y);

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to.</param>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            
            return obj is Axis && Equals((Axis)obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode() {
            unchecked {
                var hashCode = _x.GetHashCode();

                hashCode = (hashCode * 397) ^ _y.GetHashCode();
                
                return hashCode;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString() => $"({_x.ToString("F4")}, {_y.ToString("F4")})";

        public static bool operator ==(Axis x, Axis y) => x.Equals(y);
        public static bool operator !=(Axis x, Axis y) => !x.Equals(y);
        public static Vector operator *(Axis axis, float magnitude) => new Vector(axis, magnitude);
    }
}