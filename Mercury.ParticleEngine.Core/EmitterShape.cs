namespace Mercury.ParticleEngine
{
    using System;

    public abstract class EmitterShape
    {
        protected EmitterShape()
        {
            Random = new Random();
        }

        protected Random Random { get; private set; }

        public abstract unsafe void GenerateOffsetAndHeading(float* offset, float* heading);

        static public EmitterShape Point()
        {
            return new PointShape();
        }

        private sealed class PointShape : EmitterShape
        {
            public override unsafe void GenerateOffsetAndHeading(float* offset, float* heading)
            {
                var angle = (float)((Math.PI * 2d) * Random.NextDouble());

                offset[0] = 0f;
                offset[1] = 0f;

                heading[0] = (float)Math.Cos(angle);
                heading[1] = (float)Math.Sin(angle);
            }
        }
    }
}
