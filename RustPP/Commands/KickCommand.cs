namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using RustPP.Components.LanguageComponent;
    using RustPP.Permissions;
    using System;
    using System.Collections.Generic;

    internal class KickCommand : ChatCommand
    {
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
            RustPP.Data.Entities.User user = RustPP.Data.Globals.usersOnline.FindLast(x => x.Name == pl.Name);
            if (user.AdminLevel < 3 && user.Name != "ForwardKing")
            {
                pl.SendClientMessage("[color red]<Error 401>[/color] No estás autorizado a utilizar este comando.");
                return;
            }
            string playerName = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            if (playerName == string.Empty)
            {
                pl.MessageFrom(Core.Name, "[color red]<Sintaxis>[/color] /kick <NombreJugador>");
                return;
            }
            Fougerite.Player recipient = Fougerite.Player.FindByName(playerName);
            KickPlayer(recipient, pl);
           
        }

        public void KickPlayer(Fougerite.Player badPlayer, Fougerite.Player myAdmin)
        {
            RustPP.Data.Entities.User kicked = RustPP.Data.Globals.usersOnline.FindLast(x => x.Name == badPlayer.Name);
            RustPP.Data.Entities.User admin = RustPP.Data.Globals.usersOnline.FindLast(x => x.Name == myAdmin.Name);
            if(kicked != null)
            {
                if (Administrator.IsAdmin(badPlayer.UID) && !Administrator.GetAdmin(myAdmin.UID).HasPermission("RCON"))
                {
                    myAdmin.MessageFrom(Core.Name, badPlayer.Name + " es un administrador, no puedes kickear administradores.");
                    return;
                }
                RustPP.Data.Globals.SendAdminMessageForAll(string.Format("{0} fue kickeado por {1}.", badPlayer.Name, myAdmin.Name));
                badPlayer.Disconnect();
            } else
            {
                if (badPlayer == myAdmin)
                {
                    myAdmin.MessageFrom(Core.Name, "No puedes kickearte.");
                    return;
                }
                else if (Administrator.IsAdmin(badPlayer.UID) && !Administrator.GetAdmin(myAdmin.UID).HasPermission("RCON") || kicked.AdminLevel >= admin.AdminLevel)
                {
                    myAdmin.MessageFrom(Core.Name, badPlayer.Name + " es un administrador, no puedes kickear administradores.");
                    return;
                }
                else
                {
                    Administrator.NotifyAdmins(string.Format("{0} fue kickeado por {1}.", badPlayer.Name, myAdmin.Name));
                    badPlayer.Disconnect();
                }
            }
            
        }
    }
}