namespace RustPP.Commands
{
    using Fougerite;
    using RustPP.Components.LanguageComponent;
    using RustPP.Permissions;
    using System;

    public class LocationCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            string lang = LanguageComponent.GetPlayerLangOrDefault(pl);
            if (!RustPP.Data.Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(),LanguageComponent.getMessage("notice_not_logged", lang), 4f);
                return;
            }
            RustPP.Data.Entities.User user = RustPP.Data.Globals.usersOnline.FindLast(x => x.Name == pl.Name);
            string targetName = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            if (targetName.Equals(string.Empty) || targetName.Equals(Arguments.argUser.displayName))
            {
                string reply;
                if (GetLocationString(ref Arguments.argUser, pl, out reply))
                {
                    Arguments.ReplyWith(reply);
                    pl.SendClientMessage(reply);
                }
                return;
            }
            if (user.AdminLevel <= 1)
            {
                pl.SendClientMessage("[color red]<Error>[/color] Solo los administradores pueden saber la ubicación de otro usuario.");
                return;
            }
            foreach (Fougerite.Player client in Fougerite.Server.GetServer().Players)
            {
                if (targetName.Equals("todos", StringComparison.OrdinalIgnoreCase) ||
                    targetName.Equals(client.Name, StringComparison.OrdinalIgnoreCase) ||
                    targetName.ToUpperInvariant().Contains(client.Name.ToUpperInvariant()))
                {
                    string reply;
                    if (GetLocationString(ref Arguments.argUser, client, out reply))
                    {
                        Arguments.ReplyWith(reply);
                        pl.SendClientMessage(reply);
                    }
                }
            }
        }

        public bool GetLocationString(ref NetUser source, Fougerite.Player location, out string reply)
        {
            bool flag = false;
            try
            {
                string[] v3 = location.Location.ToString("F").Trim(new char[] { '(', ')', ' ' }).Split(new char[] { ',' });
                reply = string.Format("{3}: X:{0} Y:{1} Z:{2}", v3[0], v3[1], v3[2],
                    (location.PlayerClient.netUser == source ? "Tu ubicación" : string.Format("Ubicación de {0}", location.Name)));
                flag = true;
            } catch (Exception)
            {
                reply = string.Empty;
            }
            return flag;
        }
    }
}
