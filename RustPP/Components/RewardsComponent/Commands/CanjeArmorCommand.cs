using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.RewardsComponent.Commands
{
    class CanjeArmorCommand : ChatCommand
    {
        public readonly System.Random Randomizer = new System.Random();
        static readonly Dictionary<int, string> HelmetList = new Dictionary<int, string>()
        {
            {0, "Leather Helmet"},
            {1, "Cloth Helmet"},
            {2, "Kevlar Helmet"},
            {3, "Rad Suit Helmet"},
        };
        static readonly Dictionary<int, string> VestList = new Dictionary<int, string>()
        {
            {0, "Leather Vest"},
            {1, "Cloth Vest"},
            {2, "Kevlar Vest"},
            {3, "Rad Suit Vest"},
        };
        static readonly Dictionary<int, string> PantsList = new Dictionary<int, string>()
        {
            {0, "Leather Pants"},
            {1, "Cloth Pants"},
            {2, "Kevlar Pants"},
            {3, "Rad Suit Pants"},
        };
        static readonly Dictionary<int, string> BootsList = new Dictionary<int, string>()
        {
            {0, "Leather Boots"},
            {1, "Cloth Boots"},
            {2, "Kevlar Boots"},
            {3, "Rad Suit Boots"},
        };
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (!RustPP.Data.Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(), $"No estas logueado, usa /login o /registro", 4f);
                return;
            }
            if(!pl.Inventory.HasItem("Armor Part 1") ||
                !pl.Inventory.HasItem("Armor Part 2") ||
                !pl.Inventory.HasItem("Armor Part 3") ||
                !pl.Inventory.HasItem("Armor Part 4") ||
                !pl.Inventory.HasItem("Armor Part 5") ||
                !pl.Inventory.HasItem("Armor Part 6") ||
                !pl.Inventory.HasItem("Armor Part 7")) {
                pl.SendClientMessage($"[color red]<Error>[/color] Necesitas todas las partes de Armadura para canjearlas (Del 1 a la 7).");
                return;
            }
            
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            int randomHelmet = Randomizer.Next(0, 3);
            int randomVest = Randomizer.Next(0, 3);
            int randomPants = Randomizer.Next(0, 3);
            int randomBoots = Randomizer.Next(0, 3);
            string helmet = HelmetList[randomHelmet];
            string vest = VestList[randomVest];
            string pants = PantsList[randomPants];
            string boots = BootsList[randomBoots];
            if (pl.Inventory.FreeSlots >= 4)
            {
                pl.Inventory.RemoveItem("Armor Part 1", 1);
                pl.Inventory.RemoveItem("Armor Part 2", 1);
                pl.Inventory.RemoveItem("Armor Part 3", 1);
                pl.Inventory.RemoveItem("Armor Part 4", 1);
                pl.Inventory.RemoveItem("Armor Part 5", 1);
                pl.Inventory.RemoveItem("Armor Part 6", 1);
                pl.Inventory.RemoveItem("Armor Part 7", 1);
                pl.SendClientMessage($"[color green]<!>[/color] Recibiste {helmet} x 1.");
                pl.Inventory.AddItem(helmet);
                pl.SendClientMessage($"[color green]<!>[/color] Recibiste {vest} x 1.");
                pl.Inventory.AddItem(vest);
                pl.SendClientMessage($"[color green]<!>[/color] Recibiste {pants} x 1.");
                pl.Inventory.AddItem(pants);
                pl.SendClientMessage($"[color green]<!>[/color] Recibiste {boots} x 1.");
                pl.Inventory.AddItem(boots);
                
            }
            else
            {
                pl.SendClientMessage($"[color red]<Error>[/color] No puedes cambiar las Armor Parts por que no tienes espacio en el inventario (4).");
            }
        }
    }
}
