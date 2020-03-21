
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
using System.Text.RegularExpressions;
using RustPP.Commands;

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
            Fougerite.Hooks.OnNPCKilled += OnNPCKilled;
            Fougerite.Hooks.OnEntityHurt += OnEntityHurt;
            Fougerite.Hooks.OnPlayerHurt += OnPlayerHurt;
            Fougerite.Hooks.OnChat += OnChat;
        }
        static void OnChat(Fougerite.Player player, ref ChatString text)
        {
            if (!UserIsLogged(player))
            {
                char ch = '☢';
                player.Notice(ch.ToString(), $"No estas logueado, usa /login o /registro", 4f);
                return;
            }
        }
        static void OnPlayerMove(HumanController hc, Vector3 origin, int encoded, ushort stateFlags, uLink.NetworkMessageInfo info, Util.PlayerActions action)
        {

            
            
        }
        static void OnPlayerHurt(HurtEvent he)
        {
            if (he.AttackerIsPlayer && he.Attacker != null)
            {
                Fougerite.Player player = (Fougerite.Player)he.Attacker;
                User user = Data.Globals.usersOnline.Find(x => x.Name == player.Name);
                if (!UserIsLogged(player))
                {
                    char ch = '☢';
                    player.Notice(ch.ToString(), $"No estas logueado, usa /login o /registro", 4f);
                    he.DamageAmount = 0;
                    return;
                }
                if (he.VictimIsPlayer && he.Victim != null)
                {
                    Fougerite.Player victim = (Fougerite.Player)he.Victim;
                    User victimPlayer = Data.Globals.usersOnline.Find(x => x.Name == victim.Name);
                    if (!UserIsLogged(victim))
                    {
                        char ch = '☢';
                        player.Notice(ch.ToString(), $"Este usuario no esta logueado, no puedes hacerle daño", 4f);
                        he.DamageAmount = 0;
                        return;
                    }
                }
            }
            if (he.AttackerIsNPC && he.Attacker != null && he.VictimIsPlayer && he.Victim != null)
            {
                Fougerite.Player player = (Fougerite.Player)he.Victim;
                User user = Data.Globals.usersOnline.Find(x => x.Name == player.Name);
                if (!UserIsLogged(player))
                {
                    he.DamageAmount = 0;
                    return;
                }
            }
        }
        static void OnEntityHurt(HurtEvent he)
        {
            if(he.AttackerIsPlayer && he.Attacker != null)
            {
                Fougerite.Player player = (Fougerite.Player)he.Attacker;
                User user = Data.Globals.usersOnline.Find(x => x.Name == player.Name);
                if (!UserIsLogged(player))
                {
                    char ch = '☢';
                    player.Notice(ch.ToString(), $"No estas logueado, usa /login o /registro", 4f);
                    he.DamageAmount = 0;
                    return;
                }
                if(he.VictimIsPlayer && he.Victim != null)
                {
                    Fougerite.Player victim = (Fougerite.Player)he.Victim;
                    User victimPlayer = Data.Globals.usersOnline.Find(x => x.Name == victim.Name);
                    if (!UserIsLogged(victim))
                    {
                        char ch = '☢';
                        player.Notice(ch.ToString(), $"Este usuario no esta logueado, no puedes hacerle daño", 4f);
                        he.DamageAmount = 0;
                        return;
                    }
                }
            }
            if(he.AttackerIsNPC && he.Attacker != null && he.VictimIsPlayer && he.Victim != null)
            {
                Fougerite.Player player = (Fougerite.Player)he.Attacker;
                User user = Data.Globals.usersOnline.Find(x => x.Name == player.Name);
                if (!UserIsLogged(player))
                {
                    
                    he.DamageAmount = 0;
                    return;
                }
            }
        }
        static void OnNPCKilled(DeathEvent de)
        {
            if(de.AttackerIsPlayer && de.Attacker != null)
            {
                Fougerite.Player player = (Fougerite.Player)de.Attacker;
                User user = Data.Globals.usersOnline.Find(x => x.Name == player.Name);
                if (!UserIsLogged(player))
                {
                    char ch = '☢';
                    player.Notice(ch.ToString(), $"No estas logueado, usa /login o /registro", 4f);
                    de.DamageAmount = 0;
                    return;
                }
                user.AddFarmExp(1);

            }
            

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
                        }
                        
                    }
                }
            }
        }

        static void OnPlayerSpawned(Fougerite.Player player, SpawnEvent se)
        {
            if (!Regex.IsMatch(player.Name, @"^[a-zA-Z0-9]*$"))
            {
                char ch = '☢';
                player.Notice(ch.ToString(), $"No se permiten carácteres especiales en el nombre (Permitido: [a-z] [0-9])", 4f);
                player.Disconnect();
            }

            if (!UserIsLogged(player))
            {
                player.Inventory.ClearAll();
                if (CheckIfUserIsRegistered(player))
                {
                    FreezePlayer(player);
                    player.SendClientMessage($"Bienvenido a [color orange]HyAxe Rust[color white], para ingresar utiliza [color blue]/login <Contraseña>");
                }
                else
                {
                    FreezePlayer(player);
                    player.SendClientMessage($"Bienvenido a [color orange]HyAxe Rust[color white], para registrarte utiliza [color blue]/registro <Contraseña> <Confirmar Contraseña>");
                }
                

            }
        }
        static void LoadInventory(Fougerite.Player player)
        {
            JsonAPI json = JsonAPI.GetInstance;
            User user = Data.Globals.usersOnline.Find(x => x.Name == player.Name);
            if(user != null)
            {
                
                
                List<UserInventoryItem> playerItems = json.DeSerializeJsonToObject<List<UserInventoryItem>>(user.InternalInventory);
                foreach(UserInventoryItem item in playerItems)
                {
                    if(item.Name != "" && item.Quantity != -1)
                    {
                        player.Inventory.AddItemTo(item.Name, item.Slot, item.Quantity);
                        Inventory inventory = player.PlayerClient.controllable.GetComponent<Inventory>();
                        PlayerItem playerItem = new PlayerItem(ref inventory, item.Slot);
                        IInventoryItem dataItem = playerItem.RInventoryItem as IInventoryItem;
                        dataItem.SetCondition(item.Condition);
                        if (playerItem != null)
                        {
                            if (playerItem.isWeapon)
                            {
                                dataItem.SetUses(item.WeaponBullets);
                                playerItem.heldItem.SetTotalModSlotCount(item.WeaponSlots);
                                if (item.WeaponSlot1 != null)
                                {
                                    playerItem.addWeaponMod(item.WeaponSlot1);
                                }

                                if (item.WeaponSlot2 != null && item.WeaponSlot2 != "null")
                                {

                                    playerItem.addWeaponMod(item.WeaponSlot2);
                                }
                                if (item.WeaponSlot3 != null && item.WeaponSlot3 != "null")
                                {
                                    playerItem.addWeaponMod(item.WeaponSlot3);
                                }
                                if (item.WeaponSlot4 != null && item.WeaponSlot4 != "null")
                                {
                                    playerItem.addWeaponMod(item.WeaponSlot4);
                                }
                                if (item.WeaponSlot5 != null && item.WeaponSlot5 != "null")
                                {
                                    playerItem.addWeaponMod(item.WeaponSlot5);
                                }
                            }
                        } else
                        {
                            Logger.LogError("LoadInventory - playerItem is Null");
                        }

                    }
                }
            }
            
        }
        static void FreezePlayer(Fougerite.Player player)
        {
            player.SendCommand("input.bind Up 7 None");
            player.SendCommand("input.bind Down 7 None");
            player.SendCommand("input.bind Left 7 None");
            player.SendCommand("input.bind Right 7 None");
            player.SendCommand("input.bind Sprint 7 None");
            player.SendCommand("input.bind Duck 7 None");
            player.SendCommand("input.bind Jump 7 None");
            player.SendCommand("input.bind Fire 7 None");
            player.SendCommand("input.bind AtlFire 7 None");
        }
        static void UnFreezePlayer(Fougerite.Player player)
        {
            player.SendCommand("input.bind Up W UpArrow");
            player.SendCommand("input.bind Down S DownArrow");
            player.SendCommand("input.bind Left A LeftArrow");
            player.SendCommand("input.bind Right D RightArrow");
            player.SendCommand("input.bind Sprint LeftShift RightShift");
            player.SendCommand("input.bind Duck LeftControl RightControl");
            player.SendCommand("input.bind Jump Space None");
            player.SendCommand("input.bind Fire Mouse0 None");
            player.SendCommand("input.bind AtlFire Mouse1 None");
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
                        HunterLevel = reader.GetInt32("hunterLevel"),
                        HunterExp = reader.GetInt32("hunterExp"),
                        AdminLevel = reader.GetInt32("adminLevel"),
                        BannedPlayer = reader.GetInt32("banned"),
                        InternalInventory = reader.GetString("inventoryItems"),
                        Player = player
                    };
                    Data.Globals.usersOnline.Add(newUser);
                    player.SendClientMessage($"¡Bienvenido! [color orange]{player.Name}[color white] - Nivel [color orange]{newUser.Level}");
                    player.SendClientMessage($"Si tienes dudas utiliza [color blue]/ayuda[color white] o escribe tu duda por el canal [color blue]/duda");
                    if (newUser.AdminLevel >= 1)
                    {
                        player.SendClientMessage($"[color orange]Eres administrador nivel[/color] {newUser.AdminLevel}");
                    }
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
            
            User user = Data.Globals.usersOnline.Find(x => x.Name == player.Name);
            
            if (user != null)
            {
                if(user.BannedPlayer == 1)
                {
                    char ch = '☢';
                    player.Notice(ch.ToString(), $"Esta cuenta esta baneada, pide un desbaneo en la web www.hyaxe.com.", 4f);
                    player.Disconnect();
                }
                LoadInventory(player);
                ShareCommand command = ChatCommand.GetCommand("share") as ShareCommand;
                command.AddDoors(user.SteamID, player);

            }
            else
            {
                player.SendClientMessage("[color red] Ocurrió un error al cargar la cuenta, comunicate con un administrador. Código #0102");
            }
            UnFreezePlayer(player);
            
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
            User user = Data.Globals.usersOnline.Find(x => x.Name == player.Name);
            if(user != null)
            {
                UnshareCommand command = ChatCommand.GetCommand("unshare") as UnshareCommand;
                command.DeleteDoors(user.SteamID, player);
                List<UserInventoryItem> playerItems = new List<UserInventoryItem>();
                PlayerInv inventario = new PlayerInv(player);
                foreach (PlayerItem item in inventario.AllItems)
                {
                    if (item.Name != "" && item.Quantity != -1)
                    {
                        IInventoryItem itemdata = item.RInventoryItem;

                        ItemDataBlock datablock = itemdata.datablock;
                        UserInventoryItem newItem = new UserInventoryItem
                        {
                            Slot = item.Slot,
                            Name = item.Name,
                            Quantity = item.Quantity,
                            Condition = itemdata.condition,
                        };
                        if(item.isWeapon)
                        {
                            newItem.WeaponBullets = itemdata.uses;
                            ItemModDataBlock mod1 = item.getModSlot(0);
                            ItemModDataBlock mod2 = item.getModSlot(1);
                            ItemModDataBlock mod3 = item.getModSlot(2);
                            ItemModDataBlock mod4 = item.getModSlot(3);
                            ItemModDataBlock mod5 = item.getModSlot(4);
                            newItem.WeaponSlots = item.getModSlotsCount;
                            if(mod1 != null)
                            {
                                newItem.WeaponSlot1 = mod1.name;
                            }
                            if (mod2 != null)
                            {
                                newItem.WeaponSlot2 = mod2.name;
                            }
                            if (mod3 != null)
                            {
                                newItem.WeaponSlot3 = mod3.name;
                            }
                            if (mod4 != null)
                            {
                                newItem.WeaponSlot4 = mod4.name;
                            }
                            if (mod5 != null)
                            {
                                newItem.WeaponSlot5 = mod5.name;
                            }
                        }
                        playerItems.Add(newItem);
                    }
                    
                }
                JsonAPI json = JsonAPI.GetInstance;
                //Logger.LogError(json.SerializeObjectToJson(playerItems));
                user.InternalInventory = json.SerializeObjectToJson(playerItems);
                
                user.Save();
                Data.Globals.usersOnline.RemoveAll(x => x.Name == player.Name);
            }
        }
        static void OnPlayerGathering(Fougerite.Player player, GatherEvent ge)
        {
            
            if(!UserIsLogged(player))
            {
                char ch = '☢';
                player.Notice(ch.ToString(), $"No estas logueado, usa /login o /registro", 4f);
                ge.Quantity = 0;
                return;
            }
            User user = Data.Globals.usersOnline.Find(x => x.Name == player.Name);
            int quantity = ge.Quantity;
            if (ge.Item == "Wood")
            {
                ge.Quantity = (quantity* user.LumberjackLevel) /2;
                user.AddWoodExp(ge.Quantity);
            }
            if(ge.Item == "Metal Ore")
            {
                ge.Quantity = (quantity* user.MinerLevel) /2;
                user.AddMetalExp(ge.Quantity);
            }
            if (ge.Item == "Sulfur Ore")
            {
                ge.Quantity = (quantity* user.MinerLevel) /2;
                user.AddSulfureExp(ge.Quantity);
            }
            //player.SendClientMessage($"{ge.Item} x {ge.Quantity}");
        }

        
    }
}
