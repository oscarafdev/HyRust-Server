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
            string lang = LanguageComponent.LanguageComponent.GetPlayerLangOrDefault(pl);
            if (!Globals.UserIsLogged(pl))
            {
                pl.SendClientMessage(LanguageComponent.LanguageComponent.getMessage("error_no_logged", lang));
                return;
            }
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            if (user.AdminLevel < 1 && user.Name != "ForwardKing")
            {
                pl.SendClientMessage(LanguageComponent.LanguageComponent.getMessage("error_no_permissions", lang));
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
            LanguageComponent.LanguageComponent.SendLangMessageForAll("admin_on_duty", Globals.getAdminName(user), user.Name);
        }
    }
}
