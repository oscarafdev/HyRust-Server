using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RustPP.Components.ClanComponent.Commands
{
    class ClanChatCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            Fougerite.Player pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (!RustPP.Data.Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(), $"No estas logueado, usa /login o /registro", 4f);
                return;
            }
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            if(user.TimeToChat >= 1)
            {
                pl.SendClientMessage("[color red]<Error>[/color] Tienes que esperar {user.TimeToChat} para enviar otro mensaje");
                return;
            }
            if(user.ClanID == -1)
            {
                pl.SendClientMessage("[color red]<Error>[/color] No tienes Clan");
                return;
            }
            string rango = "Miembro";
            if (user.ClanRank == 2)
            {
                rango = "Reclutador";
            }
            if (user.ClanRank == 3)
            {
                rango = "Encargado";
            }
            if (user.Name == user.Clan.Owner)
            {
                rango = "Lider";
            }
            string strText = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            string rem = Regex.Replace(strText, @"\[/?color\b.*?\]", string.Empty);
            string template = "[color #2780d8][F]((-rank- -userName-: -userMessage-))";
            string setrank = Regex.Replace(template, "-rank-", rango);
            string setname = Regex.Replace(setrank, "-userName-", Arguments.argUser.displayName);
            string final = Regex.Replace(setname, "-userMessage-", rem);
            if (strText == string.Empty)
            {
                pl.SendClientMessage("[color red]<Sintaxis> [color white]/f <Texto>");
            }
            else
            {
                user.Clan.SendMessage(final);
                user.TimeToChat += 5;
            }
        }
    }
}
