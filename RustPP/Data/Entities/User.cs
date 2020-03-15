using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Data.Entities
{
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public ulong SteamID { get; set; }
        public int Exp { get; set; }
        public int Level { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Cash { get; set; }
        public int WoodFarmed { get; set; }
        public int MetalFarmed { get; set; }
        public int SulfureFarmed { get; set; }
        public int NPCFarmed { get; set; }
        public string AdminName { get; set; }
        public int AdminLevel { get; set; }
        public int ClanID { get; set; }
        public int ClanRank { get; set; }
        public int MinerLevel { get; set; }
        public int MinerExp { get; set; }
        public int HunterLevel { get; set; }
        public int HunterExp { get; set; }
        public int LumberjackLevel { get; set; }
        public int LumberjackExp { get; set; }
        public string LastKilled { get; set; }
        public int BannedPlayer { get; set; }
        public string InternalInventory { get; set; }

        public void AddWoodExp(int quantity)
        {
            if (this != null)
            {
                this.WoodFarmed += quantity;

                this.LumberjackExp += 1;
                if (this.LumberjackExp >= this.LumberjackLevel * 100)
                {
                    char cha = '♜';
                    this.LumberjackExp -= this.LumberjackLevel * 100;
                    this.LumberjackLevel += 1;
                    this.Player.Notice(cha.ToString(), $"Subiste a nivel {this.LumberjackLevel} (Leñador)", 3f);
                    this.Player.SendClientMessage($"¡Felicidades! Subiste a nivel {this.LumberjackLevel} ([color orange]Leñador[/color])");
                }
                else
                {
                    char ch = '♜';
                    this.Player.Notice(ch.ToString(), $"+ 1 Exp (Leñador)", 3f);
                    this.Player.InventoryNotice($"Leñador {this.LumberjackExp}/{this.LumberjackLevel * 100}");
                }

            }
        }
        public void AddSulfureExp(int quantity)
        {
            if (this != null)
            {
                this.SulfureFarmed += quantity;

                this.MinerExp += 1;
                if (this.MinerExp >= this.MinerLevel * 100)
                {
                    char cha = '♜';
                    this.MinerExp -= this.MinerLevel * 100;
                    this.MinerLevel += 1;
                    this.Player.Notice(cha.ToString(), $"Subiste a nivel {this.MinerLevel} (Minero)", 3f);
                    this.Player.SendClientMessage($"¡Felicidades! Subiste a nivel {this.MinerLevel} ([color orange]Minero[/color])");
                }
                else
                {
                    char ch = '♜';
                    this.Player.Notice(ch.ToString(), $"+ 1 Exp (Minero)", 3f);
                    this.Player.InventoryNotice($"Leñador {this.MinerExp}/{this.MinerLevel * 100}");
                }

            }
        }
        public void AddMetalExp(int quantity)
        {
            if (this != null)
            {
                this.MetalFarmed += quantity;

                this.MinerExp += 1;
                if (this.MinerExp >= this.MinerLevel * 100)
                {
                    char cha = '♜';
                    this.MinerExp -= this.MinerLevel * 100;
                    this.MinerLevel += 1;
                    this.Player.Notice(cha.ToString(), $"Subiste a nivel {this.MinerLevel} (Minero)", 3f);
                    this.Player.SendClientMessage($"¡Felicidades! Subiste a nivel {this.MinerLevel} ([color orange]Minero[/color])");
                }
                else
                {
                    char ch = '♜';
                    this.Player.Notice(ch.ToString(), $"+ 1 Exp (Minero)", 3f);
                    this.Player.InventoryNotice($"Leñador {this.MinerExp}/{this.MinerLevel * 100}");
                }

            }
        }
        public void AddFarmExp(int quantity)
        {
            if (this != null)
            {
                //this.MetalFarmed += quantity;

                this.HunterExp += 1;
                if (this.HunterExp >= this.HunterLevel * 100)
                {
                    char cha = '♜';
                    this.HunterExp -= this.HunterLevel * 100;
                    this.HunterLevel += 1;
                    this.Player.Notice(cha.ToString(), $"Subiste a nivel {this.HunterLevel} (Cazador)", 3f);
                    this.Player.SendClientMessage($"¡Felicidades! Subiste a nivel {this.HunterLevel} ([color orange]Cazador[/color])");
                }
                else
                {
                    char ch = '♜';
                    this.Player.Notice(ch.ToString(), $"+ 1 Exp (Cazador)", 3f);
                    this.Player.InventoryNotice($"Cazador {this.HunterExp}/{this.HunterLevel * 100}");
                }

            }
        }
        public void GiveExp(int experience)
        {
            this.Exp += experience;
            if (this.Exp >= (this.Level * 8))
            {
                this.Exp -= this.Level * 8;
                this.Level += 1;
                this.Player.SendClientMessage($"¡Felicidades! Subiste a [color orange] Nivel {this.Level} [/color] de jugador.");
                char ch = '♜';
                this.Player.Notice(ch.ToString(), $"+ {experience} Exp", 3f);
            }
        }
        public void TakeExp(int experience)
        {
            this.Exp -= experience;
            if (this.Exp < 0)
            { 
                this.Level -= 1;
                this.GiveExp(this.Level * 7);
                this.Player.SendClientMessage($"Bajaste a nivel [color orange] Nivel {this.Level} [/color] de jugador.");
            }
        }
        public void Save()
        {
            using (MySqlConnection connection = new MySqlConnection(Data.Database.Connection.GetConnectionString()))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE users SET level = @playerLevel, " +
                    "exp = @playerExp, " +
                    "kills = @playerKills, " +
                    "deaths = @playerDeaths, " +
                    "cash = @playerCash," +
                    "lastKilled = @lastKilled," +
                    "minerLevel = @minerLevel," +
                    "minerExp = @minerExp," +
                    "lumberjackLevel = @lumberjackLevel," +
                    "lumberjackExp = @lumberjackExp," +
                    "hunterLevel = @hunterLevel," +
                    "hunterExp = @hunterExp," +
                    "woodFarmed = @woodFarmed," +
                    "sulfureFarmed = @sulfureFarmed," +
                    "metalFarmed = @metalFarmed," +
                    "adminLevel = @adminLevel," +
                    "banned = @banned," +
                    "inventoryItems = @inventoryItems" +
                    " WHERE username = @username";
                command.Parameters.AddWithValue("@playerLevel", this.Level);
                command.Parameters.AddWithValue("@playerExp", this.Exp);
                command.Parameters.AddWithValue("@playerKills", this.Kills);
                command.Parameters.AddWithValue("@playerDeaths", this.Deaths);
                command.Parameters.AddWithValue("@playerCash", this.Cash);
                command.Parameters.AddWithValue("@lastKilled", this.LastKilled);
                command.Parameters.AddWithValue("@minerLevel", this.MinerLevel);
                command.Parameters.AddWithValue("@minerExp", this.MinerExp);
                command.Parameters.AddWithValue("@lumberjackLevel", this.LumberjackLevel);
                command.Parameters.AddWithValue("@lumberjackExp", this.LumberjackExp);
                command.Parameters.AddWithValue("@hunterLevel", this.HunterLevel);
                command.Parameters.AddWithValue("@hunterExp", this.HunterExp);
                command.Parameters.AddWithValue("@woodFarmed", this.WoodFarmed);
                command.Parameters.AddWithValue("@metalFarmed", this.MetalFarmed);
                command.Parameters.AddWithValue("@sulfureFarmed", this.SulfureFarmed);
                command.Parameters.AddWithValue("@adminLevel", this.AdminLevel);
                command.Parameters.AddWithValue("@banned", this.BannedPlayer);
                command.Parameters.AddWithValue("@inventoryItems", this.InternalInventory);
                command.Parameters.AddWithValue("@username", this.Name);
                MySqlDataReader reader = command.ExecuteReader();

                connection.Close();
            }
        }
        public Fougerite.Player Player { get; set; }
    }
}
