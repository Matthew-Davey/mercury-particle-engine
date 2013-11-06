namespace Mercury.ParticleEngine
{
    using System.Collections.Generic;
    using Mercury.ParticleEngine.Modifiers;

    public abstract class ModifierExecutionStrategy
    {
        internal abstract unsafe void ExecuteModifiers(ModifierCollection modifiers, float elapsedSeconds, Particle* particle, int count);

        static public ModifierExecutionStrategy Serial = new SerialModifierExecutionStrategy();
        static public ModifierExecutionStrategy Parallel = new ParallelModifierExecutionStrategy();

        internal class SerialModifierExecutionStrategy : ModifierExecutionStrategy
        {
            internal override unsafe void ExecuteModifiers(ModifierCollection modifiers, float elapsedSeconds, Particle* particle, int count)
            {
                foreach (var slot in modifiers.Slots)
                {
                    slot.Update(elapsedSeconds, particle, count);
                }
            }
        }

        internal class ParallelModifierExecutionStrategy : ModifierExecutionStrategy
        {
            internal override unsafe void ExecuteModifiers(ModifierCollection modifiers, float elapsedSeconds, Particle* particle, int count)
            {
                System.Threading.Tasks.Parallel.ForEach(modifiers.Slots, slot => slot.Update(elapsedSeconds, particle, count));
            }
        }
    }
}