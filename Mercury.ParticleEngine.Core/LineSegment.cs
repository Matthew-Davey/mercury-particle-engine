namespace Mercury.ParticleEngine {
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Defines a part of a line that is bounded by two distinct end points.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct LineSegment : IEquatable<LineSegment> {
        internal readonly Coordinate _point1;
        internal readonly Coordinate _point2;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSegment"/> structure.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        public LineSegment(Coordinate point1, Coordinate point2) {
            _point1 = point1;
            _point2 = point2;
        }

        public LineSegment(Coordinate origin, Vector vector) {
            _point1 = origin;
            _point2 = origin.Translate(vector);
        }

        public Coordinate Origin => _point1;

        public Axis Direction {
            get {
                var coord = _point2.Subtract(_point1);
                return new Axis(coord._x, coord._y);
            }
        }

        public Vector ToVector() {
            var t = _point2.Subtract(_point1);
            return new Vector(t._x, t._y);
        }

        public unsafe void CopyTo(float* destination) {
            _point1.CopyTo(destination);
            _point2.CopyTo(destination + sizeof(Coordinate));
        }

        public void Destructure(out Coordinate point1, out Coordinate point2) {
            point1 = _point1;
            point2 = _point2;
        }

        public void Match(Action<Coordinate, Coordinate> callback) {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(_point1, _point2);
        }

        public T Map<T>(Func<Coordinate, Coordinate, T> map) {
            if (map == null)
                throw new ArgumentNullException(nameof(map));

            return map(_point1, _point2);
        }

        public bool Equals(LineSegment other) => _point1.Equals(other._point1) &&
                                                 _point2.Equals(other._point2);

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;

            return obj is LineSegment & Equals((LineSegment)obj);
        }

        public override int GetHashCode() {
            var hashCode = _point1.GetHashCode();

            hashCode = (hashCode * 397) ^ _point2.GetHashCode();

            return hashCode;
        }

        public override string ToString() => $"({_point1:x}:{_point1:y},{_point2:x}:{_point2:y})";
    }
}