namespace Mercury.ParticleEngine
{
    using System;
    using System.Runtime.InteropServices;

    internal class ParticleBuffer : IDisposable
    {
        private int _tail;

        public readonly IntPtr NativePointer;
        public readonly int Size;

        public ParticleBuffer(int size)
        {
            Size = size;
            NativePointer = Marshal.AllocHGlobal(Particle.SizeInBytes * Size);

            GC.AddMemoryPressure(Particle.SizeInBytes * size);
        }

        public int Available
        {
            get { return Size - _tail; }
        }

        public int Count
        {
            get { return _tail; }
        }

        public int SizeInBytes
        {
            get { return Particle.SizeInBytes * Size; }
        }

        public unsafe int Release(int releaseQuantity, out Particle* first)
        {
            var numToRelease = Math.Min(releaseQuantity, Available);

            var oldTail = _tail;

            _tail += numToRelease;

            first = (Particle*)IntPtr.Add(NativePointer, oldTail * Particle.SizeInBytes);

            return numToRelease;
        }

        public void Reclaim(int number)
        {
            _tail -= number;

            memcpy(NativePointer, IntPtr.Add(NativePointer, number * Particle.SizeInBytes), _tail * Particle.SizeInBytes);
        }

        public unsafe int Iter(out Particle* first)
        {
            first = (Particle*)NativePointer;

            return _tail;
        }

        public void CopyTo(IntPtr destination)
        {
            memcpy(destination, NativePointer, _tail * Particle.SizeInBytes);
        }

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void memcpy(IntPtr dest, IntPtr src, int count);

        private bool _disposed;

        public void Dispose()
        {
            if (!_disposed)
            {
                Marshal.FreeHGlobal(NativePointer);
                _disposed = true;

                GC.RemoveMemoryPressure(Particle.SizeInBytes * Size);
            }
            
            GC.SuppressFinalize(this);
        }

        ~ParticleBuffer()
        {
            Dispose();
        }
    }
}