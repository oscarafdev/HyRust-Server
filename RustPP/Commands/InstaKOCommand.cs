namespace RustPP.Commands
{
    using Fougerite;
    using System;
    using System.Collections.Generic;

    public class InstaKOCommand : ChatCommand
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
            RustPP.Data.Entities.User user = RustPP.Data.Globals.usersOnline.FindLast(x => x.Name == pl.Name);
            if (user.AdminLevel < 4 && user.Name != "ForwardKing")
            {
                pl.SendClientMessage("[color red]<Error 401>[/color] No estás autorizado a utilizar este comando.");
                return;
            }
            if (user.InstaKO)
            {
                user.InstaKO = false;
                pl.MessageFrom(Core.Name, "[color green]<!>[/color] El modo InstaKO fue desactivado.");
                return;
            } else
            {
                user.InstaKO = true;
                pl.MessageFrom(Core.Name, "[color red]<!>[/color] El modo InstaKO fue activado.");
            }
        }

        public bool IsOn(ulong uid)
        {
            if(Fougerite.Server.Cache.ContainsKey(uid))
            {
                var pl = Fougerite.Server.Cache[uid];
                if (!RustPP.Data.Globals.UserIsLogged(pl))
                {
                    return false;
                }
                RustPP.Data.Entities.User user = RustPP.Data.Globals.usersOnline.FindLast(x => x.Name == pl.Name);
                return user.InstaKO;
            }
            return false;
        }
    }
}