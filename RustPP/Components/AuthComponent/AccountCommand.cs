using System;
using System.Diagnostics;
using System.Timers;
using Fougerite;

namespace RustPP.Components.AuthComponent
{
    using RustPP.Commands;
    using RustPP.Data.Entities;
    using System.Text.RegularExpressions;

    public class AccountCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if(AuthComponent.UserIsLogged(pl))
            {
                User user = Data.Globals.usersOnline.FindLast(x => x.Name == pl.Name);
                pl.SendClientMessage($"[color orange]--------[/color] {user.Name} | Cuenta [color orange]--------");
                pl.SendClientMessage($"[color orange]- Nivel :[/color] {user.Level}");
                pl.SendClientMessage($"[color orange]- Experiencia :[/color] {user.Exp}/{user.Level * 8}");
                pl.SendClientMessage($"[color orange]- Kills :[/color] {user.Kills}");
                pl.SendClientMessage($"[color orange]- Deaths :[/color] {user.Deaths}");
                pl.SendClientMessage($"[color orange]- Dinero :[/color] $ {user.Cash}");
                pl.SendClientMessage($"[color orange]- HyCoins :[/color] $ 0");
                pl.SendClientMessage($"[color orange]- Clan :[/color] Ninguno");
                pl.SendClientMessage($"[color orange]----------------------------------------------------");
            } else
            {
                pl.SendClientMessage("[color red]<Error>[/color] Primero debes estar conectado (Utiliza [color orange] /login[/color])");
            }
            
        }
    }
}
