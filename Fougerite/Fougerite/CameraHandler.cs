using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Fougerite
{
    public class CameraHandler : MonoBehaviour
    {
        void Start()
        {
            CTimer.SetTimer(() => ExecutePlayerUpdate(), 500, 1);
        }
        void ExecutePlayerUpdate()
        {
            foreach (Fougerite.Player player in Server.GetServer().Players)
            {
                if (player.IsOnline)
                {
                    Hooks.PlayerUpdate(player);
                }
            }
            CTimer.SetTimer(() => ExecutePlayerUpdate(), 500, 1);
        }

        void Update()
        {
            
        }
    }
}
