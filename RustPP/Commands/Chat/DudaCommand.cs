

namespace RustPP.Commands.Chat
{
    using Fougerite;
    using RustPP;
    using RustPP.Data;
    using System;
    using System.Text.RegularExpressions;
    using static ConsoleSystem;

    public class DudaCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            if (!Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(), $"No estas logueado, usa /login o /registro", 4f);
                return;
            }
            if(user.Muted == 1)
            {
                pl.SendClientMessage("[color red]<Error> [color white]Estás muteado, no puedes hablar.");
                return;
            }
            string strText = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            string rem = Regex.Replace(strText, @"\[/?color\b.*?\]", string.Empty);
            string template = "[color #2a6de899][OOC]((-userName-: -userMessage-))";
            string setname = Regex.Replace(template, "-userName-", Arguments.argUser.displayName);
            string final = Regex.Replace(setname, "-userMessage-", rem);
            if (strText == string.Empty)
            {
                pl.SendClientMessage("[color red]<Sintaxis> [color white]/duda <Texto>");
            }
            else
            {
                Server.GetServer().SendMessageForAll(final);
            }
        }
    }
}
