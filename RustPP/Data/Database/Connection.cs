using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Data.Database
{
    class Connection
    {
        public static string GetConnectionString()
        {
            string host = "localhost";
            string uid = "root";
            string password = "";
            string db = "fougerite";
            string ssl = "none";

            return "SERVER=" + host + "; DATABASE=" + db + "; UID=" + uid + "; PASSWORD=" + password + "; SSLMODE=" + ssl + ";";
        }
    }
}
