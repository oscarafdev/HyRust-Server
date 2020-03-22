using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fougerite;

namespace RustPP.Components.AuthComponent
{
    using RustPP.Commands;
    using UnityEngine;
    using RustPP.Data.Entities;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    class TPCommand : ChatCommand
    {
        public readonly System.Random Randomizer = new System.Random();
        private static readonly List<Vector3> Small = new List<Vector3>()
        {
            new Vector3 { x = 6051.9f, y = 385.24f, z = -3581.83f},
            new Vector3 { x = 6106.31f, y = 378.244f, z = -3468.05f},
            new Vector3 { x = 6059.69f, y = 379.962f, z = -3648.96f},
            new Vector3 { x = 6121.3f, y = 376.35f, z = -3545.34f},
            new Vector3 { x = 6118.36f, y = 377.712f, z = -3536.4f}
        };
        private static readonly List<Vector3> Factory = new List<Vector3>()
        {
            new Vector3 { x = 6468.73f, y = 361.218f, z = -4596.97f},
            new Vector3 { x = 6360.11f, y = 356.995f, z = -4451.48f},
            new Vector3 { x = 6296.03f, y =370.765f, z = -4427.77f},
            new Vector3 { x = 6263.93f, y = 355.797f, z = -4504.73f}
        };
        private static readonly List<Vector3> Vale = new List<Vector3>()
        {
            new Vector3 { x = 4697.37f, y = 437.917f, z = -3834.1f},
            new Vector3 { x = 4749.75f, y = 445.978f, z = -3747.05f},
            new Vector3 { x = 4716.98f, y = 460.96f, z = -3718.97f},
            new Vector3 { x = 4758.3f, y = 444.148f, z = -3706.58f},
            new Vector3 { x = 4806.76f, y = 448.575f, z = -3688.34f},
            new Vector3 { x = 4825.81f, y = 442.057f, z = -3690.32f},
            new Vector3 { x = 4830.61f, y = 428.411f, z = -3713.53f},
            new Vector3 { x = 4802.67f, y = 425.485f, z = -3794.99f},
            new Vector3 { x = 4871.63f, y = 423.531f, z = -3821.04f},
        };
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            if (!RustPP.Data.Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(), $"No estas logueado, usa /login o /registro", 4f);
                return;
            }
            if (ChatArguments.Length < 1)
            {
                pl.SendClientMessage("[color red]<Sintaxis>[/color] /tp <Nombre>");
                pl.SendClientMessage("Nombres: small - factory - vale");
                return;
            }
            string search = ChatArguments[0].ToLower();
            if (search == "small")
            {
                int number = Randomizer.Next(0, 4);
                Vector3 pos = Small[number];
                pl.TeleportTo(pos);
                pl.SendClientMessage("[color yellow]<!>[/color] Te teletransportaste a Small.");
            }
            if (search == "factory")
            {
                int number = Randomizer.Next(0, 3);
                Vector3 pos = Factory[number];
                pl.TeleportTo(pos);
                pl.SendClientMessage("[color yellow]<!>[/color] Te teletransportaste a Factory.");
            }
            if (search == "vale")
            {
                int number = Randomizer.Next(0, 8);
                Vector3 pos = Vale[number];
                pl.TeleportTo(pos);
                pl.SendClientMessage("[color yellow]<!>[/color] Te teletransportaste a Vale.");
            }
        }
    }
}
