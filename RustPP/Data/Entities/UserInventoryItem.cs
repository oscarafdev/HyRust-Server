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
        public int WeaponSlot1 { get; set; }
        public int WeaponSlot2 { get; set; }
        public int WeaponSlot3 { get; set; }
        public int WeaponSlot4 { get; set; }
    }
}
