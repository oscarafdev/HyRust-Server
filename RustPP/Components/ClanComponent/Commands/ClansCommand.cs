using MySql.Data.MySqlClient;
using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace RustPP.Components.ClanComponent.Commands
{
    class ClansCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            Fougerite.Player pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            string lang = LanguageComponent.LanguageComponent.GetPlayerLangOrDefault(pl);
            if (!RustPP.Data.Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(),LanguageComponent.LanguageComponent.getMessage("notice_not_logged", lang), 4f);
                return;
            }
            using (MySqlConnection connection = new MySqlConnection(Data.Database.Connection.GetConnectionString()))
            {
                pl.SendClientMessage($"--------- [color blue]Clanes - TOP 10[/color] ---------");
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM clans ORDER BY level DESC LIMIT 10";
                MySqlDataReader reader = command.ExecuteReader();
                DataTable schemaTable = reader.GetSchemaTable();
                int puesto = 1;
                
                while (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int nivel = reader.GetInt32("level");
                        string owner = reader.GetString("owner");
                        string name = reader.GetString("name");
                        pl.SendClientMessage($"#{puesto} [color blue]{name}[/color] | Nivel [color blue]{nivel.ToString()}[/color] - Lider: [color blue]{owner}[/color]");
                        puesto++;
                    }
                    reader.NextResult();
                }
            }
        }
    }
}
