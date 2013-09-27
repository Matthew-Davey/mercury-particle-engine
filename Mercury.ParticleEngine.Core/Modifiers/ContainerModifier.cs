namespace Mercury.ParticleEngine.Modifiers
{
    public sealed unsafe class ContainerModifier : Modifier
    {
        public Coordinate Position;
        public int Width;
        public int Height;

        protected internal override void Update(float elapsedSeconds, Particle* particle, int count)
        {
            var left   = Width  * -0.5f;
            var right  = Width  * 0.5f;
            var top    = Height * -0.5f;
            var bottom = Height * 0.5f;

            while (count-- > 0)
            {
                if (particle->Position[0] < left)
                {
                    particle->Position[0] = left;
                    particle->Velocity[0] = -particle->Velocity[0];
                }
                else if (particle->Position[0] > right)
                {
                    particle->Position[0] = right;
                    particle->Velocity[0] = -particle->Velocity[0];
                }

                if (particle->Position[1] < top)
                {
                    particle->Position[1] = top;
                    particle->Velocity[1] = -particle->Velocity[1];
                }
                else if (particle->Position[1] > bottom)
                {
                    particle->Position[1] = bottom;
                    particle->Velocity[1] = -particle->Velocity[1];
                }

                particle++;
            }
        }
    }
}