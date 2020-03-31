namespace RustPP.Commands
{
    using Fougerite;
    using System;
    using System.Collections.Generic;

    public class GodModeCommand : ChatCommand
    {
        public List<ulong> userIDs = new List<ulong>();

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
            if (pl.CommandCancelList.Contains("god"))
            {
                if (userIDs.Contains(pl.UID))
                {
                    userIDs.Remove(pl.UID);
                    pl.PlayerClient.controllable.character.takeDamage.SetGodMode(false);
                }
                return;
            }
            if (!this.userIDs.Contains(pl.UID))
            {
                this.userIDs.Add(pl.UID);
                pl.PlayerClient.controllable.character.takeDamage.SetGodMode(true);
                if (pl.FallDamage != null) { pl.FallDamage.ClearInjury();}
                pl.SendClientMessage("¡Activaste el modo DIOS!");
            }
            else
            {
                this.userIDs.Remove(pl.UID);
                pl.PlayerClient.controllable.character.takeDamage.SetGodMode(false);
                pl.SendClientMessage("¡Desactivaste el modo DIOS!");
            }
        }

        public bool IsOn(ulong uid)
        {
            return this.userIDs.Contains(uid);
        }
    }
}