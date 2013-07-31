namespace Mercury.ParticleEngine
{
    using System;
    using SharpDX;

    public abstract class EmitterShape
    {
        protected EmitterShape()
        {
            Random = new Random();
        }

        protected Random Random { get; private set; }

        public abstract void GetOffsetAndDirection(out Vector2 offset, out Vector2 direction);

        static public EmitterShape Point()
        {
            return new PointShape();
        }

        private sealed class PointShape : EmitterShape
        {
            public override void GetOffsetAndDirection(out Vector2 offset, out Vector2 direction)
            {
                var angle = Random.NextFloat(-MathUtil.Pi, MathUtil.Pi);

                offset = Vector2.Zero;
                direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            }
        }
    }
}
