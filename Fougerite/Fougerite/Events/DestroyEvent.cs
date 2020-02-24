namespace Fougerite.Events
{
	using Fougerite;
	using System;

	/// <summary>
	/// This class runs when an entity is destroyed.
	/// </summary>
	public class DestroyEvent
	{
		private object _attacker;
		private DamageEvent _de;
		private bool _decay;
		private Fougerite.Entity _ent;
		private string _weapon;
		private WeaponImpact _wi;

		public DestroyEvent(ref DamageEvent d, Entity ent, bool isdecay)
		{
			Fougerite.Player player = Server.Cache[d.attacker.client.userID];
			if (player != null) {
				this.Attacker = player;
			}

			this.WeaponData = null;
			this.IsDecay = isdecay;
			this.DamageEvent = d;
			this.Entity = ent;

			string weaponName = "Unknown";
			if (d.extraData != null)
			{
				WeaponImpact extraData = d.extraData as WeaponImpact;
				this.WeaponData = extraData;
				if (extraData.dataBlock != null)
				{
					weaponName = extraData.dataBlock.name;
				}
			}
			else
			{
				if (d.attacker.id is TimedExplosive)
					weaponName = "Explosive Charge";
				else if (d.attacker.id is TimedGrenade)
					weaponName = "F1 Grenade";
				else if (d.attacker.id.ToString().Contains("MutantBear"))
					weaponName = "Mutant Bear Claw";
				else if (d.attacker.id.ToString().Contains("Bear"))
					weaponName = "Bear Claw";
				else if (d.attacker.id.ToString().Contains("MutantWolf"))
					weaponName = "Mutant Wolf Claw";
				else if (d.attacker.id.ToString().Contains("Wolf"))
					weaponName = "Wolf Claw";
				else if (d.attacker.id.Equals(d.victim.id))
					weaponName = string.Format("Self ({0})", DamageType);
				else
					weaponName = "Hunting Bow";
			}
			this.WeaponName = weaponName;
		}

		/// <summary>
		/// Returns the Attacker's object. Can be anything from decay to player.
		/// </summary>
		public object Attacker
		{
			get
			{
				return this._attacker;
			}
			set
			{
				this._attacker = value;
			}
		}

		/// <summary>
		/// Gets the last damage of the event.
		/// </summary>
		public float DamageAmount
		{
			get
			{
				return this._de.amount;
			}
			set
			{
				this._de.amount = value;
			}
		}

		/// <summary>
		/// Gets the DamageEvent class
		/// </summary>
		public DamageEvent DamageEvent
		{
			get
			{
				return this._de;
			}
			set
			{
				this._de = value;
			}
		}

		/// <summary>
		/// This Getter tries to find the cause of the damage if possible.
		/// </summary>
		public string DamageType
		{
			get
			{
				string str = "Unknown";
				switch (((int)this.DamageEvent.damageTypes))
				{
				case 0:
					return "Bleeding";

				case 1:
					return "Generic";

				case 2:
					return "Bullet";

				case 3:
				case 5:
				case 6:
				case 7:
					return str;

				case 4:
					return "Melee";

				case 8:
					return "Explosion";

				case 0x10:
					return "Radiation";

				case 0x20:
					return "Cold";
				}
				return str;
			}
		}

		/// <summary>
		/// This getter returns the entity that is being destroyed.
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

		/// <summary>
		/// Gets if the destroy was caused by decay.
		/// </summary>
		public bool IsDecay
		{
			get
			{
				return this._decay;
			}
			set
			{
				this._decay = value;
			}
		}

		/// <summary>
		/// Gets the weaponimpact of the event.
		/// </summary>
		public WeaponImpact WeaponData
		{
			get
			{
				return this._wi;
			}
			set
			{
				this._wi = value;
			}
		}

		/// <summary>
		/// Gets the weapon's name that caused the damage.
		/// </summary>
		public string WeaponName
		{
			get
			{
				return this._weapon;
			}
			set
			{
				this._weapon = value;
			}
		}
	}
}

