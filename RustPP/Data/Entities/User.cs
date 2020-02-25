using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Data.Entities
{
    public class User
    {
        public string Name { get; set; }
        public ulong SteamID { get; set; }
        public int WoodLevel { get; set; }
        public int WoodExp { get; set; }
        public Fougerite.Player Player { get; set; }
    }
}
