﻿using RustPP.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Data
{
    class Globals
    {
        public static List<User> usersOnline = new List<User>();
        public static bool UserIsLogged(Fougerite.Player player)
        {
            if (Data.Globals.usersOnline.Count(x => x.Name == player.Name) >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
