using Fougerite;
using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.AdminComponent.Commands
{
    class DayCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (RustPP.Components.AuthComponent.AuthComponent.UserIsLogged(pl))
            {
                RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
                if (user.AdminLevel < 2 && user.Name != "ForwardKing")
                {
                    pl.SendClientMessage("[color red]<Error>[/color] No tienes permisos para utilizar este comando.");
                    return;
                }
                World.GetWorld().Time = 6f;
            }
            else
            {
                pl.SendClientMessage("[color red]<Error>[/color] Primero debes estar conectado (Utiliza [color orange] /login[/color])");
            }

        }
    }
    //

}
