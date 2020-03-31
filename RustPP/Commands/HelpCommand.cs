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
                pl.SendClientMessage("Secciones: cuenta - chat - amigos - tp - kit - casa - clanes [color red]¡Nuevo![/color]");
                return;
            }
            string search = ChatArguments[0].ToLower();
            if (search == "clanes")
            {
                pl.SendClientMessage($"[color orange]--------[/color] AYUDA - CLANES [color orange]--------");
                pl.SendClientMessage($"- Poseemos un sistema de clanes por niveles, y un ranking de clanes.");
                pl.SendClientMessage($"- En cada payday los miembros aportarán 1 punto de experiencia al clan.");
                pl.SendClientMessage($"- También aportarán materiales al farmear y experiencia al asesinar una persona.");
                pl.SendClientMessage($"- [color cyan]/clanes[/color] Muestra el top 10 de clanes (Ordenado por nivel).");
                pl.SendClientMessage($"- [color cyan]/crearclan[/color] Comando para fundar un nuevo clan (Cuesta $100.000).");
                pl.SendClientMessage($"- [color cyan]/clan[/color] Si estas en un clan, este comando te dirá los comandos disponibles.");
                pl.SendClientMessage($"- [color cyan]/f[/color] Canal privado del clan.");
                pl.SendClientMessage($"- [color cyan]/kitclan[/color] ¡Te da un kit del clan!. (Proximamente)");
                return;
            }
            if (search == "cuenta")
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
                pl.SendClientMessage($"- [color cyan]/reportar[/color] Reportar a un usuario.");
                pl.SendClientMessage($"- [color cyan]/g[/color] Gritar (Rango 80m).");
                pl.SendClientMessage($"- [color cyan]/w[/color] Envia un mensaje privado a un jugador.");
                pl.SendClientMessage($"- [color cyan]/r[/color] Response un mensaje privado.");
                pl.SendClientMessage($"- [color cyan]/historial[/color] Revisa tu historial de mensajes.");
            }
            else if (search == "casa")
            {
                pl.SendClientMessage($"[color orange]--------[/color] AYUDA - CASA [color orange]--------");
                pl.SendClientMessage($"- [color cyan]/sethome[/color] Guardar tu casa.");
                pl.SendClientMessage($"- [color cyan]/home[/color] Ir hacia tu casa.");
                pl.SendClientMessage($"- [color cyan]/remove[/color] Destruir una parte de tu casa.");
                pl.SendClientMessage($"- [color cyan]/removeall[/color] Destruir toda tu casa.");
                pl.SendClientMessage($"- [color cyan]/prop[/color] Te dice el propietario de una casa.");
            }
            else if (search == "amigos")
            {
                pl.SendClientMessage($"[color orange]--------[/color] AYUDA - AMIGOS [color orange]--------");
                pl.SendClientMessage($"- [color cyan]/addfriend[/color] Agregar un amigo, no podrás hacerle daño.");
                pl.SendClientMessage($"- [color cyan]/unfriend[/color] Eliminar a un amigo, podrás hacerle daño.");
                pl.SendClientMessage($"- [color cyan]/addfriendh[/color] Permite a un amigo hacer home en tu casa.");
                pl.SendClientMessage($"- [color cyan]/delfriendh[/color] Elimina el permiso de hacer home en tu casa.");
                pl.SendClientMessage($"- [color cyan]/share[/color] Compartir puertas a un jugador, podrá abrir todas tus puertas.");
                pl.SendClientMessage($"- [color cyan]/unshare[/color] Dejar de compartir puertas con un jugador.");
            }
            else if (search == "tp")
            {
                pl.SendClientMessage($"[color orange]--------[/color] AYUDA - TP [color orange]--------");
                pl.SendClientMessage($"- [color cyan]/tp[/color] Teletransportes. (Cada 30 segundos)");
                pl.SendClientMessage($"- [color cyan]/tpr[/color] Envia una solicitud para transportarte a un jugador.");
                pl.SendClientMessage($"- [color cyan]/tpa[/color] Acepta un TP.");
                pl.SendClientMessage($"- [color cyan]/tpc[/color] Rechaza un TP.");
            }
            else if (search == "kit")
            {
                pl.SendClientMessage($"[color orange]--------[/color] AYUDA - KIT [color orange]--------");
                pl.SendClientMessage($"- El sistema de Kits funciona según el nivel de Cuenta, mientras más nivel mejores y más objetos.");
                pl.SendClientMessage($"- [color cyan]/kit[/color] Obtienes un kit (Cada 15 minutos).");
            }
        }
    }
}