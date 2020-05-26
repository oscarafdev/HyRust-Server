using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.AuthComponent
{
    class DudaCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (AuthComponent.UserIsLogged(pl))
            {
                RustPP.Data.Entities.User user = Data.Globals.usersOnline.FindLast(x => x.Name == pl.Name);
                if (ChatArguments.Length < 1)
                {
                    pl.SendClientMessage("[color red]<Sintaxis>[/color] /duda <Tu Pregunta>");
                    return;
                }
                List<string> wth = ChatArguments.ToList();
                string message;
                try
                {
                    message = string.Join(" ", wth.ToArray()).Trim(new char[] { ' ', '"' }).Replace('"', 'ˮ');
                }
                catch
                {
                    pl.SendClientMessage("[color red]<Error>[/color] Algo salio mal, intentalo nuevamente más tarde");
                    return;
                }
                if (message == string.Empty)
                {
                    pl.SendClientMessage("[color red]<Sintaxis>[/color] /duda <Tu pregunta>");
                }
                else
                {
                    pl.SendClientMessage($"[color #f77777]Enviaste uan duda al staff en breve serás atendido.");

                    foreach (RustPP.Data.Entities.User usuario in RustPP.Data.Globals.usersOnline)
                    {
                        if (usuario.AdminLevel >= 1)
                        {
                            usuario.Player.SendClientMessage($"[color red]<!>[/color] [color #f77777]{pl.Name} envió una duda: {message}");
                        }
                        
                    }
                    user.TimeToDuda += 30;
                    
                }


            }
            else
            {
                pl.SendClientMessage("[color red]<Error>[/color] Primero debes estar conectado (Utiliza [color orange] /login[/color])");
            }

        }
    }
}
