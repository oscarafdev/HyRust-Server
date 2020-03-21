namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using RustPP.Permissions;
    using System;

    public class HelpCommand : ChatCommand
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
            if (ChatArguments.Length < 1)
            {
                pl.SendClientMessage("[color red]<Sintaxis>[/color] /ayuda <Sección>");
                pl.SendClientMessage("Secciones: cuenta - chat - amigos - tp");
                return;
            }
            string search = ChatArguments[0].ToLower();
            if(search == "cuenta")
            {
                pl.SendClientMessage($"[color orange]--------[/color] AYUDA - CUENTA [color orange]--------");
                pl.SendClientMessage($"- Este servidor usa un guardado MySql, podrás acceder a tu cuenta desde otra PC con tu contraseña.");
                pl.SendClientMessage($"- [color cyan]/cuenta[/color] Muestra tus stats básicos.");
                pl.SendClientMessage($"- [color cyan]/farm[/color] Muestra tus stats de farmeo.");
                pl.SendClientMessage($"- [color cyan]/ubicacion[/color] Obtiene tu ubicación en el mapa.");
                pl.SendClientMessage($"- [color cyan]/login[/color] Conectarte a tu cuenta.");
                pl.SendClientMessage($"- [color cyan]/registro[/color] Registrar una nueva cuenta.");
            }
            else if(search == "chat")
            {
                pl.SendClientMessage($"[color orange]--------[/color] AYUDA - CHAT [color orange]--------");
                pl.SendClientMessage($"- El chat común es local, te puede leer cualquier jugador que este dentro de 30 metros.");
                pl.SendClientMessage($"- [color cyan]/duda[/color] Canal general de dudas, utiliza este canal para preguntas.");
                pl.SendClientMessage($"- [color cyan]/g[/color] Gritar (Rango 80m).");
                pl.SendClientMessage($"- [color cyan]/w[/color] Envia un mensaje privado a un jugador.");
                pl.SendClientMessage($"- [color cyan]/r[/color] Response un mensaje privado.");
                pl.SendClientMessage($"- [color cyan]/historial[/color] Revisa tu historial de mensajes.");
            }
            else if (search == "amigos")
            {
                pl.SendClientMessage($"[color orange]--------[/color] AYUDA - AMIGOS [color orange]--------");
                pl.SendClientMessage($"- [color cyan]/addfriend[/color] Agregar un amigo, no podrás hacerle daño.");
                pl.SendClientMessage($"- [color cyan]/unfriend[/color] Eliminar un amigo.");
                pl.SendClientMessage($"- [color cyan]/share[/color] Compartir puertas a un jugador, podrá abrir todas tus puertas.");
                pl.SendClientMessage($"- [color cyan]/unshare[/color] Dejar de compartir puertas con un jugador.");
            }
            else if (search == "tp")
            {
                pl.SendClientMessage($"[color orange]--------[/color] AYUDA - TP [color orange]--------");
                pl.SendClientMessage($"- [color cyan]/tpr[/color] Envia una solicitud para transportarte a un jugador.");
                pl.SendClientMessage($"- [color cyan]/tpa[/color] Acepta un TP.");
                pl.SendClientMessage($"- [color cyan]/tpc[/color] Rechaza un TP.");
            }
        }
    }
}