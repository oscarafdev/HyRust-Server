using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.ClanComponent.Commands
{
    class ClanCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            Fougerite.Player pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (!RustPP.Data.Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(), $"No estas logueado, usa /login o /registro", 4f);
                return;
            }
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            if(user.ClanID == -1)
            {
                pl.SendClientMessage($"[color red]<!>[/color] ¡No tienes clan!");
                return;
            }
            if (ChatArguments.Length < 1)
            {
                pl.SendClientMessage("-[color blue] /clan stats [/color] Ver las estadisticas del clan");
                pl.SendClientMessage("-[color blue] /clan miembros [/color] Ver todos los miembros del clan conectados");
                if (user.ClanRank >= 2)
                {
                    pl.SendClientMessage("-[color blue] /clan invitar [/color] Invita a una persona al clan");
                    pl.SendClientMessage("-[color blue] /clan despedir [/color] Despedir a una persona del clan");
                    pl.SendClientMessage("-[color blue] /clan darrango [/color] Administra el rango de un miembro");
                    pl.SendClientMessage("-[color blue] /clan motd [/color] Establece un mensaje de bienvenida");
                }
                if(user.Clan.Owner == user.Name)
                {
                    pl.SendClientMessage("-[color blue] /clan darlider [/color] Entrega el liderazgo del clan a otra persona.");
                }
                pl.SendClientMessage("-[color blue] /clan salir [/color] Abandonar el clan");
                return;
            }
            string search = ChatArguments[0].ToLower();
            if(search == "stats")
            {
                pl.SendClientMessage($"---------------[color blue] {user.Clan.Name} [/color]---------------");
                pl.SendClientMessage($"-[color blue] MOTD: [/color]{user.Clan.MOTD}");
                pl.SendClientMessage($"-[color blue] Dueño: [/color]{user.Clan.Owner}");
                pl.SendClientMessage($"-[color blue] Nivel: [/color]{user.Clan.Level}");
                pl.SendClientMessage($"-[color blue] Exp: [/color]{user.Clan.Exp}");
                pl.SendClientMessage($"-[color blue] Materiales: [/color]{user.Clan.Mats}");
                pl.SendClientMessage($"-[color blue] Asesinatos: [/color]{user.Clan.Kills}");
                pl.SendClientMessage($"-[color blue] Muertes: [/color]{user.Clan.Deaths}");
                pl.SendClientMessage($"-[color blue] Banco: [/color]${user.Clan.Cash}");
                return;
            }
            if (search == "miembros")
            {
                pl.SendClientMessage($"[color blue] Miembros de {user.Clan.Name} [/color]");
                string miembros = "";
                foreach (RustPP.Data.Entities.User player in Data.Globals.usersOnline)
                {
                    
                    if (player.ClanID == user.ClanID)
                    {
                        miembros = miembros + " - " + player.Name;
                        
                    }

                }
                pl.SendClientMessage($"{miembros}");
                return;
            }
            if (search == "salir")
            {
                if(user.Clan.Owner == user.Name)
                {
                    pl.SendClientMessage($"[color red]<!>[/color] No puedes abandonar el clan {user.Clan.Name} ya que eres el dueño, usa [color blue]/clan darlider[/color]. ");
                    pl.SendClientMessage($"[color red]<!>[/color] Utiliza [color blue]/clan darlider[/color] para otorgar el liderazgo del clan {user.Clan.Name} a otra persona. ");
                    return;
                }
                pl.SendClientMessage($"[color red]<!>[/color] ¿Estás seguro de querer abandonar el clan {user.Clan.Name}? Usa: [color blue]/clan salir confirmar [/color].");
                string confirmar = ChatArguments[1].ToLower();
                if(confirmar == "confirmar")
                {
                    Data.Globals.SendMessageForClan(user.ClanID, $"El usuario [color red]{user.Name}[/color] abandonó el clan.");
                    user.ClanID = -1;
                    user.ClanRank = 0;
                    user.Save();
                }
                return;
            }
            if(search == "invitar")
            {
                if(ChatArguments.Length < 2)
                {
                    pl.SendClientMessage("[color red]<Sintaxis>[/color] /clan invitar <NombreJugador>");
                    return;
                }
                if (user.ClanRank == 1)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] Solo el lider o el encargado del clan pueden invitar a otros miembros.");
                    return;
                }
                string invite = ChatArguments[1];
                Fougerite.Player recipient = Fougerite.Player.FindByName(invite);
                if (!Data.Globals.UserIsLogged(recipient))
                {
                    pl.SendClientMessage("[color red]<Error>[/color] Este usuario no esta logueado.");
                    return;
                }
                RustPP.Data.Entities.User recipientUser = Data.Globals.GetInternalUser(recipient);
                if (recipient == null)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] No se encontró al usuario {invite}");
                    return;
                }
                if (recipientUser.Name == user.Name)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] No puedes usar este comando contigo.");
                    return;
                }
                if (recipientUser.InvitedClan == user.ClanID)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] Este usuario ya tiene una invitación para unirse a {recipientUser.Clan.Name}");
                    return;
                }
                if (recipientUser.ClanID != -1)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] Este usuario ya pertenece a un clan ({recipientUser.Clan.Name})");
                    return;
                }

                recipientUser.InvitedClan = user.ClanID;
                recipient.SendClientMessage($"[color purple]<!>[/color] El usuario [color purple]{user.Name}[/color] te invito a unirte a [color purple]{user.Clan.Name}[/color]");
                recipient.SendClientMessage($"[color purple]<!>[/color] Utiliza [color purple]/aceptar[/color] para aceptar la invitación.");
            }
            if (search == "despedir")
            {
                if (ChatArguments.Length < 2)
                {
                    pl.SendClientMessage("[color red]<Sintaxis>[/color] /clan despedir <NombreJugador>");
                    return;
                }
                if (user.ClanRank == 1)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] Solo el lider o el encargado del clan pueden despedir a un miembro.");
                    return;
                }
                string invite = ChatArguments[1];
                Fougerite.Player recipient = Fougerite.Player.FindByName(invite);
                if (!Data.Globals.UserIsLogged(recipient))
                {
                    pl.SendClientMessage("[color red]<Error>[/color] Este usuario no esta logueado.");
                    return;
                }
                RustPP.Data.Entities.User recipientUser = Data.Globals.GetInternalUser(recipient);
                if (recipient == null)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] No se encontró al usuario {invite}");
                    return;
                }
                if (recipientUser.ClanID == user.ClanID && recipientUser.ClanRank >= user.ClanRank && user.Clan.Owner != user.Name)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] No tienes permisos para despedir a {recipientUser.Name}");
                    return;
                }
                if (recipientUser.ClanID != user.ClanID)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] Este usuario no pertenece a tu clan.");
                    return;
                }
                if (recipientUser.Name == user.Name)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] No puedes usar este comando contigo.");
                    return;
                }
                Data.Globals.SendMessageForClan(user.ClanID, $"El usuario [color red]{user.Name}[/color] despidió a {recipientUser.Name} del clan.");
                recipientUser.ClanID = -1;
                recipientUser.ClanRank = 0;
                recipientUser.Save();
            }
            if (search == "motd")
            {
                if (ChatArguments.Length < 2)
                {
                    pl.SendClientMessage("[color red]<Sintaxis>[/color] /clan motd <Texto>");
                    return;
                }
                if (user.ClanRank == 3)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] Solo el lider o el encargado del clan pueden dar rango.");
                    return;
                }
                List<string> wth = ChatArguments.ToList();
                wth.Remove(wth[0]);
                string message;
                try
                {
                    message = string.Join(" ", wth.ToArray()).Replace(search, "").Trim(new char[] { ' ', '"' }).Replace('"', 'ˮ');
                }
                catch
                {
                    pl.SendClientMessage("[color red]<Error>[/color] Algo salio mal, intentalo nuevamente más tarde");
                    return;
                }
                if (message == string.Empty)
                {
                    pl.SendClientMessage("[color red]<Sintaxis>[/color] /reportar <NombreJugador> <Motivo>");
                }
                else
                {
                    user.Clan.MOTD = message;
                    pl.SendClientMessage("[color green]<!>[/color] Cambiaste el MOTD del clan.");
                    user.Clan.save();
                }
return;
            }
            if (search == "darrango")
            {
                if (ChatArguments.Length < 3)
                {
                    pl.SendClientMessage("[color red]<Sintaxis>[/color] /clan darrango <NombreJugador> <RANGO>");
                    pl.SendClientMessage("[color red]<Sintaxis>[/color] Rangos: 1 (miembro) - 2 (reclutador) - 3 (encargado)");
                    return;
                }
                if (user.ClanRank != 3 && user.Clan.Owner != user.Name)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] Solo el lider o el encargado del clan pueden dar rango.");
                    return;
                }
                string invite = ChatArguments[1];
                int rank = Int32.Parse(ChatArguments[2]);
                Fougerite.Player recipient = Fougerite.Player.FindByName(invite);
                if (!Data.Globals.UserIsLogged(recipient))
                {
                    pl.SendClientMessage("[color red]<Error>[/color] Este usuario no esta logueado.");
                    return;
                }
                RustPP.Data.Entities.User recipientUser = Data.Globals.GetInternalUser(recipient);
                if (recipient == null)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] No se encontró al usuario {invite}");
                    return;
                }
                if (recipientUser.Name == user.Name)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] No puedes usar este comando contigo.");
                    return;
                }
                if (recipientUser.ClanID == user.ClanID && recipientUser.ClanRank >= user.ClanRank && user.Clan.Owner != user.Name)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] No tienes permisos para cambiar el rango de {recipientUser.Name}");
                    return;
                }
                if(rank > user.ClanRank)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] No tienes permisos para asignar este rango");
                    return;
                }
                if (rank > 3)
                {
                    pl.SendClientMessage("[color red]<Sintaxis>[/color] /clan darrango <NombreJugador> <RANGO>");
                    pl.SendClientMessage("[color red]<Sintaxis>[/color] Rangos: 1 (miembro) - 2 (reclutador) - 3 (encargado)");
                    return;
                }
                if (recipientUser.ClanID != user.ClanID)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] Este usuario no pertenece a tu clan.");
                    return;
                }
                Data.Globals.SendMessageForClan(user.ClanID, $"El usuario [color purple]{user.Name}[/color] asignó a [color purple]{recipientUser.Name}[/color] el rango numero {rank}.");
                recipientUser.ClanRank = rank;
                recipientUser.Save();
            }
            if (search == "darlider")
            {
                if (ChatArguments.Length < 3)
                {
                    pl.SendClientMessage("[color red]<Sintaxis>[/color] /clan darlider <NombreJugador>");
                    return;
                }
                if (user.Name == user.Clan.Owner)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] Solo el lider puede usar este comando.");
                    return;
                }
                string invite = ChatArguments[1];
                Fougerite.Player recipient = Fougerite.Player.FindByName(invite);
                if (!Data.Globals.UserIsLogged(recipient))
                {
                    pl.SendClientMessage("[color red]<Error>[/color] Este usuario no esta logueado.");
                    return;
                }
                RustPP.Data.Entities.User recipientUser = Data.Globals.GetInternalUser(recipient);
                if (recipient == null)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] No se encontró al usuario {invite}");
                    return;
                }
                if (recipientUser.Name == user.Name)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] No puedes usar este comando contigo.");
                    return;
                }

                if (recipientUser.ClanID != user.ClanID)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] Este usuario no pertenece a tu clan.");
                    return;
                }
                Data.Globals.SendMessageForClan(user.ClanID, $"El usuario [color purple]{user.Name}[/color] asignó el liderazco del clan a [color purple]{recipientUser.Name}[/color].");
                user.Clan.Owner = recipientUser.Name;
                recipientUser.ClanRank = 3;
                recipientUser.Save();
            }
        }
    }
}
