using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.AdminComponent.Commands
{
    using Fougerite;
    using RustPP;
    using RustPP.Commands;
    using RustPP.Data;
    using System;
    using System.Text.RegularExpressions;
    using static ConsoleSystem;

    public class AdminChatCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (!Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(), $"No estas logueado, usa /login o /registro", 4f);
                return;
            }
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            
            string strText = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            string rem = Regex.Replace(strText, @"\[/?color\b.*?\]", string.Empty);
            string template = "[color #ff968f][A]((Admin -adminLevel- -userName-: -userMessage-))";
            string setname = Regex.Replace(template, "-userName-", Arguments.argUser.displayName);
            string setadmin = Regex.Replace(setname, "-adminLevel-", user.AdminLevel.ToString());
            string final = Regex.Replace(setadmin, "-userMessage-", rem);
            if (strText == string.Empty)
            {
                pl.SendClientMessage("[color red]<Sintaxis> [color white]/a <Texto>");
            }
            else
            {
                foreach (Fougerite.Player player in Fougerite.Server.GetServer().Players)
                {
                    if (player.IsOnline && Globals.UserIsLogged(player))
                    {
                        RustPP.Data.Entities.User uuser = RustPP.Data.Globals.GetInternalUser(player);
                        if(uuser.AdminLevel >= 1)
                        {
                            player.SendClientMessage(final);
                        }
                        
                    }
                }
            }
        }
    }
}
