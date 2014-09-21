namespace Mercury.ParticleEngine {
    public class ParticleEffect {
        public Emitter[] Emitters { get; set; }

        public ParticleEffect() {
            Emitters = new Emitter[0];
        }

        public int ActiveParticles {
            get {
                int sum = 0;
                for (var i = 0; i < Emitters.Length; i++) {
                    sum += Emitters[i].ActiveParticles;
                }
                return sum;
            }
        }

        public void Update(float elapsedSeconds) {
            for (var i = 0; i < Emitters.Length; i++) {
                Emitters[i].Update(elapsedSeconds);
            }
        }

        public void Trigger(Coordinate position) {
            for (var i = 0; i < Emitters.Length; i++) {
                Emitters[i].Trigger(position);
            }
        }

        public void Trigger(LineSegment line) {
            for (var i = 0; i < Emitters.Length; i++)
                Emitters[i].Trigger(line);
        }
    }
}