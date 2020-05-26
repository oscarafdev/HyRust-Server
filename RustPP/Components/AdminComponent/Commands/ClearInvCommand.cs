using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.AdminComponent.Commands
{
    class ClearInvCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            string lang = LanguageComponent.LanguageComponent.GetPlayerLangOrDefault(pl);
            if (!Data.Globals.UserIsLogged(pl))
            {
                pl.SendClientMessage(LanguageComponent.LanguageComponent.getMessage("error_no_logged", lang));
                return;
            }
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            if (user.AdminLevel < 3 && user.Name != "ForwardKing")
            {
                pl.SendClientMessage(LanguageComponent.LanguageComponent.getMessage("error_no_permissions", lang));
                return;
            }
            if (ChatArguments.Length < 1)
            {
                LanguageComponent.LanguageComponent.SendSyntaxError(pl, "/limpiarinv <NombreJugador>", "/limpiarinv <PlayerName>");
                return;
            }
            string search = ChatArguments[0];
            Fougerite.Player recipient = Fougerite.Player.FindByName(search);
            if (!Data.Globals.UserIsLogged(recipient))
            {
                pl.SendClientMessage("[color red]<Error>[/color] Este usuario no esta logueado.");
                return;
            }
            RustPP.Data.Entities.User recipientUser = Data.Globals.GetInternalUser(recipient);
            if (recipient == null)
            {
                pl.SendClientMessage($"[color red]<Error>[/color] No se encontró al usuario {search}");
                return;
            }
            if (recipientUser.AdminLevel >= user.AdminLevel && user.Name != "ForwardKing" && user.Name != recipientUser.Name)
            {
                pl.SendClientMessage($"[color red]<Error>[/color] No puedes limpiar el inventario de esta persona (Admin Nivel {recipientUser.AdminLevel})");
                return;
            }
            recipient.Inventory.ClearAll();

            pl.SendClientMessage($"[color #34ebde]Limpiaste el inventario de {recipientUser.Name}.");
            recipient.SendClientMessage($"[color #34ebde]El administrador {user.Name} limpió tu inventario.");

        } 
    }
}
