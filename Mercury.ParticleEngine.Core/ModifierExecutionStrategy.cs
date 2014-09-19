namespace Mercury.ParticleEngine {
    using System.Collections.Generic;
    using Mercury.ParticleEngine.Modifiers;
    using TPL = System.Threading.Tasks;

    public abstract class ModifierExecutionStrategy {
        internal abstract unsafe void ExecuteModifiers(IEnumerable<Modifier> modifiers, float elapsedSeconds, Particle* particle, int count);

        static public ModifierExecutionStrategy Serial = new SerialModifierExecutionStrategy();
        static public ModifierExecutionStrategy Parallel = new ParallelModifierExecutionStrategy();

        internal class SerialModifierExecutionStrategy : ModifierExecutionStrategy {
            internal override unsafe void ExecuteModifiers(IEnumerable<Modifier> modifiers, float elapsedSeconds, Particle* particle, int count) {
                foreach (var modifier in modifiers) {
                    modifier.InternalUpdate(elapsedSeconds, particle, count);
                }
            }
        }

        internal class ParallelModifierExecutionStrategy : ModifierExecutionStrategy {
            internal override unsafe void ExecuteModifiers(IEnumerable<Modifier> modifiers, float elapsedSeconds, Particle* particle, int count) {
                TPL.Parallel.ForEach(modifiers, modifier => modifier.InternalUpdate(elapsedSeconds, particle, count));
            }
        }
    }
}