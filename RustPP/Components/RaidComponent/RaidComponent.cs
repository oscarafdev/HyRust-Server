using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fougerite;
using Fougerite.Events;
using RustPP.Social;
using UnityEngine;

namespace RustPP.Components.RaidComponent
{
    class RaidComponent
    {
        public static Dictionary<ulong, double> OwnerTimeData;
        public static Dictionary<ulong, int> RaiderTime;
        public static int RaidTime = 20;
        public static int MaxRaidTime = 60;
        public static bool AllowAllModerators = false;
        public static bool CanOpenChestIfThereIsNoStructureClose = true;
        public static bool AutoWhiteListFriends = false;
        public static readonly List<string> WhiteListedIDs = new List<string>();
        public static readonly List<string> DSNames = new List<string>();
        public static readonly IEnumerable<string> Guns = new string[]
        {
            "M4", "MP5A4", "9mm Pistol", "Hunting Bow", "Bolt Action Rifle", "Shotgun", "Pipe Shotgun", "HandCannon",
            "P250", "Revolver"
        };
        public static IniParser Settings;
        public static string PathC;
        public static string PathLog;
        public static System.IO.StreamWriter file;

        public const string red = "[color #FF0000]";
        public const string yellow = "[color yellow]";
        public const string green = "[color green]";
        public const string orange = "[color #ffa500]";

        public static void initComponent()
        {
            if (!File.Exists(Path.Combine(RustPPModule.ConfigsFolder, "Logs.log"))) { File.Create(Path.Combine(RustPPModule.ConfigsFolder, "Logs.log")).Dispose(); }
            PathLog = Path.Combine(RustPPModule.ConfigsFolder, "Logs.log");
            PathC = Path.Combine(RustPPModule.ConfigsFolder, "LegitRaid.ini");
            RaiderTime = new Dictionary<ulong, int>();
            OwnerTimeData = new Dictionary<ulong, double>();
            if (!File.Exists(PathC))
            {
                File.Create(PathC).Dispose();
                Settings = new IniParser(PathC);
                Settings.AddSetting("Settings", "RaidTime", "20");
                Settings.AddSetting("Settings", "MaxRaidTime", "60");
                Settings.AddSetting("Settings", "AllowAllModerators", "false");
                Settings.AddSetting("Settings", "CanOpenChestIfThereIsNoStructureClose", "true");
                Settings.AddSetting("Settings", "DataStoreTables", "RaidComponent");
                Settings.AddSetting("Settings", "WhiteListedIDs", "76561197961872487");
                Settings.AddSetting("Settings", "AutoWhiteListFriends", "True");
                Settings.Save();
            }
            else
            {
                Settings = new IniParser(PathC);
                RaidTime = int.Parse(Settings.GetSetting("Settings", "RaidTime"));
                MaxRaidTime = int.Parse(Settings.GetSetting("Settings", "MaxRaidTime"));
                AllowAllModerators = Settings.GetBoolSetting("Settings", "AllowAllModerators");
                CanOpenChestIfThereIsNoStructureClose = Settings.GetBoolSetting("Settings", "CanOpenChestIfThereIsNoStructureClose");
                AutoWhiteListFriends = Settings.GetBoolSetting("Settings", "AutoWhiteListFriends");
                var Collect = Settings.GetSetting("Settings", "WhiteListedIDs");
                var splits = Collect.Split(Convert.ToChar(","));
                foreach (var x in splits)
                {
                    WhiteListedIDs.Add(x);
                }
                var Collect2 = Settings.GetSetting("Settings", "DataStoreTables");
                var splits2 = Collect2.Split(Convert.ToChar(","));
                foreach (var x in splits2)
                {
                    DSNames.Add(x);
                }
            }
            Fougerite.Hooks.OnLootUse += OnLootUse;
            Fougerite.Hooks.OnEntityDestroyed += OnEntityDestroyed;
            Fougerite.Hooks.OnEntityHurt += OnEntityHurt;
            Fougerite.Hooks.OnModulesLoaded += OnModulesLoaded;
            Fougerite.Hooks.OnCommand += OnCommand;
            Fougerite.Hooks.OnServerSaved += OnServerSaved;
            Fougerite.Hooks.OnEntityDeployedWithPlacer += OnEntityDeployedWithPlacer;
        }
        public static void destroyComponent()
        {
            Fougerite.Hooks.OnLootUse -= OnLootUse;
            Fougerite.Hooks.OnEntityDestroyed -= OnEntityDestroyed;
            Fougerite.Hooks.OnEntityDeployedWithPlacer -= OnEntityDeployedWithPlacer;
            Fougerite.Hooks.OnEntityHurt -= OnEntityHurt;
            Fougerite.Hooks.OnModulesLoaded -= OnModulesLoaded;
            Fougerite.Hooks.OnCommand -= OnCommand;
            Fougerite.Hooks.OnServerSaved -= OnServerSaved;
        }

        public static void OnEntityDeployedWithPlacer(Fougerite.Player player, Entity e, Fougerite.Player actualplacer)
        {
            if (actualplacer == null || e == null)
            {
                return;
            }
            if (!e.Name.ToLower().Contains("box") && !e.Name.ToLower().Contains("stash"))
            {
                return;
            }
            ulong id = GetHouseOwner(e.Location);
            if (id == 0UL)
            {
                return;
            }
            if (actualplacer.UID != id)
            {
                if (DataStore.GetInstance().Get("LegitRaidED", id) != null)
                {
                    List<string> list = (List<string>)DataStore.GetInstance().Get("LegitRaidED", id);
                    if (!list.Contains(actualplacer.PlayerClient.userID.ToString()))
                    {
                        list.Add(actualplacer.PlayerClient.userID.ToString());
                    }
                    DataStore.GetInstance().Add("LegitRaidED", id, list);
                }
                else
                {
                    List<string> list = new List<string>();
                    list.Add(actualplacer.PlayerClient.userID.ToString());
                    DataStore.GetInstance().Add("LegitRaidED", id, list);
                }
            }
        }

        

        public static void OnServerSaved(int amount, double seconds)
        {
            var instance = DataStore.GetInstance();
            instance.Flush("LOwnerTimeData");
            instance.Flush("LRaiderTime");
            foreach (var x in RaiderTime.Keys)
            {
                instance.Add("LRaiderTime", x, RaiderTime[x]);
            }
            foreach (var x in OwnerTimeData.Keys)
            {
                instance.Add("LOwnerTimeData", x, OwnerTimeData[x]);
            }
        }

        public static void OnCommand(Fougerite.Player player, string cmd, string[] args)
        {
            if (cmd == "legitraid")
            {
                
                if (player.Admin)
                {
                    Settings = new IniParser(PathC);
                    RaidTime = int.Parse(Settings.GetSetting("Settings", "RaidTime"));
                    MaxRaidTime = int.Parse(Settings.GetSetting("Settings", "MaxRaidTime"));
                    AllowAllModerators = Settings.GetBoolSetting("Settings", "AllowAllModerators");
                    CanOpenChestIfThereIsNoStructureClose = Settings.GetBoolSetting("Settings", "CanOpenChestIfThereIsNoStructureClose");
                    AutoWhiteListFriends = Settings.GetBoolSetting("Settings", "AutoWhiteListFriends");
                    var Collect = Settings.GetSetting("Settings", "WhiteListedIDs");
                    var splits = Collect.Split(Convert.ToChar(","));
                    WhiteListedIDs.Clear();
                    foreach (var x in splits)
                    {
                        WhiteListedIDs.Add(x);
                    }
                    var Collect2 = Settings.GetSetting("Settings", "DataStoreTables");
                    var splits2 = Collect2.Split(Convert.ToChar(","));
                    foreach (var x in splits2)
                    {
                        DSNames.Add(x);
                    }
                    player.MessageFrom("LegitRaid", "Reloaded!");
                }
            }
            else if (cmd == "flushlegita")
            {
                if (player.Admin)
                {
                    DataStore.GetInstance().Flush("LegitRaidED");
                    player.MessageFrom("LegitRaid", "Flushed!");
                }
            }
            else if (cmd == "raida")
            {
                if (player.Admin || (player.Moderator && AllowAllModerators) || (player.Moderator && WhiteListedIDs.Contains(player.SteamID)))
                {
                    bool contains = DataStore.GetInstance().ContainsKey("LegitRaidA", player.UID);
                    if (!contains)
                    {
                        DataStore.GetInstance().Add("LegitRaidA", player.UID, true);
                        player.MessageFrom("LegitRaid", "<!> Ahora puedes abrir todos los cofres.");
                        file = new System.IO.StreamWriter(PathLog, true);
                        file.WriteLine(DateTime.Now + " " + player.Name + "-" + player.SteamID + " ahora puede abrir todos los cofres.");
                        file.Close();
                    }
                    else
                    {
                        DataStore.GetInstance().Remove("LegitRaidA", player.UID);
                        player.MessageFrom("LegitRaid", "Disabled");
                        file = new System.IO.StreamWriter(PathLog, true);
                        file.WriteLine(DateTime.Now + " " + player.Name + "-" + player.SteamID + " quito el modo raid.");
                        file.Close();
                    }
                }
            }
        }

        public static void OnModulesLoaded()
        {
            var instance = DataStore.GetInstance();
            if (instance.GetTable("LRaiderTime") != null)
            {
                foreach (var x in instance.Keys("LRaiderTime"))
                {
                    try
                    {
                        RaiderTime[(ulong)x] = (int)instance.Get("LRaiderTime", x);
                    }
                    catch
                    {

                    }
                }
            }
            if (instance.GetTable("LOwnerTimeData") != null)
            {
                foreach (var x in instance.Keys("LOwnerTimeData"))
                {
                    try
                    {
                        OwnerTimeData[(ulong)x] = (double)instance.Get("LOwnerTimeData", x);
                    }
                    catch
                    {

                    }
                }
            }
            instance.Flush("LOwnerTimeData");
            instance.Flush("LRaiderTime");
        }
        
        public static void OnEntityHurt(HurtEvent he)
        {
            if (he.AttackerIsPlayer && he.VictimIsEntity)
            {
                if (he.Attacker != null && he.Entity != null)
                {
                    Fougerite.Entity entity = he.Entity;
                    Fougerite.Player player = (Fougerite.Player)he.Attacker;
                    Data.Entities.User user = Data.Globals.GetInternalUser(player);
                    if(user.SpectingOwner)
                    {
                        string OwnerName = Data.Globals.GetUserNameBySteamid(entity.OwnerID);
                        player.SendClientMessage($"Dueño: {entity.OwnerName} - {GetHouseOwner(entity.Location).ToString()}");
                        SetMaterialOfHouse(entity.Location);
                        user.SpectingOwner = false;
                    }
                    if (entity.Name.ToLower().Contains("box") || entity.Name.ToLower().Contains("stash"))
                    {
                        if (entity.IsStorage())
                        {
                            if(entity.hasInventory)
                            {
                                if (entity.Inventory.FreeSlots < entity.Inventory.Items.Length)
                                {
                                    he.DamageAmount = 0f;
                                    player.Notice("No puedes destruir cofres con cosas adentro.");
                                }
                            }
                        }
                    }
                }
            }
        }
        public static int GetMasterID(Vector3 entitypos)
        {
            RaycastHit cachedRaycast;
            StructureComponent cachedStructure;
            Collider cachedCollider;
            StructureMaster cachedMaster;
            Facepunch.MeshBatch.MeshBatchInstance cachedhitInstance;
            bool cachedBoolean;
            if (!Facepunch.MeshBatch.MeshBatchPhysics.Raycast(new Ray(entitypos, new Vector3(0f, -1f, 0f)),
                out cachedRaycast, out cachedBoolean, out cachedhitInstance))
            {
                return 0;
            }
            if (cachedhitInstance != null)
            {
                cachedCollider = cachedhitInstance.physicalColliderReferenceOnly;
                if (cachedCollider == null)
                {
                    return 0;
                }
                cachedStructure = cachedCollider.GetComponent<StructureComponent>();
                if (cachedStructure != null && cachedStructure._master != null)
                {
                    cachedMaster = cachedStructure._master;
                    int id = cachedMaster.GetInstanceID();
                    return id;
                }
            }
            return 0;
        }
        public static ulong GetHouseOwner(Vector3 entitypos)
        {
            RaycastHit cachedRaycast;
            StructureComponent cachedStructure;
            Collider cachedCollider;
            StructureMaster cachedMaster;
            Facepunch.MeshBatch.MeshBatchInstance cachedhitInstance;
            bool cachedBoolean;
            if (!Facepunch.MeshBatch.MeshBatchPhysics.Raycast(new Ray(entitypos, new Vector3(0f, -1f, 0f)),
                out cachedRaycast, out cachedBoolean, out cachedhitInstance))
            {
                return 0;
            }
            if (cachedhitInstance != null)
            {
                cachedCollider = cachedhitInstance.physicalColliderReferenceOnly;
                if (cachedCollider == null)
                {
                    return 0;
                }
                cachedStructure = cachedCollider.GetComponent<StructureComponent>();
                if (cachedStructure != null && cachedStructure._master != null)
                {
                    cachedMaster = cachedStructure._master;
                    var id = cachedMaster.ownerID;
                    return id;
                }
            } else
            {
                if (!Facepunch.MeshBatch.MeshBatchPhysics.Raycast(new Ray(entitypos, new Vector3(0f, -2f, 0f)),
                out cachedRaycast, out cachedBoolean, out cachedhitInstance))
                {
                    return 0;
                }
                if (cachedhitInstance != null)
                {
                    cachedCollider = cachedhitInstance.physicalColliderReferenceOnly;
                    if (cachedCollider == null)
                    {
                        return 0;
                    }
                    cachedStructure = cachedCollider.GetComponent<StructureComponent>();
                    if (cachedStructure != null && cachedStructure._master != null)
                    {
                        cachedMaster = cachedStructure._master;
                        var id = cachedMaster.ownerID;
                        return id;
                    }
                }
                else
                {
                    if (!Facepunch.MeshBatch.MeshBatchPhysics.Raycast(new Ray(entitypos, new Vector3(1f, 0f, 0f)),
                    out cachedRaycast, out cachedBoolean, out cachedhitInstance))
                    {
                        return 0;
                    }
                    if (cachedhitInstance != null)
                    {
                        cachedCollider = cachedhitInstance.physicalColliderReferenceOnly;
                        if (cachedCollider == null)
                        {
                            return 0;
                        }
                        cachedStructure = cachedCollider.GetComponent<StructureComponent>();
                        if (cachedStructure != null && cachedStructure._master != null)
                        {
                            cachedMaster = cachedStructure._master;
                            var id = cachedMaster.ownerID;
                            return id;
                        }
                    }
                    else
                    {
                        if (!Facepunch.MeshBatch.MeshBatchPhysics.Raycast(new Ray(entitypos, new Vector3(0f, 0f, 1f)),
                    out cachedRaycast, out cachedBoolean, out cachedhitInstance))
                        {
                            return 0;
                        }
                        if (cachedhitInstance != null)
                        {
                            cachedCollider = cachedhitInstance.physicalColliderReferenceOnly;
                            if (cachedCollider == null)
                            {
                                return 0;
                            }
                            cachedStructure = cachedCollider.GetComponent<StructureComponent>();
                            if (cachedStructure != null && cachedStructure._master != null)
                            {
                                cachedMaster = cachedStructure._master;
                                var id = cachedMaster.ownerID;
                                return id;
                            }
                        }
                    }
                }
            }
            return 0;
        }
        public static StructureMaster GetStructureMaster(Vector3 entitypos)
        {
            RaycastHit cachedRaycast;
            StructureComponent cachedStructure;
            Collider cachedCollider;
            StructureMaster cachedMaster;
            Facepunch.MeshBatch.MeshBatchInstance cachedhitInstance;
            bool cachedBoolean;
            if (!Facepunch.MeshBatch.MeshBatchPhysics.Raycast(new Ray(entitypos, new Vector3(0f, -1f, 0f)),
                out cachedRaycast, out cachedBoolean, out cachedhitInstance))
            {
                return null;
            }
            if (cachedhitInstance != null)
            {
                cachedCollider = cachedhitInstance.physicalColliderReferenceOnly;
                if (cachedCollider == null)
                {
                    return null;
                }
                cachedStructure = cachedCollider.GetComponent<StructureComponent>();
                if (cachedStructure != null && cachedStructure._master != null)
                {
                    cachedMaster = cachedStructure._master;
                    var id = cachedMaster.ownerID;
                    return cachedMaster;
                }
            }
            return null;
        }
        public static void SetMaterialOfHouse(Vector3 entitypos)
        {
            RaycastHit cachedRaycast;
            StructureComponent cachedStructure;
            Collider cachedCollider;
            StructureMaster cachedMaster;
            Facepunch.MeshBatch.MeshBatchInstance cachedhitInstance;
            bool cachedBoolean;
            if (!Facepunch.MeshBatch.MeshBatchPhysics.Raycast(new Ray(entitypos, new Vector3(0f, -1f, 0f)),
                out cachedRaycast, out cachedBoolean, out cachedhitInstance))
            {
                return; 
            }
            if (cachedhitInstance != null)
            {
                cachedCollider = cachedhitInstance.physicalColliderReferenceOnly;
                if (cachedCollider == null)
                {
                    return;
                }
                cachedStructure = cachedCollider.GetComponent<StructureComponent>();
                if (cachedStructure != null && cachedStructure._master != null)
                {
                    cachedMaster = cachedStructure._master;
                    cachedMaster.SetMaterialType(StructureMaster.StructureMaterialType.Brick);
                    var id = cachedMaster.ownerID;
                }
            }
        }

        public static void OnEntityDestroyed(DestroyEvent de)
        {
            if (de.Attacker != null && de.Entity != null && !de.IsDecay)
            {
                if (((Fougerite.Player)de.Attacker).UID == de.Entity.UOwnerID)
                {
                    return;
                }
                if ((de.WeaponName.ToLower().Contains("explosive") || de.WeaponName.ToLower().Contains("grenade")
                    || de.WeaponName.ToLower().Contains("hatchet") || de.WeaponName.ToLower().Contains("axe")
                    || de.WeaponName.ToLower().Contains("rock")) && (de.Entity.Name.ToLower().Contains("wall")
                    || de.Entity.Name.ToLower().Contains("door")))
                {
                    Fougerite.Entity entity = de.Entity;
                    OwnerTimeData[entity.UOwnerID] = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds;
                    //DataStore.GetInstance().Add("LegitRaidA", ((Fougerite.Player)de.Attacker).UID, true);
                    if (RaiderTime.ContainsKey(entity.UOwnerID))
                    {
                        if (MaxRaidTime < RaiderTime[entity.UOwnerID] + RaidTime)
                        {
                            RaiderTime[entity.UOwnerID] = MaxRaidTime;
                            return;
                        }
                        RaiderTime[entity.UOwnerID] = RaiderTime[entity.UOwnerID] + RaidTime;
                    }
                    else
                    {
                        RaiderTime[entity.UOwnerID] = RaidTime;
                    }
                }
            }
        }

        public static int GetMaster(Vector3 entitypos)
        {
            RaycastHit cachedRaycast;
            StructureComponent cachedStructure;
            Collider cachedCollider;
            StructureMaster cachedMaster;
            Facepunch.MeshBatch.MeshBatchInstance cachedhitInstance;
            bool cachedBoolean;
            if (!Facepunch.MeshBatch.MeshBatchPhysics.Raycast(new Ray(entitypos, new Vector3(0f, -1f, 0f)),
                out cachedRaycast, out cachedBoolean, out cachedhitInstance))
            {
                Logger.LogError($"Retornando: 0, 1");
                return 0;
            }

            if (cachedhitInstance != null)
            {
                cachedCollider = cachedhitInstance.physicalColliderReferenceOnly;
                if (cachedCollider == null)
                {
                    Logger.LogError($"Retornando: 0, 2");
                    return 0;
                }
                cachedStructure = cachedCollider.GetComponent<StructureComponent>();
                if (cachedStructure != null && cachedStructure._master != null)
                {
                    cachedMaster = cachedStructure._master;
                    Logger.LogError($"Foundation: {cachedMaster.GetMasterID()}");
                    return cachedMaster.GetInstanceID();
                }
                Logger.LogError($"Retornando cachedStructure: null");
                cachedMaster = cachedCollider.GetComponent<StructureMaster>();
                if(cachedMaster != null)
                {
                    Logger.LogError($"Foundation: {cachedMaster.GetMasterID()}");
                    return cachedMaster.GetInstanceID();
                }
                Logger.LogError($"Retornando cachedMaster: null");
            }
            Logger.LogError($"Retornando 0, cachedhitInstance = null");
            return 0;
        }
        public static void OnLootUse(LootStartEvent lootstartevent)
        {
            if (lootstartevent.Player == null)
            {
                return;
            }
            if (!lootstartevent.IsObject || DataStore.GetInstance().ContainsKey("LegitRaidA", lootstartevent.Player.UID)
                || DataStore.GetInstance().ContainsKey("HGIG", lootstartevent.Player.SteamID)) {
                Logger.LogError("Incumple 1");
                return;
            }
            if (DSNames.Any(table => DataStore.GetInstance().ContainsKey(table, lootstartevent.Player.SteamID) ||
                                     DataStore.GetInstance().ContainsKey(table, lootstartevent.Player.UID)))
            {
                Logger.LogError("Incumple 2");
                return;
            }
            if (!lootstartevent.Entity.IsStorage())
            {
                Logger.LogError("Incumple 3");
                return;
            }
            ulong owneruid = GetHouseOwner(lootstartevent.Entity.Location);
            if(owneruid == 0UL)
            {
                Logger.LogError("No se encuentra al dueño");
                return;
            }
            if (owneruid == lootstartevent.Player.UID)
            {
                Logger.LogError("Cumple 4");
                return;
            }
            if (CanOpenChestIfThereIsNoStructureClose)
            {
                Logger.LogError("Esta abierta");
                var objects = Physics.OverlapSphere(lootstartevent.Entity.Location, 3.8f);
                var names = new List<string>();
                foreach (var x in objects.Where(x => !names.Contains(x.name.ToLower())))
                {
                    names.Add(x.name.ToLower());
                }
                string ncollected = string.Join(" ", names.ToArray());
                if (ncollected.Contains("shelter") && !ncollected.Contains("door"))
                {
                    return;
                }
                if (!ncollected.Contains("meshbatch"))
                {
                    return;
                }
            }
            StructureMaster structureMaster = GetStructureMaster(lootstartevent.Entity.Location);
            Logger.LogError($"SM: InstanceID: {structureMaster.GetInstanceID()}");
            ulong id = GetHouseOwner(lootstartevent.Entity.Location);
            RustPP.Data.Entities.User owner = RustPP.Data.Globals.GetUserBySteamID(lootstartevent.Entity.OwnerName);
            RustPP.Data.Entities.User player = RustPP.Data.Globals.GetUserByName(lootstartevent.Player.Name);

            if(owner.ID == player.ID)
            {
                return;
            }

            if (FriendComponent.FriendComponent.IsFriendOf(owner, player))
            {
                Logger.LogError("Es amigo");
                return;
            }
            
            if (OwnerTimeData.ContainsKey(id))
            {
                Logger.LogError("Cumple 5");
                var ticks = OwnerTimeData[id];
                var calc = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds - ticks;
                int timeraid = RaidTime;
                if (RaiderTime.ContainsKey(id))
                {
                    timeraid = RaiderTime[id];
                }
                if (double.IsNaN(calc) || double.IsNaN(ticks))
                {
                    lootstartevent.Cancel();
                    lootstartevent.Player.Notice("", "Necesitas romper una pared con C4/Granadas en una pared y podrás raidear por " + RaidTime + " minutos!", 8f);
                    lootstartevent.Player.MessageFrom("LegitRaid", "[color cyan]<!>[/color] Si intentas abrir el cofre de un amigo el deberá usar /addfriend para darte permiso.");
                    //lootstartevent.Player.MessageFrom("LegitRaid", "After that tell him to type /friendraid !");
                    OwnerTimeData.Remove(id);
                    if (RaiderTime.ContainsKey(id))
                    {
                        RaiderTime.Remove(id);
                    }
                }
                if (calc >= (RaidTime + timeraid) * 60)
                {
                    lootstartevent.Cancel();
                    lootstartevent.Player.Notice("", "Necesitas romper una pared con C4/Granadas en una pared y podrás raidear por " + RaidTime + " minutos!", 8f);
                    lootstartevent.Player.MessageFrom("LegitRaid", "[color cyan]<!>[/color] Si intentas abrir el cofre de un amigo el deberá usar /addfriend para darte permiso.");
                    OwnerTimeData.Remove(id);
                    if (RaiderTime.ContainsKey(id))
                    {
                        RaiderTime.Remove(id);
                    }
                }
                else
                {
                    Logger.LogError("Cumple 5");
                    var done = Math.Round(calc);
                    lootstartevent.Player.Notice("¡Puedes lootear por " + ((timeraid * 60) - done) + " segundos!");
                }
                return;
            }
            else
            {
                Logger.LogError("incumple 6");
                if (DataStore.GetInstance().Get("LegitRaidED", id) != null)
                {
                    List<string> list = (List<string>)DataStore.GetInstance().Get("LegitRaidED", id);
                    if (list.Contains(id.ToString()) && OwnerTimeData.ContainsKey(id))
                    {
                        var ticks = OwnerTimeData[id];
                        var calc = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds - ticks;
                        int timeraid = RaidTime;
                        if (RaiderTime.ContainsKey(id))
                        {
                            timeraid = RaiderTime[id];
                        }
                        if (double.IsNaN(calc) || double.IsNaN(ticks))
                        {
                            lootstartevent.Cancel();
                            lootstartevent.Player.Notice("", "Necesitas romper una pared con C4/Granadas en una pared y podrás raidear por " + RaidTime + " minutos!", 8f);
                            lootstartevent.Player.MessageFrom("LegitRaid", "[color cyan]<!>[/color] Si intentas abrir el cofre de un amigo el deberá usar /addfriend para darte permiso.");
                            OwnerTimeData.Remove(id);
                            if (RaiderTime.ContainsKey(id))
                            {
                                RaiderTime.Remove(id);
                            }
                        }
                        if (calc >= (RaidTime + timeraid) * 60)
                        {
                            lootstartevent.Cancel();
                            lootstartevent.Player.Notice("", "Necesitas romper una pared con C4/Granadas en una pared y podrás raidear por " + RaidTime + " minutos!", 8f);
                            lootstartevent.Player.MessageFrom("LegitRaid", "[color cyan]<!>[/color] Si intentas abrir el cofre de un amigo el deberá usar /addfriend para darte permiso.");
                            OwnerTimeData.Remove(id);
                            if (RaiderTime.ContainsKey(id))
                            {
                                RaiderTime.Remove(id);
                            }
                        }
                        else
                        {
                            var done = Math.Round(calc);
                            lootstartevent.Player.Notice("¡Puedes lootear por " + ((timeraid * 60) - done) + " segundos!");
                        }
                        return;
                    }
                }
                lootstartevent.Cancel();
                lootstartevent.Player.Notice("", "Necesitas romper una pared con C4/Granadas en una pared y podrás raidear por " + RaidTime + " minutos!", 8f);
                lootstartevent.Player.MessageFrom("LegitRaid", "[color cyan]<!>[/color] Si intentas abrir el cofre de un amigo el deberá usar /addfriend para darte permiso.");
            }
        }
    
}
}
