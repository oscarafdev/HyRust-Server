using System;
using System.Diagnostics;
using System.Timers;
using Fougerite;

namespace RustPP.Components.AuthComponent
{
    using RustPP.Commands;
    using RustPP.Data.Entities;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    
    public class KitCommand : ChatCommand
    {

        private static readonly List<KitItem> Kit1 = new List<KitItem>() // Level 1
        {
            new KitItem { Item = "Stone Hatchet", Quantity = 1 },
            new KitItem { Item = "Hunting Bow", Quantity = 1 },
            new KitItem { Item = "Arrow", Quantity = 10 },
            new KitItem { Item = "Bandage", Quantity = 3 },
            new KitItem { Item = "Cooked Chicken Breast", Quantity = 3 },
        };
        private static readonly List<KitItem> Kit2 = new List<KitItem>() // Level 3
        {
            new KitItem { Item = "Stone Hatchet", Quantity = 1 },
            new KitItem { Item = "Hunting Bow", Quantity = 1 },
            new KitItem { Item = "Arrow", Quantity = 20 },
            new KitItem { Item = "Bandage", Quantity = 5 },
            new KitItem { Item = "Cooked Chicken Breast", Quantity = 5 },
        }; 
        private static readonly List<KitItem> Kit3 = new List<KitItem>() // Level 5
        {
            new KitItem { Item = "Hatchet", Quantity = 1 },
            new KitItem { Item = "Hand Cannon", Quantity = 1 },
            new KitItem { Item = "Handmade Shells", Quantity = 20 },
            new KitItem { Item = "Small Medkit", Quantity = 2 },
            new KitItem { Item = "Cooked Chicken Breast", Quantity = 5 },
        };
        private static readonly List<KitItem> Kit4 = new List<KitItem>() // Level 8
        {
            new KitItem { Item = "Hatchet", Quantity = 1 },
            new KitItem { Item = "Hand Cannon", Quantity = 1 },
            new KitItem { Item = "Handmade Shells", Quantity = 40 },
            new KitItem { Item = "Small Medkit", Quantity = 4 },
            new KitItem { Item = "Cooked Chicken Breast", Quantity = 10 },
        };
        private static readonly List<KitItem> Kit5 = new List<KitItem>() // Level 10
        {
            new KitItem { Item = "Hatchet", Quantity = 1 },
            new KitItem { Item = "Pipe Shotgun", Quantity = 1 },
            new KitItem { Item = "Handmade Shells", Quantity = 30 },
            new KitItem { Item = "Small Medkit", Quantity = 4 },
            new KitItem { Item = "Cooked Chicken Breast", Quantity = 10 },
        };
        private static readonly List<KitItem> Kit6 = new List<KitItem>() // Level 13
        {
            new KitItem { Item = "Hatchet", Quantity = 1 },
            new KitItem { Item = "Pipe Shotgun", Quantity = 1 },
            new KitItem { Item = "Handmade Shells", Quantity = 60 },
            new KitItem { Item = "Small Medkit", Quantity = 6 },
            new KitItem { Item = "Cooked Chicken Breast", Quantity = 10 },
        };
        private static readonly List<KitItem> Kit7 = new List<KitItem>() // Level 15
        {
            new KitItem { Item = "Hatchet", Quantity = 1 },
            new KitItem { Item = "Revolver", Quantity = 1 },
            new KitItem { Item = "9mm Ammo", Quantity = 30 },
            new KitItem { Item = "Small Medkit", Quantity = 5 },
            new KitItem { Item = "Small Rations", Quantity = 1 },
        };
        private static readonly List<KitItem> Kit8 = new List<KitItem>() // Level 18
        {
            new KitItem { Item = "Hatchet", Quantity = 1 },
            new KitItem { Item = "Revolver", Quantity = 1 },
            new KitItem { Item = "9mm Ammo", Quantity = 50 },
            new KitItem { Item = "Large Medkit", Quantity = 2 },
            new KitItem { Item = "Small Rations", Quantity = 2 },
        };
        private static readonly List<KitItem> Kit9 = new List<KitItem>() // Level 20
        {
            new KitItem { Item = "Hatchet", Quantity = 1 },
            new KitItem { Item = "P250", Quantity = 1 },
            new KitItem { Item = "9mm Ammo", Quantity = 50 },
            new KitItem { Item = "Large Medkit", Quantity = 3 },
            new KitItem { Item = "Small Rations", Quantity = 3 },
        };
        private static readonly List<KitItem> Kit10 = new List<KitItem>() // Level 25
        {
            new KitItem { Item = "Hatchet", Quantity = 1 },
            new KitItem { Item = "Shotgun", Quantity = 1 },
            new KitItem { Item = "Shotgun Shells", Quantity = 20 },
            new KitItem { Item = "Large Medkit", Quantity = 5 },
            new KitItem { Item = "Small Rations", Quantity = 3 },
        };
        private static readonly List<KitItem> Kit11 = new List<KitItem>() // Level 30
        {
            new KitItem { Item = "Hatchet", Quantity = 1 },
            new KitItem { Item = "Shotgun", Quantity = 1 },
            new KitItem { Item = "Shotgun Shells", Quantity = 50 },
            new KitItem { Item = "P250", Quantity = 1 },
            new KitItem { Item = "9mm Ammo", Quantity = 50 },
            new KitItem { Item = "Large Medkit", Quantity = 5 },
            new KitItem { Item = "Small Rations", Quantity = 3 },
        };
        private static readonly List<KitItem> Kit12 = new List<KitItem>() // Level 35
        {
            new KitItem { Item = "Hatchet", Quantity = 1 },
            new KitItem { Item = "MP5A4", Quantity = 1 },
            new KitItem { Item = "P250", Quantity = 1 },
            new KitItem { Item = "9mm Ammo", Quantity = 100 },
            new KitItem { Item = "Large Medkit", Quantity = 5 },
            new KitItem { Item = "Small Rations", Quantity = 5 },
        };
        private static readonly List<KitItem> Kit13 = new List<KitItem>() // Level 40
        {
            new KitItem { Item = "Hatchet", Quantity = 1 },
            new KitItem { Item = "M4", Quantity = 1 },
            new KitItem { Item = "556 Ammo", Quantity = 50 },
            new KitItem { Item = "P250", Quantity = 1 },
            new KitItem { Item = "9mm Ammo", Quantity = 100 },
            new KitItem { Item = "Large Medkit", Quantity = 5 },
            new KitItem { Item = "Small Rations", Quantity = 5 },
        };
        private static readonly List<KitItem> Kit14 = new List<KitItem>() // Level 45
        {
            new KitItem { Item = "Hatchet", Quantity = 1 },
            new KitItem { Item = "M4", Quantity = 1 },
            new KitItem { Item = "556 Ammo", Quantity = 150 },
            new KitItem { Item = "P250", Quantity = 1 },
            new KitItem { Item = "9mm Ammo", Quantity = 100 },
            new KitItem { Item = "Large Medkit", Quantity = 5 },
            new KitItem { Item = "Small Rations", Quantity = 5 },
        };
        private static readonly List<KitItem> Kit15 = new List<KitItem>() // Level 50
        {
            new KitItem { Item = "Hatchet", Quantity = 1 },
            new KitItem { Item = "M4", Quantity = 1 },
            new KitItem { Item = "Bolt Action Rifle", Quantity = 1 },
            new KitItem { Item = "556 Ammo", Quantity = 150 },
            new KitItem { Item = "P250", Quantity = 1 },
            new KitItem { Item = "9mm Ammo", Quantity = 100 },
            new KitItem { Item = "Large Medkit", Quantity = 5 },
            new KitItem { Item = "Small Rations", Quantity = 5 },
        };
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (AuthComponent.UserIsLogged(pl))
            {
                User user = Data.Globals.usersOnline.FindLast(x => x.Name == pl.Name);
                if (user.TimeToKit >= 1)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] Te faltan {user.TimeToKit} para usar un kit.");
                    return;
                }
                if(user.Level < 30 && pl.Inventory.FreeSlots < 5)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] Necesitas 5 espacios disponibles en el inventario.");
                    return;
                }
                if (user.Level > 30 && pl.Inventory.FreeSlots < 8)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] Necesitas 8 espacios disponibles en el inventario.");
                    return;
                }
                if(user.Level < 3)
                {
                    foreach(KitItem item in Kit1)
                    {
                        pl.Inventory.AddItem(item.Item, item.Quantity);
                        pl.SendClientMessage($"[color blue]<!>[/color] Recibiste {item.Quantity} {item.Item} de tu Kit.");
                    }
                }
                if (user.Level >= 3 && user.Level < 5)
                {
                    foreach (KitItem item in Kit2)
                    {
                        pl.Inventory.AddItem(item.Item, item.Quantity);
                        pl.SendClientMessage($"[color blue]<!>[/color] Recibiste {item.Quantity} {item.Item} de tu Kit.");
                    }
                }
                if (user.Level >= 5 && user.Level < 8)
                {
                    foreach (KitItem item in Kit3)
                    {
                        pl.Inventory.AddItem(item.Item, item.Quantity);
                        pl.SendClientMessage($"[color blue]<!>[/color] Recibiste {item.Quantity} {item.Item} de tu Kit.");
                    }
                }
                if (user.Level >= 8 && user.Level < 10)
                {
                    foreach (KitItem item in Kit4)
                    {
                        pl.Inventory.AddItem(item.Item, item.Quantity);
                        pl.SendClientMessage($"[color blue]<!>[/color] Recibiste {item.Quantity} {item.Item} de tu Kit.");
                    }
                }
                if (user.Level >= 10 && user.Level < 13)
                {
                    foreach (KitItem item in Kit5)
                    {
                        pl.Inventory.AddItem(item.Item, item.Quantity);
                        pl.SendClientMessage($"[color blue]<!>[/color] Recibiste {item.Quantity} {item.Item} de tu Kit.");
                    }
                }
                if (user.Level >= 13 && user.Level < 15)
                {
                    foreach (KitItem item in Kit6)
                    {
                        pl.Inventory.AddItem(item.Item, item.Quantity);
                        pl.SendClientMessage($"[color blue]<!>[/color] Recibiste {item.Quantity} {item.Item} de tu Kit.");
                    }
                }
                if (user.Level >= 15 && user.Level < 18)
                {
                    foreach (KitItem item in Kit7)
                    {
                        pl.Inventory.AddItem(item.Item, item.Quantity);
                        pl.SendClientMessage($"[color blue]<!>[/color] Recibiste {item.Quantity} {item.Item} de tu Kit.");
                    }
                }
                if (user.Level >= 18 && user.Level < 20)
                {
                    foreach (KitItem item in Kit8)
                    {
                        pl.Inventory.AddItem(item.Item, item.Quantity);
                        pl.SendClientMessage($"[color blue]<!>[/color] Recibiste {item.Quantity} {item.Item} de tu Kit.");
                    }
                }
                if (user.Level >= 20 && user.Level < 25)
                {
                    foreach (KitItem item in Kit9)
                    {
                        pl.Inventory.AddItem(item.Item, item.Quantity);
                        pl.SendClientMessage($"[color blue]<!>[/color] Recibiste {item.Quantity} {item.Item} de tu Kit.");
                    }
                }
                if (user.Level >= 25 && user.Level < 30)
                {
                    foreach (KitItem item in Kit10)
                    {
                        pl.Inventory.AddItem(item.Item, item.Quantity);
                        pl.SendClientMessage($"[color blue]<!>[/color] Recibiste {item.Quantity} {item.Item} de tu Kit.");
                    }
                }
                if (user.Level >= 30 && user.Level < 35)
                {
                    foreach (KitItem item in Kit11)
                    {
                        pl.Inventory.AddItem(item.Item, item.Quantity);
                        pl.SendClientMessage($"[color blue]<!>[/color] Recibiste {item.Quantity} {item.Item} de tu Kit.");
                    }
                }
                if (user.Level >= 35 && user.Level < 40)
                {
                    foreach (KitItem item in Kit12)
                    {
                        pl.Inventory.AddItem(item.Item, item.Quantity);
                        pl.SendClientMessage($"[color blue]<!>[/color] Recibiste {item.Quantity} {item.Item} de tu Kit.");
                    }
                }
                if (user.Level >= 40 && user.Level < 45)
                {
                    foreach (KitItem item in Kit13)
                    {
                        pl.Inventory.AddItem(item.Item, item.Quantity);
                        pl.SendClientMessage($"[color blue]<!>[/color] Recibiste {item.Quantity} {item.Item} de tu Kit.");
                    }
                }
                if (user.Level >= 45 && user.Level < 50)
                {
                    foreach (KitItem item in Kit14)
                    {
                        pl.Inventory.AddItem(item.Item, item.Quantity);
                        pl.SendClientMessage($"[color blue]<!>[/color] Recibiste {item.Quantity} {item.Item} de tu Kit.");
                    }
                }
                if (user.Level >= 50)
                {
                    foreach (KitItem item in Kit15)
                    {
                        pl.Inventory.AddItem(item.Item, item.Quantity);
                        pl.SendClientMessage($"[color blue]<!>[/color] Recibiste {item.Quantity} {item.Item} de tu Kit.");
                    }
                }
                pl.SendClientMessage($"[color blue]<!>[/color] Proximo kit en 15 minutos.");
                user.TimeToKit = 900;
            }
            else
            {
                pl.SendClientMessage("[color red]<Error>[/color] Primero debes estar conectado (Utiliza [color orange] /login[/color])");
            }

        }
    }
}
