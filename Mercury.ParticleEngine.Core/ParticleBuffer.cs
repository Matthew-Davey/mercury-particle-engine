namespace Mercury.ParticleEngine {
    using System;
    using System.Runtime.InteropServices;

    internal class ParticleBuffer : IDisposable {
        private int _tail;

        public readonly IntPtr NativePointer;
        public readonly int Size;

        public ParticleBuffer(int size) {
            Size = size;
            NativePointer = Marshal.AllocHGlobal(SizeInBytes);

            GC.AddMemoryPressure(SizeInBytes);
        }

        public int Available => Size - _tail;
        public int Count => _tail;
        public int SizeInBytes => Particle.SizeInBytes * Size;
        public int ActiveSizeInBytes => Particle.SizeInBytes * _tail;

        public unsafe int Release(int releaseQuantity, out Particle* first) {
            var numToRelease = Math.Min(releaseQuantity, Available);

            var oldTail = _tail;

            _tail += numToRelease;

            first = (Particle*)IntPtr.Add(NativePointer, oldTail * Particle.SizeInBytes);

            return numToRelease;
        }

        public void Reclaim(int number) {
            _tail -= number;

            memcpy(NativePointer, IntPtr.Add(NativePointer, number * Particle.SizeInBytes), ActiveSizeInBytes);
        }

        public void CopyTo(IntPtr destination) => memcpy(destination, NativePointer, ActiveSizeInBytes);

        public void CopyToReverse(IntPtr destination) {
            int offset = 0;
            for (var i = ActiveSizeInBytes - Particle.SizeInBytes; i >= 0; i -= Particle.SizeInBytes) {
                memcpy(IntPtr.Add(destination, offset), IntPtr.Add(NativePointer, i), Particle.SizeInBytes);
                offset += Particle.SizeInBytes;
            }
        }

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void memcpy(IntPtr dest, IntPtr src, int count);

        private bool _disposed;

        public void Dispose() {
            if (!_disposed) {
                Marshal.FreeHGlobal(NativePointer);
                _disposed = true;

                GC.RemoveMemoryPressure(Particle.SizeInBytes * Size);
            }
            
            GC.SuppressFinalize(this);
        }

        ~ParticleBuffer() {
            Dispose();
        }
    }
}