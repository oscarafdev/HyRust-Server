using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.AdminComponent.Commands
{
    using Fougerite;
    using RustPP;
    using RustPP.Permissions;
    using RustPP.Commands;
    using RustPP.Data;
    using System;

    public class AdminHelpCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            if (!RustPP.Data.Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(), $"No estas logueado, usa /login o /registro", 4f);
                return;
            }
            if (user.AdminLevel < 1 && user.Name != "ForwardKing")
            {
                pl.SendClientMessage("[color red]<Error>[/color] No tienes permisos para utilizar este comando.");
                return;
            }
            pl.SendClientMessage($"[color orange]--------[/color] AYUDA - ADMIN [color orange]--------");
            pl.SendClientMessage($"- Trate siempre con respeto al usuario, y no se deje guiar por sus emociones.");
            pl.SendClientMessage($"- [color cyan]/ah[/color] Comando de ayuda para administradores.");
            pl.SendClientMessage($"- [color cyan]/ir[/color] Teletransportarse a un jugador.");
            pl.SendClientMessage($"- [color cyan]/traer[/color] Traer a un jugador.");
            pl.SendClientMessage($"- [color cyan]/a[/color] Canal de administradores.");
            pl.SendClientMessage($"- [color cyan]/mutear[/color] Mutear a un jugador.");
            pl.SendClientMessage($"- [color cyan]/desmutear[/color] Desmutea a un jugador.");
            if(user.AdminLevel >= 2)
            {
                pl.SendClientMessage($"- [color cyan]/anuncio[/color] Anuncio general.");
                pl.SendClientMessage($"- [color cyan]/adminkit[/color] Admin Kit.");
            }
            if (user.AdminLevel >= 3)
            {
                pl.SendClientMessage($"- [color cyan]/ban[/color] Banear a un usuario.");
                pl.SendClientMessage($"- [color cyan]/limpiarinv[/color] Limpiar el inventario a un usuario.");
            }
            if (user.AdminLevel >= 4)
            {
                pl.SendClientMessage($"- [color cyan]/ao[/color] Chat general como administrador.");
                pl.SendClientMessage($"- [color cyan]/payday[/color] Llama al PayDay inmediatamente.");
            }
            if (user.AdminLevel >= 5)
            {
                pl.SendClientMessage($"- [color cyan]/i[/color] Spawneas un item.");
                pl.SendClientMessage($"- [color cyan]/dar[/color] Dar un item.");
                pl.SendClientMessage($"- [color cyan]/saveloc[/color] Guarda una ubicación en la base de datos.");
            }
            if (user.AdminLevel >= 6)
            {
                pl.SendClientMessage($"- [color cyan]/daradmin[/color] Dar admin.");
            }
        }
    }
}
