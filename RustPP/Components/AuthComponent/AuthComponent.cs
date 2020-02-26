
using Fougerite;
using Fougerite.Events;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RustPP.Components.AuthComponent
{
    class AuthComponent
    {
        public static void Init()
        {
            Fougerite.Hooks.OnPlayerGathering += OnPlayerGathering;
            Fougerite.Hooks.OnPlayerConnected += OnPlayerConnect;
            Fougerite.Hooks.OnPlayerDisconnected += OnPlayerDisconnected;
            Fougerite.Hooks.OnPlayerSpawned += OnPlayerSpawned;
        }
        static void OnPlayerSpawned(Fougerite.Player player, SpawnEvent se)
        {
            player.Character.AttentionMessage("HOLA, ESTO ES UNA PRUEBA");
            player.Character.SendMessage("HOLA, ESTO ES UNA PRUEBA");
            player.Character.ObliviousMessage("HOLA, ESTO ES UNA PRUEBA");
        }
        static void OnPlayerConnect(Fougerite.Player player)
        {
            if (CheckIfUserIsRegistered(player))
            {
                player.SendClientMessage($"Bienvenido a [color orange]Rainbow Rust[color white], para ingresar utiliza [color blue]/login <Contraseña>");
            }
            else
            {
                player.SendClientMessage($"Bienvenido a [color orange]Rainbow Rust[color white], para registrarte utiliza [color blue]/registro <Contraseña> <Confirmar Contraseña>");
            }
        }
        public static bool CheckIfUserIsRegistered(Fougerite.Player player)
        {
            using (MySqlConnection connection = new MySqlConnection(Data.Database.Connection.GetConnectionString()))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM users WHERE username = @username";
                command.Parameters.AddWithValue("@username", player.Name);
                MySqlDataReader reader = command.ExecuteReader();
                
                if (reader.HasRows)
                {
                    connection.Close();
                    return true;
                }
                else
                {
                    connection.Close();
                    return false;
                }
            }
        }
        public static string GetUserSalt(Fougerite.Player player)
        {
            using (MySqlConnection connection = new MySqlConnection(Data.Database.Connection.GetConnectionString()))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM users WHERE username = @username";
                command.Parameters.AddWithValue("@username", player.Name);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    //connection.Close();
                    return reader.GetString("salt");
                }
                else
                {
                    connection.Close();
                    return "";
                }
            }
        }
        public static void LoginPlayer(Fougerite.Player player, string username, string password)
        {
            if(UserIsLogged(player))
            {
                player.SendClientMessage("[color red]<Error>[color white] Ya te encuentras logueado.");
                return;
            }
            if (!CheckIfUserIsRegistered(player))
            {
                player.SendClientMessage("[color red]<Error>[color white] Este usuario no se encuentra registrado en la base de datos, usa /registro.");
                return;
            }
            
            using (MySqlConnection connection = new MySqlConnection(Data.Database.Connection.GetConnectionString()))
            {

                connection.Open();
                string pEncrypt = BCrypt.Net.BCrypt.HashPassword(password, GetUserSalt(player));
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM users WHERE username = @username AND password = @password";
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", pEncrypt);
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    Fougerite.Player newplayer = Fougerite.Player.Search(reader.GetString("steamId"));
                    player.OnConnect(newplayer.NetUser);
                    Data.Entities.User newUser = new Data.Entities.User { Name = reader.GetString("username") };
                    Data.Globals.usersOnline.Add(newUser);
                    connection.Close();
                    player.SendClientMessage($"¡Bienvenido! [color green]{player.Name}[color white] - Nivel [color green]4");
                    player.SendClientMessage($"Si tienes dudas utiliza [color blue]/ayuda[color white] o escribe tu duda por el canal [color blue]/duda");

                }
                else
                {
                    connection.Close();
                    player.SendClientMessage($"[color red] <Error> [color white]Los datos ingresados son incorrectos, utiliza [color blue]/login[color white] para intentarlo nuevamente.");
                    return;
                }
            }
        }
        public static bool UserIsLogged(Fougerite.Player player)
        {
            if (Data.Globals.usersOnline.Count(x => x.Name == player.Name) >= 1)
            {
                return true;
            }
            else {
                return false;
            }


        }
        public static void RegisterPlayer(Fougerite.Player player, string password, string confirmpassword)
        {
            if (UserIsLogged(player))
            {
                player.SendClientMessage("[color red]<Error>[color white] Ya te encuentras logueado.");
                return;
            }
            if(CheckIfUserIsRegistered(player))
            {
                player.SendClientMessage("[color red]<Error>[color white] Este usuario ya esta registrado en la base de datos, intenta cambiando tu nombre de usuario.");
            }
            else
            {
                if(password == confirmpassword)
                {
                    using (MySqlConnection connection = new MySqlConnection(Data.Database.Connection.GetConnectionString()))
                    {

                        connection.Open();
                        MySqlCommand command = connection.CreateCommand();
                        string salt = BCrypt.Net.BCrypt.GenerateSalt();
                        string pEncrypt = BCrypt.Net.BCrypt.HashPassword(password, salt);
                        command.CommandText = "INSERT INTO users (username, password, salt, ip, steamId) VALUES (@username, @password, @salt, @ip, @steamid)";
                        command.Parameters.AddWithValue("@username", player.Name);
                        command.Parameters.AddWithValue("@password", pEncrypt);
                        command.Parameters.AddWithValue("@salt", salt);
                        command.Parameters.AddWithValue("@ip", player.IP);
                        command.Parameters.AddWithValue("@steamid", player.UID);
                        MySqlDataReader reader = command.ExecuteReader();
                        player.SendClientMessage("Te registraste correctamente");
                        Data.Entities.User newUser = new Data.Entities.User { Name = player.Name };
                        Data.Globals.usersOnline.Add(newUser);

                    }
                }
                else
                {
                    player.SendClientMessage("[color red]<Error>[color white] Las contraseñas no coinciden, intentalo nuevamente.");
                }
            }
            
        }

        static void OnPlayerDisconnected(Fougerite.Player player)
        {
            Data.Globals.usersOnline.RemoveAll(x => x.Name == player.Name);
        }
        static void OnPlayerGathering(Fougerite.Player player, GatherEvent ge)
        {
            
        }
    }
}
