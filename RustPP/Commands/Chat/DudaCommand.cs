

namespace RustPP.Commands.Chat
{
    using Fougerite;
    using RustPP;
    using RustPP.Components.LanguageComponent;
    using RustPP.Data;
    using System;
    using System.Text.RegularExpressions;
    using static ConsoleSystem;

    public class GeneralChatCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            string lang = LanguageComponent.GetPlayerLangOrDefault(pl);
            if (!Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(),LanguageComponent.getMessage("notice_not_logged", lang), 4f);
                return;
            }
            if (user.TimeToChat >= 1)
            {
                pl.SendClientMessage($"[color red]<Error> [color white]Espera {user.TimeToChat} para enviar otro mensaje.");
                return;
            }
            if (user.Muted == 1)
            {
                pl.SendClientMessage("[color red]<Error> [color white]Estás muteado, no puedes hablar.");
                return;
            }
            string strText = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            string rem = Regex.Replace(strText, @"\[/?color\b.*?\]", string.Empty);
            string template = "[color #2a6de899][OOC]((-userName-: -userMessage-))";
            if(user.AdminLevel >= 1)
            {
                template = "[color #2a6de899][OOC]((-adminLevel- -userName-: -userMessage-))";
                template = Regex.Replace(template, "-adminLevel-", Globals.getAdminName(user));
            }
            string setname = Regex.Replace(template, "-userName-", Arguments.argUser.displayName);
            string final = Regex.Replace(setname, "-userMessage-", rem);
            if (strText == string.Empty)
            {
                pl.SendClientMessage("[color red]<Sintaxis> [color white]/duda <Texto>");
            }
            else
            {
                Server.GetServer().SendMessageForAll(final);
                user.TimeToChat += 5;
            }
        }
    }
}
