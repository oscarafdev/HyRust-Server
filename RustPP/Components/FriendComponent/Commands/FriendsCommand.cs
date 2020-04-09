using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.FriendComponent.Commands
{
    class FriendsCommand : ChatCommand
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
            if(ChatArguments.Length < 1)
            {
                pl.SendClientMessage($"[color red]<Sintaxis>[/color] /{Command} <Opcion>");
                pl.SendClientMessage($"[color #fc038c]<Amigos>[/color] Opciones: [color $ffb3dd] agregar - eliminar - editar");
                return;
            }
            string search = ChatArguments[0];
            if(search == "agregar")
            {
                if (ChatArguments.Length < 2)
                {
                    pl.SendClientMessage($"[color red]<Sintaxis>[/color] /{Command} [color $ffb3dd]agregar[/color] <NombreJugador>");
                    return;
                }
                string invite = ChatArguments[1];
                Fougerite.Player recipient = Fougerite.Player.FindByName(invite);
            }
            else if (search == "eliminar")
            {
                if (ChatArguments.Length < 2)
                {
                    pl.SendClientMessage($"[color red]<Sintaxis>[/color] /{Command} [color $ffb3dd]eliminar[/color] <NombreJugador>");
                    return;
                }
                string invite = ChatArguments[1];
                Fougerite.Player recipient = Fougerite.Player.FindByName(invite);
            }
            else if (search == "editar")
            {
                if (ChatArguments.Length < 3)
                {
                    pl.SendClientMessage($"[color red]<Sintaxis>[/color] /{Command} [color $ffb3dd]editar[/color] <NombreJugador> <Permiso>");
                    pl.SendClientMessage($"[color #fc038c]<Amigos>[/color] Permisos: [color $ffb3dd] loot - puertas - home - construir");
                    return;
                }
                string invite = ChatArguments[1];
                Fougerite.Player recipient = Fougerite.Player.FindByName(invite);
                string option = ChatArguments[3];
            }
        }
    }
}
