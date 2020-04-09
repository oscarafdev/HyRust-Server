using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.EconomyComponent.Commands
{
    class ToggleShopCommand : ChatCommand
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
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            if(user.TiendaEnabled)
            {
                user.TiendaEnabled = false;
                pl.SendClientMessage("[color yellow]<!>[/color] Desactivaste los mensajes de la tienda.");
            } else
            {
                user.TiendaEnabled = true;
                pl.SendClientMessage("[color yellow]<!>[/color] Activaste los mensajes de la tienda.");
            }
        }
    }
}
