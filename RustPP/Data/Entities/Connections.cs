using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Data.Entities
{
    class Connections
    {
        public string IP { get; set; }
        public string Name { get; set; }
        public float Time { get; set; }
        public Fougerite.Player Player { get; set; }
    }
}
