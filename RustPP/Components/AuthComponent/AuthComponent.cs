using Fougerite.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.AuthComponent
{
    class AuthComponent
    {
        public static void Init()
        {
            Fougerite.Hooks.OnPlayerGathering += OnPlayerGathering;
            Fougerite.Hooks.OnPlayerConnected += OnPlayerConnect;
            Fougerite.Hooks.OnPlayerDisconnected += OnPlayerDisconnected;
            Fougerite.Hooks.OnPlayerSpawned += OnPlayerSpawned;
        }
        static void OnPlayerSpawned(Fougerite.Player player, SpawnEvent se)
        {
            player.Character.AttentionMessage("HOLA, ESTO ES UNA PRUEBA");
            player.Character.SendMessage("HOLA, ESTO ES UNA PRUEBA");
            player.Character.ObliviousMessage("HOLA, ESTO ES UNA PRUEBA");
        }
        static void OnPlayerConnect(Fougerite.Player player)
        {
            player.SendClientMessage($"Bienvenido a [color orange]Rainbow Rust[color white], para ingresar utilice [color blue]/login <Contraseña>");
            
        }
        static void OnPlayerDisconnected(Fougerite.Player player)
        {

        }
        static void OnPlayerGathering(Fougerite.Player player, GatherEvent ge)
        {
            
        }
    }
}
