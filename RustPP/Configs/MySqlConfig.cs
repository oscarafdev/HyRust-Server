using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Configs
{
    class MySqlConfig
    {
        public static string host { get; } = "localhost";
        public static string database { get; } = "fougerite";
        public static string user { get; } = "root";
        public static string password { get; } = "";
    }
}
