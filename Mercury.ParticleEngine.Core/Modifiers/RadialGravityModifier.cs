namespace Mercury.ParticleEngine.Modifiers
{
    using System;

    public class RadialGravityModifier : Modifier
    {
        public Coordinate Position;
        public float Radius;
        public float Strength;

        protected internal override unsafe void Update(float elapsedSeconds, ref ParticleIterator iterator)
        {
            float strengthDelta = Strength * elapsedSeconds;
            var radiusSquared = Radius * Radius;

            var particle = iterator.First;

            do
            {
                var offsetFromCentre = new Vector(Position._x - particle->Position[0], Position._y - particle->Position[1]);

                float distanceFromCentreSquared = ((offsetFromCentre._x * offsetFromCentre._x) + (offsetFromCentre._y * offsetFromCentre._y));

                if (distanceFromCentreSquared < radiusSquared)
                {
                    var distanceFromCentre = (float)Math.Sqrt(distanceFromCentreSquared);
                    var heading = new Vector(offsetFromCentre._x / distanceFromCentre, offsetFromCentre._y / distanceFromCentre);
                    var normalizedDistanceFromCentre = Radius / distanceFromCentre;

                    heading *= normalizedDistanceFromCentre * strengthDelta;

                    particle->Velocity[0] += heading._x;
                    particle->Velocity[1] += heading._y;
                }
            }
            while (iterator.MoveNext(&particle));
        }
    }
}