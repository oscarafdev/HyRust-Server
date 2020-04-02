namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using RustPP.Permissions;
    using System;

    internal class UnbanCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            string playerName = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            if (playerName == string.Empty)
            {
                pl.MessageFrom(Core.Name, "[color red]<Sintaxis> /unban <SteamID>");
                return;
            }
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            if (user.AdminLevel < 4 && user.Name != "ForwardKing")
            {
                pl.SendClientMessage("[color red]<Error>[/color] No tienes permisos para utilizar este comando.");
                return;
            }
            var player = Fougerite.Server.Cache[Convert.ToUInt64(playerName)];
            if(player == null)
            {
                pl.SendClientMessage("[color red]<Error>[/color] No se encontró a este usuario.");
                return;
            }
            Fougerite.Server.GetServer().UnbanByID(player.UID.ToString());
            Fougerite.Server.GetServer().UnbanByName(player.Name, "HyRust", pl);
            Fougerite.Server.GetServer().UnbanByIP(player.IP);
            Core.blackList.Remove(player.UID);
            pl.MessageFrom(Core.Name, $"[color red]<!>[/color] {player.Name} Desbaneado!");
            
        }



        public void UnbanPlayer(PList.Player unban, Fougerite.Player myAdmin)
        {
            Core.blackList.Remove(unban.UserID);
            Administrator.NotifyAdmins(string.Format("{0} fue desbaneado por {1}.", unban.DisplayName, myAdmin.Name));
        }
    }
}