using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Fougerite;
using MySql.Data.MySqlClient;
using RustPP.Data.Entities;

namespace RustPP.Components.FriendComponent
{
    class FriendComponent
    {
        public static List<Friend> Friends = new List<Friend>();
        public static string tableName = "user_friends";
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
                        Friend newFriend = new Friend
                        {
                            UserID = reader.GetInt32("user_id"),
                            FriendID = reader.GetInt32("friend_id"),
                            CanEditHome = reader.GetBoolean("canEditHome"),
                            CanLoot = reader.GetBoolean("canLoot"),
                            CanOpenDoors = reader.GetBoolean("canOpenDoors"),
                            CanSetHome = reader.GetBoolean("canSetHome")
                        };
                        Friends.Add(newFriend);
                    }
                    reader.NextResult();
                }
            }
        }
        public static void DestroyComponent()
        {
            foreach(Friend friend in Friends)
            {
                friend.Save();
            }
            Logger.Log("Se guardaron todos los amigos.");
        }
        public static void RemoveFriend(User user, User removedFriend)
        {
            if(IsFriendOf(user, removedFriend))
            {
                Friend removed = Friends.Find(x => x.UserID == user.ID && x.FriendID == removedFriend.ID);
                Friends.Remove(removed);
                removed.Delete();
                user.Player.SendClientMessage($"[color #fc038c]<Amigos>[/color] Removiste a [color #ffb3dd]{removedFriend.Name}[/color] de tu lista de amigos.");
                if (removedFriend.Connected == 1)
                {
                    removedFriend.Player.SendClientMessage($"[color #fc038c]<Amigos>[/color] [color #ffb3dd]{user.Name}[/color] te removió de su lista de amigos.");
                }
            } else
            {
                user.Player.SendClientMessage($"[color #fc038c]<!>[/color] No eres amigo de {removedFriend.Name}.");
            }
            
        }
        public static void AddFriend(User user, User friend)
        {
            if(!IsFriendOf(user, friend))
            {
                Friend newFriend = new Friend
                {
                    UserID = user.ID,
                    FriendID = friend.ID,
                    CanEditHome = true,
                    CanLoot = true,
                    CanOpenDoors = true,
                    CanSetHome = true
                };
                newFriend.Create();
                Friends.Add(newFriend);
            }
            else
            {
                user.Player.SendClientMessage($"[color #fc038c]<!>[/color] Ya eres amigo de {friend.Name}.");
            }
        }
        public static List<Friend> GetUserFriends(User user)
        {
            List<Friend> userFriends = Friends.FindAll(x => x.UserID == user.ID);
            return userFriends;
        }
        public static Friend GetUserFriend(User user, User friend)
        {
            Friend userFriend = Friends.Find(x => x.UserID == user.ID && x.FriendID == friend.ID);
            return userFriend;
        }
        public static bool IsFriendOf(User user, User friend)
        {
            List<Friend> userFriends = Friends.FindAll(x => x.UserID == user.ID && x.FriendID == friend.ID);

            if(userFriends.Count >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
