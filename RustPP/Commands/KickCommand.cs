namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using RustPP.Permissions;
    using System;
    using System.Collections.Generic;

    internal class KickCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (!RustPP.Data.Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(), $"No estas logueado, usa /login o /registro", 4f);
                return;
            }
            RustPP.Data.Entities.User user = RustPP.Data.Globals.usersOnline.FindLast(x => x.Name == pl.Name);
            if (user.AdminLevel < 3)
            {
                pl.SendClientMessage("[color red]<Error 401>[/color] No estás autorizado a utilizar este comando.");
                return;
            }
            string playerName = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            if (playerName == string.Empty)
            {
                pl.MessageFrom(Core.Name, "[color red]<Sintaxis>[/color] /kick <NombreJugador>");
            }
            PList list = new PList();
            list.Add(0, "Cancel");
            foreach (Fougerite.Player client in Fougerite.Server.GetServer().Players)
            {
                if (client.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                {
                    KickPlayer(client, pl);
                    return;
                } else if (client.Name.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
                    list.Add(client.UID, client.Name);
            }
            if (list.Count == 1)
            {
                pl.MessageFrom(Core.Name, "No player matches the name: " + playerName);
                return;
            }
            pl.MessageFrom(Core.Name, string.Format("{0}  player{1} {2}: ", ((list.Count - 1)).ToString(), (((list.Count - 1) > 1) ? "s match" : " matches"), playerName));
            for (int i = 1; i < list.Count; i++)
            {
                pl.MessageFrom(Core.Name, string.Format("{0} - {1}", i, list.PlayerList[i].DisplayName));
            }
            pl.MessageFrom(Core.Name, "0 - Cancel");
            pl.MessageFrom(Core.Name, "Please enter the number matching the player to kick.");
            Core.kickWaitList[pl.UID] = list;
        }

        public void PartialNameKick(ref ConsoleSystem.Arg Arguments, int id)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (id == 0)
            {
                pl.SendClientMessage("¡Comando cancelado!");
                return;
            }
                
            PList list = (PList)Core.kickWaitList[pl.UID];
            Fougerite.Player client = Fougerite.Server.GetServer().FindPlayer(list.PlayerList[id].UserID.ToString());
            if (client != null) KickPlayer(client, pl);
        }

        public void KickPlayer(Fougerite.Player badPlayer, Fougerite.Player myAdmin)
        {
            if (badPlayer == myAdmin)
            {
                myAdmin.MessageFrom(Core.Name, "No puedes kickearte.");
            } else if (Administrator.IsAdmin(badPlayer.UID) && !Administrator.GetAdmin(myAdmin.UID).HasPermission("RCON"))
            {
                myAdmin.MessageFrom(Core.Name, badPlayer.Name + " es un administrador, no puedes kickear administradores.");
            } else
            {
                Administrator.NotifyAdmins(string.Format("{0} fue kickeado por {1}.", badPlayer.Name, myAdmin.Name));
                badPlayer.Disconnect();
            }
        }
    }
}