namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using RustPP.Permissions;
    using System;
    using System.Collections.Generic;

    internal class UnmuteCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            string playerName = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            if (playerName == string.Empty)
            {
                pl.SendClientMessage("[color red]<Sintaxis>[/color] /desmutear <NombreJugador>");
                return;
            }
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            if (user.AdminLevel < 1 && user.Name != "ForwardKing")
            {
                pl.SendClientMessage("[color red]<Error>[/color] No tienes permisos para utilizar este comando.");
                return;
            }
            string search = ChatArguments[0];
            Fougerite.Player recipient = Fougerite.Player.FindByName(search);
            RustPP.Data.Entities.User recipientUser = RustPP.Data.Globals.GetInternalUser(recipient);
            if (recipient == null)
            {
                pl.SendClientMessage($"[color red]<Error>[/color] No se encontró al usuario {search}");
                return;
            }

            UnmutePlayer(recipient, pl);
            
        }


        public void UnmutePlayer(Fougerite.Player unmute, Fougerite.Player myAdmin)
        {
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(myAdmin);
            var mutedPlayer = Fougerite.Server.Cache[unmute.UID];
            if (!RustPP.Data.Globals.UserIsLogged(mutedPlayer))
            {
                myAdmin.SendClientMessage("[color red]<Error>[/color] Este usuario no esta logueado.");
                return;
            }
            RustPP.Data.Entities.User muted = RustPP.Data.Globals.GetInternalUser(mutedPlayer);
            foreach (RustPP.Data.Entities.User usuario in RustPP.Data.Globals.usersOnline)
            {
                if (usuario.AdminLevel >= 1)
                {
                    usuario.Player.SendClientMessage($"[color red]<Admin>[/color] {user.Name} desmuteo a {muted.Name}");
                }
            }
            muted.Muted = 0;
            muted.Save();
            muted.Player.SendClientMessage($"Fuiste desmuteado por {user.Name}");
        }
    }
}