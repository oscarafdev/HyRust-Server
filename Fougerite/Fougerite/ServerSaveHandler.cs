using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Facepunch.Clocks.Counters;
using Google.ProtocolBuffers.Serialization;
using RustProto;
using RustProto.Helpers;
using UnityEngine;
using Avatar = RustProto.Avatar;
using Debug = UnityEngine.Debug;

namespace Fougerite
{
    /// <summary>
    /// This class uses MonoBehaviour's invoke method, and using
    /// BackGroundWorker to make sure that the server won't lagg when the server
    /// has a huge amount of objects, and doesn't cause thread problems.
    /// Based on Salva's method, tested with a map that has 80007 objects.
    /// This is due to legacy's shitty saving system that caused a lot of troubles
    /// back in the official server's day too.
    /// </summary>
    public class ServerSaveHandler : MonoBehaviour
    {
        internal SystemTimestamp timestamp2;
        internal SystemTimestamp timestamp3;
        internal SystemTimestamp timestamp4;
        internal static ServerSaveManager s;
        internal WorldSave.Builder builder;
        internal WorldSave fsave;
        internal SystemTimestamp restart = SystemTimestamp.Restart;
        internal string path;

        internal static readonly Dictionary<ServerSave, byte> UnProcessedSaves = new Dictionary<ServerSave, byte>();

        /// <summary>
        /// Returns the current saving filepath.
        /// </summary>
        public static string ServerSavePath
        {
            get;
            internal set;
        }

        /// <summary>
        /// Tells if the server is saving the map.
        /// </summary>
        public static bool ServerIsSaving
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or Sets the Server's Save time in minutes.
        /// DO NOT Set this to 0.
        /// </summary>
        public static int ServerSaveTime
        {
            get;
            set;
        }

        /// <summary>
        /// How many save copies should we store on the server.
        /// Do not set this below 5.
        /// </summary>
        public static int SaveCopies
        {
            get;
            set;
        }

        /// <summary>
        /// Returns you the last DateTime when the server was saved.
        /// </summary>
        public static DateTime LastSaveTime
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the next DateTime when the server will be saved.
        /// </summary>
        public static DateTime NextServerSaveTime
        {
            get;
            set;
        }

        /// <summary>
        /// Stop the server if the try-catch block catches an error?
        /// </summary>
        public static bool StopServerOnSaveFail
        {
            get;
            set;
        }

        /// <summary>
        /// Ensure that manual save won't be executed, if the server is going to autosave? How many minutes should be the critical point
        /// where the manual save won't run if less than X minutes is left before autosave? This should be LESS than 'SaveTime'. 0 to disable.
        /// </summary>
        public static int CrucialSavePoint
        {
            get;
            set;
        }

        /// <summary>
        /// Saves the server without BackgroundWorker.
        /// </summary>
        public void ManualSave()
        {
            SalvarMapaSemBackgroundWorker();
        }

        /// <summary>
        /// Saves the server with BackgroundWorker.
        /// </summary>
        public void ManualBackGroundSave()
        {
            SalvarMapaComBackgroundWorker();
        }

        /// <summary>
        /// Runs when the component loaded.
        /// </summary>
        private void Start()
        {
            ServerSavePath = ServerSaveManager.autoSavePath;
            LastSaveTime = DateTime.Now;
            NextServerSaveTime = LastSaveTime.AddMinutes(ServerSaveTime);
            Invoke(nameof(SalvarMapaComBackgroundWorker), ServerSaveTime * 60);
        }

        // funcoes decisivas 1
        private void SalvarMapaSemBackgroundWorker()
        {
            Logger.Log("SalvarMapaSemBackgroundWorker");
            try
            {
                if (ProcessoDeSalvamento())
                {
                    SaveServer(null, null);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("[ServerSaveHandler Error2] " + ex);
            }
        }
        private void SalvarMapaComBackgroundWorker()
        {
            Logger.Log("SalvarMapaComBackgroundWorker");
            try
            {
                if (ProcessoDeSalvamento())
                {
                    BackgroundWorker BGW = new BackgroundWorker();
                    BGW.DoWork += new DoWorkEventHandler(SaveServer);
                    BGW.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("[ServerSaveHandler Error1] " + ex);
            }
        }

        // funcoes normais
        private void SaveServer(object sender, DoWorkEventArgs e)
        {
            Logger.Log("SaveServer");
            try
            {
                Logger.Log("SaveScene...");
                SaveScene(ref builder);
                Logger.Log("SaveInstances...");
                SaveInstances(ref builder);
                Logger.Log("Next saves...");

                timestamp2.Stop();
                timestamp3 = SystemTimestamp.Restart;
                fsave = builder.Build();
                timestamp3.Stop();
                int num = fsave.SceneObjectCount + fsave.InstanceObjectCount;

                Logger.Log("Salvando arquivo..." + path + " e seus backup.");

                SystemTimestamp timestamp5 = timestamp4 = SystemTimestamp.Restart;
                using (FileStream stream2 = File.Open(path + ".new", FileMode.Create, FileAccess.Write))
                {
                    fsave.WriteTo(stream2);
                    stream2.Flush();
                }

                timestamp4.Stop();
                if (File.Exists(path + ".old." + (SaveCopies + 1)))
                {
                    File.Delete(path + ".old." + (SaveCopies + 1));
                }

                for (int i = SaveCopies; i >= 0; i--)
                {
                    if (File.Exists(path + ".old." + i))
                    {
                        File.Move(path + ".old." + i, path + ".old." + (i + 1));
                    }
                }

                if (File.Exists(path))
                {
                    File.Move(path, path + ".old.0");
                }

                if (File.Exists(path + ".new"))
                {
                    File.Move(path + ".new", path);
                }
                timestamp5.Stop();
                restart.Stop();

                if (Hooks.IsShuttingDown)
                {
                    ServerIsSaving = false;
                    Logger.Log(string.Concat(new object[] { " Saved ", num, " Object(s). Took ", restart.ElapsedSeconds, " seconds." }));
                    //ProjectX.ProjectX.BroadCast(ProjectX.ProjectX.configServer.NameServer, "O servidor salvou [color green]" + num + "[/color] Objetos do mapa");
                    return;
                }

                Loom.QueueOnMainThread(() =>
                {
                    if (save.profile)
                    {
                        object[] args = new object[]
                        {
                            num, timestamp2.ElapsedSeconds,
                            timestamp2.ElapsedSeconds / restart.ElapsedSeconds, timestamp3.ElapsedSeconds,
                            timestamp3.ElapsedSeconds / restart.ElapsedSeconds, timestamp4.ElapsedSeconds,
                            timestamp4.ElapsedSeconds / restart.ElapsedSeconds, timestamp5.ElapsedSeconds,
                            timestamp5.ElapsedSeconds / restart.ElapsedSeconds, restart.ElapsedSeconds,
                            restart.ElapsedSeconds / restart.ElapsedSeconds
                        };
                        Logger.Log(string.Format(" Saved {0} Object(s) [times below are in elapsed seconds]\r\n  Logic:\t{1,-16:0.000000}\t{2,7:0.00%}\r\n  Build:\t{3,-16:0.000000}\t{4,7:0.00%}\r\n  Stream:\t{5,-16:0.000000}\t{6,7:0.00%}\r\n  All IO:\t{7,-16:0.000000}\t{8,7:0.00%}\r\n  Total:\t{9,-16:0.000000}\t{10,7:0.00%}", args));
                    }
                    else
                    {
                        Logger.Log(string.Concat(new object[] { " Saved ", num, " Object(s). Took ", restart.ElapsedSeconds, " seconds." }));
                        //ProjectX.ProjectX.BroadCast(ProjectX.ProjectX.configServer.NameServer, "O servidor salvou [color green]" + num + "[/color] Objetos do mapa");
                    }

                    Hooks.OnServerSaveEvent(num, restart.ElapsedSeconds);
                    ServerIsSaving = false;
                    LastSaveTime = DateTime.Now;

                    // Process the unprocessed hashset values here without causing HashSet modified error.
                    List<ServerSave> RemovableKeys = new List<ServerSave>();

                    foreach (ServerSave x in UnProcessedSaves.Keys)
                    {
                        try
                        {
                            if (UnProcessedSaves.ContainsKey(x))
                            {
                                byte value = UnProcessedSaves[x];
                                if (value == 1)
                                {
                                    if (ServerSaveManager.Instances.registers.Add(x))
                                    {
                                        ServerSaveManager.Instances.ordered.Add(x);
                                    }

                                    ServerSaveManager.Instances.ordered.Add(x);
                                }
                                else
                                {
                                    if (ServerSaveManager.Instances.registers.Remove(x))
                                    {
                                        ServerSaveManager.Instances.ordered.Remove(x);
                                    }
                                }

                                RemovableKeys.Add(x);
                            }
                        }
                        catch (KeyNotFoundException ex)
                        {
                            Logger.LogError("[RegisterHook KeyNotFoundException] " + ex);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("[RegisterHook Error] " + ex);
                        }
                    }

                    foreach (var x in RemovableKeys)
                    {
                        UnProcessedSaves.Remove(x);
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogError("[ServerSaveHandler Error 0x2] " + ex);
                ServerIsSaving = false;
                LastSaveTime = DateTime.Now;
                NextServerSaveTime = LastSaveTime.AddMinutes(ServerSaveTime);
                if (StopServerOnSaveFail)
                {
                    Logger.LogWarning("[Fougerite WorldSave] We have caught an error. Killing server as requested.");
                    Process.GetCurrentProcess().Kill();
                }
            }
            finally
            {
                if (e != null && sender != null)
                {
                    Invoke(nameof(SalvarMapaComBackgroundWorker), ServerSaveTime * 60);
                }
            }
        }
        private void SaveScene(ref WorldSave.Builder save)
        {
            if (s.keys != null)
            {
                using (Recycler<SavedObject, SavedObject.Builder> recycler = SavedObject.Recycler())
                {
                    SavedObject.Builder saveobj = recycler.OpenBuilder();
                    for (int i = 0; i < s.keys.Length; i++)
                    {
                        int num2 = s.keys[i];
                        ServerSave save2 = s.values[i];
                        if (save2 != null)
                        {
                            try
                            {
                                saveobj.Clear();
                                saveobj.SetId(num2);
                                save2.SaveServerSaveables(ref saveobj);
                                save.AddSceneObject(saveobj);
                            }
                            catch (Exception ex)
                            {
                                string infoObj = "";
                                try
                                {
                                    infoObj = "_OwnerID: " + saveobj.StructMaster.OwnerID.ToString() + " (" + saveobj.Coords.Pos.X + ", " + saveobj.Coords.Pos.Y + ", " + saveobj.Coords.Pos.Z + ")";
                                }
                                catch (Exception ex2)
                                {
                                    Logger.LogError("[ServerSAveHandler] SaveServerSaveables _OwnerID Error");
                                }
                                Logger.LogError("[ServerSAveHandler] SaveServerSaveables Error " + infoObj);
                            }

                        }
                    }
                }
            }
        }
        private void SaveInstances(ref WorldSave.Builder save)
        {
            using (Recycler<SavedObject, SavedObject.Builder> recycler = SavedObject.Recycler())
            {
                if (recycler == null)
                {
                    Logger.LogError("[Fougerite WorldSave] Recycler is null, what the hell?");
                    return;
                }
                SavedObject.Builder builder = recycler.OpenBuilder();
                int num = -2147483648;
                List<ServerSave> CopiedList = new List<ServerSave>();

                int count = ServerSaveManager.Instances.All.Count;
                for (int i = 0; i < count; i++)
                {

                    // Ensure check, since the list can change during save.
                    if (ServerSaveManager.Instances.All.Count <= i) break;

                    ServerSave save2 = ServerSaveManager.Instances.All[i];

                    if (save2 != null && save2.gameObject != null)
                    {
                        //StructureMaster element = save2.gameObject.GetComponent<StructureMaster>();

                        //if (element)
                        //{
                        //    if (element._structureComponents != null && element._structureComponents.Count > 0) {
                        //        CopiedList.Add(save2);
                        //    } else
                        //    {
                        //        Logger.Log("[Save Error] item bugado removido =  ownerID:" + element.ownerID + " " + element.name + " " + element.transform.position.ToString());
                        //        element.OnDestroy();
                        //    }
                        //} else
                        //{
                        CopiedList.Add(save2);
                        //}
                    }
                }

                foreach (ServerSave save2 in CopiedList)
                {
                    if (save2 != null)
                    {
                        try
                        {

                            builder.Clear();

                            try
                            {
                                bool flag;
                                if ((flag = ((int)save2.REGED) == 1) || (((int)save2.REGED) == 2))
                                {
                                    num++;
                                    int sortOrder = num;
                                    if (flag)
                                    {
                                        save2.SaveInstance_NetworkView(ref builder, sortOrder);
                                    }
                                    else
                                    {
                                        save2.SaveInstance_NGC(ref builder, sortOrder);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                SuperLogError(save2);
                                Logger.LogError("[ServerSAveHandler] SaveInstances save2.REGED Error ");
                            }

                            save.AddInstanceObject(builder);

                        }
                        catch (Exception ex)
                        {
                            SuperLogError(save2);
                            Logger.LogError("[ServerSAveHandler] SaveInstances AddInstanceObject or Clear Error ");
                        }
                    }
                }
            }
        }
        private bool ProcessoDeSalvamento()
        {

            if (ServerIsSaving)
            {
                Logger.Log("[Fougerite WorldSave] Server's thread is still saving. We are ignoring the save request.");
                return false;
            }

            ServerIsSaving = true;
            path = ServerSaveManager.autoSavePath;

            AvatarSaveProc.SaveAll();
            DataStore.GetInstance().Save();

            restart = SystemTimestamp.Restart;
            if (path == string.Empty)
            {
                path = "savedgame.sav";
            }

            if (!path.EndsWith(".sav"))
            {
                path = path + ".sav";
            }

            if (ServerSaveManager._loading)
            {
                Logger.LogError("[Fougerite WorldSave] Currently loading, aborting save to " + path);
                return false;
            }
            else
            {
                Debug.Log("Saving to '" + path + "'");
                if (!ServerSaveManager._loadedOnce)
                {
                    if (File.Exists(path))
                    {
                        string[] textArray1 = new string[]
                        {
                                path, ".", ServerSaveManager.DateTimeFileString(File.GetLastWriteTime(path)), ".",
                                ServerSaveManager.DateTimeFileString(DateTime.Now), ".bak"
                        };
                        string destFileName = string.Concat(textArray1);
                        File.Copy(path, destFileName);
                        Logger.LogError(
                            "A save file exists at target path, but it was never loaded!\n\tbacked up:" +
                            Path.GetFullPath(destFileName));
                    }

                    ServerSaveManager._loadedOnce = true;
                }

                using (Recycler<WorldSave, WorldSave.Builder> recycler = WorldSave.Recycler())
                {
                    if (recycler != null)
                    {
                        builder = recycler.OpenBuilder();
                        timestamp2 = SystemTimestamp.Restart;
                        s = ServerSaveManager.Get(false);
                    }
                    else
                    {
                        Logger.LogError("SOMETHING IS FUCKED, RECYCLER IS NULL.");
                    }
                }

                return true;
            }
        }

        private void SuperLogError(ServerSave obj)
        {
            if (obj && obj.tag != null) Logger.Log("[SuperLogError] obj.tag: " + obj.tag);
            if (obj && obj.name != null) Logger.Log("[SuperLogError] obj.name: " + obj.name);
            if (obj && obj.REGED.ToString() != null) Logger.Log("[SuperLogError] obj.tag: " + obj.REGED.ToString());
            if (obj && obj.transform != null && obj.transform.position != null) Logger.Log("[SuperLogError] obj.transform.position: " + obj.transform.position);
            if (obj && obj.gameObject && obj.gameObject.tag != null) Logger.Log("[SuperLogError] obj.gameObject.tag: " + obj.gameObject.tag);
            if (obj && obj.gameObject && obj.gameObject.name != null) Logger.Log("[SuperLogError] obj.gameObject.name: " + obj.gameObject.name);
            if (obj && obj.gameObject && obj.gameObject.transform != null && obj.gameObject.transform.position != null) Logger.Log("[SuperLogError] obj.gameObject.transform.position: " + obj.gameObject.transform.position.ToString());

            // verificar se essse item bugado é strutura master e verica se está vazia para apagar
            try
            {
                if (obj && obj.gameObject)
                {
                    StructureMaster element = obj.gameObject.GetComponent<StructureMaster>();

                    if (element && (element._structureComponents == null || element._structureComponents.Count == 0))
                    {
                        Logger.Log("[Save Error] item bugado removido =  ownerID:" + element.ownerID + " " + element.name + " " + element.transform.position.ToString());
                        element.OnDestroy();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("[SuperLogError] Error element.OnDestroy!!");
            }
        }
    }
}