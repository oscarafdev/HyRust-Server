namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using RustPP.Components.LanguageComponent;
    using System;

    public class AboutCommand : ChatCommand
    {
        /*DEPRECATED*/
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
            pl.SendClientMessage("[color orange]HyAxe Rust v" + Core.Version);
            pl.SendClientMessage("Desarrollado por [color orange]FR0Z3NH34R7");
        }
    }
}