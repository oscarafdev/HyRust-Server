using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components
{
    internal class ErrorCommand : ChatCommand
    {
        public string NewCommand = "";
        public ErrorCommand(string command) => NewCommand = command;
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            Fougerite.Player pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (!RustPP.Data.Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(), $"No estas logueado, usa /login o /registro", 4f);
                return;
            }
            pl.SendClientMessage($"[color red]¡Ups![/color] Parece que este comando ya no existe, intente con el comando [color cyan]{NewCommand}[/color].");
        }
    }
}
