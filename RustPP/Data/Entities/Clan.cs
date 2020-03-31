﻿using Fougerite;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Data.Entities
{
    public class Clan
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public string Tag { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Mats { get; set; }
        public int Cash { get; set; }
        public string MOTD { get; set; }

        public void addExp(int quantity)
        {
            if (this != null)
            {
                this.Exp += quantity;
                if (this.Exp >= this.Level * 500)
                {
                    char cha = '♜';
                    this.Exp -= this.Level * 500;
                    this.Level += 1;
                    Data.Globals.SendMessageForClan(this.ID, $"El clan {this.Name} subió al nivel {this.Level}");
                }
            }
        }
        public void create()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Data.Database.Connection.GetConnectionString()))
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();

                    command.CommandText = "INSERT INTO clans (name, owner, tag, level, exp, kills, deaths, mats, cash, motd) VALUES (@name, @owner, @tag, @level, @exp, @kills, @deaths, @mats, @cash, @motd)";
                    command.Parameters.AddWithValue("@name", this.Name);
                    command.Parameters.AddWithValue("@owner", this.Owner);
                    command.Parameters.AddWithValue("@tag", this.Tag);
                    command.Parameters.AddWithValue("@level", this.Level);
                    command.Parameters.AddWithValue("@exp", this.Exp);
                    command.Parameters.AddWithValue("@kills", this.Kills);
                    command.Parameters.AddWithValue("@deaths", this.Deaths);
                    command.Parameters.AddWithValue("@mats", this.Mats);
                    command.Parameters.AddWithValue("@cash", this.Cash);
                    command.Parameters.AddWithValue("@motd", this.MOTD);
                    MySqlDataReader reader = command.ExecuteReader();
                    connection.Close();
                }
            }
            catch
            {
                Logger.LogError("Error al crear un clan");
            }
        }
        public bool save()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Data.Database.Connection.GetConnectionString()))
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = "UPDATE clans SET name = @name, " +
                        "owner = @owner, " +
                        "tag = @tag, " +
                        "level = @level, " +
                        "exp = @exp," +
                        "kills = @kills," +
                        "deaths = @deaths," +
                        "mats = @mats," +
                        "cash = @cash," +
                        "motd = @motd" +
                        " WHERE id = @id";
                    command.Parameters.AddWithValue("@name", this.Name);
                    command.Parameters.AddWithValue("@owner", this.Owner);
                    command.Parameters.AddWithValue("@tag", this.Tag);
                    command.Parameters.AddWithValue("@level", this.Level);
                    command.Parameters.AddWithValue("@exp", this.Exp);
                    command.Parameters.AddWithValue("@kills", this.Kills);
                    command.Parameters.AddWithValue("@deaths", this.Deaths);
                    command.Parameters.AddWithValue("@mats", this.Mats);
                    command.Parameters.AddWithValue("@cash", this.Cash);
                    command.Parameters.AddWithValue("@motd", this.MOTD);
                    command.Parameters.AddWithValue("@id", this.ID);
                    MySqlDataReader reader = command.ExecuteReader();
                    connection.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
