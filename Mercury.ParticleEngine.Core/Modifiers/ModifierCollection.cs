namespace Mercury.ParticleEngine.Modifiers
{
    using System.Collections.Generic;
    using System.Linq;
    
    public class ModifierCollection : IEnumerable<Modifier>
    {
        public ModifierCollection()
        {
            Slots = new List<ModifierSlot>();
        }

        internal IList<ModifierSlot> Slots { get; private set; }

        public IEnumerator<Modifier> GetEnumerator()
        {
            return Slots.Select(slot => slot.Modifier)
                        .GetEnumerator();
        }

        public void Add(Modifier modifier, float frequency = 60f)
        {
            Slots.Add(new ModifierSlot(modifier, frequency));
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}