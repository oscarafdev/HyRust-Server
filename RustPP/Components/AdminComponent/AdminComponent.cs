using Fougerite.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.AdminComponent
{
    class AdminComponent
    {
        public static void InitComponent()
        {
            Fougerite.Hooks.OnShoot += OnShoot;
        }
        public static void OnShoot(ShootEvent se)
        {
            
            if (RustPP.Data.Globals.UserIsLogged(se.Player))
            {
                RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(se.Player);
                if(user.SpawningPrefab)
                {
                    UnityEngine.Quaternion rotation = se.Player.Character.rotation;
                    /*Controllable controllable = se.Player.Character.controllable;
                    if (user.PrefabName == ":player_soldier")
                    {
                        ServerManagement.Get().EraseCharactersForClient(se.Player.PlayerClient, true, se.Player.NetUser);
                    }*/
                    NetCull.InstantiateDynamic(user.PrefabName, se.EndPos, rotation);
                    /*if(user.PrefabName == ":player_soldier")
                    {
                        se.Player.PlayerClient = ServerManagement.Get().CreatePlayerClientForUser(se.Player.NetUser);
                    }*/
                }
                
            }
            
        }
    }
}
