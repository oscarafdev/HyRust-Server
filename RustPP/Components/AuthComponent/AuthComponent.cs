
using Fougerite;
using Fougerite.Events;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Converters;
using RustPP.Data.Entities;
using UnityEngine;

namespace RustPP.Components.AuthComponent
{
    class AuthComponent
    {
        public readonly System.Random Randomizer = new System.Random();
        public readonly Dictionary<int, string> RewardList = new Dictionary<int, string>()
        {
            {1, "Armor Part 1"},
            {2, "Armor Part 2"},
            {3, "Armor Part 3"},
            {4, "Armor Part 4"},
            {5, "Armor Part 5"},
            {6, "Armor Part 6"},
            {7, "Armor Part 7"},
            {8, "Weapon Part 1"},
            {9, "Weapon Part 2"},
            {10, "Weapon Part 3"},
            {11, "Weapon Part 4"},
            {12, "Weapon Part 5"},
            {13, "Weapon Part 6"},
            {14, "Weapon Part 7"}
        };
        public static void Init()
        {

            Fougerite.Hooks.OnPlayerGathering += OnPlayerGathering;
            Fougerite.Hooks.OnPlayerKilled += OnPlayerKilled;
            Fougerite.Hooks.OnPlayerConnected += OnPlayerConnect;
            Fougerite.Hooks.OnPlayerDisconnected += OnPlayerDisconnected;
            Fougerite.Hooks.OnPlayerSpawned += OnPlayerSpawned;
            Fougerite.Hooks.OnPlayerMove += OnPlayerMove;
        }
        static void OnPlayerMove(HumanController hc, Vector3 origin, int encoded, ushort stateFlags, uLink.NetworkMessageInfo info, Util.PlayerActions action)
        {

            /*Fougerite.Player player = Fougerite.Server.GetServer().FindByNetworkPlayer(info.sender);
            if(action != Util.PlayerActions.Standing)
            {
                player.SendClientMessage($"[color yellow][Debug][/color] {action}");
            }*/
            
        }
        static void OnPlayerKilled(DeathEvent de)
        {
            if(de.AttackerIsPlayer && de.Attacker != null && de.VictimIsPlayer && de.Victim != null)
            {
                Fougerite.Player attacker = (Fougerite.Player) de.Attacker;
                Fougerite.Player victim = (Fougerite.Player) de.Victim;
                if(attacker != null && victim !=null)
                {
                    User userAttacker = Data.Globals.usersOnline.Find(x => x.Name == attacker.Name);
                    User userVictim = Data.Globals.usersOnline.Find(x => x.Name == victim.Name);
                    if (userAttacker != null && userVictim != null)
                    {
                        if(userAttacker.LastKilled != userVictim.Name)
                        {
                            userAttacker.GiveExp(1 * userVictim.Level);
                            int expLost = (userVictim.Exp * 30) / 100;
                            victim.SendClientMessage($"Te mataron, perdiste {expLost} Exp");
                            userVictim.TakeExp(expLost);
                            userAttacker.Kills += 1;
                            userVictim.Deaths += 1;
                            userAttacker.LastKilled = userVictim.Name;
                            userAttacker.Save();
                            userVictim.Save();
                        }
                        
                    }
                }
            }
        }

        static void OnPlayerSpawned(Fougerite.Player player, SpawnEvent se)
        {
            if (!UserIsLogged(player))
            {
                if (CheckIfUserIsRegistered(player))
                {
                    player.SendClientMessage($"Bienvenido a [color orange]HyAxe Rust[color white], para ingresar utiliza [color blue]/login <Contraseña>");
                }
                else
                {
                    player.SendClientMessage($"Bienvenido a [color orange]HyAxe Rust[color white], para registrarte utiliza [color blue]/registro <Contraseña> <Confirmar Contraseña>");
                }
                Character character = player.PlayerClient.controllable.GetComponent<Character>();
                character.lockMovement = false;
                character.lockLook = false;
            }
        }
        static void OnPlayerConnect(Fougerite.Player player)
        {
            
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

                    User newUser = new User
                    {
                        Name = player.Name,
                        Level = reader.GetInt32("level"),
                        Exp = reader.GetInt32("exp"),
                        Kills = reader.GetInt32("kills"),
                        Deaths = reader.GetInt32("deaths"),
                        Cash = reader.GetInt32("cash"),
                        LastKilled = reader.GetString("lastKilled"),
                        MinerLevel = reader.GetInt32("minerLevel"),
                        MinerExp = reader.GetInt32("minerExp"),
                        LumberjackLevel = reader.GetInt32("lumberjackLevel"),
                        LumberjackExp = reader.GetInt32("lumberjackExp"),
                        WoodFarmed = reader.GetInt32("woodFarmed"),
                        MetalFarmed = reader.GetInt32("metalFarmed"),
                        SulfureFarmed = reader.GetInt32("sulfureFarmed"),
                        AdminLevel = reader.GetInt32("adminLevel"),
                        Player = player
                    };
                    Data.Globals.usersOnline.Add(newUser);
                    player.SendClientMessage($"¡Bienvenido! [color orange]{player.Name}[color white] - Nivel [color orange]{newUser.Level}");
                    if (newUser.AdminLevel >= 1)
                    {
                        player.SendClientMessage($"[color orange]- Admin :[/color] {newUser.AdminLevel}");
                    }
                    player.SendClientMessage($"Si tienes dudas utiliza [color blue]/ayuda[color white] o escribe tu duda por el canal [color blue]/duda");
                    connection.Close();
                    LoadPlayer(player);

                }
                else
                {
                    connection.Close();
                    player.SendClientMessage($"[color red] <Error> [color white]Los datos ingresados son incorrectos, utiliza [color blue]/login[color white] para intentarlo nuevamente.");
                    return;
                }
            }
        }
        public static void LoadPlayer(Fougerite.Player player)
        {

            Character character = player.PlayerClient.controllable.GetComponent<Character>();
            character.lockMovement = false;
            character.lockLook = false;
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
                        player.SendClientMessage("¡Bienvenido a HyAxe Rust! Este servidor es algo diferente a los demás.");
                        player.SendClientMessage("Para informarte utiliza el comando [color orange]/ayuda[color white] o [color orange]/duda[color]");
                        LoginPlayer(player, player.Name, password);


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
            User user = Data.Globals.usersOnline.Find(x => x.Name == player.Name);
            if(!UserIsLogged(player))
            {
                char ch = '☢';
                player.Notice(ch.ToString(), $"No estas logueado, usa /login o /registro", 4f);
                ge.Quantity = 0;
                return;
            }
            
            if (ge.Item == "Wood")
            {
                ge.Quantity = 5*user.LumberjackLevel;
                AddWoodExp(player, ge.Quantity);
            }
            if(ge.Item == "Metal Ore")
            {

            }
            if (ge.Item == "Sulfur Ore")
            {

            }
            player.SendClientMessage($"{ge.Item} x {ge.Quantity}");
        }

        static void AddWoodExp(Fougerite.Player player, int quantity)
        {
            User user = Data.Globals.usersOnline.Find(x => x.Name == player.Name);
            if(user != null)
            {
                user.WoodFarmed += quantity;

                user.LumberjackExp += 1;
                if(user.LumberjackExp >= user.LumberjackLevel * 100)
                {
                    char cha = '♜';
                    user.LumberjackLevel += 1;
                    user.LumberjackExp -= user.LumberjackLevel * 100;
                    player.Notice(cha.ToString(), $"Subiste a nivel {user.LumberjackLevel} (Leñador)", 3f);
                } else
                {
                    char ch = '♜';
                    player.Notice(ch.ToString(), $"+ 1 Exp (Leñador)", 3f);
                    player.InventoryNotice($"+ {quantity} Madera (Leñador)");
                }
                user.Save();
                
            }
        }
    }
}
