using Fougerite.Events;
using MySql.Data.MySqlClient;
using RustPP.Data.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace RustPP.Components.LanguageComponent
{
    class LanguageComponent
    {
        public static List<RustPP.Data.Entities.LangMessage> LangMessages = new List<RustPP.Data.Entities.LangMessage>();
        public static string tableName = "lang_messages";
        public static List<ulong> waitingLanguage = new List<ulong>();
        public static string GetPlayerLang(Fougerite.Player pl)
        {
            if (!Core.userLang.ContainsKey(pl.UID))
            {
                Fougerite.Logger.LogError("Devolviendo null");
                return null;
            }
            Fougerite.Logger.LogError($"Devolviendo {Core.userLang[pl.UID]}");
            return Core.userLang[pl.UID];
        }
        public static string GetPlayerLangOrDefault(Fougerite.Player pl)
        {
            if (!Core.userLang.ContainsKey(pl.UID))
            {
                return "ES";
            }
            return Core.userLang[pl.UID];
        }
        public static void InitComponent()
        {
            using (MySqlConnection connection = new MySqlConnection(Data.Database.Connection.GetConnectionString()))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM {tableName}";
                MySqlDataReader reader = command.ExecuteReader();
                DataTable schemaTable = reader.GetSchemaTable();
                while (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        LangMessage newMessage = new LangMessage
                        {
                            Slug = reader.GetString("slug"),
                            Lang = reader.GetString("lang"),
                            Message = reader.GetString("message")
                        };
                        LangMessages.Add(newMessage);
                    }
                    reader.NextResult();
                }
            }
        }
        public static string getMessage(string slug, string lang)
        {
            if(LangMessages.Exists(x => x.Slug == slug && x.Lang == lang))
            {
                LangMessage langMessage = LangMessages.First(x => x.Slug == slug && x.Lang == lang);
                if (string.IsNullOrEmpty(langMessage.Message))
                {
                    return $"No se encontró el mensaje {slug}, reportelo a un administrador.";
                }
                return langMessage.Message;
            }
            else
            {
                return $"No se encontró el mensaje {slug}, reportelo a un administrador.";
            }
            
        }
        public static void ReloadMessages()
        {
            LangMessages.RemoveAll(x => x.Slug != null);
            using (MySqlConnection connection = new MySqlConnection(Data.Database.Connection.GetConnectionString()))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM {tableName}";
                MySqlDataReader reader = command.ExecuteReader();
                DataTable schemaTable = reader.GetSchemaTable();
                while (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        LangMessage newMessage = new LangMessage
                        {
                            Slug = reader.GetString("slug"),
                            Lang = reader.GetString("lang"),
                            Message = reader.GetString("message")
                        };
                        LangMessages.Add(newMessage);
                    }
                    reader.NextResult();
                }
            }
        }
    }
}
