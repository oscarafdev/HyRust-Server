using RustPP.Commands;
using RustPP.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.AuthComponent
{
    class FarmCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (AuthComponent.UserIsLogged(pl))
            {
                User user = Data.Globals.usersOnline.FindLast(x => x.Name == pl.Name);
                pl.SendClientMessage($"[color orange]--------[/color] {user.Name} | Farm [color orange]--------");
                pl.SendClientMessage($"[color orange]- Leñador Nivel :[/color] {user.LumberjackLevel} ({user.LumberjackExp}/{user.LumberjackLevel * 100})");
                pl.SendClientMessage($"[color orange]- Minero Nivel :[/color] {user.MinerLevel} ({user.MinerExp}/{user.MinerLevel*100})");
                pl.SendClientMessage($"[color orange]- Cazador Nivel :[/color] {user.HunterLevel} ({user.HunterExp}/{user.HunterLevel*30})");
                pl.SendClientMessage($"[color orange]- Madera Obtenida :[/color] {user.WoodFarmed}");
                pl.SendClientMessage($"[color orange]- Metal Obtenido :[/color] {user.MetalFarmed}");
                pl.SendClientMessage($"[color orange]- Azufre Obtenido :[/color] {user.SulfureFarmed}");
                pl.SendClientMessage($"[color orange]----------------------------------------------------");
            }
            else
            {
                pl.SendClientMessage("[color red]<Error>[/color] Primero debes estar conectado (Utiliza [color orange] /login[/color])");
            }

        }
    }
}
