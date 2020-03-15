﻿using System.Runtime.CompilerServices;

namespace RustPP
{
    using Fougerite;
    using RustPP.Commands;
    using RustPP.Permissions;
    using RustPP.Social;
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using MySql.Data.MySqlClient;
    using uLink;
    using RustPP.Components.AuthComponent;
    using RustPP.Commands.Chat;
    using RustPP.Components.AdminComponent.Commands;

    public class Core
    {
        public static string Name = "Rust++";
        public static string Version = "1.1.8.0";
        public static IniParser config;
        public static PList blackList = new PList();
        public static PList whiteList = new PList();
        public static PList muteList = new PList();
        public static List<ulong> tempConnect = new List<ulong>();
        public static Dictionary<ulong, string> userCache;
        public static Hashtable banWaitList = new Hashtable();
        public static Hashtable unbanWaitList = new Hashtable();
        public static Hashtable kickWaitList = new Hashtable();
        public static Hashtable killWaitList = new Hashtable();
        public static Hashtable whiteWaitList = new Hashtable();
        public static Hashtable adminFlagsWaitList = new Hashtable();
        public static Hashtable adminFlagWaitList = new Hashtable();
        public static Hashtable adminFlagsList = new Hashtable();
        public static Hashtable adminUnflagWaitList = new Hashtable();
        public static Hashtable adminAddWaitList = new Hashtable();
        public static Hashtable adminRemoveWaitList = new Hashtable();
        public static Hashtable muteWaitList = new Hashtable();
        public static Hashtable unmuteWaitList = new Hashtable();
        public static Hashtable friendWaitList = new Hashtable();
        public static Hashtable unfriendWaitList = new Hashtable();
        public static Hashtable shareWaitList = new Hashtable();
        public static Hashtable unshareWaitList = new Hashtable();

        public static void Init()
        {
            InitializeCommands();
            ShareCommand command = ChatCommand.GetCommand("share") as ShareCommand;
            FriendsCommand command2 = ChatCommand.GetCommand("amigos") as FriendsCommand;
            bool success = false;
            if (File.Exists(RustPPModule.GetAbsoluteFilePath("doorsSave.xml")))
            {
                SerializableDictionary<ulong, List<ulong>> doorsDict;
                doorsDict = Helper.ObjectFromXML<SerializableDictionary<ulong, List<ulong>>>(RustPPModule.GetAbsoluteFilePath("doorsSave.xml"));
                Hashtable doorsSave = new Hashtable();
                foreach (KeyValuePair<ulong, List<ulong>> kvp in doorsDict)
                {
                    ArrayList arr = new ArrayList(kvp.Value);
                    doorsSave.Add(kvp.Key, arr);
                }
                command.SetSharedDoors(doorsSave);
                success = true;
            }
            if (File.Exists(RustPPModule.GetAbsoluteFilePath("doorsSave.rpp")) && !success)
                command.SetSharedDoors(Helper.ObjectFromFile<Hashtable>(RustPPModule.GetAbsoluteFilePath("doorsSave.rpp")));

            if (!File.Exists(RustPPModule.GetAbsoluteFilePath("doorsSave.xml")))
            {
                SerializableDictionary<ulong, List<ulong>> doorsSave = new SerializableDictionary<ulong, List<ulong>>();
                foreach (DictionaryEntry entry in command.GetSharedDoors())
                {
                    ulong key = (ulong)entry.Key;
                    ArrayList value = (ArrayList)entry.Value;
                    List<ulong> list = new List<ulong>(value.OfType<ulong>());
                    doorsSave.Add(key, list);
                }
                Helper.ObjectToXML<SerializableDictionary<ulong, List<ulong>>>(doorsSave, RustPPModule.GetAbsoluteFilePath("doorsSave.xml"));
            }
            if (File.Exists(RustPPModule.GetAbsoluteFilePath("friendsSave.rpp")))
            {
                command2.SetFriendsLists(Helper.ObjectFromFile<Hashtable>(RustPPModule.GetAbsoluteFilePath("friendsSave.rpp")));
            }
            if (File.Exists(RustPPModule.GetAbsoluteFilePath("admins.xml")))
            {
                Administrator.AdminList = Helper.ObjectFromXML<List<Administrator>>(RustPPModule.GetAbsoluteFilePath("admins.xml"));
            }
            success = false;
            if (File.Exists(RustPPModule.GetAbsoluteFilePath("userCache.xml")))
            {
                FileInfo fi = new FileInfo(RustPPModule.GetAbsoluteFilePath("userCache.xml"));
                float mega = (fi.Length / 1024f) / 1024f;
                if (mega > 0.70)
                {
                    Logger.LogWarning("Rust++ Cache.xml and Cache.rpp is getting big. Deletion is suggested.");
                }
                SerializableDictionary<ulong, string> userDict = Helper.ObjectFromXML<SerializableDictionary<ulong, string>>(RustPPModule.GetAbsoluteFilePath("userCache.xml"));
                userCache = new Dictionary<ulong, string>(userDict);
                success = true;
            }
            if (File.Exists(RustPPModule.GetAbsoluteFilePath("cache.rpp")) && !success)
            {
                userCache = Helper.ObjectFromFile<Dictionary<ulong, string>>(RustPPModule.GetAbsoluteFilePath("cache.rpp"));
                if (!File.Exists(RustPPModule.GetAbsoluteFilePath("userCache.xml")))
                    Helper.ObjectToXML<SerializableDictionary<ulong, string>>(new SerializableDictionary<ulong, string>(userCache), RustPPModule.GetAbsoluteFilePath("userCache.xml"));
            }
            else if (!success)
            {
                userCache = new Dictionary<ulong, string>();
            }
            if (File.Exists(RustPPModule.GetAbsoluteFilePath("whitelist.xml")))
            {
                whiteList = new PList(Helper.ObjectFromXML<List<PList.Player>>(RustPPModule.GetAbsoluteFilePath("whitelist.xml")));
            } else
            {
                whiteList = new PList();
            }
            if (File.Exists(RustPPModule.GetAbsoluteFilePath("mutelist.xml")))
            {
                muteList = new PList(Helper.ObjectFromXML<List<PList.Player>>(RustPPModule.GetAbsoluteFilePath("mutelist.xml")));
            } else
            {
                muteList = new PList();
            }
            if (File.Exists(RustPPModule.GetAbsoluteFilePath("bans.xml")))
            {
                blackList = new PList(Helper.ObjectFromXML<List<PList.Player>>(RustPPModule.GetAbsoluteFilePath("bans.xml")));
            } else
            {
                blackList = new PList();
            }
        }

        public static void handleCommand(ref ConsoleSystem.Arg arg)
        {
            string displayname = arg.argUser.user.Displayname;
            string[] strArray = arg.GetString(0).Trim().Split(new char[] { ' ' });
            string cmd = strArray[0].Trim();
            string[] chatArgs = new string[strArray.Length - 1];
            for (int i = 1; i < strArray.Length; i++)
            {
                chatArgs[i - 1] = strArray[i];
            }
            ChatCommand.CallCommand(cmd, ref arg, ref chatArgs);
        }

        private static void InitializeCommands()
        {
            // AdminComponent
            ChatCommand.AddCommand("/daradmin", new DarAdminCommand());
            ChatCommand.AddCommand("/adminkit", new AdminKitCommand());
            ChatCommand.AddCommand("/traer", new TeleportHereCommand());
            ChatCommand.AddCommand("/ir", new TeleportToCommand());
            ChatCommand.AddCommand("/a", new AdminChatCommand());
            ChatCommand.AddCommand("/saveloc", new SaveLocationCommand());
            ChatCommand.AddCommand("/limpiarinv", new ClearInvCommand());
            // AuthComponent
            ChatCommand.AddCommand("/login", new LoginCommand());
            ChatCommand.AddCommand("/registro", new RegisterCommand());
            ChatCommand.AddCommand("/cuenta", new AccountCommand()); // Logged
            ChatCommand.AddCommand("/farm", new FarmCommand()); // Logged
            ChatCommand.AddCommand("/creditos", new AboutCommand());
            ChatCommand.AddCommand("/g", new ShoutCommand());
            ChatCommand.AddCommand("/duda", new DudaCommand());
            ChatCommand.AddCommand("/agregar", new AddFriendCommand());
            ChatCommand.AddCommand("/r", new ReplyCommand());
            ChatCommand.AddCommand("/reglas", new RulesCommand());
            ChatCommand.AddCommand("/amigos", new FriendsCommand());
            ChatCommand.AddCommand("/ayuda", new HelpCommand());
            ChatCommand.AddCommand("/history", new HistoryCommand());
            ChatCommand.AddCommand("/motd", new MOTDCommand());
            ChatCommand.AddCommand("/location", new LocationCommand());
            ChatCommand.AddCommand("/ping", new PingCommand());
            ChatCommand.AddCommand("/players", new PlayersCommand());
            ChatCommand.AddCommand("/w", new PrivateMessagesCommand());
            ChatCommand.AddCommand("/share", new ShareCommand());
            ChatCommand.AddCommand("/starter", new StarterCommand());
            ChatCommand.AddCommand("/unfriend", new UnfriendCommand());
            ChatCommand.AddCommand("/unshare", new UnshareCommand());

            /* Dar Admin */
            AddAdminCommand command = new AddAdminCommand();
            command.AdminFlags = "CanAddAdmin";
            ChatCommand.AddCommand("/addadmin", command);
            /* Dar Flag */
            AddFlagCommand command2 = new AddFlagCommand();
            command2.AdminFlags = "CanAddFlags";
            ChatCommand.AddCommand("/addflag", command2);
            /* Anuncio */
            AnnounceCommand command3 = new AnnounceCommand();
            command3.AdminFlags = "CanAnnounce";
            ChatCommand.AddCommand("/announce", command3);
            /* Ban */
            BanCommand command4 = new BanCommand();
            command4.AdminFlags = "CanBan";
            ChatCommand.AddCommand("/ban", command4);
            /* Obtener Flags */
            GetFlagsCommand command5 = new GetFlagsCommand();
            command5.AdminFlags = "CanGetFlags";
            ChatCommand.AddCommand("/getflags", command5);
            /* Dar Items */
            GiveItemCommand command6 = new GiveItemCommand();
            command6.AdminFlags = "CanGiveItem";
            ChatCommand.AddCommand("/dar", command6);
            /* Dar Flag */
            GodModeCommand command7 = new GodModeCommand();
            command7.AdminFlags = "CanGodMode";
            ChatCommand.AddCommand("/god", command7);
            
            SpawnItemCommand command8 = new SpawnItemCommand();
            command8.AdminFlags = "CanSpawnItem";
            ChatCommand.AddCommand("/i", command8);

            InstaKOCommand command9 = new InstaKOCommand();
            command9.AdminFlags = "CanInstaKO";
            ChatCommand.AddCommand("/instako", command9);

            KickCommand command10 = new KickCommand();
            command10.AdminFlags = "CanKick";
            ChatCommand.AddCommand("/kick", command10);

            KillCommand command11 = new KillCommand();
            command11.AdminFlags = "CanKill";
            ChatCommand.AddCommand("/kill", command11);

            LoadoutCommand command12 = new LoadoutCommand();
            command12.AdminFlags = "CanLoadout";
            ChatCommand.AddCommand("/loadout", command12);
            
            MuteCommand command13 = new MuteCommand();
            command13.AdminFlags = "CanMute";
            ChatCommand.AddCommand("/mute", command13);
            
            ReloadCommand command14 = new ReloadCommand();
            command14.AdminFlags = "CanReload";
            ChatCommand.AddCommand("/reload", command14);

            RemoveAdminCommand command15 = new RemoveAdminCommand();
            command15.AdminFlags = "CanDeleteAdmin";
            ChatCommand.AddCommand("/unadmin", command15);
            
            SaveAllCommand command16 = new SaveAllCommand();
            command16.AdminFlags = "CanSaveAll";
            ChatCommand.AddCommand("/saveall", command16);

            MasterAdminCommand command17 = new MasterAdminCommand();
            command17.AdminFlags = "RCON";
            ChatCommand.AddCommand("/setmasteradmin", command17);


            

            UnbanCommand command20 = new UnbanCommand();
            command20.AdminFlags = "CanUnban";
            ChatCommand.AddCommand("/unban", command20);

            RemoveFlagsCommand command21 = new RemoveFlagsCommand();
            command21.AdminFlags = "CanUnflag";
            ChatCommand.AddCommand("/unflag", command21);

            UnmuteCommand command22 = new UnmuteCommand();
            command22.AdminFlags = "CanUnmute";
            ChatCommand.AddCommand("/unmute", command22);
            
            WhiteListAddCommand command23 = new WhiteListAddCommand();
            command23.AdminFlags = "CanWhiteList";
            ChatCommand.AddCommand("/addwl", command23);

            ShutDownCommand command24 = new ShutDownCommand();
            command24.AdminFlags = "CanShutdown";
            ChatCommand.AddCommand("/shutdown", command24);

            InstaKOAllCommand command25 = new InstaKOAllCommand();
            command25.AdminFlags = "CanInstaKOAll";
            ChatCommand.AddCommand("/instakoall", command25);
        }

        public static bool IsEnabled()
        {
            return config.GetBoolSetting("Settings", "rust++_enabled");
        }

        public static void motd(Fougerite.Player player)
        {
            if (config.GetBoolSetting("Settings", "motd"))
            {
                int num = 1;
                do
                {
                    string setting = config.GetSetting("Settings", "motd" + num);
                    if (setting != null)
                    {
                        player.MessageFrom(Core.Name, setting);
                        num++;
                    }
                    else
                    {
                        num = 0;
                    }
                }
                while (num != 0);
            }
        }
    }
}