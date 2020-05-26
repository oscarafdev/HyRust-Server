using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.AdminComponent.Commands
{
    class PayDayCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            string lang = LanguageComponent.LanguageComponent.GetPlayerLangOrDefault(pl);
            if (!Data.Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(),LanguageComponent.LanguageComponent.getMessage("notice_not_logged", lang), 4f);
                return;
            }
            RustPP.Data.Entities.User user = Data.Globals.usersOnline.FindLast(x => x.Name == pl.Name);
            if(user.AdminLevel < 4)
            {
                pl.SendClientMessage($"[color red]<Error>[/color] ¡No tienes permisos para utilizar este comando!");
                return;
            }
            foreach (RustPP.Data.Entities.User usuario in RustPP.Data.Globals.usersOnline)
            {
                usuario.TimeToPayDay = 3;
            }
            Fougerite.Server.GetServer().SendMessageForAll($"[color #d311ea] El administrador {pl.Name} adelantó el PayDay.");
        }
    }
}
