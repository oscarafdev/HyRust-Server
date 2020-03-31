using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.AuthComponent
{
    class PagarCommand : ChatCommand
    {

        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (!Data.Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(), $"No estas logueado, usa /login o /registro", 4f);
                return;
            }
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);

            if (ChatArguments.Length < 2)
            {
                pl.SendClientMessage("[color red]<Sintaxis>[/color] /pagar <NombreJugador> <Cantidad>");
                return;
            }
            string search = ChatArguments[0];
            string level = ChatArguments[1];
            int quantity = Int32.Parse(level);
            Fougerite.Player recipient = Fougerite.Player.FindByName(search);
            RustPP.Data.Entities.User recipientUser = Data.Globals.GetInternalUser(recipient);
            if (recipient == null)
            {
                pl.SendClientMessage($"[color red]<Error>[/color] No se encontró al usuario {search}");
                return;
            }
            if (quantity > user.Cash && user.Name != "ForwardKing")
            {
                pl.SendClientMessage($"[color red]<Error>[/color] No tienes tanto dinero! (Cuenta: ${user.Cash})");
                return;
            }
 
            recipientUser.Cash += quantity;
            user.Cash -= quantity;
            recipientUser.Save();
            user.Save();
            pl.SendClientMessage($"[color #34ebde]Le pagaste a {recipientUser.Name} ${quantity}.");
            recipient.SendClientMessage($"[color #34ebde]El usuario {user.Name} te pagó ${quantity}.");

        }
    }
}
