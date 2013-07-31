namespace Mercury.ParticleEngine
{
    using System.Runtime.InteropServices;
    using SharpDX;

    [StructLayout(LayoutKind.Sequential)]
    public struct Particle
    {
        public float Inception;
        public float Age;
        public Vector2 Position;
        public Vector2 Velocity;
        public Color4 Colour;

        static public int SizeInBytes
        {
            get
            {
                return Marshal.SizeOf(typeof(Particle));
            }
        }
    }
}