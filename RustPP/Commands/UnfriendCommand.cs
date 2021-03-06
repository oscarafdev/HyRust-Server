﻿namespace RustPP.Commands
{
    using Fougerite;
    using RustPP.Social;
    using System;
    using System.Collections.Generic;

    public class UnfriendCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            string playerName = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            if (playerName == string.Empty)
            {
                pl.MessageFrom(Core.Name, "[color red]<Sintaxis> /unfriend <NombreJugador>");
                return;
            }
            FriendsCommand command = (FriendsCommand)ChatCommand.GetCommand("amigos");
            FriendList friendsList = (FriendList)command.GetFriendsLists()[pl.UID];
            if (friendsList == null)
            {
                pl.MessageFrom(Core.Name, "Actualmente no tienes amigos (Pvta ke sad).");
                return;
            }
            if (friendsList.isFriendWith(playerName))
            {
                friendsList.RemoveFriend(playerName);
                pl.MessageFrom(Core.Name, "Removiste a " + playerName + " de tu lista de amigos.");
                if (friendsList.HasFriends())
                {
                    command.GetFriendsLists()[pl.UID] = friendsList;
                }
                else
                {
                    command.GetFriendsLists().Remove(pl.UID);
                }
            }
            else
            {
                PList list = new PList();
                list.Add(0, "Cancel");
                foreach (KeyValuePair<ulong, string> entry in Core.userCache)
                {
                    if (friendsList.isFriendWith(entry.Key) && entry.Value.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
                        list.Add(entry.Key, entry.Value);
                }
                if (list.Count == 1)
                {
                    foreach (Fougerite.Player client in Fougerite.Server.GetServer().Players)
                    {
                        if (friendsList.isFriendWith(client.UID) && client.Name.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
                            list.Add(client.UID, client.Name);
                    }
                }
                if (list.Count == 1)
                {
                    pl.MessageFrom(Core.Name, string.Format("No eres amigo de {0}.", playerName));
                    return;
                }

                pl.MessageFrom(Core.Name, string.Format("{0}  amigo{1} {2}: ", ((list.Count - 1)).ToString(), (((list.Count - 1) > 1) ? "s encontrados" : " encontrado"), playerName));
                for (int i = 1; i < list.Count; i++)
                {
                    pl.MessageFrom(Core.Name, string.Format("{0} - {1}", i, list.PlayerList[i].DisplayName));
                }
                pl.MessageFrom(Core.Name, "0 - Cancelar");
                pl.MessageFrom(Core.Name, "Selecciona el amigo al que quieres eliminar.");
                Core.unfriendWaitList[pl.UID] = list;
            }
        }

        public void PartialNameUnfriend(ref ConsoleSystem.Arg Arguments, int id)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (id == 0)
            {
                pl.SendClientMessage("¡Comando cancelado!");
                return;
            }
            PList list = (PList)Core.unfriendWaitList[pl.UID];
            Unfriend(list.PlayerList[id], pl);
        }

        public void Unfriend(PList.Player exfriend, Fougerite.Player unfriending)
        {
            FriendsCommand command = (FriendsCommand)ChatCommand.GetCommand("amigos");
            FriendList friendsList = (FriendList)command.GetFriendsLists()[unfriending.UID];

            friendsList.RemoveFriend(exfriend.UserID);
            command.GetFriendsLists()[unfriending.UID] = friendsList;
            unfriending.MessageFrom(Core.Name, string.Format("Removiste a {0} de tu lista de amigos.", exfriend.DisplayName));
        }
    }
}