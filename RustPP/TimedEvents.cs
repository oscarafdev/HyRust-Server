﻿namespace RustPP
{
    using Fougerite;
    using System;
    using System.Diagnostics;
    using System.Timers;
    using UnityEngine;

    public class TimedEvents
    {
        public static bool init = false;
        public static int time = 60;
        public static System.Timers.Timer timer;

        private static void advertise_begin()
        {
            for (int i = 0; i < int.Parse(Core.config.GetSetting("Settings", "notice_messages_amount")); i++)
            {
                Server.GetServer().BroadcastFrom(Core.Name, Core.config.GetSetting("Settings", "notice" + (i + 1)));
            }
        }

        private static void airdrop_begin()
        {
            int num = int.Parse(Core.config.GetSetting("Settings", "airdrop_count"));
            World.GetWorld().Airdrop(num);
        }

        public static void savealldata()
        {
            try
            {
                AvatarSaveProc.SaveAll();
                ServerSaveManager.AutoSave();
            }
            catch{}
        }

        public static void shutdown()
        {
            savealldata();
            time = int.Parse(Core.config.GetSetting("Settings", "shutdown_countdown"));
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 10000.0;
            timer.AutoReset = true;
            timer.Elapsed += delegate (object x, ElapsedEventArgs y)
            {
                shutdown_tick();
            };
            timer.Start();
            shutdown_tick();
        }

        public static void shutdown_tick()
        {
            if (time == 0)
            {
                //savealldata();
                Helper.CreateSaves();
                Server.GetServer().BroadcastFrom(Core.Name, "Server Shutdown NOW!");
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                Logger.Log("Server Shutting down in " + time + " seconds");
                Server.GetServer().BroadcastFrom(Core.Name, "Server Shutting down in " + time + " seconds");
            }
            time -= 10;
        }

        public static void startEvents()
        {
            if (!init)
            {
                init = true;
                if (Core.config.GetBoolSetting("Settings", "pvp"))
                {
                    server.pvp = true;
                }
                else
                {
                    server.pvp = false;
                }
                if (Core.config.GetBoolSetting("Settings", "instant_craft"))
                {
                    crafting.instant = true;
                }
                else
                {
                    crafting.instant = false;
                }
                if (Core.config.GetSetting("Settings", "sleepers") == "true")
                {
                    sleepers.on = true;
                }
                else
                {
                    sleepers.on = false;
                }
                if (Core.config.GetBoolSetting("Settings", "enforce_truth"))
                {
                    truth.punish = true;
                }
                else
                {
                    truth.punish = false;
                }
                if (!Core.config.GetBoolSetting("Settings", "voice_proximity"))
                {
                    voice.distance = 2.147484E+09f;
                }
                if (Core.config.GetBoolSetting("Settings", "notice_enabled"))
                {
                    timer = new System.Timers.Timer();
                    timer.Interval = int.Parse(Core.config.GetSetting("Settings", "notice_interval"));
                    timer.AutoReset = true;
                    timer.Elapsed += delegate(object x, ElapsedEventArgs y)
                    {
                        advertise_begin();
                    };
                    timer.Start();
                }
            }
        }
    }
}