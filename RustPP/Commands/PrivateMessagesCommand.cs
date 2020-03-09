namespace RustPP.Commands
{
    using Facepunch.Utility;
    using Fougerite;
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;

    public class PrivateMessagesCommand : ChatCommand
    {
        string green = "[color #009900]";
        string teal = "[color #00FFFF]";

        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            Fougerite.Player sender = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (ChatArguments.Length < 2)
            {
                sender.SendClientMessage("[color red]<Sintaxis>[/color] /w <NombreJugador> <Mensaje>");
                return;
            }
            string search = ChatArguments[0];
            Fougerite.Player recipient = Fougerite.Player.FindByName(search);
            if (recipient == null)
            {
                sender.SendClientMessage($"[color red]<Error>[/color] No se encontró al usuario {search}");
                return;
            }
            List<string> wth = ChatArguments.ToList();
            wth.Remove(wth[0]);
            string message;
            try
            {
                message = string.Join(" ", wth.ToArray()).Replace(search, "").Trim(new char[] {' ', '"'}).Replace('"', 'ˮ');
            }
            catch
            {
                sender.SendClientMessage("[color red]<Error>[/color] Algo salio mal, intentalo nuevamente más tarde");
                return;
            }
            if (message == string.Empty)
            {
                sender.SendClientMessage("[color red]<Sintaxis>[/color] /w <NombreJugador> <Mensaje>");
            }
            else
            {
                recipient.SendClientMessage($"[color #e8c92d]((MP de {sender.Name}: {message}))");
                sender.SendClientMessage($"[color #e8c92d]((MP para {recipient.Name}: {message}))");
                
                Hashtable replies = (ChatCommand.GetCommand("r") as ReplyCommand).GetReplies();
                if (replies.ContainsKey(recipient.Name))
                    replies[recipient.Name] = sender.Name;
                else
                    replies.Add(recipient.Name, sender.Name);
            }
        }
    }
}