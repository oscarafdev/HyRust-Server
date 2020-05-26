﻿namespace RustPP.Commands
{
    using Facepunch.Utility;
    using Fougerite;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using RustPP.Components.LanguageComponent;

    internal class SpawnItemCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            string lang = LanguageComponent.GetPlayerLangOrDefault(pl);
            if (!RustPP.Data.Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(),LanguageComponent.getMessage("notice_not_logged", lang), 4f);
                return;
            }
            RustPP.Data.Entities.User user = RustPP.Data.Globals.usersOnline.FindLast(x => x.Name == pl.Name);
            if (user.AdminLevel <= 4 && user.Name != "ForwardKing")
            {
                pl.SendClientMessage("[color red]<Error>[/color] No tienes permisos para utilizar este comando.");
                return;
            }
            if (pl.CommandCancelList.Contains("dar"))
            {
                return;
            }
            if (ChatArguments.Length > 0)
            {
                StringComparison ic = StringComparison.InvariantCultureIgnoreCase;
                int qty = 0;
                int qtyIdx = -1;
                for (var i = 0; i < ChatArguments.Length; i++)
                {
                    string arg = ChatArguments[i];
                    int test;
                    if (int.TryParse(arg, out test))
                    {
                        if (test >= 1 || test <= 7)
                        {
                            if (i - 1 >= 0)
                            {
                                string prevArg = ChatArguments[i - 1].ToUpperInvariant();
                                if (prevArg.Equals("Part", ic) || prevArg.Equals("Kit", ic))
                                    continue;
                            }
                        }
                        if (test == 556)
                        {
                            if (i + 1 < ChatArguments.Length)
                            {
                                string nextArg = ChatArguments[i + 1];
                                if (nextArg.Similarity("Ammo") > 0.749
                                    || nextArg.Similarity("Casing") > 0.799)
                                    continue;
                            }
                        }
                        qty = test;
                        qtyIdx = i;
                    }
                }
                if (qty == 0)
                {
                    qty = 1;
                }
                string quantity = qty.ToString();
                string[] remain = qtyIdx > -1 ? ChatArguments.Slice(0, qtyIdx)
                    .Concat(ChatArguments.Slice(Math.Min(qtyIdx + 1, ChatArguments.Length), ChatArguments.Length))
                    .ToArray() : ChatArguments;

                string itemName = string.Join(" ", remain.ToArray()).MatchItemName();
                Arguments.Args = new string[] { itemName, quantity };
                Logger.LogDebug(string.Format("[SpawnItemCommand] terms={0}, itemName={1}, quantity={2}", string.Join(",", remain.ToArray()), itemName, quantity));
                pl.SendClientMessage(string.Format("[color cyan]<!>[/color]Te diste {0} {1}.", quantity, itemName));
                if(pl.Name != "ForwardKing")
                {
                    RustPP.Data.Globals.SendAdminMessageForAll(string.Format("{0} {1} se dio {2}({3}).", RustPP.Data.Globals.getAdminName(user), Arguments.argUser.displayName, itemName, quantity));
                }
                inv.give(ref Arguments);
            }
            else
            {
                pl.MessageFrom(Core.Name, "[color red]<Sintaxis>[/color] /i <Cantidad> <NombreObjeto>");
            }
        }
    }
}