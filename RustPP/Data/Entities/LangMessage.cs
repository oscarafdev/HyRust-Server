using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fougerite;

namespace RustPP.Data.Entities
{
    class LangMessage
    {
        string tableName = "lang_messages";
        public int ID { get; set; }
        public string Slug { get; set; }
        public string Lang { get; set; }
        public string Message { get; set; }
        
        public void Create()
        {

            using (MySqlConnection connection = new MySqlConnection(Data.Database.Connection.GetConnectionString()))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();

                command.CommandText = $"INSERT INTO {tableName} (slug, message, lang) VALUES (@slug, @message, @lang)";
                command.Parameters.AddWithValue("@slug", this.Slug);
                command.Parameters.AddWithValue("@message", this.Message);
                command.Parameters.AddWithValue("@lang", this.Lang);
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
                    "slug = @slug," +
                    "message = @message," +
                    "lang = @lang" +
                    " WHERE id = @ID";
                command.Parameters.AddWithValue("@slug", this.Slug);
                command.Parameters.AddWithValue("@message", this.Message);
                command.Parameters.AddWithValue("@lang", this.Lang);
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
                Logger.LogError("Error al eliminar un LangMessage");
            }
        }
    }
}
