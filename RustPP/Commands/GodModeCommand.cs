namespace RustPP.Commands
{
    using Fougerite;
    using RustPP.Components.LanguageComponent;
    using System;
    using System.Collections.Generic;

    public class GodModeCommand : ChatCommand
    {
        public List<ulong> userIDs = new List<ulong>();

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
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            if (user.AdminLevel < 2 && user.Name != "ForwardKing")
            {
                pl.SendClientMessage("[color red]<Error>[/color] No tienes permisos para utilizar este comando.");
                return;
            }
            if (user.GodMode)
            {
                user.GodMode = false;
                pl.SendClientMessage("¡Desactivaste el modo DIOS!");
                pl.PlayerClient.controllable.character.takeDamage.SetGodMode(false);
                return;
            }
            else
            {
                user.GodMode = true;
                pl.PlayerClient.controllable.character.takeDamage.SetGodMode(true);
                if (pl.FallDamage != null) { pl.FallDamage.ClearInjury();}
                pl.SendClientMessage("¡Activaste el modo DIOS!");
            }
        }

        public bool IsOn(ulong uid)
        {
            if (Fougerite.Server.Cache.ContainsKey(uid))
            {
                var pl = Fougerite.Server.Cache[uid];
                if (!RustPP.Data.Globals.UserIsLogged(pl))
                {
                    return false;
                }
                RustPP.Data.Entities.User user = RustPP.Data.Globals.usersOnline.FindLast(x => x.Name == pl.Name);
                return user.GodMode;
            }
            return false;
        }
    }
}