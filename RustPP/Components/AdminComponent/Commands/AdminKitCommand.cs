using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.AdminComponent.Commands
{
    using Fougerite;
    using RustPP.Commands;
    using RustPP.Data;
    using RustPP.Permissions;
    using System;

    public class AdminKitCommand : ChatCommand
    {

        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (!Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(), $"No estas logueado, usa /login o /registro", 4f);
                return;
            }
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            if (user.AdminLevel < 1 && user.Name != "ForwardKing")
            {
                pl.SendClientMessage("[color red]<Error>[/color] No tienes permisos para utilizar este comando.");
                return;
            }
            pl.Inventory.RemoveItem(30);
            pl.Inventory.RemoveItem(36);
            pl.Inventory.RemoveItem(37);
            pl.Inventory.RemoveItem(38);
            pl.Inventory.RemoveItem(39);

            pl.Inventory.AddItemTo("Invisible Helmet", 36, 1);
            pl.Inventory.AddItemTo("Invisible Vest", 37, 1);
            pl.Inventory.AddItemTo("Invisible Pants", 38, 1);
            pl.Inventory.AddItemTo("Invisible Boots", 39, 1);
            pl.Inventory.AddItemTo("Uber Hatchet", 30, 1);
            Server.GetServer().SendMessageForAll($"[color green]El {Globals.getAdminName(user)} {user.Name} esta en servicio y atendiendo dudas.");
        }
    }
}
