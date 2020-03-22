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
            PList list = new PList();
            list.Add(0, "Cancelar");
            foreach (PList.Player muted in Core.muteList.PlayerList)
            {
                Logger.LogDebug(string.Format("[UnmuteCommand] muted.DisplayName={0}, playerName={1}", muted.DisplayName, playerName));
                if (muted.DisplayName.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                {
                    UnmutePlayer(muted, pl);
                    return;
                }
                if (muted.DisplayName.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
                    list.Add(muted);
            }
            if (list.Count == 1)
            {
                pl.SendClientMessage("[color red]<Error>[/color] No se encontraron jugadores con el nombre: " + playerName);
                return;
            }
            pl.SendClientMessage(string.Format("{0}  jugador{1} {2}: ", ((list.Count - 1)).ToString(), (((list.Count - 1) > 1) ? "es encontrados" : " encontrado"), playerName));
            for (int i = 1; i < list.Count; i++)
            {
                pl.SendClientMessage(string.Format("{0} - {1}", i, list.PlayerList[i].DisplayName));
            }
            pl.SendClientMessage("0 - Cancelar");
            pl.SendClientMessage("Por favor escriba el numero del usuario que desea desmutear.");
            Core.unmuteWaitList[pl.UID] = list;
        }

        public void PartialNameUnmute(ref ConsoleSystem.Arg Arguments, int id)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (id == 0)
            {
                pl.SendClientMessage("¡Comando cancelado!");
                return;
            }
            PList list = (PList)Core.unmuteWaitList[pl.UID];
            UnmutePlayer(list.PlayerList[id], pl);
        }

        public void UnmutePlayer(PList.Player unmute, Fougerite.Player myAdmin)
        {
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(myAdmin);
            var mutedPlayer = Fougerite.Server.Cache[unmute.UserID];
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
            muted.Player.SendClientMessage($"Fuiste desmuteado por {user.Name}");
        }
    }
}