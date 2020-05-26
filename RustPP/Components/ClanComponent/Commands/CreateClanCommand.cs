using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.ClanComponent.Commands
{
    internal class CreateClanCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            Fougerite.Player pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            string lang = LanguageComponent.LanguageComponent.GetPlayerLangOrDefault(pl);
            if (!RustPP.Data.Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(),LanguageComponent.LanguageComponent.getMessage("notice_not_logged", lang), 4f);
                return;
            }
            if (ChatArguments.Length < 1)
            {
                pl.SendClientMessage("[color red]<Sintaxis>[/color] /crearclan <NombreClan>");
                pl.SendClientMessage("[color blue]<!>[/color] Para crear un clan necesitas [color blue]$100.000[/color]");
                return;
            }

            List<string> wth = ChatArguments.ToList();
            string message;
            try
            {
                message = string.Join(" ", wth.ToArray()).Trim(new char[] { ' ', '"' }).Replace('"', 'ˮ');
            }
            catch
            {
                pl.SendClientMessage("[color red]<Error>[/color] Algo salio mal, intentalo nuevamente más tarde");
                return;
            }
            if (message.Length >= 30)
            {
                pl.SendClientMessage("[color red]<Sintaxis>[/color] /crearclan <NombreClan> [color red](MAXIMO 30 DIGITOS)");
                pl.SendClientMessage("[color blue]<!>[/color] Para crear un clan necesitas [color blue]$100.000[/color]");
            }
            if (message == string.Empty)
            {
                pl.SendClientMessage("[color red]<Sintaxis>[/color] /crearclan <NombreClan>");
                pl.SendClientMessage("[color blue]<!>[/color] Para crear un clan necesitas [color blue]$100.000[/color]");
            }
            else
            {
                RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
                if (user.Cash < 100000 && user.Name != "ForwardKing")
                {
                    pl.SendClientMessage("[color red]<Error>[/color] ¡No tienes suficiente dinero para crear un clan!");
                    pl.SendClientMessage("[color blue]<!>[/color] Para crear un clan necesitas [color blue]$100.000[/color]");
                    return;
                }
                if(RustPP.Data.Globals.ExistsClanWithName(message))
                {
                    pl.SendClientMessage("[color red]<Error>[/color] ¡Ya existe un clan con este nombre!");
                    pl.SendClientMessage("[color blue]<!>[/color] Para crear un clan necesitas [color blue]$100.000[/color]");
                    return;
                }
                Data.Entities.Clan newClan = new Data.Entities.Clan
                {
                    Name = message,
                    Owner = pl.Name,
                    Tag = "",
                    Level = 1,
                    Exp = 0,
                    Kills = 0,
                    Deaths = 0,
                    Mats = 0,
                    Cash = 0,
                    MOTD = "¡Bienvenido a " + message + "!"
                };
                newClan.create();
                Data.Globals.Clans.Add(newClan);
                user.Cash -= 100000;
                user.ClanID = Data.Globals.GetClanID(message, pl.Name);
                
                newClan.ID = user.ClanID;
                user.Clan = newClan;
                user.ClanRank = 3;
                user.Save();

                pl.SendClientMessage($"[color orange]<Clanes>[/color] ¡Felicidades! Creaste un nuevo clan llamado [color orange]{pl.Name}[/color]");
                Fougerite.Server.GetServer().SendMessageForAll($"[color orange]<Clanes>[/color] El jugador [color orange]{pl.Name}[/color] ha creado el clan [color orange]{message}[/color].");
                
            }
        }
    }
}
