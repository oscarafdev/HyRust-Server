using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Data.Entities
{
    public class User
    {
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
        public int LumberjackLevel { get; set; }
        public int LumberjackExp { get; set; }
        public string LastKilled { get; set; }
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
                    "woodFarmed = @woodFarmed," +
                    "sulfureFarmed = @sulfureFarmed," +
                    "metalFarmed = @metalFarmed," +
                    "adminLevel = @adminLevel" +
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
                command.Parameters.AddWithValue("@woodFarmed", this.WoodFarmed);
                command.Parameters.AddWithValue("@metalFarmed", this.MetalFarmed);
                command.Parameters.AddWithValue("@sulfureFarmed", this.SulfureFarmed);
                command.Parameters.AddWithValue("@adminLevel", this.AdminLevel);
                command.Parameters.AddWithValue("@username", this.Name);
                MySqlDataReader reader = command.ExecuteReader();

                connection.Close();
            }
        }
        public Fougerite.Player Player { get; set; }
    }
}
