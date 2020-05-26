using Fougerite;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Data.Entities
{
    class Friend
    {
        string tableName = "user_friends";
        public int ID { get; set; }
        public int UserID { get; set; }
        public int FriendID { get; set; }
        public bool CanOpenDoors { get; set; }
        public bool CanSetHome { get; set; }
        public bool CanEditHome { get; set; }
        public bool CanLoot { get; set; }
        public void Create()
        {

                using (MySqlConnection connection = new MySqlConnection(Data.Database.Connection.GetConnectionString()))
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();

                    command.CommandText = $"INSERT INTO {tableName} (user_id, friend_id, canLoot, canOpenDoors, canSetHome, canEditHome) VALUES (@user_id, @friend_id, @canLoot, @canOpenDoors, @canSetHome, @canEditHome)";
                    command.Parameters.AddWithValue("@user_id", this.UserID);
                    command.Parameters.AddWithValue("@friend_id", this.FriendID);
                    command.Parameters.AddWithValue("@canLoot", this.CanLoot);
                    command.Parameters.AddWithValue("@canOpenDoors", this.CanOpenDoors);
                    command.Parameters.AddWithValue("@canSetHome", this.CanSetHome);
                    command.Parameters.AddWithValue("@canEditHome", this.CanEditHome);
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
                command.CommandText = $"UPDATE {tableName} SET " +
                    "user_id = @userId," +
                    "friend_id = @friendId," +
                    "canLoot = @canLoot," +
                    "canOpenDoors = @canOpenDoors," +
                    "canSetHome = @canSetHome," +
                    "canEditHome = @canEditHome" +
                    " WHERE id = @ID";
                command.Parameters.AddWithValue("@userId", this.UserID);
                command.Parameters.AddWithValue("@friend_id", this.FriendID);
                command.Parameters.AddWithValue("@canLoot", this.CanLoot);
                command.Parameters.AddWithValue("@canOpenDoors", this.CanOpenDoors);
                command.Parameters.AddWithValue("@canSetHome", this.CanSetHome);
                command.Parameters.AddWithValue("@canEditHome", this.CanEditHome);
                command.Parameters.AddWithValue("@ID", this.ID);
                MySqlDataReader reader = command.ExecuteReader();
                connection.Close();
            }
        }
        public void Delete()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Data.Database.Connection.GetConnectionString()))
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = $"DELETE FROM {tableName} WHERE id = @id";
                    command.Parameters.AddWithValue("@id", this.ID);
                    MySqlDataReader reader = command.ExecuteReader();
                    connection.Close();
                }
            }
            catch
            {
                Logger.LogError("Error al eliminar un UserFriend");
            }
        }
    }
}
