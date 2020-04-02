using Fougerite;
using MySql.Data.MySqlClient;
using RustPP.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace RustPP.Data
{
    class Globals
    {
        public static List<User> usersOnline = new List<User>();
        public static List<KitItem> KitItems = new List<KitItem>();
        public static List<Clan> Clans = new List<Clan>();
        public static List<Connections> IncommingConections = new List<Connections>();
        public static bool UserIsLogged(Fougerite.Player player)
        {
            if (Data.Globals.usersOnline.Count(x => x.Name == player.Name) >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool ExistsClanWithName(string name)
        {
            int count = Data.Globals.Clans.Count(x => x.Name == name);
            if (count >= 1)
            {
                return true;
            }
            return false;
        }
        public static bool ExistsClanWithTag(string name)
        {
            int count = Data.Globals.Clans.Count(x => x.Tag == name);
            if (count >= 1)
            {
                return true;
            }
            return false;
        }
        public static int GetClanID(string name, string owner)
        {
            using (MySqlConnection connection = new MySqlConnection(Data.Database.Connection.GetConnectionString()))
            {

                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM clans WHERE name = @name AND owner = @owner";
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@owner", owner);
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    return reader.GetInt32("id");
                }
                else
                {
                    connection.Close();
                    return -1;
                }
            }
        }
        public static void LoadClans()
        {
            using (MySqlConnection connection = new MySqlConnection(Data.Database.Connection.GetConnectionString()))
            {

                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM clans";
                MySqlDataReader reader = command.ExecuteReader();
                DataTable schemaTable = reader.GetSchemaTable();
                while (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Clan newClan = new Clan
                        {
                            ID = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            Owner = reader.GetString("owner"),
                            Tag = reader.GetString("tag"),
                            Level = reader.GetInt32("level"),
                            Exp = reader.GetInt32("exp"),
                            Kills = reader.GetInt32("kills"),
                            Deaths = reader.GetInt32("deaths"),
                            Mats = reader.GetInt32("mats"),
                            Cash = reader.GetInt32("cash"),
                            MOTD = reader.GetString("motd")
                        };
                        Data.Globals.Clans.Add(newClan);
                        Logger.Log($"Clan {newClan.Name} cargado.");
                    }
                    reader.NextResult();
                }
            }
        }
        public static void SendAdminMessageForAll(string message)
        {
            foreach(User user in Data.Globals.usersOnline)
            {
                if(user.AdminLevel >= 1)
                {
                    user.Player.SendClientMessage($"[color orange] {message}");
                }
                
            }
            Logger.LogDebug(message);
        }
        public static void SendMessageForClan(int clan, string message)
        {
            foreach (User user in Data.Globals.usersOnline)
            {
                if (user.ClanID >= clan)
                {
                    user.Player.SendClientMessage($"[color orange]<Clan>[/color] {message}");
                }

            }
            Logger.LogDebug(message);
        }
        public static void GuardarClanes()
        {
            foreach(RustPP.Data.Entities.Clan clan in Clans)
            {
                clan.save();
            }
        }
        public static string getAdminName(User user)
        {
            if(user.AdminLevel == 1)
            {
                return "Ayudante";
            }
            else if (user.AdminLevel == 2)
            {
                return "Moderador";
            }
            else if (user.AdminLevel == 3)
            {
                return "Mod. General";
            }
            else if (user.AdminLevel == 4)
            {
                return "Admin";
            }
            else if (user.AdminLevel == 5)
            {
                return "Encargado";
            }
            else if (user.AdminLevel == 6)
            {
                return "Dueño";
            }
            return "Usuario";
        }
        public static void GuardarCuentas()
        {
            foreach (RustPP.Data.Entities.User user in usersOnline)
            {
                if(user.Connected == 0)
                {
                    Data.Globals.usersOnline.RemoveAll(x => x.Name == user.Name);
                }
                if (user != null && user.Player != null)
                {
                    user.Save();  
                }
            }
        }
        public static Entities.User GetInternalUser(Fougerite.Player player)
        {
            return Data.Globals.usersOnline.FindLast(x => x.Name == player.Name);
        }
    }
}
