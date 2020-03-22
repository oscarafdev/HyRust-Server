namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using System;
    using System.Text.RegularExpressions;

    public class ShoutCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (!RustPP.Data.Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(), $"No estas logueado, usa /login o /registro", 4f);
                return;
            }
            string strText = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            string rem = Regex.Replace(strText, @"\[/?color\b.*?\]", string.Empty);
            string template = "-userName- grita: ¡-userMessage-!";
            string setname = Regex.Replace(template, "-userName-", Arguments.argUser.displayName);
            string final = Regex.Replace(setname, "-userMessage-", rem);
            if (strText == string.Empty)
            {
                pl.SendClientMessage("[color red]<Sintaxis> [color white]/g <Texto>");
            }
            else
            {
                pl.SendMessageToNearUsers(final, 80.0f);
            }
        }
    }
}