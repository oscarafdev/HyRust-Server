using Fougerite;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace RustPP.Components.EconomyComponent
{
    class EconomyComponent
    {
        public static List<RustPP.Data.Entities.StoreItem> StoreItems = new List<RustPP.Data.Entities.StoreItem>();


        public static List<RustPP.Data.Entities.StoreItem> GetPageItemss(List<RustPP.Data.Entities.StoreItem> list, int page, int pageSize)
        {
            List<RustPP.Data.Entities.StoreItem> lista = new List<RustPP.Data.Entities.StoreItem>();
            for (int i = pageSize * (page-1); i < pageSize * page; i++)
            {
                if (list.Count > i)
                {
                    lista.Add(list[i]);
                }
                // add values to the page or display whatever..
            }
            return lista;
        }
        public enum ItemCategory
        {
            Weapon = 1,
            Armor = 2,
            Resource = 3,
            Other = 4
        }
        public static void InitComponent()
        {
            using (MySqlConnection connection = new MySqlConnection(Data.Database.Connection.GetConnectionString()))
            {

                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM store_items";
                MySqlDataReader reader = command.ExecuteReader();
                DataTable schemaTable = reader.GetSchemaTable();
                int InternalID = 1;
                while (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Data.Entities.StoreItem newItem = new Data.Entities.StoreItem
                        {
                            ID = reader.GetInt32("id"),
                            InternalID = InternalID,
                            UserID = reader.GetInt32("user_id"),
                            Price = reader.GetInt32("price"),
                            Date = reader.GetString("date"),
                            ItemName = reader.GetString("item_name"),
                            ItemQuantity = reader.GetInt32("item_quantity"),
                            ItemCategory = reader.GetInt32("item_category"),
                            ItemCondition = reader.GetInt32("item_condition"),
                            ItemWeaponSlots = reader.GetInt32("item_weapon_slots"),
                            ItemWeaponBullets = reader.GetInt32("item_weapon_bullets"),
                            ItemWeaponSlot1 = reader.GetString("item_weapon_slot_1"),
                            ItemWeaponSlot2 = reader.GetString("item_weapon_slot_2"),
                            ItemWeaponSlot3 = reader.GetString("item_weapon_slot_3"),
                            ItemWeaponSlot4 = reader.GetString("item_weapon_slot_4"),
                            ItemWeaponSlot5 = reader.GetString("item_weapon_slot_5"),
                        };
                        if(reader.GetInt32("purchased") == 1)
                        {
                            newItem.Purchased = true;
                        }
                        
                        StoreItems.Add(newItem);
                        InternalID += 1;
                        Logger.Log($"StoreItem {InternalID} cargado.");
                    }
                    reader.NextResult();
                }
            }
        }
        
        public static List<RustPP.Data.Entities.StoreItem> GetItemsByCategory(string category)
        {
            List<RustPP.Data.Entities.StoreItem> others = new List<RustPP.Data.Entities.StoreItem>();
            foreach (Data.Entities.StoreItem item in StoreItems)
            {
                if (item.ItemCategory == GetCategoryNumber(category))
                {
                    others.Add(item);
                }
            }
            return others;
        }
        
        public static PlayerItem GetItem(Fougerite.PlayerInv inv, int slot)
        {
            foreach (PlayerItem item in inv.AllItems)
            {

                if (item.Slot == slot)
                {
                    return item;
                }
            }
            return null;
        }
        public static int GetItemSlot(int number)
        {
            if(number > 6)
            {
                return -1;
            }
            return number + 29;
        }
        public static void CheckPurchases(Data.Entities.User user)
        {
            if (StoreItems.Count(x => x.UserID == user.ID && x.Purchased == true) > 0)
            {
                foreach(Data.Entities.StoreItem storeItem in StoreItems.FindAll(x => x.UserID == user.ID && x.Purchased == true))
                {
                    user.Cash += storeItem.Price;
                    user.Save();
                    user.Player.SendClientMessage($"[color purple]<Tienda>[/color] Vendiste una [color #db8cff]{storeItem.ItemName}[/color] a [color #db8cff]${storeItem.Price}[/color] mientras no estabas.");
                    storeItem.Delete();
                    StoreItems.Remove(storeItem);
                }
            }
        }
        public static void PurchaseItem(Data.Entities.User user, int InternalID)
        {
            if(StoreItems.Exists(x => x.InternalID == InternalID && x.Purchased == false))
            {
                Data.Entities.StoreItem storeItem = StoreItems.Find(x => x.InternalID == InternalID);
                if (storeItem != null && storeItem.ItemName != "" && storeItem.ItemQuantity != -1)
                {
                    int Slot = user.Player.Inventory.FirstFreeSlot;
                    if(Slot == -1)
                    {
                        user.Player.SendClientMessage($"No tienes espacio en el inventario para recibir el objeto.");
                        return;
                    }
                    Logger.LogError($"Vendedor {user.ID} - {storeItem.UserID}");
                    if (user.Cash < storeItem.Price && user.ID != storeItem.UserID)
                    {
                        user.Player.SendClientMessage($"No tienes dinero suficiente para comprar este objeto");
                        return;
                    }

                    user.Player.Inventory.AddItemTo(storeItem.ItemName, Slot, storeItem.ItemQuantity);
                    Inventory inventory = user.Player.PlayerClient.controllable.GetComponent<Inventory>();
                    PlayerItem playerItem = new PlayerItem(ref inventory, Slot);
                    IInventoryItem dataItem = playerItem.RInventoryItem as IInventoryItem;
                    dataItem.SetCondition(storeItem.ItemCondition);
                    if(user.ID != storeItem.UserID)
                    {
                        user.Cash -= storeItem.Price;
                        if (Data.Globals.UserIsConnected(storeItem.UserID))
                        {
                            Data.Entities.User seller = Data.Globals.usersOnline.FindLast(x => x.ID == storeItem.UserID);
                            seller.Cash += storeItem.Price;
                            seller.Save();
                            seller.Player.SendClientMessage($"[color purple]<Tienda>[/color] Vendiste una [color #db8cff]{storeItem.ItemName}[/color] a [color #db8cff]${storeItem.Price}[/color].");
                            storeItem.Delete();
                            StoreItems.Remove(storeItem);
                        }
                        else
                        {
                            storeItem.Purchased = true;
                            storeItem.Save();
                        }
                        user.Player.SendClientMessage($"[color purple]<Tienda>[/color] Compraste una [color #db8cff]{storeItem.ItemName}[/color] a [color #db8cff]${storeItem.Price}[/color].");
                    } else
                    {
                        storeItem.Delete();
                        StoreItems.Remove(storeItem);
                        user.Player.SendClientMessage($"[color purple]<Tienda>[/color] Retiraste una [color #db8cff]{storeItem.ItemName}[/color] de la tienda.");
                    }

                    if (playerItem != null)
                    {
                        if (playerItem.isWeapon)
                        {
                            dataItem.SetUses(storeItem.ItemWeaponBullets);
                            playerItem.heldItem.SetTotalModSlotCount(storeItem.ItemWeaponSlots);
                            if (storeItem.ItemWeaponSlot1 != null && storeItem.ItemWeaponSlot1 != "null")
                            {
                                playerItem.addWeaponMod(storeItem.ItemWeaponSlot1);
                            }

                            if (storeItem.ItemWeaponSlot2 != null && storeItem.ItemWeaponSlot2 != "null")
                            {

                                playerItem.addWeaponMod(storeItem.ItemWeaponSlot2);
                            }
                            if (storeItem.ItemWeaponSlot3 != null && storeItem.ItemWeaponSlot3 != "null")
                            {
                                playerItem.addWeaponMod(storeItem.ItemWeaponSlot3);
                            }
                            if (storeItem.ItemWeaponSlot4 != null && storeItem.ItemWeaponSlot4 != "null")
                            {
                                playerItem.addWeaponMod(storeItem.ItemWeaponSlot4);
                            }
                            if (storeItem.ItemWeaponSlot5 != null && storeItem.ItemWeaponSlot5 != "null")
                            {
                                playerItem.addWeaponMod(storeItem.ItemWeaponSlot5);
                            }
                        }
                    }
                    
                }
            }
            else
            {
                user.Player.SendClientMessage($"[color red]<Error>[/color] El objeto que intentas comprar ya no esta en venta.");
                return;
            }
        }
        public static Data.Entities.StoreItem GetStoreItem(Data.Entities.User user, Fougerite.PlayerItem item, int price = 1)
        {
            if (item.Name != "" && item.Quantity != -1 && item != null)
            {
                IInventoryItem itemdata = item.RInventoryItem;
                string category = item.RInventoryItem.datablock.category.ToString();
                int internalID = 1;
                if(StoreItems.Count > 0)
                {
                    Data.Entities.StoreItem lastItem = StoreItems.Last<Data.Entities.StoreItem>();

                    if (lastItem != null)
                    {
                        internalID = lastItem.InternalID + 1;
                    }
                }
                Data.Entities.StoreItem newItem = new Data.Entities.StoreItem
                {
                    InternalID = internalID,
                    UserID = user.ID,
                    Price = price,
                    Date = DateTime.Now.ToString(),
                    ItemName = item.Name,
                    ItemCategory = GetCategoryNumber(category),
                    ItemQuantity = item.Quantity,
                    ItemCondition = itemdata.condition,
                    Item = item,
                };
                if(user.Player != null)
                {
                    newItem.Player = user.Player;
                }
                else
                {
                    return null;
                }
                if (item.isWeapon)
                {
                    newItem.ItemWeaponBullets = itemdata.uses;
                    ItemModDataBlock mod1 = item.getModSlot(0);
                    ItemModDataBlock mod2 = item.getModSlot(1);
                    ItemModDataBlock mod3 = item.getModSlot(2);
                    ItemModDataBlock mod4 = item.getModSlot(3);
                    ItemModDataBlock mod5 = item.getModSlot(4);
                    newItem.ItemWeaponSlots = item.getModSlotsCount;
                    if (mod1 != null)
                    {
                        newItem.ItemWeaponSlot1 = mod1.name;
                    }
                    if (mod2 != null)
                    {
                        newItem.ItemWeaponSlot2 = mod2.name;
                    }
                    if (mod3 != null)
                    {
                        newItem.ItemWeaponSlot3 = mod3.name;
                    }
                    if (mod4 != null)
                    {
                        newItem.ItemWeaponSlot4 = mod4.name;
                    }
                    if (mod5 != null)
                    {
                        newItem.ItemWeaponSlot5 = mod5.name;
                    }
                }

                return newItem;
            }
            return null;
        }
        /*
         Survival = 0,
        Weapons = 1,
        Ammo = 2,
        Misc = 3,
        Medical = 4,
        Armor = 5,
        Blueprint = 6,
        Food = 7,
        Tools = 8,
        Mods = 9,
        Parts = 10,
        Resource = 11*/
        public static int GetCategoryNumber(string cat)
        {
            switch(cat)
            {
                case "Survival": return 0;
                case "Weapons": return 1;
                case "Ammo": return 2;
                case "Misc": return 3;
                case "Medical": return 4;
                case "Armor": return 5;
                case "Blueprint": return 6;
                case "Food": return 7;
                case "Tools": return 8;
                case "Mods": return 9;
                case "Parts": return 10;
                case "Resource": return 11;
            }
            return -1;
        }
        public static string GetCategoryString(string cat)
        {
            switch(cat)
            {
                case "Medical": 
                    return "Medicina";
                case "Weapons": 
                    return "Armas";
                case "Survival":
                    return "Survival";
                case "Parts":
                    return "Partes";
            }
            return cat;
        }
    }
}
