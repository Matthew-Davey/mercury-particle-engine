namespace Mercury.ParticleEngine
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Particle
    {
        public float Inception;
        public float Age;
        public fixed float Position[2];
        public fixed float Velocity[2];
        public fixed float Colour[4];

        static public int SizeInBytes
        {
            get
            {
                return Marshal.SizeOf(typeof(Particle));
            }
        }
    }
}