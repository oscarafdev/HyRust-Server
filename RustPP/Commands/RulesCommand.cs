namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using RustPP.Components.LanguageComponent;
    using System;
    using System.IO;

    public class RulesCommand : ChatCommand
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
            if (File.Exists(RustPPModule.GetAbsoluteFilePath("rules.txt")))
            {
                foreach (string str in File.ReadAllLines(RustPPModule.GetAbsoluteFilePath("rules.txt")))
                {
                    pl.SendClientMessage(str);
                }
            }
            else
            {
                pl.SendClientMessage("Todavía no hay reglas establecidas en el servidor.");
            }
        }
    }
}