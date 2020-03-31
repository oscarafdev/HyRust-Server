namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class ShareCommand : ChatCommand
    {
        public static Hashtable shared_doors = new Hashtable();

        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (!RustPP.Data.Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(), $"No estas logueado, usa /login o /registro", 4f);
                return;
            }
            string playerName = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            if (playerName == string.Empty)
            {
                pl.SendClientMessage("<Sintaxis> /share <NombreJugador>");
                return;
            }
            PList list = new PList();
            list.Add(0, "Cancelar");
            foreach (KeyValuePair<ulong, string> entry in Core.userCache)
            {
                if (entry.Value.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                {
                    DoorShare(new PList.Player(entry.Key, entry.Value), pl);
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
                        DoorShare(new PList.Player(client.UID, client.Name), pl);
                        return;
                    }
                    if (client.Name.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
                        list.Add(client.UID, client.Name);
                }
            }
            if (list.Count == 1)
            {
                pl.SendClientMessage(string.Format("[color red]<Error>[/color] No se encontraron jugadores con el nombre {0}.", playerName));
                return;
            }
            pl.SendClientMessage(string.Format("{0}  jugador{1} {2}: ", ((list.Count - 1)).ToString(), (((list.Count - 1) > 1) ? "es encontrados" : " encontrado"), playerName));
            for (int i = 1; i < list.Count; i++)
            {
                pl.SendClientMessage(string.Format("{0} - {1}", i, list.PlayerList[i].DisplayName));
            }
            pl.SendClientMessage("0 - Cancelar");
            pl.SendClientMessage("Ingresa el numero del jugador al que deseas compartir tus puertas");
            Core.shareWaitList[pl.UID] = list;
        }

        public void PartialNameDoorShare(ref ConsoleSystem.Arg Arguments, int id)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (id == 0)
            {
                pl.SendClientMessage("¡Share Cancelado!");
                return;
            }
            PList list = (PList)Core.shareWaitList[pl.UID];
            DoorShare(list.PlayerList[id], pl);
        }

        public void DoorShare(PList.Player friend, Fougerite.Player sharing)
        {
            if (friend.UserID == sharing.UID)
            {
                sharing.SendClientMessage("[color red]<Error>[/color] No puedes compartir tus puertas a ti mismo");
                return;
            }
            Fougerite.Player client = Fougerite.Server.GetServer().FindPlayer(friend.UserID.ToString());
            if (!RustPP.Data.Globals.UserIsLogged(client))
            {
                sharing.SendClientMessage($"[color red]<Error>[/color] {client.Name} no esta logueado.");
                return;
            }
            ArrayList shareList = (ArrayList)shared_doors[sharing.UID];
            if (shareList == null)
            {
                shareList = new ArrayList();
                shared_doors.Add(sharing.UID, shareList);

            }
            if (shareList.Contains(friend.UserID))
            {
                sharing.SendClientMessage(string.Format("[color red]<Error>[/color] Ya compartes tus puertas con {0}.", friend.DisplayName));
                return;
            }
            shareList.Add(friend.UserID);
            shared_doors[sharing.UID] = shareList;
            sharing.SendClientMessage(string.Format("Ahora compartes tus puertas con {0}.", friend.DisplayName));
            
            if (client != null)
                client.SendClientMessage(string.Format("[color cyan]<!> [/color]{0} te dio las llaves de sus puertas.", sharing.Name));
        }
        public void AddDoors(ulong UID, Fougerite.Player player)
        {
            if (player.UID == UID)
            {
                return;
            }
            ArrayList shareList = (ArrayList)shared_doors[UID];
            if (shareList == null)
            {
                shareList = new ArrayList();
                shared_doors.Add(UID, shareList);

            }
            if (shareList.Contains(player.UID))
            {
                
                return;
            }
            shareList.Add(player.UID);
            shared_doors[UID] = shareList;

        }

        public Hashtable GetSharedDoors()
        {
            return shared_doors;
        }

        public void SetSharedDoors(Hashtable sd)
        {
            shared_doors = sd;
        }
    }
}