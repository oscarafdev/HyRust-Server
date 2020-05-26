using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.RewardsComponent.Commands
{
    using RustPP.Data.Entities;
    class CanjeWeaponsCommand : ChatCommand
    {
        public readonly System.Random Randomizer = new System.Random();
        private static readonly List<KitItem> Option1 = new List<KitItem>() // Level 1
        {
            new KitItem { Item = "M4", Quantity = 1 },
            new KitItem { Item = "556 Ammo", Quantity = 100 },
        };
        private static readonly List<KitItem> Option2 = new List<KitItem>() // Level 1
        {
            new KitItem { Item = "Bolt Action Rifle", Quantity = 1 },
            new KitItem { Item = "556 Ammo", Quantity = 80 },
        };
        private static readonly List<KitItem> Option3 = new List<KitItem>() // Level 1
        {
            new KitItem { Item = "9mm Pistol", Quantity = 1 },
            new KitItem { Item = "9mm Ammo", Quantity = 50 },
        };
        private static readonly List<KitItem> Option4 = new List<KitItem>() // Level 1
        {
            new KitItem { Item = "P250", Quantity = 1 },
            new KitItem { Item = "9mm Ammo", Quantity = 50 },
        };
        private static readonly List<KitItem> Option5 = new List<KitItem>() // Level 1
        {
            new KitItem { Item = "MP5A4", Quantity = 1 },
            new KitItem { Item = "9mm Ammo", Quantity = 100 },
        };
        private static readonly List<KitItem> Option6 = new List<KitItem>() // Level 1
        {
            new KitItem { Item = "Shotgun", Quantity = 1 },
            new KitItem { Item = "Shotgun Shells", Quantity = 30 },
        };
        static readonly Dictionary<int, List<KitItem>> RewardList = new Dictionary<int, List<KitItem>>()
        {
            { 0, Option1 },
            { 1, Option2 },
            { 2, Option3 },
            { 3, Option4 },
            { 4, Option5 },
            { 5, Option6 },

        };

        static readonly Dictionary<int, string> ModList = new Dictionary<int, string>()
        {
            {0, "Silencer"},
            {1, "Laser Sight"},
            {2, "Holo sight"},
            {3, "Flash Light"},
        };
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            string lang = LanguageComponent.LanguageComponent.GetPlayerLangOrDefault(pl);
            if (!RustPP.Data.Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(),LanguageComponent.LanguageComponent.getMessage("notice_not_logged", lang), 4f);
                return;
            }
            if (!pl.Inventory.HasItem("Weapon Part 1") ||
                !pl.Inventory.HasItem("Weapon Part 2") ||
                !pl.Inventory.HasItem("Weapon Part 3") ||
                !pl.Inventory.HasItem("Weapon Part 4") ||
                !pl.Inventory.HasItem("Weapon Part 5") ||
                !pl.Inventory.HasItem("Weapon Part 6") ||
                !pl.Inventory.HasItem("Weapon Part 7"))
            {
                pl.SendClientMessage($"[color red]<Error>[/color] Necesitas todas las partes de Arma para canjearlas (Del 1 a la 7).");
                return;
            }

            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            int randomKit = Randomizer.Next(0, 5);
            int randomMod = Randomizer.Next(0, 3);
            string mod = ModList[randomMod];
            List<KitItem> kit = RewardList[randomKit];
            if (pl.Inventory.FreeSlots >= 3)
            {
                pl.Inventory.RemoveItem("Weapon Part 1", 1);
                pl.Inventory.RemoveItem("Weapon Part 2", 1);
                pl.Inventory.RemoveItem("Weapon Part 3", 1);
                pl.Inventory.RemoveItem("Weapon Part 4", 1);
                pl.Inventory.RemoveItem("Weapon Part 5", 1);
                pl.Inventory.RemoveItem("Weapon Part 6", 1);
                pl.Inventory.RemoveItem("Weapon Part 7", 1);
                foreach (KitItem item in kit)
                {
                    pl.Inventory.AddItem(item.Item, item.Quantity);
                    pl.SendClientMessage($"[color green]<!>[/color] Recibiste{item.Item} x {item.Quantity}.");
                }
                pl.SendClientMessage($"[color green]<!>[/color] Recibiste {mod} x 1.");
                pl.Inventory.AddItem(mod);
            }
            else
            {
                pl.SendClientMessage($"[color red]<Error>[/color] No puedes cambiar las Weapon Parts por que no tienes espacio en el inventario (3).");
            }
        }
    }
}
