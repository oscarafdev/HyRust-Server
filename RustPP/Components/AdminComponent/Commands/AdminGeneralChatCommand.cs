using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.AdminComponent.Commands
{
    using Fougerite;
    using RustPP;
    using RustPP.Commands;
    using RustPP.Data;
    using System;
    using System.Text.RegularExpressions;
    using static ConsoleSystem;

    public class AdminGeneralChatCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            string lang = LanguageComponent.LanguageComponent.GetPlayerLangOrDefault(pl);
            if (!Globals.UserIsLogged(pl))
            {
                pl.SendClientMessage(LanguageComponent.LanguageComponent.getMessage("error_no_logged", lang));
                return;
            }
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            if (user.AdminLevel < 1 && user.Name != "ForwardKing")
            {
                pl.SendClientMessage(LanguageComponent.LanguageComponent.getMessage("error_no_permissions", lang));
                return;
            }
            string strText = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            string rem = Regex.Replace(strText, @"\[/?color\b.*?\]", string.Empty);
            string template = "[color #d311ea][AO]((-adminLevel- -userName-: -userMessage-))";
            string setname = Regex.Replace(template, "-userName-", Arguments.argUser.displayName);
            string setadmin = Regex.Replace(setname, "-adminLevel-", Globals.getAdminName(user));
            string final = Regex.Replace(setadmin, "-userMessage-", rem);
            if (strText == string.Empty)
            {
                LanguageComponent.LanguageComponent.SendSyntaxError(pl, "/ao <Texto>", "/ao <Text>");
            }
            else
            {
                foreach (RustPP.Data.Entities.User player in RustPP.Data.Globals.usersOnline)
                {
                    player.Player.SendClientMessage(final);
                }
            }
        }
    }
}
