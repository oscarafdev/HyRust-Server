using Fougerite;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Data.Entities
{
    public class StoreItem
    {
        public int ID { get; set; }
        public int InternalID { get; set; }
        public int UserID { get; set; }
        public int Price { get; set; }
        public bool Purchased { get; set; } = false;
        public string Date { get; set; } = DateTime.Now.ToString();
        public string ItemName { get; set; }
        public int ItemQuantity { get; set; }
        public int ItemCategory { get; set; } // 1 = Cualquier item // 2 = Arma // 3 = Armadura // 4 = Recursos
        public float ItemCondition { get; set; } = 1.0f;
        public int ItemWeaponSlots { get; set; } = 0;
        public int ItemWeaponBullets { get; set; } = 0;
        public string ItemWeaponSlot1 { get; set; } = "null";
        public string ItemWeaponSlot2 { get; set; } = "null";
        public string ItemWeaponSlot3 { get; set; } = "null";
        public string ItemWeaponSlot4 { get; set; } = "null";
        public string ItemWeaponSlot5 { get; set; } = "null";
        public Fougerite.Player Player { get; set; }
        public PlayerItem Item { get; set; } = null;
        public void Delete()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Data.Database.Connection.GetConnectionString()))
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM store_items WHERE id = @id";
                    command.Parameters.AddWithValue("@id", this.ID);
                    MySqlDataReader reader = command.ExecuteReader();
                    connection.Close();
                }
            }
            catch
            {
                Logger.LogError("Error al eliminar un storeItem");
            }
        }
        public void Create()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Data.Database.Connection.GetConnectionString()))
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();

                    command.CommandText = "INSERT INTO store_items (user_id, price, date, item_name, item_quantity, item_category, item_condition, item_weapon_slots, item_weapon_bullets, item_weapon_slot_1, item_weapon_slot_2, item_weapon_slot_3, item_weapon_slot_4, item_weapon_slot_5) VALUES (@user_id, @price, @date, @item_name, @item_quantity, @item_category, @item_condition, @item_weapon_slots, @item_weapon_bullets, @item_weapon_slot_1, @item_weapon_slot_2, @item_weapon_slot_3, @item_weapon_slot_4, @item_weapon_slot_5)";
                    command.Parameters.AddWithValue("@user_id", this.UserID);
                    command.Parameters.AddWithValue("@price", this.Price);
                    command.Parameters.AddWithValue("@date", this.Date);
                    command.Parameters.AddWithValue("@item_name", this.ItemName);
                    command.Parameters.AddWithValue("@item_quantity", this.ItemQuantity);
                    command.Parameters.AddWithValue("@item_category", this.ItemCategory);
                    command.Parameters.AddWithValue("@item_condition", this.ItemCondition);
                    command.Parameters.AddWithValue("@item_weapon_slots", this.ItemWeaponSlots);
                    command.Parameters.AddWithValue("@item_weapon_bullets", this.ItemWeaponBullets);
                    command.Parameters.AddWithValue("@item_weapon_slot_1", this.ItemWeaponSlot1);
                    command.Parameters.AddWithValue("@item_weapon_slot_2", this.ItemWeaponSlot2);
                    command.Parameters.AddWithValue("@item_weapon_slot_3", this.ItemWeaponSlot3);
                    command.Parameters.AddWithValue("@item_weapon_slot_4", this.ItemWeaponSlot4);
                    command.Parameters.AddWithValue("@item_weapon_slot_5", this.ItemWeaponSlot5);
                    MySqlDataReader reader = command.ExecuteReader();
                    connection.Close();
                }
            }
            catch
            {
                Logger.LogError("Error al crear un StoreItem");
            }
        }
        public void Save()
        {
            
                using (MySqlConnection connection = new MySqlConnection(Data.Database.Connection.GetConnectionString()))
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();
                    int purchased = 0;
                    if(this.Purchased)
                    {
                        purchased = 1;
                    }
                    command.CommandText = "UPDATE store_items SET user_id = @user_id, " +
                        "price = @price, " +
                        "purchased = @purchased," +
                        "date = @date, " +
                        "item_name = @item_name, " +
                        "item_quantity = @item_quantity," +
                        "item_category = @item_category," +
                        "item_condition = @item_condition," +
                        "item_weapon_slots = @item_weapon_slots," +
                        "item_weapon_bullets = @item_weapon_bullets," +
                        "item_weapon_slot_1 = @item_weapon_slot_1," +
                        "item_weapon_slot_2 = @item_weapon_slot_2," +
                        "item_weapon_slot_3 = @item_weapon_slot_3," +
                        "item_weapon_slot_4 = @item_weapon_slot_4," +
                        "item_weapon_slot_5 = @item_weapon_slot_5" +
                        " WHERE id = @id";
                    command.Parameters.AddWithValue("@user_id", this.UserID);
                    command.Parameters.AddWithValue("@price", this.Price);
                    command.Parameters.AddWithValue("@purchased", purchased);
                    command.Parameters.AddWithValue("@date", this.Date);
                    command.Parameters.AddWithValue("@item_name", this.ItemName);
                    command.Parameters.AddWithValue("@item_quantity", this.ItemQuantity);
                    command.Parameters.AddWithValue("@item_category", this.ItemCategory);
                    command.Parameters.AddWithValue("@item_condition", this.ItemCondition);
                    command.Parameters.AddWithValue("@item_weapon_slots", this.ItemWeaponSlots);
                    command.Parameters.AddWithValue("@item_weapon_bullets", this.ItemWeaponBullets);
                    command.Parameters.AddWithValue("@item_weapon_slot_1", this.ItemWeaponSlot1);
                    command.Parameters.AddWithValue("@item_weapon_slot_2", this.ItemWeaponSlot2);
                    command.Parameters.AddWithValue("@item_weapon_slot_3", this.ItemWeaponSlot3);
                    command.Parameters.AddWithValue("@item_weapon_slot_4", this.ItemWeaponSlot4);
                    command.Parameters.AddWithValue("@item_weapon_slot_5", this.ItemWeaponSlot5);
                    command.Parameters.AddWithValue("@id", this.ID);
                    MySqlDataReader reader = command.ExecuteReader();
                    connection.Close();
                }
            
        }
    }
}
