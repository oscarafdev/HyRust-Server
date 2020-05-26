namespace Fougerite.Events
{

    /// <summary>
    /// This class is created on decay event.
    /// </summary>
    public class DecayEvent
    {
        private float _dmg;
        private Fougerite.Entity _ent;

        public DecayEvent(Fougerite.Entity en, ref float dmg)
        {
            this.Entity = en;
            this.DamageAmount = dmg;
        }

        /// <summary>
        /// Gets / Sets the damage of the decay event.
        /// </summary>
        public float DamageAmount
        {
            get
            {
                return this._dmg;
            }
            set
            {
                this._dmg = value;
            }
        }

        /// <summary>
        /// Gets the Entity that the decay is running on.
        /// </summary>
        public Fougerite.Entity Entity
        {
            get
            {
                return this._ent;
            }
            set
            {
                this._ent = value;
            }
        }
    }
}