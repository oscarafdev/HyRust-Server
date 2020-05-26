using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.EconomyComponent.Commands
{
    class ShopCommand : ChatCommand
    {
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
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            if (ChatArguments.Length < 1)
            {
                pl.SendClientMessage("-[color #db8cff] /tienda lista [/color] Ver todos los objetos en venta");
                pl.SendClientMessage("-[color #db8cff] /tienda ver [/color] Mira los detalles de un objeto en venta, ejemplo: Estado [color red]¡Importante!");
                pl.SendClientMessage("-[color #db8cff] /tienda vender [/color] Inserta en la tienda un objeto que tengas en el inventario.");
                pl.SendClientMessage("-[color #db8cff] /tienda comprar [/color] Compra un objeto de la tienda");
                pl.SendClientMessage("-[color #db8cff] /tienda cuenta [/color] Para ver los objetos en venta de tu cuenta.");
                pl.SendClientMessage("-[color #db8cff] /tienda retirar [/color] Para retirar un objeto de tu pertenencia.");
                return;
            }
            string search = ChatArguments[0].ToLower();
            if(search == "lista")
            {
                if(ChatArguments.Length < 2)
                {
                    List<Data.Entities.StoreItem> playerItems = EconomyComponent.StoreItems.FindAll(x => x.Purchased == false);
                    List<RustPP.Data.Entities.StoreItem> itemList = EconomyComponent.GetPageItemss(playerItems, 1, 10);
                    pl.SendClientMessage("[color #db8cff] ---------------------- [color white]HYRUST TIENDA[/color] ----------------------");
                    foreach (Data.Entities.StoreItem item in itemList)
                    {

                            if (item.ItemQuantity > 1)
                            {
                                pl.SendClientMessage($"[color #db8cff]# {item.InternalID}[/color] | [color purple]{item.ItemName}[/color] x [color #db8cff]{item.ItemQuantity}[/color] | [color purple]${item.Price}[/color] | [color #db8cff]{Data.Globals.GetUserNameByID(item.UserID)}");
                            }
                            else
                            {
                                pl.SendClientMessage($"[color #db8cff]# {item.InternalID}[/color] | [color purple]{item.ItemName}[/color] | [color purple]${item.Price}[/color] | [color #db8cff]{Data.Globals.GetUserNameByID(item.UserID)}");
                            }
                        
                        
                        
                    }
                    List<RustPP.Data.Entities.StoreItem> nextPage = EconomyComponent.GetPageItemss(playerItems, 2, 10);
                    if(nextPage.Count > 0)
                    {
                        pl.SendClientMessage("[color purple]<!>[/color] Use [color #db8cff]/tienda lista 2[/color] para ver más resultados.");
                    }
                    pl.SendClientMessage("[color purple]<!>[/color] TIP: Use [color #db8cff]/tienda ver <Numero>[/color] para ver los detalles del Item y [color #db8cff]/tienda comprar[/color] para comprarlo.");
                }
                else
                {
                    string pagestring = ChatArguments[1];
                    int page = Int32.Parse(pagestring);
                    List<Data.Entities.StoreItem> playerItems = EconomyComponent.StoreItems.FindAll(x => x.Purchased == false);
                    List<RustPP.Data.Entities.StoreItem> itemList = EconomyComponent.GetPageItemss(playerItems, page, 10);
                    pl.SendClientMessage($"[color #db8cff] ---------------------- [color white]HYRUST TIENDA - Pagina {page}[/color] ----------------------");
                    foreach (Data.Entities.StoreItem item in itemList)
                    {
                        if (item.ItemQuantity > 1)
                        {
                            pl.SendClientMessage($"[color #db8cff]# {item.InternalID}[/color] | [color purple]{item.ItemName}[/color] x [color #db8cff]{item.ItemQuantity}[/color] | [color purple]${item.Price}[/color] | [color #db8cff]{Data.Globals.GetUserNameByID(item.UserID)}");
                        }
                        else
                        {
                            pl.SendClientMessage($"[color #db8cff]# {item.InternalID}[/color] | [color purple]{item.ItemName}[/color] | [color purple]${item.Price}[/color] | [color #db8cff]{Data.Globals.GetUserNameByID(item.UserID)}");
                        }
                    }
                    List<RustPP.Data.Entities.StoreItem> nextPage = EconomyComponent.GetPageItemss(playerItems, page+1, 10);
                    if (nextPage.Count > 0)
                    {
                        pl.SendClientMessage($"[color purple]<!>[/color] Use [color #db8cff]/tienda lista {page+1}[/color] para ver más resultados.");
                    }
                }
            }
            else if (search == "ver")
            {
                if (ChatArguments.Length < 2)
                {
                    pl.SendClientMessage("[color red]<Sintaxis>[/color] Use [color #db8cff] /tienda ver <Numero> [/color] El numero es el que se encuentra después del # en la lista de items.");
                    return;
                }
                string itemstring = ChatArguments[1];
                int itemID = Int32.Parse(itemstring);
                if (EconomyComponent.StoreItems.Exists(x => x.InternalID == itemID && x.Purchased == false))
                {
                    Data.Entities.StoreItem storeItem = EconomyComponent.StoreItems.Find(x => x.InternalID == itemID);
                    pl.SendClientMessage($"[color #db8cff] ----- [color white]{storeItem.ItemName} [color #db8cff]|[/color] #{storeItem.InternalID}[/color] -----");
                    pl.SendClientMessage($"[color purple]Propietario: [/color] [color #db8cff]{Data.Globals.GetUserNameByID(storeItem.UserID)}[/color]");
                    pl.SendClientMessage($"[color purple]Precio: [/color] [color #db8cff]${storeItem.Price}[/color]");
                    pl.SendClientMessage($"[color purple]Cantidad: [/color] [color #db8cff]{storeItem.ItemQuantity}[/color]");
                    pl.SendClientMessage($"[color purple]Estado: [/color] [color #db8cff]{storeItem.ItemCondition*100}/100[/color]");                    
                    if(storeItem.ItemCategory == EconomyComponent.GetCategoryNumber("Weapons"))
                    {
                        pl.SendClientMessage($"[color purple]Balas: [/color] [color #db8cff]{storeItem.ItemWeaponBullets}[/color]");
                        pl.SendClientMessage($"[color purple]Slots: [/color] [color #db8cff]{storeItem.ItemWeaponSlots}[/color]");
                        if(storeItem.ItemWeaponSlot1 != "null")
                        {
                            pl.SendClientMessage($"[color purple]Slot 1: [/color] [color #db8cff]{storeItem.ItemWeaponSlot1}[/color]");
                        }
                        if (storeItem.ItemWeaponSlot2 != "null")
                        {
                            pl.SendClientMessage($"[color purple]Slot 1: [/color] [color #db8cff]{storeItem.ItemWeaponSlot2}[/color]");
                        }
                        if (storeItem.ItemWeaponSlot3 != "null")
                        {
                            pl.SendClientMessage($"[color purple]Slot 1: [/color] [color #db8cff]{storeItem.ItemWeaponSlot3}[/color]");
                        }
                        if (storeItem.ItemWeaponSlot4 != "null")
                        {
                            pl.SendClientMessage($"[color purple]Slot 1: [/color] [color #db8cff]{storeItem.ItemWeaponSlot4}[/color]");
                        }
                        if (storeItem.ItemWeaponSlot5 != "null")
                        {
                            pl.SendClientMessage($"[color purple]Slot 1: [/color] [color #db8cff]{storeItem.ItemWeaponSlot5}[/color]");
                        }
                    }
                    pl.SendClientMessage($"[color purple]<!>[/color] Use [color #db8cff]/tienda comprar {storeItem.InternalID}[/color] para comprar este objeto.");
                }
                else
                {
                    pl.SendClientMessage("[color red]<Error>[/color] Este objeto no existe en la tienda o ya no esta a la venta.");
                    pl.SendClientMessage("[color red]<Sintaxis>[/color] Use [color #db8cff] /tienda ver <Numero> [/color] El numero es el que se encuentra después del # en la lista de items.");
                    return;
                }

            }
            else if (search == "vender")
            {
                if (ChatArguments.Length < 3)
                {
                    pl.SendClientMessage("[color red]<!>[/color] Para vender un objeto coloca lo que quieres vender en tu barra de acceso rápido e indica el SLOT.");
                    pl.SendClientMessage("[color red]<Sintaxis>[/color] /tienda vender <Slot> <Precio>");
                    return;
                }
                string slotstring = ChatArguments[1];
                int slot = Int32.Parse(slotstring);
                if (slot < 1 || slot > 6)
                {
                    pl.SendClientMessage("[color red]<!>[/color] Slot inválido, selecciona un slot del 1 al 6.");
                    pl.SendClientMessage("[color red]<Sintaxis>[/color] /tienda vender <Slot> <Precio>");
                    return;
                }
                string pricestring = ChatArguments[2];
                int price = Int32.Parse(pricestring);
                if (price < 1)
                {
                    pl.SendClientMessage("[color red]<!>[/color] Precio inválido, tiene que ser mas de $1.");
                    pl.SendClientMessage("[color red]<Sintaxis>[/color] /tienda vender <Slot> <Precio>");
                    return;
                }
                int sloot = EconomyComponent.GetItemSlot(slot);
                Fougerite.PlayerItem item = EconomyComponent.GetItem(pl.Inventory, sloot);
                if (item == null || item.Name == "" && item.Quantity == -1)
                {
                    pl.SendClientMessage($"[color red]<!>[/color] Probablemente tu ranura este vacía, coloca un item en la ranura {slot}.");
                    pl.SendClientMessage("[color red]<Sintaxis>[/color] /tienda vender <Slot> <Precio>");
                    return;
                }
                string cat = item.RInventoryItem.datablock.category.ToString();
                pl.SendClientMessage($"[color purple]<Tienda>[/color] Estas por publlicar {item.Name} x {item.Quantity} en {cat} por ${price}.");
                pl.SendClientMessage($"[color purple]<Tienda>[/color] Se necesita confirmación, utilice /confirmar.");
                Data.Entities.StoreItem storeItem = EconomyComponent.GetStoreItem(user, item, price);
                user.SellingItem = storeItem;
            }
            else if (search == "comprar")
            {
                if (ChatArguments.Length < 2)
                {
                    pl.SendClientMessage("[color red]<!>[/color] Asegurese de que el numero de objeto a comprar sea correcto, no hay vuelta atrás.");
                    pl.SendClientMessage("[color red]<Sintaxis>[/color] /tienda comprar <Numero>");
                    return;
                }
                string item = ChatArguments[1];
                int internalid = Int32.Parse(item);
                EconomyComponent.PurchaseItem(user, internalid);
            }
            else if (search == "retirar")
            {
                if (ChatArguments.Length < 2)
                {
                    pl.SendClientMessage("[color red]<!>[/color] Para retirar un objeto de su propiedad de la tienda ingrese el numero de Objeto.");
                    pl.SendClientMessage("[color red]<Sintaxis>[/color] /tienda retirar <Numero>");
                    return;
                }
                string item = ChatArguments[1];
                int internalid = Int32.Parse(item);
                EconomyComponent.PurchaseItem(user, internalid);
            }
            else if (search == "cuenta")
            {
                if (ChatArguments.Length < 2)
                {
                    List<Data.Entities.StoreItem> playerItems = EconomyComponent.StoreItems.FindAll(x => x.UserID == user.ID);
                    List<RustPP.Data.Entities.StoreItem> itemList = EconomyComponent.GetPageItemss(playerItems, 1, 10);
                    pl.SendClientMessage("[color #db8cff] ---------------------- [color white]HYRUST TIENDA[/color] ----------------------");
                    foreach (Data.Entities.StoreItem item in itemList)
                    {
                        if (item.ItemQuantity > 1)
                        {
                            pl.SendClientMessage($"[color #db8cff]# {item.InternalID}[/color] | [color purple]{item.ItemName}[/color] x [color #db8cff]{item.ItemQuantity}[/color] | [color purple]${item.Price}[/color] | [color #db8cff]{Data.Globals.GetUserNameByID(item.UserID)}");
                        }
                        else
                        {
                            pl.SendClientMessage($"[color #db8cff]# {item.InternalID}[/color] | [color purple]{item.ItemName}[/color] | [color purple]${item.Price}[/color] | [color #db8cff]{Data.Globals.GetUserNameByID(item.UserID)}");
                        }

                    }
                    List<RustPP.Data.Entities.StoreItem> nextPage = EconomyComponent.GetPageItemss(EconomyComponent.StoreItems, 2, 10);
                    if (nextPage.Count > 0)
                    {
                        pl.SendClientMessage("[color purple]<!>[/color] Use [color #db8cff]/tienda cuenta 2[/color] para ver más resultados.");
                    }
                    pl.SendClientMessage("[color purple]<!>[/color] TIP: Use [color #db8cff]/tienda ver <Numero>[/color] para ver los detalles del Item y [color #db8cff]/tienda comprar[/color] para comprarlo.");
                }
                else
                {
                    string pagestring = ChatArguments[1];
                    int page = Int32.Parse(pagestring);
                    List<Data.Entities.StoreItem> playerItems = EconomyComponent.StoreItems.FindAll(x => x.UserID == user.ID);
                    List<RustPP.Data.Entities.StoreItem> itemList = EconomyComponent.GetPageItemss(playerItems, page, 10);
                    pl.SendClientMessage($"[color #db8cff] ---------------------- [color white]HYRUST TIENDA - Pagina {page}[/color] ----------------------");
                    foreach (Data.Entities.StoreItem item in itemList)
                    {
                        if (item.ItemQuantity > 1)
                        {
                            pl.SendClientMessage($"[color #db8cff]# {item.InternalID}[/color] | [color purple]{item.ItemName}[/color] x [color #db8cff]{item.ItemQuantity}[/color] | [color purple]${item.Price}[/color] | [color #db8cff]{Data.Globals.GetUserNameByID(item.UserID)}");
                        }
                        else
                        {
                            pl.SendClientMessage($"[color #db8cff]# {item.InternalID}[/color] | [color purple]{item.ItemName}[/color] | [color purple]${item.Price}[/color] | [color #db8cff]{Data.Globals.GetUserNameByID(item.UserID)}");
                        }
                    }
                    List<RustPP.Data.Entities.StoreItem> nextPage = EconomyComponent.GetPageItemss(EconomyComponent.StoreItems, page + 1, 10);
                    if (nextPage.Count > 0)
                    {
                        pl.SendClientMessage($"[color purple]<!>[/color] Use [color #db8cff]/tienda cuenta {page + 1}[/color] para ver más resultados.");
                    }
                }
            }

        }
    }
}
