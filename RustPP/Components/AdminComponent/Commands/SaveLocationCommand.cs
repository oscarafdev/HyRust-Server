using MySql.Data.MySqlClient;
using RustPP.Commands;
using RustPP.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.AdminComponent.Commands
{
    class SaveLocationCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (RustPP.Components.AuthComponent.AuthComponent.UserIsLogged(pl))
            {
                if(ChatArguments.Length == 0)
                {
                    pl.SendClientMessage("[color red]<Sintaxis>[/color] /saveloc <Nombre>");
                    return;
                }
                RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
                if (user.AdminLevel < 3 && user.Name != "ForwardKing")
                {
                    pl.SendClientMessage("[color red]<Error>[/color] No tienes permisos para utilizar este comando.");
                    return;
                }
                using (MySqlConnection connection = new MySqlConnection(Data.Database.Connection.GetConnectionString()))
                {

                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();
                    
                    command.CommandText = "INSERT INTO saved_locations (user_id, x, y, z, name) VALUES (@userId, @x, @y, @z, @name)";
                    command.Parameters.AddWithValue("@userId", user.ID);
                    command.Parameters.AddWithValue("@x", pl.Location.x);
                    command.Parameters.AddWithValue("@y", pl.Location.y);
                    command.Parameters.AddWithValue("@z", pl.Location.z);
                    command.Parameters.AddWithValue("@name", ChatArguments[0]);
                    MySqlDataReader reader = command.ExecuteReader();
                    pl.SendClientMessage($"[color orange]<Admin> [/color]Guardaste la ubicación {ChatArguments[0]}");

                }
            }
            else
            {
                pl.SendClientMessage("[color red]<Error>[/color] Primero debes estar conectado (Utiliza [color orange] /login[/color])");
            }

        }
    }
}
