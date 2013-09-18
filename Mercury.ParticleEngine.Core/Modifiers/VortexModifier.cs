namespace Mercury.ParticleEngine.Modifiers
{
    using System;

    public class VortexModifier : Modifier
    {
        public Coordinate Position;
        public float Radius;
        public float Strength;

        protected internal override unsafe void Update(float elapsedSeconds, Particle* particle, int count)
        {
            var strengthDelta = Strength * elapsedSeconds;
            var radiusSquared = Radius * Radius;

            while (count-- > 0)
            {
                var offsetx = Position._x - particle->Position[0];
                var offsety = Position._y - particle->Position[1];

                var distanceFromCentreSquared = ((offsetx * offsetx) + (offsety * offsety));

                if (distanceFromCentreSquared < radiusSquared)
                {
                    var distanceFromCentre = (float)Math.Sqrt(distanceFromCentreSquared);
                    var headingx = offsetx / distanceFromCentre;
                    var headingy = offsety / distanceFromCentre;
                    var normalizedDistanceFromCentre = Radius / distanceFromCentre;

                    //var strength = Math.Min((normalizedDistanceFromCentre * strengthDelta), 100f);
                    var strength = (normalizedDistanceFromCentre * strengthDelta);
                    headingx *= strength;
                    headingy *= strength;

                    particle->Velocity[0] += headingx;
                    particle->Velocity[1] += headingy;
                }

                particle++;
            }
        }
    }
}