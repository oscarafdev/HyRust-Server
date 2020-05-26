using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.EconomyComponent.Commands
{
    class BuyCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            string lang = LanguageComponent.LanguageComponent.GetPlayerLangOrDefault(pl);
            if (!RustPP.Data.Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(),LanguageComponent.LanguageComponent.getMessage("notice_not_logged", lang), 4f);
                return;
            }
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            if(user.AdminLevel != 6)
            {
                return;
            }
            string invite = ChatArguments[0];
            if(invite != string.Empty)
            {
                user.PrefabName = invite;
                user.SpawningPrefab = true;
            }
            else
            {
                user.SpawningPrefab = false;
            }
            
            
        }
    }
}
