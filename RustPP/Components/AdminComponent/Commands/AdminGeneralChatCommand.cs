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

    public class AdminGeneralChatCommand : ChatCommand
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
            if (user.AdminLevel < 1 && user.Name != "ForwardKing")
            {
                pl.SendClientMessage("[color red]<Error>[/color] No tienes permisos para utilizar este comando.");
                return;
            }
            string strText = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            string rem = Regex.Replace(strText, @"\[/?color\b.*?\]", string.Empty);
            string template = "[color #d311ea][AO]((Admin -adminLevel- -userName-: -userMessage-))";
            string setname = Regex.Replace(template, "-userName-", Arguments.argUser.displayName);
            string setadmin = Regex.Replace(setname, "-adminLevel-", user.AdminLevel.ToString());
            string final = Regex.Replace(setadmin, "-userMessage-", rem);
            if (strText == string.Empty)
            {
                pl.SendClientMessage("[color red]<Sintaxis> [color white]/ao <Texto>");
            }
            else
            {
                foreach (RustPP.Data.Entities.User player in RustPP.Data.Globals.usersOnline)
                {
                    player.Player.SendClientMessage(final);
                }
            }
        }
    }
}
