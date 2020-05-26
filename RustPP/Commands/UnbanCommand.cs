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
            
            if(Fougerite.Server.Cache.ContainsKey(Convert.ToUInt64(playerName)))
            {
                var player = Fougerite.Server.Cache[Convert.ToUInt64(playerName)];
                Fougerite.Server.GetServer().UnbanByID(player.UID.ToString());
                Fougerite.Server.GetServer().UnbanByName(player.Name, "HyRust", pl);
                Fougerite.Server.GetServer().UnbanByIP(player.IP);
                Core.blackList.Remove(player.UID);
                pl.MessageFrom(Core.Name, $"[color red]<!>[/color] {player.Name} Desbaneado!");
                return;
            }
            else
            {
                RustPP.Data.Entities.User player = RustPP.Data.Globals.GetUserBySteamID(playerName);
                if(player == null)
                {
                    pl.SendClientMessage("[color red]<Error>[/color] No se encontró a este usuario.");
                    return;
                }
                player.BannedPlayer = 0;
                player.Save();
                Fougerite.Server.GetServer().UnbanByID(player.SteamID.ToString());
                //Fougerite.Server.GetServer().UnbanByName(player.Name, "HyRust", pl);
                //Fougerite.Server.GetServer().UnbanByIP(player.IP);
                Core.blackList.Remove(player.SteamID);
                pl.MessageFrom(Core.Name, $"[color red]<!>[/color] {player.Name} Desbaneado!");
                return;
            }
            
            
        }



        public void UnbanPlayer(PList.Player unban, Fougerite.Player myAdmin)
        {
            Core.blackList.Remove(unban.UserID);
            Administrator.NotifyAdmins(string.Format("{0} fue desbaneado por {1}.", unban.DisplayName, myAdmin.Name));
        }
    }
}