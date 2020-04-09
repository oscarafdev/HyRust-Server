using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RustPP.Components.EconomyComponent;

namespace RustPP.Components.UtilityComponent
{
    class ConfirmCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (!RustPP.Data.Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(), $"No estas logueado, usa /login o /registro", 4f);
                return;
            }
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            if(user.SellingItem == null)
            {
                pl.SendClientMessage("[color red]<Error>[/color] No tienes nada para confirmar.");
                return;
            }
            Data.Entities.StoreItem item = user.SellingItem;
            if(item.Item == null)
            {
                pl.SendClientMessage("[color red]<Error>[/color] Al parecer ya no tienes el objeto que pretendías vender.");
                return;
            }
            Fougerite.PlayerItem playerItem = EconomyComponent.EconomyComponent.GetItem(pl.Inventory, item.Item.Slot);
            if(playerItem == null)
            {
                pl.SendClientMessage("[color red]<Error>[/color] Al parecer ya no tienes el objeto que pretendías vender.");
                return;
            }
            Data.Entities.StoreItem actualItem = EconomyComponent.EconomyComponent.GetStoreItem(user, playerItem, item.Price);
            if (actualItem == null)
            {
                pl.SendClientMessage("[color red]<Error>[/color] Al parecer ya no tienes el objeto que pretendías vender.");
                return;
            }
            if (item.ItemName == actualItem.ItemName && 
                item.ItemQuantity == actualItem.ItemQuantity && 
                item.ItemCondition == actualItem.ItemCondition &&
                item.ItemWeaponSlots == actualItem.ItemWeaponSlots &&
                item.ItemWeaponBullets == actualItem.ItemWeaponBullets &&
                item.ItemWeaponSlot1 == actualItem.ItemWeaponSlot1 &&
                item.ItemWeaponSlot2 == actualItem.ItemWeaponSlot2 &&
                item.ItemWeaponSlot3 == actualItem.ItemWeaponSlot3 &&
                item.ItemWeaponSlot4 == actualItem.ItemWeaponSlot4 &&
                item.ItemWeaponSlot5 == actualItem.ItemWeaponSlot5)
            {
                EconomyComponent.EconomyComponent.StoreItems.Add(item);
                pl.Inventory.RemoveItem(item.Item.Slot);
                item.Create();
                foreach (Fougerite.Player player in Fougerite.Server.GetServer().Players)
                {
                    if (player.IsOnline && Data.Globals.UserIsLogged(player))
                    {
                        RustPP.Data.Entities.User uuser = RustPP.Data.Globals.GetInternalUser(player);
                        if (uuser.TiendaEnabled || uuser.Name == user.Name)
                        {
                            string quantity = "";
                            if(item.ItemQuantity > 1)
                            {
                                quantity = $" x {item.ItemQuantity}";
                            }
                            player.SendClientMessage($"[color purple]<Tienda>[/color] [color #db8cff]{user.Name}{quantity}[/color] publicó un objeto en la tienda - [color #db8cff]{item.ItemName}[/color] [color #db8cff]${item.Price}[/color] ({item.InternalID})");
                        }
                    }
                }
                return;
            }
            else
            {
                pl.SendClientMessage("[color red]<Error>[/color] Al parecer ya no tienes el objeto que pretendías vender o el objeto cambió su estado.");
                return;
            }

        }
    }
}
