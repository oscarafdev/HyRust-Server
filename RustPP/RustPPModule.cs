namespace RustPP
{
    using Fougerite;
    using Fougerite.Events;
    using Rust;
    using RustPP.Commands;
    using RustPP.Components.AuthComponent;
    using RustPP.Components.RaidComponent;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Timers;
    using UnityEngine;

    public class RustPPModule : Fougerite.Module
    {
        public override string Name
        {
            get { return "RustPP"; }
        }
        public override string Author
        {
            get { return "xEnt22, mikec, DreTaX"; }
        }
        public override string Description
        {
            get { return "Rust++ Legacy Module"; }
        }
        public override Version Version
        {
            get { return new Version("1.1.8.1"); }
        }

        public static string GetAbsoluteFilePath(string fileName)
        {
            return Path.Combine(ConfigsFolder, fileName);
        }

        public override uint Order
        {
            get { return uint.MinValue; }
        }

        public static string ConfigFile;
        public static string ConfigsFolder;
        public static string JoinMsg = "has joined the server";
        public static string LeaveMsg = "has left the server";
        public static string StarterMsg = "You have spawned a Starter Kit!";
        public static string StarterCDMsg = "You must wait awhile before using this..";

        public override void Initialize()
        {
            ConfigsFolder = ModuleFolder;
            ConfigFile = Path.Combine(ConfigsFolder, "Rust++.cfg");
            Core.Init();
            Core.config = new IniParser(ConfigFile);

            if (Core.config == null)
            {
                Logger.LogError("[RPP] Can't load config!");
                return;
            }
            string chatname = Core.config.GetSetting("Settings", "chatname");
            if (!string.IsNullOrEmpty(chatname))
            {
                Core.Name = Core.config.GetSetting("Settings", "chatname");
            }
            if (Core.config.ContainsSetting("Settings", "joinmsg"))
            {
                JoinMsg = Core.config.GetSetting("Settings", "joinmsg");
            }
            if (Core.config.ContainsSetting("Settings", "leavemsg"))
            {
                LeaveMsg = Core.config.GetSetting("Settings", "leavemsg");
            }
            if (Core.config.ContainsSetting("Settings", "startermsg"))
            {
                StarterMsg = Core.config.GetSetting("Settings", "startermg");
            }
            if (Core.config.ContainsSetting("Settings", "startercdmsg"))
            {
                StarterCDMsg = Core.config.GetSetting("Settings", "startercdmsg");
            }
            TimedEvents.startEvents();
            AuthComponent.Init();
            Fougerite.Hooks.OnDoorUse += DoorUse;
            Fougerite.Hooks.OnEntityHurt += EntityHurt;
            Fougerite.Hooks.OnPlayerConnected += PlayerConnect;
            Fougerite.Hooks.OnPlayerDisconnected += PlayerDisconnect;
            Fougerite.Hooks.OnPlayerHurt += PlayerHurt;
            Fougerite.Hooks.OnPlayerKilled += PlayerKilled;
            Fougerite.Hooks.OnServerShutdown += OnServerShutdown;
            Fougerite.Hooks.OnShowTalker += ShowTalker;
            Fougerite.Hooks.OnChatRaw += ChatReceived;
            Fougerite.Hooks.OnChat += Chat;
            Fougerite.Hooks.OnFallDamage += OnFallDamage;
            Fougerite.Hooks.OnServerSaved += OnServerSaved;
            Fougerite.Hooks.OnEntityDeployedWithPlacer += OnEntityDeployedWithPlacer;
            Server.GetServer().LookForRustPP();
            RaidComponent.initComponent();
            Components.EconomyComponent.EconomyComponent.InitComponent();
            Components.AdminComponent.AdminComponent.InitComponent();
            Components.FriendComponent.FriendComponent.InitComponent();
        }

        public override void DeInitialize()
        {
            Logger.LogDebug("DeInitialize RPP");

            Fougerite.Hooks.OnDoorUse -= DoorUse;
            Fougerite.Hooks.OnEntityHurt -= EntityHurt;
            Fougerite.Hooks.OnEntityDeployedWithPlacer -= OnEntityDeployedWithPlacer;
            Fougerite.Hooks.OnPlayerConnected -= PlayerConnect;
            Fougerite.Hooks.OnPlayerDisconnected -= PlayerDisconnect;
            Fougerite.Hooks.OnPlayerHurt -= PlayerHurt;
            Fougerite.Hooks.OnPlayerKilled -= PlayerKilled;
            Fougerite.Hooks.OnServerShutdown -= OnServerShutdown;
            Fougerite.Hooks.OnShowTalker -= ShowTalker;
            Fougerite.Hooks.OnChatRaw -= ChatReceived;
            Fougerite.Hooks.OnChat -= Chat;
            Fougerite.Hooks.OnFallDamage -= OnFallDamage;
            Fougerite.Hooks.OnServerSaved -= OnServerSaved;
            Components.AuthComponent.AuthComponent.Exit();
            TimedEvents.timer.Stop();
            RaidComponent.destroyComponent();
            Components.FriendComponent.FriendComponent.DestroyComponent();

            Logger.LogDebug("DeInitialized RPP");
        }
        void OnEntityDeployedWithPlacer(Fougerite.Player player, Entity entity, Fougerite.Player placer)
        {
            Logger.LogError($"Guardando: {entity.InstanceID}");
            RustPP.Core.structureCache.Add(entity.InstanceID,player.Name);
        }

        void OnServerSaved(int amount, double seconds)
        {
            Helper.CreateSaves();
        }

        void TimeEvent(object x, ElapsedEventArgs y)
        {
            TimedEvents.startEvents();
        }

        void ChatReceived(ref ConsoleSystem.Arg arg)
        {

            Fougerite.Player pl = Fougerite.Server.Cache[arg.argUser.userID];
            var command = ChatCommand.GetCommand("ir") as TeleportToCommand;
            if (IsSpam(arg.ArgsStr))
            {
                pl.SendClientMessage("[color red]<!>[/color]El SPAM no esta permitido en este servidor");
                arg.ArgsStr = string.Empty;
                return;
            }
            if (command.GetTPWaitList().Contains(pl.UID))
            {
                command.PartialNameTP(ref arg, arg.GetInt(0));
                arg.ArgsStr = string.Empty;
            }
            else if (Core.shareWaitList.Contains(pl.UID))
            {
                (ChatCommand.GetCommand("share") as ShareCommand).PartialNameDoorShare(ref arg, arg.GetInt(0));
                Core.shareWaitList.Remove(pl.UID);
                arg.ArgsStr = string.Empty;
            }  else if (Core.killWaitList.Contains(pl.UID))
            {
                (ChatCommand.GetCommand("kill") as KillCommand).PartialNameKill(ref arg, arg.GetInt(0));
                Core.killWaitList.Remove(pl.UID);
                arg.ArgsStr = string.Empty;
            } else if (Core.unfriendWaitList.Contains(pl.UID))
            {
                (ChatCommand.GetCommand("unfriend") as UnfriendCommand).PartialNameUnfriend(ref arg, arg.GetInt(0));
                Core.unfriendWaitList.Remove(pl.UID);
                arg.ArgsStr = string.Empty;
            } else if (Core.unshareWaitList.Contains(pl.UID))
            {
                (ChatCommand.GetCommand("unshare") as UnshareCommand).PartialNameUnshareDoors(ref arg, arg.GetInt(0));
                Core.unshareWaitList.Remove(pl.UID);
                arg.ArgsStr = string.Empty;
            } else if (Core.whiteWaitList.Contains(pl.UID))
            {
                (ChatCommand.GetCommand("addwl") as WhiteListAddCommand).PartialNameWhitelist(ref arg, arg.GetInt(0));
                Core.whiteWaitList.Remove(pl.UID);
                arg.ArgsStr = string.Empty;
            } else if (Core.adminAddWaitList.Contains(arg.argUser.userID))
            {
                (ChatCommand.GetCommand("addadmin") as AddAdminCommand).PartialNameNewAdmin(ref arg, arg.GetInt(0));
                Core.adminAddWaitList.Remove(arg.argUser.userID);
                arg.ArgsStr = string.Empty;
            } else if (Core.adminRemoveWaitList.Contains(pl.UID))
            {
                (ChatCommand.GetCommand("unadmin") as RemoveAdminCommand).PartialNameRemoveAdmin(ref arg, arg.GetInt(0));
                Core.adminRemoveWaitList.Remove(pl.UID);
                arg.ArgsStr = string.Empty;
            } else if (Core.adminFlagsWaitList.Contains(pl.UID))
            {
                (ChatCommand.GetCommand("getflags") as GetFlagsCommand).PartialNameGetFlags(ref arg, arg.GetInt(0));
                Core.adminFlagsWaitList.Remove(pl.UID);
                arg.ArgsStr = string.Empty;
            } else if (Core.muteWaitList.Contains(pl.UID))
            {
                (ChatCommand.GetCommand("mutear") as MuteCommand).PartialNameMute(ref arg, arg.GetInt(0));
                Core.muteWaitList.Remove(pl.UID);
                arg.ArgsStr = string.Empty;
            } else if (Core.adminFlagWaitList.Contains(pl.UID))
            {
                (ChatCommand.GetCommand("addflag") as AddFlagCommand).PartialNameAddFlags(ref arg, arg.GetInt(0));
                Core.adminFlagWaitList.Remove(pl.UID);
                arg.ArgsStr = string.Empty;
            } else if (Core.adminUnflagWaitList.Contains(pl.UID))
            {
                (ChatCommand.GetCommand("unflag") as RemoveFlagsCommand).PartialNameRemoveFlags(ref arg, arg.GetInt(0));
                Core.adminUnflagWaitList.Remove(pl.UID);
                arg.ArgsStr = string.Empty;
            }

            if (Core.IsEnabled())
                Core.handleCommand(ref arg);
        }

        bool IsSpam(string text)
        {
            if (text.Contains("net.connect") ||
                text.Contains("playznt") ||
                text.Contains("28015") ||
                text.Contains("ofirerust.ddns.net") ||
                text.Contains("ddns.net") ||
                text.Contains("ofirerust") ||
                text.Contains("paladium") ||
                text.Contains("chernobyl") ||
                text.Contains("gamingrust") ||
                text.Contains("duckdns.org") ||
                text.Contains("legionrust") ||
                text.Contains("legionrust") ||
                text.Contains("servegame.com") ||
                text.Contains(".com") ||
                text.Contains(".net") ||
                text.Contains(". net") ||
                text.Contains(". com") ||
                text.Contains("servegame"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        void Chat(Fougerite.Player p, ref ChatString text)
        {
            if (IsSpam(text))
            {
                p.SendClientMessage("[color red]<!>[/color]El SPAM no esta permitido en este servidor");
                text.NewText = string.Empty;
                return;
            }
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(p);
            if(user.TimeToChat >= 1)
            {
                p.SendClientMessage($"[color red]<!>[/color]Espera {user.TimeToChat} para enviar otro mensaje.");
                return;
            }
            if (Core.IsEnabled() && Core.muteList.Contains(p.UID))
            {
                text.NewText = "";
                p.MessageFrom(Core.Name, "Estás Muteado.");
                return;
            }
            user.TimeToChat += 5;
        }

        void OnFallDamage(FallDamageEvent falldamageevent)
        {
            if (Core.IsEnabled())
            {
                if (falldamageevent.Player != null)
                {
                    GodModeCommand command = (GodModeCommand) ChatCommand.GetCommand("god");
                    if (command.IsOn(falldamageevent.Player.UID))
                    {
                        falldamageevent.Cancel();
                    }
                }
            }
        }

        void ShowTalker(uLink.NetworkPlayer player, Fougerite.Player p)
        {
            if (!Core.IsEnabled())
                return;

            if (!Core.config.GetBoolSetting("Settings", "voice_notifications"))
                return;

            if (Fougerite.Hooks.talkerTimers.ContainsKey(p.UID))
            {
                if ((Environment.TickCount - ((int)Fougerite.Hooks.talkerTimers[p.UID])) < int.Parse(Core.config.GetSetting("Settings", "voice_notification_delay")))
                    return;

                Fougerite.Hooks.talkerTimers[p.UID] = Environment.TickCount;
            } else
            {   
                Fougerite.Hooks.talkerTimers.Add(p.UID, Environment.TickCount);
            }
            Notice.Inventory(player, p.Name);
        }

        void OnServerShutdown()
        {
            if (Core.IsEnabled()) Helper.CreateSaves();
        }

        void PlayerKilled(DeathEvent event2)
        {
            event2.DropItems = !RustPP.Hooks.KeepItem();
            if (event2.AttackerIsPlayer && event2.VictimIsPlayer)
            {
                if (event2.Attacker == null || event2.Victim == null)
                {
                    return;
                }
                Fougerite.Player attacker = event2.Attacker as Fougerite.Player;
                Fougerite.Player victim = event2.Victim as Fougerite.Player;
                if (attacker != victim)
                    RustPP.Hooks.broadcastDeath(victim.Name, attacker.Name, event2.WeaponName);
            }
        }

        void DoorUse(Fougerite.Player p, DoorEvent de)
        {
            if (Core.IsEnabled() && !de.Open)
            {
                ShareCommand command = ChatCommand.GetCommand("share") as ShareCommand;
                ArrayList list = (ArrayList)command.GetSharedDoors()[Convert.ToUInt64(de.Entity.OwnerID)];
                if (list == null)
                    de.Open = false;
                else if (list.Contains(p.UID))
                    de.Open = true;
                else
                    de.Open = false;
            }
        }

        void PlayerHurt(HurtEvent he)
        {
            if (RustPP.Hooks.IsFriend(he))
                he.DamageAmount = 0f;
        }

        void PlayerDisconnect(Fougerite.Player player)
        {
            if (Core.IsEnabled())
                RustPP.Hooks.logoffNotice(player);
        }
        internal static Dictionary<string, float> ConnectionsIncoming = new Dictionary<string, float>();
        void PlayerConnect(Fougerite.Player player)
        {

            Data.Entities.Connections conection = new Data.Entities.Connections
            {
                IP = player.IP,
                Name = player.Name,
                Time = Time.realtimeSinceStartup,
                Player = player
            };
            Data.Globals.IncommingConections.Add(conection);
            if (Data.Globals.IncommingConections.Count(x => x.IP == player.IP) >= 3)
            {
                float timeAtual = Time.realtimeSinceStartup;
                int matches = 0;
                foreach (Data.Entities.Connections Connection in Data.Globals.IncommingConections.FindAll(x => x.IP == player.IP))
                {
                    if(Connection.Name != player.Name)
                    {
                        float difference = timeAtual - conection.Time;

                        if (difference <= 5)
                        {
                            matches += 1;
                        }
                    }
                }
                if(matches >= 3)
                {
                    foreach (Data.Entities.Connections Connection in Data.Globals.IncommingConections.FindAll(x => x.IP == player.IP))
                    {
                        Connection.Player.Disconnect();
                        Fougerite.Server.GetServer().BanPlayerIP(Connection.IP);
                        Data.Globals.IncommingConections.Remove(Connection);
                    }
                    Server.GetServer().SendMessageForAll($"[color red]<Server>[/color] [color #f77777]Se detectó un ataque con bots desde la IP: {player.IP}. Bloqueando conexiones.");
                }
            }
            

            if (Core.IsEnabled())
                RustPP.Hooks.loginNotice(player);
        }

        void EntityHurt(HurtEvent he)
        {
            if (Core.IsEnabled())
            {
                if (!he.AttackerIsPlayer || he.Attacker == null)
                {
                    return;
                }
                InstaKOCommand command = ChatCommand.GetCommand("instako") as InstaKOCommand;
                InstaKOAllCommand command2 = ChatCommand.GetCommand("instakoall") as InstaKOAllCommand;
                Fougerite.Player pl = (Fougerite.Player)he.Attacker;
                if (command != null)
                {
                    if (command.IsOn(pl.UID))
                    {
                        if (he.Entity != null)
                        {
                            try
                            {
                                if (!he.IsDecay)
                                    he.Entity.Destroy();
                                else if (Fougerite.Hooks.decayList.Contains(he.Entity))
                                    Fougerite.Hooks.decayList.Remove(he.Entity);
                            }
                            catch (Exception ex)
                            {
                                Logger.LogDebug("EntityHurt EX: " + ex);
                            }
                        }
                        else Logger.LogDebug("he.Entity is null!");
                    }
                }
                if (command2 != null)
                {
                    if (command2.IsOn(pl.UID))
                    {
                        if (he.Entity != null)
                        {
                            var list = he.Entity.GetLinkedStructs();
                            foreach (var x in list)
                            {
                                try
                                {
                                    x.Destroy();
                                    if (Fougerite.Hooks.decayList.Contains(x))
                                        Fougerite.Hooks.decayList.Remove(x);
                                }
                                catch
                                {
                                    
                                }
                            }
                            try
                            {
                                he.Entity.Destroy();
                                if (Fougerite.Hooks.decayList.Contains(he.Entity))
                                    Fougerite.Hooks.decayList.Remove(he.Entity);
                            }
                            catch (Exception ex)
                            {
                                Logger.LogDebug("EntityHurt EX: " + ex);
                            }
                        }
                        else Logger.LogDebug("he.Entity is null!");
                    }
                }
            }
        }
    }
}
