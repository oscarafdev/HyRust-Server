

namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using RustPP.Components.LanguageComponent;
    using RustPP.Social;
    using System;
    using System.Collections.Generic;
    using System.Security;

    internal class AddFriendCommand : ChatCommand
    {
        /*DEPRECATED*/
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
            string search = ChatArguments[0];
            Fougerite.Player recipient = Fougerite.Player.FindByName(search);
            if (recipient == null)
            {
                pl.SendClientMessage("[color red]<Sintaxis>[/color] /addfriend <NombreJugador>");
                return;
            }
            AddFriend(recipient, pl);
        }


        public void AddFriend(Fougerite.Player friend, Fougerite.Player friending)
        {
            if (friending.UID == friend.UID)
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
            if (list.isFriendWith(friend.UID))
            {
                friending.SendClientMessage(string.Format("Usted ya es amigo de {0}.", friend.Name));
                return;
            }
            list.AddFriend(SecurityElement.Escape(friend.Name), friend.UID);
            command.GetFriendsLists()[friending.UID] = list;
            friending.SendClientMessage(string.Format("Agregaste a {0} a tu lista de amigos.", friend.Name));
            if (friend != null)
                friend.SendClientMessage(string.Format("{0} te agregó a su lista de amigos.", friending.Name));
        }
    }
}