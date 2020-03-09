

namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using RustPP.Social;
    using System;
    using System.Collections.Generic;
    using System.Security;

    internal class AddFriendCommand : ChatCommand
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

            string playerName = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            if (playerName == string.Empty)
            {
                pl.SendClientMessage("[color red]<Sintaxis>[/color] /addfriend <NombreJugador>");
                return;
            }

            PList list = new PList();
            list.Add(0, "Cancelar");
            foreach (KeyValuePair<ulong, string> entry in Core.userCache)
            {
                if (entry.Value.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                {
                    AddFriend(new PList.Player(entry.Key, entry.Value), pl);
                    return;
                }
                else if (entry.Value.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
                    list.Add(entry.Key, entry.Value);
            }
            if (list.Count == 1)
            {
                foreach (Fougerite.Player client in Fougerite.Server.GetServer().Players)
                {
                    if (client.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                    {
                        AddFriend(new PList.Player(client.UID, client.Name), pl);
                        return;
                    }
                    if (client.Name.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
                        list.Add(client.UID, client.Name);
                }
            }
            if (list.Count == 1)
            {
                pl.SendClientMessage(string.Format("[color red]<Error>[/color]No se encontraron jugadores con el nombre {0}.", playerName));
                return;
            }
            pl.SendClientMessage(string.Format("Se encontro a {0} jugador{1} con el nombre {2}: ", ((list.Count - 1)).ToString(), (((list.Count - 1) > 1) ? "es" : ""), playerName));
            for (int i = 1; i < list.Count; i++)
            {
                pl.SendClientMessage(string.Format("{0} - {1}", i, list.PlayerList[i].DisplayName));
            }
            pl.SendClientMessage("0 - Cancelar");
            pl.SendClientMessage("Por favor ingrese el numero del jugador que esta buscando.");
            Core.friendWaitList[pl.UID] = list;
        }

        public void PartialNameAddFriend(ref ConsoleSystem.Arg Arguments, int id)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (id == 0)
            {
                pl.SendClientMessage("¡Se canceló la busqueda del usuario!");
                return;
            }
            PList list = (PList)Core.friendWaitList[pl.UID];
            AddFriend(list.PlayerList[id], pl);
        }

        public void AddFriend(PList.Player friend, Fougerite.Player friending)
        {
            if (friending.UID == friend.UserID)
            {
                friending.SendClientMessage("[color red]<Error>[/color] ¡No puedes agregarte a tí mismo como amigo!");
                return;
            }
            FriendsCommand command = (FriendsCommand)ChatCommand.GetCommand("amigos");
            FriendList list = (FriendList)command.GetFriendsLists()[friending.UID];
            if (list == null)
            {
                list = new FriendList();
            }
            if (list.isFriendWith(friend.UserID))
            {
                friending.SendClientMessage(string.Format("Usted ya es amigo de {0}.", friend.DisplayName));
                return;
            }
            list.AddFriend(SecurityElement.Escape(friend.DisplayName), friend.UserID);
            command.GetFriendsLists()[friending.UID] = list;
            friending.SendClientMessage(string.Format("Agregaste a {0} a tu lista de amigos.", friend.DisplayName));
            Fougerite.Player ffriend = Fougerite.Server.GetServer().FindPlayer(friend.UserID.ToString());
            if (ffriend != null)
                ffriend.SendClientMessage(string.Format("{0} te agregó a su lista de amigos.", friending.Name));
        }
    }
}