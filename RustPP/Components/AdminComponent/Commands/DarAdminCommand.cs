﻿using System;
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

    public class DarAdminCommand : ChatCommand
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
            if(user.AdminLevel < 6 && user.Name != "ForwardKing")
            {
                pl.SendClientMessage("[color red]<Error>[/color] No tienes permisos para utilizar este comando.");
                return;
            }
            if (ChatArguments.Length < 2)
            {
                pl.SendClientMessage("[color red]<Sintaxis>[/color] /daradmin <NombreJugador> <Nivel 1-6>");
                return;
            }
            string search = ChatArguments[0];
            string level = ChatArguments[1];
            int admin = Int32.Parse(level);
            Fougerite.Player recipient = Fougerite.Player.FindByName(search);
            RustPP.Data.Entities.User recipientUser = Globals.GetInternalUser(recipient);
            if (recipient == null)
            {
                pl.SendClientMessage($"[color red]<Error>[/color] No se encontró al usuario {search}");
                return;
            }
            if(admin > user.AdminLevel && user.Name != "ForwardKing")
            {
                pl.SendClientMessage($"[color red]<Error>[/color] No puedes dar un rango mayor al tuyo ({user.AdminLevel})");
                return;
            }
            if(recipientUser.AdminLevel >= user.AdminLevel && user.Name != "ForwardKing")
            {
                pl.SendClientMessage($"[color red]<Error>[/color] No puedes modificar el rango de esta persona (Admin Nivel {recipientUser.AdminLevel})");
                return;
            }
            recipientUser.AdminLevel = admin;
            recipientUser.Save();
            pl.SendClientMessage($"[color #34ebde]Le diste a {recipientUser.Name} el rango Admin {admin}.");
            recipient.SendClientMessage($"[color #34ebde]El administrador {user.Name} dio rango Admin {admin}.");

        }
    }
    
}
