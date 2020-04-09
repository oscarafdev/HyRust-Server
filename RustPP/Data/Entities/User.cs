using Fougerite;
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
        public string IP { get; set; }
        public ulong SteamID { get; set; }
        public int Muted { get; set; }
        public int Exp { get; set; }
        public int Level { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Cash { get; set; }
        public float XPos { get; set; }
        public float YPos { get; set; }
        public float ZPos { get; set; }
        public int TimeToPayDay { get; set; }
        public int TimeToKit { get; set; }
        public int TimeToTP { get; set; }
        public int WoodFarmed { get; set; }
        public int MetalFarmed { get; set; }
        public int SulfureFarmed { get; set; }
        public int NPCFarmed { get; set; }
        public int Connected { get; set; }
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
        public int InvitedClan { get; set; }
        public string PrefabName { get; set; }
        public bool SpawningPrefab { get; set; } = false;
        public bool SpectingOwner { get; set; } = false;
        public bool TiendaEnabled { get; set; } = true;
        public bool InstaKO { get; set; } = false;
        public bool InstaKOAll { get; set; } = false;
        public bool GodMode { get; set; } = false;
        public string InternalInventory { get; set; }
        public bool FPS { get; set; } = false;
        public int TimeToDuda { get; set; } = 0;
        public int TimeToChat { get; set; } = 0;

        public StoreItem SellingItem { get; set; } = null;
        public enum Language
        {
            ES,
            EN,
            PT
        }

        public void AddWoodExp(int quantity)
        {
            if (this != null)
            {
                this.WoodFarmed += quantity;

                this.LumberjackExp += 1 * Globals.EventoExp;
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
                    this.Player.Notice(ch.ToString(), $"Leñador {this.LumberjackExp}/{this.LumberjackLevel * 100}", 3f);
                }

            }
        }
        public void AddSulfureExp(int quantity)
        {
            if (this != null)
            {
                this.SulfureFarmed += quantity;

                this.MinerExp += 1 * Globals.EventoExp;
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
                    this.Player.Notice(ch.ToString(), $"Minero {this.MinerExp}/{this.MinerLevel * 100}", 3f);
                }

            }
        }
        public void AddMetalExp(int quantity)
        {
            if (this != null)
            {
                this.MetalFarmed += quantity;

                this.MinerExp += 1 * Globals.EventoExp;
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
                    this.Player.Notice(ch.ToString(), $"Minero {this.MinerExp}/{this.MinerLevel * 100}", 3f);
                }

            }
        }
        public void AddFarmExp(int quantity)
        {
            if (this != null)
            {
                //this.MetalFarmed += quantity;

                this.HunterExp += 1 * Globals.EventoExp;
                if (this.HunterExp >= this.HunterLevel * 30)
                {
                    char cha = '♜';
                    this.HunterExp -= this.HunterLevel * 30;
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
            this.Exp += experience * Globals.EventoExp;
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
        public void Connect()
        {
            using (MySqlConnection connection = new MySqlConnection(Data.Database.Connection.GetConnectionString()))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE users SET " +
                    "connected = @connected" +
                    " WHERE username = @username";
                command.Parameters.AddWithValue("@connected", this.Connected);

                command.Parameters.AddWithValue("@username", this.Name);
                MySqlDataReader reader = command.ExecuteReader();

                connection.Close();
            }
        }
        public void Save()
        {
            using (MySqlConnection connection = new MySqlConnection(Data.Database.Connection.GetConnectionString()))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE users SET level = @playerLevel," +
                    "ip = @ip, " +
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
                    "inventoryItems = @inventoryItems," +
                    "timeToPayDay = @timeToPayDay," +
                    "timeToKit = @timeToKit," +
                    "timeToTP = @timeToTP," +
                    "connected = @connected," +
                    "xPos = @xPos," +
                    "yPos = @yPos," +
                    "zPos = @zPos," +
                    "muted = @muted," +
                    "clan = @clan," +
                    "clanRank = @clanRank" +
                    " WHERE username = @username";
                command.Parameters.AddWithValue("@playerLevel", this.Level);
                command.Parameters.AddWithValue("@ip", this.IP);
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
                command.Parameters.AddWithValue("@timeToPayDay", this.TimeToPayDay);
                command.Parameters.AddWithValue("@timeToKit", this.TimeToKit);
                command.Parameters.AddWithValue("@timeToTP", this.TimeToTP);
                command.Parameters.AddWithValue("@connected", this.Connected);
                command.Parameters.AddWithValue("@xPos", this.XPos);
                command.Parameters.AddWithValue("@yPos", this.YPos);
                command.Parameters.AddWithValue("@zPos", this.ZPos);
                command.Parameters.AddWithValue("@muted", this.Muted);
                command.Parameters.AddWithValue("@clan", this.ClanID);
                command.Parameters.AddWithValue("@clanRank", this.ClanRank);
                command.Parameters.AddWithValue("@username", this.Name);
                MySqlDataReader reader = command.ExecuteReader();
                Logger.LogDebug($"La cuenta {this.Name} fue guardada.");
                connection.Close();
            }
        }
        public void GetClan()
        {
            int count = Data.Globals.Clans.Count(x => x.ID == this.ClanID);
            if(count >= 1)
            {
                RustPP.Data.Entities.Clan clan = Data.Globals.Clans.Find(x => x.ID == this.ClanID);
                this.Clan = clan;
            }
        }
        public RustPP.Data.Entities.Clan Clan { get; set; }
        public Fougerite.Player Player { get; set; }
    }
}
