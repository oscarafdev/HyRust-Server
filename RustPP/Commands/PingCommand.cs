namespace RustPP.Commands
{
    using Fougerite;
    using System;

    public class PingCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            int ping = pl.Ping - 100;
            pl.MessageFrom(Core.Name, "Ping: " + ping+" ms");
        }
    }
}