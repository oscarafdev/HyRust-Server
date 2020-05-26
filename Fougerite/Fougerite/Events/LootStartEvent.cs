using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using UnityEngine;

namespace Fougerite.Events
{
    /// <summary>
    /// This class is created when someone starts looting something.
    /// </summary>
    public class LootStartEvent
    {
        private bool _cancel;
        private readonly Fougerite.Player _player;
        private readonly LootableObject _lo;
        private readonly Useable _ue;
        private readonly Entity _entity;
        private readonly bool _isobject;
        private readonly uLink.NetworkPlayer _np;
        private Timer _delayer = null;

        public LootStartEvent(LootableObject lo, Fougerite.Player player, Useable use, uLink.NetworkPlayer nplayer)
        {
            _lo = lo;
            _ue = use;
            _player = player;
            _np = nplayer;
            foreach (Collider collider in Physics.OverlapSphere(lo._inventory.transform.position, 1.2f))
            {
                if (collider.GetComponent<DeployableObject>() != null)
                {
                    _entity = new Entity(collider.GetComponent<DeployableObject>());
                    _isobject = true;
                    break;
                }
                if (collider.GetComponent<LootableObject>() != null)
                {
                    _entity = new Entity(collider.GetComponent<LootableObject>());
                    _isobject = false;
                    break;
                }
            }
        }

        internal void LootCancelTimer(object sender, ElapsedEventArgs e)
        {
            _delayer.Dispose();
            // Change callstate, and call eject, for a proper nice exit.
            _ue.callState = (FunctionCallState)0;
            _ue.Eject();
        }

        /// <summary>
        /// The cancel is done by a timer at the moment. AFAIK or remember the actual Use event is protected by
        /// some sort of obfuscation. I will try to patch the method once again since the DLL is cleaned by de4dot.
        /// The timer idea is a bit awful but does work flawlessly since I found that the _ue.callState = (FunctionCallState)0;
        /// has to be set to eject the player. The original "position" changing stuff caused bugs before.
        /// TODO: Patch and remove the timer cancel, find a way to bypass the addon's shitty modification bypass.
        /// </summary>
        public void Cancel()
        {
            if (!_cancel)
            {
                _cancel = true;
                if (_delayer != null) return;
                // Delay It, Let LootEnter Hook in Rust to finish.
                _delayer = new Timer(130);
                _delayer.Elapsed += LootCancelTimer;
                _delayer.Start();
            }
        }

        /// <summary>
        /// Checks if the stuff we are looting is a storage.
        /// </summary>
        public bool IsObject
        {
            get
            {
                return _isobject;
            }
        }

        /// <summary>
        /// Gets the Entity we are looting.
        /// </summary>
        public Entity Entity
        {
            get
            {
                return _entity;
            }
        }

        /// <summary>
        /// Gets the player who is looting.
        /// </summary>
        public Fougerite.Player Player
        {
            get
            {
                return _player;
            }
        }

        /// <summary>
        /// Gets the Useable class.
        /// </summary>
        public Useable Useable
        {
            get
            {
                return _ue;
            }
        }

        /// <summary>
        /// Gets the LootableObject class.
        /// </summary>
        public LootableObject LootableObject
        {
            get
            {
                return _lo;
            }
        }

        /// <summary>
        /// Checks if the event is cancelled.
        /// </summary>
        public bool IsCancelled
        {
            get
            {
                return _cancel;
            }
        }

        /// <summary>
        /// Gets the lootable object's name.
        /// </summary>
        public string LootName
        {
            get
            {
                return _lo.name;
            }
        }

        /// <summary>
        /// Gets the original Inventory class.
        /// </summary>
        public Inventory RustInventory
        {
            get
            {
                return _lo._inventory;
            }
        }

        /// <summary>
        /// Can change the occupied text?
        /// </summary>
        public string OccupiedText
        {
            get
            {
                return _lo.occupierText;
            }
            set
            {
                _lo.occupierText = value;
            }
        }
    }
}
