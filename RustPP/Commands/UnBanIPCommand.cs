using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using RustPP.Permissions;
    using System;

    internal class UnBanIPCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            string playerName = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            if (playerName == string.Empty)
            {
                pl.MessageFrom(Core.Name, "[color red]<Sintaxis> /unbanip <IP>");
                return;
            }
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            if (user.AdminLevel < 3 && user.Name != "ForwardKing")
            {
                pl.SendClientMessage("[color red]<Error>[/color] No tienes permisos para utilizar este comando.");
                return;
            }

            Fougerite.Server.GetServer().UnbanByIP(playerName);
            pl.MessageFrom(Core.Name, $"[color red]<!>[/color] {playerName} Desbaneado!");


        }



        public void UnbanPlayer(PList.Player unban, Fougerite.Player myAdmin)
        {
            Core.blackList.Remove(unban.UserID);
            Administrator.NotifyAdmins(string.Format("{0} fue desbaneado por {1}.", unban.DisplayName, myAdmin.Name));
        }
    }
}
