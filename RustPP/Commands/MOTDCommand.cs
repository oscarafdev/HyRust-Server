﻿namespace RustPP.Commands
{
    using RustPP;
    using System;

    internal class MOTDCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            Core.motd(Fougerite.Server.Cache[Arguments.argUser.userID]);
        }
    }
}