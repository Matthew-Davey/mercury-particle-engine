namespace Mercury.ParticleEngine.Modifiers {
    public sealed unsafe class ContainerModifier : Modifier {
        public Coordinate Position;
        public int Width;
        public int Height;
        public float RestitutionCoefficient;

        protected internal override void Update(float elapsedSeconds, Particle* particle, int count) {
            var left = Width * -0.5f;
            var right = Width * 0.5f;
            var top = Height * -0.5f;
            var bottom = Height * 0.5f;

            while (count-- > 0) {
                if ((int)particle->Position[0] < left) {
                    particle->Position[0] = left + (left - particle->Position[0]);
                    particle->Velocity[0] = -particle->Velocity[0] * RestitutionCoefficient;
                }
                else if ((int)particle->Position[0] > right) {
                    particle->Position[0] = right - (particle->Position[0] - right);
                    particle->Velocity[0] = -particle->Velocity[0] * RestitutionCoefficient;
                }

                if ((int)particle->Position[1] < top) {
                    particle->Position[1] = top + (top - particle->Position[1]);
                    particle->Velocity[1] = -particle->Velocity[1] * RestitutionCoefficient;
                }
                else if ((int)particle->Position[1] > bottom) {
                    particle->Position[1] = bottom - (particle->Position[1] - bottom);
                    particle->Velocity[1] = -particle->Velocity[1] * RestitutionCoefficient;
                }

                particle++;
            }
        }
    }
}