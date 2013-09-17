namespace Mercury.ParticleEngine
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;

    /// <summary>
    /// An immutable data structure representing a 24bit colour composed of separate red, green and blue channels.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Colour : IEquatable<Colour>
    {
        /// <summary>
        /// Gets the value of the red channel.
        /// </summary>
        public readonly float R;

        /// <summary>
        /// Gets the value of the green channel.
        /// </summary>
        public readonly float G;
        
        /// <summary>
        /// Gets the value of the blue channel.
        /// </summary>
        public readonly float B;

        /// <summary>
        /// Initializes a new instance of the <see cref="Colour"/> struct.
        /// </summary>
        /// <param name="r">The value of the red channel.</param>
        /// <param name="g">The value of the green channel.</param>
        /// <param name="b">The value of the blue channel.</param>
        public Colour(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="T:Mercury.ParticleEngine.Colour"/> structure
        /// by parsing its string representation.
        /// </summary>
        /// <param name="value">A <see cref="T:System.String"/> representing a colour.</param>
        /// <returns>A new colour structure.</returns>
        /// <exception cref="T:System.FormatException">
        /// Thrown if the input string was not in the correct format to represent a colour.
        /// </exception>
        /// <example>
        ///     <code lang="C#">
        ///     <![CDATA[
        ///     var colour = Colour.Parse("#FED3D3");
        ///     ]]>
        ///     </code>
        /// </example>
        static public Colour Parse(string value)
        {
            const string regexPattern = @"^#(?<Red>[0-9A-F]{2})(?<Green>[0-9A-F]{2})(?<Blue>[0-9A-F]{2})$";

            var match = Regex.Match(value, regexPattern, RegexOptions.IgnoreCase);

            if (!match.Success)
                throw new FormatException("Value '" + value + "' is not in the correct format for a Colour.");

            Func<string, float> parseGroup = groupName =>
            {
                var groupValue = match.Groups[groupName].Value;

                return Byte.Parse(groupValue, NumberStyles.HexNumber) / 255f;
            };

            var r = parseGroup("Red");
            var g = parseGroup("Green");
            var b = parseGroup("Blue");

            return new Colour(r, g, b);
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
            return R.Equals(value.R) &&
                   G.Equals(value.G) &&
                   B.Equals(value.B);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return R.GetHashCode() ^
                   G.GetHashCode() ^
                   B.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("#{0:X2}{1:X2}{2:X2}", Convert.ToByte(R * 255f),
                                                        Convert.ToByte(G * 255f),
                                                        Convert.ToByte(B * 255f));
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

        static public Colour operator -(Colour x, Colour y)
        {
            return new Colour(x.R - y.R, x.G - y.G, x.B - y.B);
        }
    }
}