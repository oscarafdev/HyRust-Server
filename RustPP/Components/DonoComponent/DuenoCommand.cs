using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.DonoComponent
{
    class DuenoCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (RustPP.Components.AuthComponent.AuthComponent.UserIsLogged(pl))
            {
                Data.Entities.User user = Data.Globals.GetInternalUser(pl);
                user.SpectingOwner = true;
                pl.SendClientMessage("Golpea una estructura para saber quien es el dueño");
            }
            else
            {
                pl.SendClientMessage("[color red]<Error>[/color] Debes estar logueado para utilizar este comando.");
                return;
            }

        }

    }
}
