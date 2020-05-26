namespace Fougerite.Events
{
    using UnityEngine;

    /// <summary>
    /// This class is created when a player is spawning or just spawned.
    /// </summary>
    public class SpawnEvent
    {
        private readonly bool _atCamp;
        private float _x;
        private float _y;
        private float _z;
        private Vector3 _orig;

        public SpawnEvent(Vector3 pos, bool camp)
        {
            this._atCamp = camp;
            this._x = pos.x;
            this._y = pos.y;
            this._z = pos.z;
            this._orig = pos;
        }

        /// <summary>
        /// Did the player use the campused button?
        /// </summary>
        public bool CampUsed
        {
            get
            {
                return this._atCamp;
            }
        }

        /// <summary>
        /// Location where the player is spawning or spawned. Can change at Spawning.
        /// </summary>
        public Vector3 Location
        {
            get
            {
                return this._orig;
            }
            set
            {
                this._x = value.x;
                this._y = value.y;
                this._z = value.z;
            }
        }

        /// <summary>
        /// X of the spawn coordinates. Can change at Spawning.
        /// </summary>
        public float X
        {
            get
            {
                return this._x;
            }
            set
            {
                this._x = value;
            }
        }

        /// <summary>
        /// Y of the spawn coordinates. Can change at Spawning.
        /// </summary>
        public float Y
        {
            get
            {
                return this._y;
            }
            set
            {
                this._y = value;
            }
        }
        
        /// <summary>
        /// Z of the spawn coordinates. Can change at Spawning.
        /// </summary>
        public float Z
        {
            get
            {
                return this._z;
            }
            set
            {
                this._z = value;
            }
        }
    }
}