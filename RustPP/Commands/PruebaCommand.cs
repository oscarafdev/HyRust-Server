
namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using System;

    public class PruebaCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            pl.SendClientMessage("Holis" + Core.Version);
            pl.MessageFrom(Core.Name, "Brought to you by xEnt & EquiFox17 & the Fougerite project.");
            pl.SendCommand("chat.add " + "\\n" + " " + "Hola");
            pl.SendCommand("chat.add " + "\n" + " " + "Hola2");
        }
    }
}
