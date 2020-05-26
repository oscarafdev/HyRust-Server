using System;
using System.Diagnostics;
using System.Timers;
using Fougerite;

namespace RustPP.Components.AuthComponent
{
    using RustPP.Commands;
    using System.Text.RegularExpressions;

    public class LoginCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            
            string strText = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            if(strText == string.Empty)
            {
                pl.SendClientMessage("[color red]<Sintaxis> [color white]/login <Contraseña>");
            }
            AuthComponent.LoginPlayer(pl, pl.SteamID, strText);

        }
    }
}
