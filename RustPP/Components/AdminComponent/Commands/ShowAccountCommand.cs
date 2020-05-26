using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.AdminComponent.Commands
{
    class ShowAccountCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            string lang = LanguageComponent.LanguageComponent.GetPlayerLangOrDefault(pl);
            if (!Data.Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(),LanguageComponent.LanguageComponent.getMessage("notice_not_logged", lang), 4f);
                return;
            }
            RustPP.Data.Entities.User MyUser = RustPP.Data.Globals.GetInternalUser(pl);
            if (MyUser.AdminLevel < 1 && MyUser.Name != "ForwardKing")
            {
                pl.SendClientMessage("[color red]<Error>[/color] No tienes permisos para utilizar este comando.");
                return;
            }
            if (ChatArguments.Length < 1)
            {
                pl.SendClientMessage("[color red]<Sintaxis>[/color] /vercuenta <NombreJugador>");
                return;
            }
            string search = ChatArguments[0];
            Fougerite.Player recipient = Fougerite.Player.FindByName(search);
            if (recipient == null)
            {
                pl.SendClientMessage($"[color red]<Error>[/color] No se encontró al usuario {search}");
                return;
            }
            if (!Data.Globals.UserIsLogged(recipient))
            {
                pl.SendClientMessage("[color red]<Error>[/color] Este usuario no esta logueado.");
                return;
            }
            RustPP.Data.Entities.User user = Data.Globals.GetInternalUser(recipient);
            pl.SendClientMessage($"[color orange]--------[/color] {user.Name} | Cuenta [color orange]--------");
            pl.SendClientMessage($"[color orange]- Nivel :[/color] {user.Level} ({user.Exp}/{user.Level * 8})");
            if (user.AdminLevel >= 1)
            {
                pl.SendClientMessage($"[color orange]- Admin :[/color] {user.AdminLevel}");
            }
            pl.SendClientMessage($"[color orange]- Kills / Deaths :[/color] {user.Kills} / {user.Deaths}");
            pl.SendClientMessage($"[color orange]- Dinero :[/color] $ {user.Cash}");
            pl.SendClientMessage($"[color orange]- HyCoins :[/color] $ 0");
            if (user.ClanID != -1)
            {
                pl.SendClientMessage($"[color orange]- Clan :[/color] {user.Clan.Name}");
            }
            else
            {
                pl.SendClientMessage($"[color orange]- Clan :[/color] Ninguno");
            }
            pl.SendClientMessage($"[color orange]- Leñador Nivel :[/color] {user.LumberjackLevel} ({user.LumberjackExp}/{user.LumberjackLevel * 100})");
            pl.SendClientMessage($"[color orange]- Minero Nivel :[/color] {user.MinerLevel} ({user.MinerExp}/{user.MinerLevel * 100})");
            pl.SendClientMessage($"[color orange]- Cazador Nivel :[/color] {user.HunterLevel} ({user.HunterExp}/{user.HunterLevel * 30})");
            pl.SendClientMessage($"[color orange]- Madera Obtenida :[/color] {user.WoodFarmed}");
            pl.SendClientMessage($"[color orange]- Metal Obtenido :[/color] {user.MetalFarmed}");
            pl.SendClientMessage($"[color orange]- Azufre Obtenido :[/color] {user.SulfureFarmed}");
            pl.SendClientMessage($"[color orange]----------------------------------------------------");

        }
    }
}
