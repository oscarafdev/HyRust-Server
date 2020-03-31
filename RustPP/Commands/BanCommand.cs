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
            string queryName = Arguments.ArgsStr.Trim(new char[] { ' ', '"' });
            if (queryName == string.Empty)
            {
                pl.SendClientMessage("[color red]<Sintaxis>[/color] /ban <NombreJugador>");
                return;
            }

            var query = from entry in Core.userCache
                        let sim = entry.Value.Similarity(queryName)
                        where sim > 0.4d
                        group new PList.Player(entry.Key, entry.Value) by sim into matches
                        select matches.FirstOrDefault();

            if (query.Count() == 1)
            {
                BanPlayer(query.First(), pl);
            }
            else
            {
                pl.SendClientMessage(string.Format("Se encontraron {0} con el nombre {1}: ", query.Count(), queryName));
                for (int i = 1; i < query.Count(); i++)
                {
                    pl.SendClientMessage(string.Format("{0} - {1}", i, query.ElementAt(i).DisplayName));
                }
                pl.SendClientMessage("0 - Cancelar");
                pl.SendClientMessage("Ingrese el numero del usuario que esta buscando para banear.");
                Core.banWaitList[pl.UID] = query;
            }
        }

        public void PartialNameBan(ref ConsoleSystem.Arg Arguments, int id)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (id == 0)
            {
                pl.SendClientMessage("[color yellow]<Advertencia>[/color] El comando fue cancelado");
                return;
            }
            var list = Core.banWaitList[pl.UID] as IEnumerable<PList.Player>;
            BanPlayer(list.ElementAt(id), pl);
        }

        public void BanPlayer(PList.Player ban, Fougerite.Player myAdmin)
        {
            if (ban.UserID == myAdmin.UID)
            {
                myAdmin.SendClientMessage("[color red]<Error>[/color] No puedes banearte a ti mismo.");
                return;
            }
            var bannedPlayer = Fougerite.Server.Cache[ban.UserID];
            if (!RustPP.Data.Globals.UserIsLogged(bannedPlayer))
            {
                myAdmin.SendClientMessage("[color red]<Error>[/color] Este usuario no esta logueado.");
                return;
            }
            if (!RustPP.Data.Globals.UserIsLogged(bannedPlayer) && !Administrator.GetAdmin(myAdmin.UID).HasPermission("RCON"))
            {
                myAdmin.SendClientMessage(ban.DisplayName + " no esta conectado, por lo que no se lo puede banear.");
                return; 
            } else
            {
                var bannedUser = RustPP.Data.Globals.GetInternalUser(bannedPlayer);
                var adminUser = RustPP.Data.Globals.GetInternalUser(myAdmin);
                if (bannedUser.AdminLevel >= adminUser.AdminLevel && adminUser.Name != "ForwardKing")
                {
                    myAdmin.SendClientMessage($"[color red]<Error>[/color] {ban.DisplayName} es administrador nivel {bannedUser.AdminLevel}, no puedes banearlo.");
                    return;
                } else
                {
                    bannedUser.BannedPlayer = 1;
                }
            }
            if (RustPP.Core.blackList.Contains(ban.UserID))
            {
                Logger.LogError(string.Format("[BanPlayer] {0}, id={1} ya esta en la lista de baneados.", ban.DisplayName, ban.UserID));
                Core.blackList.Remove(ban.UserID);
            }
            Core.blackList.Add(ban);
            Administrator.DeleteAdmin(ban.UserID);
            Server.GetServer().SendMessageForAll(string.Format("[color red]{0} fue baneado permanentemente por {1}.", ban.DisplayName, myAdmin.Name));
            Fougerite.Player client = Fougerite.Server.GetServer().FindPlayer(ban.UserID.ToString());
            if (client != null)
                client.Disconnect();
        }
    }
}