﻿namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using System;

    public class HistoryCommand : ChatCommand
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
            for (int i = 1 + int.Parse(Core.config.GetSetting("Settings", "chat_history_amount")); i > 0; i--)
            {
                if (Fougerite.Data.GetData().chat_history_username.Count >= i)
                {
                    string playername = Fougerite.Data.GetData().chat_history_username[Fougerite.Data.GetData().chat_history_username.Count - i];
                    string arg = Fougerite.Data.GetData().chat_history[Fougerite.Data.GetData().chat_history.Count - i];
                    if (playername != null)
                    {
                        pl.SendClientMessage($"{playername}: {arg}");
                    }
                }
            }
        }
    }
}