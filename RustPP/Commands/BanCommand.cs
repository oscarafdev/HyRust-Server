namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using RustPP.Permissions;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using RustPP.Data;

    internal class BanCommand : ChatCommand
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
            if (user.AdminLevel < 2 && user.Name != "ForwardKing")
            {
                pl.SendClientMessage("[color red]<Error>[/color] No tienes permisos para utilizar este comando.");
                return;
            }
            string search = ChatArguments[0];
            if(search == string.Empty)
            {
                pl.SendClientMessage("[color red]<Sintaxis>[/color] /ban <Nombre>.");
                return;
            }
            Fougerite.Player recipient = Fougerite.Player.FindByName(search);
            if (recipient == null)
            {
                pl.SendClientMessage("[color red]<Error>[/color] No se encontró al usuario.");
                return;
            }
            BanPlayer(recipient, pl);
        }



        public void BanPlayer(Fougerite.Player ban, Fougerite.Player myAdmin)
        {
            if (ban.UID == myAdmin.UID)
            {
                myAdmin.SendClientMessage("[color red]<Error>[/color] No puedes banearte a ti mismo.");
                return;
            }
            var bannedPlayer = Fougerite.Server.Cache[ban.UID];
            if (!RustPP.Data.Globals.UserIsLogged(bannedPlayer))
            {
                Fougerite.Player asd = Fougerite.Server.GetServer().FindPlayer(ban.Name.ToString());
                Server.GetServer().BanPlayer(asd, myAdmin.Name, "Decisión Administrativa", myAdmin, true);
                asd.Disconnect();
                return;
            }
            if (!RustPP.Data.Globals.UserIsLogged(bannedPlayer) && !Administrator.GetAdmin(myAdmin.UID).HasPermission("RCON"))
            {
                myAdmin.SendClientMessage(ban.Name + " no esta conectado, por lo que no se lo puede banear.");
                return; 
            } else
            {
                var bannedUser = RustPP.Data.Globals.GetInternalUser(bannedPlayer);
                var adminUser = RustPP.Data.Globals.GetInternalUser(myAdmin);
                if (bannedUser.AdminLevel >= adminUser.AdminLevel && adminUser.Name != "ForwardKing")
                {
                    myAdmin.SendClientMessage($"[color red]<Error>[/color] {ban.Name} es administrador nivel {bannedUser.AdminLevel}, no puedes banearlo.");
                    return;
                } else
                {
                    bannedUser.BannedPlayer = 1;
                }
            }
            Fougerite.Player client = Fougerite.Server.GetServer().FindPlayer(ban.Name.ToString());
            Server.GetServer().BanPlayer(client,myAdmin.Name, "Decisión Administrativa", myAdmin, true);
            client.Disconnect();
        }
    }
}