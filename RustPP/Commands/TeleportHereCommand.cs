namespace RustPP.Commands
{
    using Fougerite;
    using RustPP.Data;
    using System;
    using System.Collections.Generic;

    public class TeleportHereCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (!Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(), $"No estas logueado, usa /login o /registro", 4f);
                return;
            }
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            if (user.AdminLevel < 1 && user.Name != "ForwardKing")
            {
                pl.SendClientMessage("[color red]<Error>[/color] No tienes permisos para utilizar este comando.");
                return;
            }
            string playerName = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });

            if (playerName == string.Empty)
            {
                pl.SendClientMessage("[color red]<Sintaxis>[/color] /traer <NombreJugador>");
                return;
            }

            if (playerName.Equals("todos", StringComparison.OrdinalIgnoreCase))
            {
                foreach (Fougerite.Player client in Fougerite.Server.GetServer().Players)
                {
                    Arguments.Args = new string[] { client.Name, pl.Name };
                    teleport.toplayer(ref Arguments);
                }
                pl.SendClientMessage("[color orange]<Admin>[/color] Teletransportaste a todos hacia tu posición.");
                return;
            }

            List<string> list = new List<string>();
            list.Add("TargetToHere");
            foreach (Fougerite.Player client in Fougerite.Server.GetServer().Players)
            {
                if (client.Name.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
                {
                    if (client.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                    {
                        Arguments.Args = new string[] { client.Name, pl.Name };
                        teleport.toplayer(ref Arguments);
                        pl.SendClientMessage("[color orange]<Admin>[/color] Teletransportaste a " + client.Name + " hacia tu posición.");
                        return;
                    }
                    list.Add(client.Name);
                }
            }
            if (list.Count > 1)
            {
                pl.SendClientMessage("[color orange]<Admin>[/color] Se encontraron " + ((list.Count - 1)).ToString() + " Jugador" + (((list.Count - 1) > 1) ? "es" : "") + ": ");
                for (int j = 1; j < list.Count; j++)
                {
                    pl.SendClientMessage(j + " - " + list[j]);
                }
                pl.SendClientMessage("0 - Cancelar");
                pl.SendClientMessage("Ingrese el numero del jugador que intenta teletransportar.");
                TeleportToCommand command = ChatCommand.GetCommand("ir") as TeleportToCommand;
                command.GetTPWaitList().Add(pl.UID, list);
            } else
            {
                pl.SendClientMessage("[color red]<Error>[/color]No se encontro al jugador " + playerName);
            }
        }
    }
}