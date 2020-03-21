using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Data.Entities
{
    public class UserInventoryItem
    {
        public int Slot { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public float Condition { get; set; }
        public int WeaponSlots { get; set; }
        public int WeaponBullets { get; set; }
        public string WeaponSlot1 { get; set; }
        public string WeaponSlot2 { get; set; }
        public string WeaponSlot3 { get; set; }
        public string WeaponSlot4 { get; set; }
        public string WeaponSlot5 { get; set; }
    }
}
