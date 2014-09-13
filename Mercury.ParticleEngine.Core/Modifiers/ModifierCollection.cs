namespace Mercury.ParticleEngine.Modifiers {
    using System.Collections.Generic;
    
    public class ModifierCollection : IEnumerable<Modifier> {
        public ModifierCollection() {
            Modifiers = new List<Modifier>();
        }

        internal IList<Modifier> Modifiers { get; private set; }

        public IEnumerator<Modifier> GetEnumerator() {
            return Modifiers.GetEnumerator();
        }

        public void Add(Modifier modifier) {
            Modifiers.Add(modifier);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}