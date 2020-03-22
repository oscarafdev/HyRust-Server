using System;
using System.Diagnostics;
using System.Timers;
using Fougerite;

namespace RustPP.Components.AuthComponent
{
    using RustPP.Commands;
    using RustPP.Data.Entities;
    using System.Text.RegularExpressions;

    public class ReportCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (AuthComponent.UserIsLogged(pl))
            {
                User user = Data.Globals.usersOnline.FindLast(x => x.Name == pl.Name);
                if (ChatArguments.Length < 2)
                {
                    pl.SendClientMessage("[color red]<Sintaxis>[/color] /reportar <NombreJugador> <Motivo>");
                    return;
                }
                string search = ChatArguments[0];
                string reason = ChatArguments[1];
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
                pl.SendClientMessage($"[color red]Reportaste a {recipient.Name} por {reason}.");

                foreach (RustPP.Data.Entities.User usuario in RustPP.Data.Globals.usersOnline)
                {
                    if (usuario.AdminLevel >= 1)
                    {
                        usuario.Player.SendClientMessage($"[color red]<!>[/color] [color #f77777]{pl.Name} reporto a {recipient.Name} por: {reason}");
                    }
                }

            }
            else
            {
                pl.SendClientMessage("[color red]<Error>[/color] Primero debes estar conectado (Utiliza [color orange] /login[/color])");
            }

        }
    }
}
