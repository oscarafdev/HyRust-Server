using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RustPP.Components.AuthComponent
{
    public class RegisterCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (!Regex.IsMatch(pl.Name, @"^[a-zA-Z0-9]*$"))
            {
                pl.SendClientMessage($"[color red]No se permiten carácteres especiales en el nombre (Permitido: [a-z] [0-9])");
                return;
            }
            if (ChatArguments.Length < 2)
            {
                pl.SendClientMessage("[color red]<Sintaxis> [color white]/registro <Contraseña> <ConfirmarContraseña>");
                return;
            }
            //string strText = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            AuthComponent.RegisterPlayer(pl, ChatArguments[0], ChatArguments[1]);
        }
    }
}
