namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using RustPP.Components.LanguageComponent;
    using RustPP.Permissions;
    using System;
    using System.Collections.Generic;

    internal class MuteCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            string lang = LanguageComponent.GetPlayerLangOrDefault(pl);
            if (!RustPP.Data.Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(),LanguageComponent.getMessage("notice_not_logged", lang), 4f);
                return;
            }
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            if (user.AdminLevel < 1 && user.Name != "ForwardKing")
            {
                pl.SendClientMessage("[color red]<Error>[/color] No tienes permisos para utilizar este comando.");
                return;
            }
            string playerName = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            if (playerName == string.Empty)
            {
                pl.SendClientMessage("[color red]<Sintaxis>[/color] /mute <NombreJugador>");
                return;
            }
            PList list = new PList();
            list.Add(0, "Cancel");
            foreach (KeyValuePair<ulong, string> entry in Core.userCache)
            {
                if (entry.Value.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                {
                    MutePlayer(new PList.Player(entry.Key, entry.Value), pl);
                    return;
                }
                if (entry.Value.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
                    list.Add(entry.Key, entry.Value);
            }
            if (list.Count == 1)
            {
                foreach (Fougerite.Player client in Fougerite.Server.GetServer().Players)
                {
                    if (client.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                    {
                        MutePlayer(new PList.Player(client.UID, client.Name), pl);
                        return;
                    }
                    if (client.Name.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
                        list.Add(client.UID, client.Name);
                }
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
            pl.SendClientMessage("Escriba el numero del jugador que desea mutear.");
            Core.muteWaitList[pl.UID] = list;
        }

        public void PartialNameMute(ref ConsoleSystem.Arg Arguments, int id)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (id == 0)
            {
                pl.SendClientMessage("¡Comando cancelado!");
                return;
            }
            PList list = (PList)Core.muteWaitList[pl.UID];
            MutePlayer(list.PlayerList[id], pl);
        }

        public void MutePlayer(PList.Player mute, Fougerite.Player myAdmin)
        {
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(myAdmin);
            var mutedPlayer = Fougerite.Server.Cache[mute.UserID];
            if (!RustPP.Data.Globals.UserIsLogged(mutedPlayer))
            {
                myAdmin.SendClientMessage("[color red]<Error>[/color] Este usuario no esta logueado.");
                return;
            }
            RustPP.Data.Entities.User muted = RustPP.Data.Globals.GetInternalUser(mutedPlayer);
            if (mute.UserID == myAdmin.UID)
            {
                myAdmin.SendClientMessage("[color red]<Error>[/color] No puedes mutearte a ti mismo.");
                return;
            }
            
            if (muted.Muted == 1)
            {
                myAdmin.SendClientMessage(string.Format("[color red]<Error>[/color] {0} ya esta muteado.", mute.DisplayName));
                return;
            }
            if (muted.AdminLevel >= user.AdminLevel && !(user.Name == "ForwardKing"))
            {
                myAdmin.SendClientMessage(string.Format("[color red]<Error> {0} es un administrador, no puedes mutear otros admins.", mute.DisplayName));
                return;
            }
            else { 
                muted.Muted = 1;
                muted.Player.SendClientMessage($"Fuiste muteado por {user.Name}");
            }
            foreach(RustPP.Data.Entities.User usuario in RustPP.Data.Globals.usersOnline)
            {
                if (usuario.AdminLevel >= 1)
                {
                    usuario.Player.SendClientMessage($"[color red]<Admin>[/color] {user.Name} muteo a {muted.Name}");
                }
            }
        }
    }
}