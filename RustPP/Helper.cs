﻿namespace RustPP
{
    using Fougerite;
    using RustPP.Commands;
    using RustPP.Permissions;
    using RustPP.Social;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using System.Linq;

    public static class Helper
    {
        public static void CreateSaves()
        {
            Logger.Log("Guardando clanes");
            RustPP.Data.Globals.GuardarClanes();
            Logger.Log("Guardando cuentas");
            RustPP.Data.Globals.GuardarCuentas();
            ShareCommand command = (ShareCommand)ChatCommand.GetCommand("share");
            FriendsCommand command2 = (FriendsCommand)ChatCommand.GetCommand("amigos");
            if (command.GetSharedDoors().Count != 0)
            {
                Logger.Log("Guardando puertas");
                ObjectToFile<Hashtable>(command.GetSharedDoors(), RustPPModule.GetAbsoluteFilePath("doorsSave.rpp"));
                SerializableDictionary<ulong, List<ulong>> doorsSave = new SerializableDictionary<ulong, List<ulong>>();
                foreach (DictionaryEntry entry in command.GetSharedDoors())
                {
                    ulong key = (ulong)entry.Key;
                    ArrayList value = (ArrayList)entry.Value;
                    List<ulong> list = new List<ulong>(value.OfType<ulong>());
                    doorsSave.Add(key, list);
                }
                ObjectToXML<SerializableDictionary<ulong, List<ulong>>>(doorsSave, RustPPModule.GetAbsoluteFilePath("doorsSave.xml"));
            }
            else if (File.Exists(RustPPModule.GetAbsoluteFilePath("doorsSave.rpp")))
            {
                File.Delete(RustPPModule.GetAbsoluteFilePath("doorsSave.rpp"));
            }
            if (command2.GetFriendsLists().Count != 0)
            {
                Logger.Log("Saving friends lists.");
                ObjectToFile<Hashtable>(command2.GetFriendsLists(), RustPPModule.GetAbsoluteFilePath("friendsSave.rpp"));
            }
            else if (File.Exists(RustPPModule.GetAbsoluteFilePath("friendsSave.rpp")))
            {
                File.Delete(RustPPModule.GetAbsoluteFilePath("friendsSave.rpp"));
            }
            if (Administrator.AdminList.Count != 0)
            {
                Logger.Log("Saving administrator list.");
                ObjectToXML<List<Administrator>>(Administrator.AdminList, RustPPModule.GetAbsoluteFilePath("admins.xml"));
            }
            else if (File.Exists(RustPPModule.GetAbsoluteFilePath("admins.xml")))
            {
                File.Delete(RustPPModule.GetAbsoluteFilePath("admins.xml"));
            }
            if (Core.userCache.Count != 0)
            {
                Logger.Log("Saving user cache.");
                ObjectToXML<SerializableDictionary<ulong, string>>(new SerializableDictionary<ulong, string>(Core.userCache), RustPPModule.GetAbsoluteFilePath("userCache.xml"));
                ObjectToFile<Dictionary<ulong, string>>(Core.userCache, RustPPModule.GetAbsoluteFilePath("cache.rpp"));
            }
            else if (File.Exists(RustPPModule.GetAbsoluteFilePath("cache.rpp")))
            {
                File.Delete(RustPPModule.GetAbsoluteFilePath("cache.rpp"));
            }
            if (Core.structureCache.Count != 0)
            {
                Logger.Log("Saving structure cache.");
                ObjectToXML<SerializableDictionary<int, string>>(new SerializableDictionary<int, string>(Core.structureCache), RustPPModule.GetAbsoluteFilePath("structureCache.xml"));
                ObjectToFile<Dictionary<int, string>>(Core.structureCache, RustPPModule.GetAbsoluteFilePath("structureCache.rpp"));
            }
            else if (File.Exists(RustPPModule.GetAbsoluteFilePath("structureCache.rpp")))
            {
                File.Delete(RustPPModule.GetAbsoluteFilePath("structureCache.rpp"));
            }
            if (Core.userLang.Count != 0)
            {
                Logger.Log("Guardando preferencias de lenguaje.");
                ObjectToXML<SerializableDictionary<ulong, string>>(new SerializableDictionary<ulong, string>(Core.userLang), RustPPModule.GetAbsoluteFilePath("userLang.xml"));
                ObjectToFile<Dictionary<ulong, string>>(Core.userLang, RustPPModule.GetAbsoluteFilePath("userLang.rpp"));
            }
            else if (File.Exists(RustPPModule.GetAbsoluteFilePath("userLang.rpp")))
            {
                File.Delete(RustPPModule.GetAbsoluteFilePath("userLang.rpp"));
            }
            if (Core.whiteList.Count != 0)
            {
                Logger.Log("Saving whitelist.");
                ObjectToXML<List<PList.Player>>(Core.whiteList.PlayerList, RustPPModule.GetAbsoluteFilePath("whitelist.xml"));
            }
            else if (File.Exists(RustPPModule.GetAbsoluteFilePath("whitelist.xml")))
            {
                File.Delete(RustPPModule.GetAbsoluteFilePath("whitelist.xml"));
            }
            if (Core.muteList.Count != 0)
            {
                Logger.Log("Saving mutelist.");
                ObjectToXML<List<PList.Player>>(Core.muteList.PlayerList, RustPPModule.GetAbsoluteFilePath("mutelist.xml"));
            }
            else if (File.Exists(RustPPModule.GetAbsoluteFilePath("mutelist.xml")))
            {
                File.Delete(RustPPModule.GetAbsoluteFilePath("mutelist.xml"));
            }
            if (Core.blackList.Count != 0)
            {
                Logger.Log("Saving blacklist.");
                ObjectToXML<List<PList.Player>>(Core.blackList.PlayerList, RustPPModule.GetAbsoluteFilePath("bans.xml"));
            }
            else if (File.Exists(RustPPModule.GetAbsoluteFilePath("bans.xml")))
            {
                File.Delete(RustPPModule.GetAbsoluteFilePath("bans.xml"));
            }
        }

        public static void Log(string logName, string msg)
        {
            File.AppendAllText(RustPPModule.GetAbsoluteFilePath(logName), string.Format("[{0} {1}] {2}\r\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), msg));
        }

        public static T ObjectFromFile<T>(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Binder = new MagmaToRustPPModuleDeserializationBinder();
            return (T)formatter.Deserialize(reader.BaseStream);
        }

        public static T ObjectFromXML<T>(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            TextReader textReader = new StreamReader(path);
            T local = (T)serializer.Deserialize(textReader);
            textReader.Close();
            return local;
        }

        public static void ObjectToFile<T>(T ht, string path)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Create);
                StreamWriter writer = new StreamWriter(stream);
                formatter.Serialize(writer.BaseStream, ht);
                writer.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError("[Rust++] " + path + " seems to be corrupted. Stop the server and delete the file. Error: " + ex);
            }
        }

        public static void ObjectToXML<T>(T obj, string path)
        {
            try
            { 
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                TextWriter textWriter = new StreamWriter(path);
                serializer.Serialize(textWriter, obj);
                textWriter.Close();
            }
            catch (Exception ex)
            {
                Logger.LogError("[Rust++] " + path + " seems to be corrupted. Stop the server and delete the file. Error: " + ex);
            }
        }
    }

    sealed class MagmaToRustPPModuleDeserializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToBind = null;
            if (assemblyName.StartsWith("Magma"))
            {
                string tFriendList = typeof(RustPP.Social.FriendList).ToString();
                string tFriend = tFriendList.Insert(tFriendList.Length, "+Friend");
                if (typeName == tFriendList)
                    assemblyName = Assembly.GetExecutingAssembly().FullName;

                if (typeName == tFriend)
                    assemblyName = Assembly.GetExecutingAssembly().FullName;
            }
            typeToBind = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
            return typeToBind;
        }
    }
}