namespace RustPP.Commands
{
    using Fougerite;
    using Rust;
    using System;

    public class AnnounceCommand : ChatCommand
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
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            if (user.AdminLevel < 2 && user.Name != "ForwardKing")
            {
                pl.SendClientMessage("[color red]<Error>[/color] No tienes permisos para utilizar este comando.");
                return;
            }
            string strText = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });

            if (strText == string.Empty)
            {
                pl.SendClientMessage("[color red]<Sintaxis>[/color] /anuncio <Texto>");
            }
            else
            {
                char ch = '☢';
                foreach (Fougerite.Player client in Fougerite.Server.GetServer().Players)
                {
                    client.Notice(ch.ToString(), strText, 5f);
                }
            }   
        }
    }
}