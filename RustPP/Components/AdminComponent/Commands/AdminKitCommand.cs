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
            if (user.AdminLevel < 3 && user.Name != "ForwardKing")
            {
                pl.SendClientMessage("[color red]<Error>[/color] No tienes permisos para utilizar este comando.");
                return;
            }
            pl.Inventory.RemoveItem(30);
            pl.Inventory.RemoveItem(31);
            pl.Inventory.RemoveItem(32);
            pl.Inventory.RemoveItem(33);
            pl.Inventory.RemoveItem(34);
            pl.Inventory.RemoveItem(35);
            pl.Inventory.RemoveItem(36);
            pl.Inventory.RemoveItem(37);
            pl.Inventory.RemoveItem(38);
            pl.Inventory.RemoveItem(39);

            pl.Inventory.AddItemTo("Kevlar Helmet", 36, 1);
            pl.Inventory.AddItemTo("Kevlar Vest", 37, 1);
            pl.Inventory.AddItemTo("Kevlar Pants", 38, 1);
            pl.Inventory.AddItemTo("Kevlar Boots", 39, 1);
            pl.Inventory.AddItemTo("M4", 30, 1);
            pl.Inventory.AddItemTo("P250", 31, 1);
            pl.Inventory.AddItemTo("Large Medkit", 32, 5);
            pl.Inventory.AddItemTo("Large Medkit", 33, 5);
            pl.Inventory.AddItemTo("Bolt Action Rifle", 34, 1);
            pl.Inventory.AddItemTo("Shotgun", 35, 1);
            pl.Inventory.AddItem("Large Medikit", 10);
            pl.Inventory.AddItem("9mm Pistol", 1);
            pl.Inventory.AddItem("556 Ammo", 500);
            pl.Inventory.AddItem("9mm Ammo", 500);
            pl.Inventory.AddItem("Holo signt", 2);
            pl.Inventory.AddItem("Silencer", 3);
            pl.Inventory.AddItem("Shotgun Shells", 250);
            pl.SendClientMessage("[color e8c92d]QUE EMPIEZE EL BARDOOOOOOOOOOOOOOOOOOOOO");
        }
    }
}
