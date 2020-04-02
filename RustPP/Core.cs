using System.Runtime.CompilerServices;

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
    using RustPP.Components.ClanComponent.Commands;
    using RustPP.Components.DonoComponent;
    using RustPP.Components.UtilityComponent;
    using RustPP.Components;

    public class Core
    {
        public static string Name = "HyRust";
        public static string Version = "0.8";
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
            RustPP.Data.Globals.LoadClans();
        }

        public static void handleCommand(ref ConsoleSystem.Arg arg)
        {
            string displayname = arg.argUser.user.Displayname;
            string[] strArray = arg.GetString(0).Trim().Split(new char[] { ' ' });
            string cmd = strArray[0].Trim().ToLower();
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
            ChatCommand.AddCommand("/ah", new AdminHelpCommand()); // Admin 1
            ChatCommand.AddCommand("/traer", new TeleportHereCommand()); // Nivel 1
            ChatCommand.AddCommand("/ir", new TeleportToCommand()); // Nivel 1
            ChatCommand.AddCommand("/a", new AdminChatCommand()); // Nivel 1
            ChatCommand.AddCommand("/mutear", new MuteCommand()); // Nivel 1
            
            ChatCommand.AddCommand("/desmutear", new UnmuteCommand()); // Nivel 1
            ChatCommand.AddCommand("/anuncio", new AnnounceCommand()); // Nivel 2
            ChatCommand.AddCommand("/god", new GodModeCommand()); // Nivel 2
            ChatCommand.AddCommand("/adminkit", new AdminKitCommand());  // Nivel 2
            ChatCommand.AddCommand("/day", new DayCommand());// Nivel 2
            ChatCommand.AddCommand("/ban", new BanCommand()); // Nivel 3
            ChatCommand.AddCommand("/ao", new AdminGeneralChatCommand()); // Admin 4
            ChatCommand.AddCommand("/payday", new PayDayCommand()); // Admin 4
            ChatCommand.AddCommand("/limpiarinv", new ClearInvCommand()); // Admin 3
            ChatCommand.AddCommand("/saveloc", new SaveLocationCommand()); // Admin 5
            ChatCommand.AddCommand("/i", new SpawnItemCommand()); // Admin 5
            ChatCommand.AddCommand("/dar", new GiveItemCommand()); // Admin 5
            ChatCommand.AddCommand("/daradmin", new DarAdminCommand()); // Nivel 6
            ChatCommand.AddCommand("/instako", new InstaKOCommand());
            ChatCommand.AddCommand("/kick", new KickCommand());
            ChatCommand.AddCommand("/unban", new UnbanCommand());
            ChatCommand.AddCommand("/instakoall", new InstaKOAllCommand());


            // AuthComponent
            ChatCommand.AddCommand("/login", new LoginCommand());
            ChatCommand.AddCommand("/report", new ErrorCommand("/reportar"));
            ChatCommand.AddCommand("/reportar", new ReportCommand());
            ChatCommand.AddCommand("/tp", new TPCommand());
            ChatCommand.AddCommand("/registro", new RegisterCommand());
            ChatCommand.AddCommand("/pagar", new PagarCommand());
            ChatCommand.AddCommand("/cuenta", new AccountCommand()); // Logged
            ChatCommand.AddCommand("/farm", new FarmCommand()); // Logged
            ChatCommand.AddCommand("/creditos", new AboutCommand());
            ChatCommand.AddCommand("/g", new ShoutCommand());
            ChatCommand.AddCommand("/duda", new DudaCommand());
            ChatCommand.AddCommand("/o", new GeneralChatCommand());
            ChatCommand.AddCommand("/report", new ErrorCommand("/reportar"));
            ChatCommand.AddCommand("/reportar", new ReportCommand());
            ChatCommand.AddCommand("/addfriend", new AddFriendCommand());
            ChatCommand.AddCommand("/r", new ReplyCommand());
            ChatCommand.AddCommand("/rules", new ErrorCommand("/reglas"));
            ChatCommand.AddCommand("/reglas", new RulesCommand());
            ChatCommand.AddCommand("/friends", new ErrorCommand("/amigos"));
            ChatCommand.AddCommand("/amigos", new FriendsCommand());
            ChatCommand.AddCommand("/help", new ErrorCommand("/ayuda"));
            ChatCommand.AddCommand("/ayuda", new HelpCommand());
            ChatCommand.AddCommand("/historial", new HistoryCommand());
            ChatCommand.AddCommand("/motd", new MOTDCommand());
            ChatCommand.AddCommand("/loc", new ErrorCommand("/ubicacion"));
            ChatCommand.AddCommand("/location", new ErrorCommand("/ubicacion"));
            ChatCommand.AddCommand("/ubicacion", new LocationCommand());
            ChatCommand.AddCommand("/ping", new PingCommand());
            ChatCommand.AddCommand("/players", new PlayersCommand());
            ChatCommand.AddCommand("/pm", new ErrorCommand("/w"));
            ChatCommand.AddCommand("/w", new PrivateMessagesCommand());
            ChatCommand.AddCommand("/share", new ShareCommand());
            ChatCommand.AddCommand("/starter", new StarterCommand());
            ChatCommand.AddCommand("/unfriend", new UnfriendCommand());
            ChatCommand.AddCommand("/unshare", new UnshareCommand());
            ChatCommand.AddCommand("/kit", new KitCommand());
            // Clans Component

            ChatCommand.AddCommand("/crearclan", new CreateClanCommand());
            ChatCommand.AddCommand("/clanes", new ClansCommand());
            ChatCommand.AddCommand("/clan", new ClanCommand());
            ChatCommand.AddCommand("/aceptar", new AceptarCommand());
            ChatCommand.AddCommand("/f", new ClanChatCommand());

            //DonoComponent
            ChatCommand.AddCommand("/dono", new ErrorCommand("/prop"));
            ChatCommand.AddCommand("/prop", new DonoCommand());

            //UtilityComponent
            ChatCommand.AddCommand("/fps", new FPSCommand());
            /* Dar Admin */
            AddAdminCommand command = new AddAdminCommand();
            command.AdminFlags = "CanAddAdmin";
            ChatCommand.AddCommand("/addadmin", command);
            /* Dar Flag */
            AddFlagCommand command2 = new AddFlagCommand();
            command2.AdminFlags = "CanAddFlags";
            ChatCommand.AddCommand("/addflag", command2);
            /* Anuncio */

            
            /* Ban */

            
            /* Obtener Flags */
            GetFlagsCommand command5 = new GetFlagsCommand();
            command5.AdminFlags = "CanGetFlags";
            ChatCommand.AddCommand("/getflags", command5);
            /* Dar Items */

            

            
            KillCommand command11 = new KillCommand();
            command11.AdminFlags = "CanKill";
            ChatCommand.AddCommand("/kill", command11);

            LoadoutCommand command12 = new LoadoutCommand();
            command12.AdminFlags = "CanLoadout";
            ChatCommand.AddCommand("/loadout", command12);

            
            
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


           
            
            RemoveFlagsCommand command21 = new RemoveFlagsCommand();
            command21.AdminFlags = "CanUnflag";
            ChatCommand.AddCommand("/unflag", command21);

            
            
            WhiteListAddCommand command23 = new WhiteListAddCommand();
            command23.AdminFlags = "CanWhiteList";
            ChatCommand.AddCommand("/addwl", command23);

            ShutDownCommand command24 = new ShutDownCommand();
            command24.AdminFlags = "CanShutdown";
            ChatCommand.AddCommand("/shutdown", command24);


            
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