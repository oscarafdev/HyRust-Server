using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.ClanComponent.Commands
{
    class AceptarCommand : ChatCommand
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
            if (user.ClanID == -1)
            {
                if(user.InvitedClan != -1)
                {
                    user.ClanID = user.InvitedClan;
                    Data.Entities.Clan clan = Data.Globals.Clans.Find(x => x.ID == user.ClanID);
                    user.Clan = clan;
                    user.ClanRank = 1;
                    user.Save();
                    Data.Globals.SendMessageForClan(user.ClanID, $"[color purple]<!>[/color] El usuario {user.Name} aceptó unirse al clan.");
                }
                else
                {
                    pl.SendClientMessage($"[color red]<!>[/color] ¡No tienes una invitación de clan!");
                    return;
                }
            }
            else
            {
                pl.SendClientMessage($"[color red]<!>[/color] ¡Ya estas en un clan! Usa /clan salir primero");
                return;
            }
        }
    }
}
