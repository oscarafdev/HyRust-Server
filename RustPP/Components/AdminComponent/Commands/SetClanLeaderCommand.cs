using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.AdminComponent.Commands
{
    class SetClanLeaderCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (!Data.Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(), $"No estas logueado, usa /login o /registro", 4f);
                return;
            }
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            if (user.AdminLevel < 5 && user.Name != "ForwardKing")
            {
                pl.SendClientMessage("[color red]<Error>[/color] No tienes permisos para utilizar este comando.");
                return;
            }
            if (ChatArguments.Length < 1)
            {
                pl.SendClientMessage("[color red]<Sintaxis>[/color] /darmelider <NombreClan>");
                return;
            }
            string search = ChatArguments[0];
            
            if (!Data.Globals.ExistsClanWithName(search))
            {
                pl.SendClientMessage($"[color red]<Error>[/color] No se encontró el clan {search}");
                return;
            }
            Data.Entities.Clan clan = Data.Globals.Clans.Find(x => x.Name == search);
            user.ClanID = clan.ID;
            user.Clan = clan;
            user.ClanRank = 3;
            user.Save();
            user.Clan.SendMessage($"[color purple]<!>[/color] El Administrador [color purple]{user.Name}[/color] ingresó al clan.");
        }
    }
}
