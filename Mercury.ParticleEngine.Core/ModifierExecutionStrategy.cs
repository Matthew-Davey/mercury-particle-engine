namespace Mercury.ParticleEngine
{
    using System.Collections.Generic;
    using Mercury.ParticleEngine.Modifiers;

    public abstract class ModifierExecutionStrategy
    {
        public abstract unsafe void ExecuteModifiers(IList<Modifier> modifiers, float elapsedSeconds, Particle* particle, int count);

        static public ModifierExecutionStrategy Serial = new SerialModifierExecutionStrategy();
        static public ModifierExecutionStrategy Parallel = new ParallelModifierExecutionStrategy();

        internal class SerialModifierExecutionStrategy : ModifierExecutionStrategy
        {
            public override unsafe void ExecuteModifiers(IList<Modifier> modifiers, float elapsedSeconds, Particle* particle, int count)
            {
                foreach (var modifier in modifiers)
                {
                    modifier.Update(elapsedSeconds, particle, count);
                }
            }
        }

        internal class ParallelModifierExecutionStrategy : ModifierExecutionStrategy
        {
            public override unsafe void ExecuteModifiers(IList<Modifier> modifiers, float elapsedSeconds, Particle* particle, int count)
            {
                System.Threading.Tasks.Parallel.ForEach(modifiers, modifier => modifier.Update(elapsedSeconds, particle, count));
            }
        }
    }
}