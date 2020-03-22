using Fougerite;
using RustPP.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Data
{
    class Globals
    {
        public static List<User> usersOnline = new List<User>();
        public static List<KitItem> KitItems = new List<KitItem>();
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
        public static void SendAdminMessageForAll(string message)
        {
            foreach(User user in Data.Globals.usersOnline)
            {
                if(user.AdminLevel >= 1)
                {
                    user.Player.SendClientMessage($"[color orange] {message}");
                }
                
            }
            Logger.LogDebug(message);
        }
        public static Entities.User GetInternalUser(Fougerite.Player player)
        {
            return Data.Globals.usersOnline.FindLast(x => x.Name == player.Name);
        }
    }
}
