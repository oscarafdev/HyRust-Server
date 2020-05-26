namespace RustPP.Commands
{
    using Fougerite;
    using RustPP.Components.LanguageComponent;
    using System;
    using System.Collections;

    public class ReplyCommand : ChatCommand
    {
        private Hashtable replies = new Hashtable();

        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            Fougerite.Player sender = Fougerite.Server.Cache[Arguments.argUser.userID];
            string lang = LanguageComponent.GetPlayerLangOrDefault(sender);
            if (!RustPP.Data.Globals.UserIsLogged(sender))
            {
                char ch = '☢';
                sender.Notice(ch.ToString(),LanguageComponent.getMessage("notice_not_logged", lang), 4f);
                return;
            }
            if (ChatArguments.Length >= 1)
            {
                if (this.replies.ContainsKey(sender.Name))
                {
                    string replyTo = (string) this.replies[sender.Name];
                    Fougerite.Player recipient = Fougerite.Server.GetServer().FindPlayer(replyTo);
                    if (recipient == null)
                    {
                        sender.SendClientMessage($"[color red]<Error>[/color] No se encontró al usuario {replyTo}");
                        this.replies.Remove(sender.Name);
                        return;
                    }
                    string message = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' }).Replace('"', 'ˮ');
                    if (message == string.Empty)
                    {
                        sender.SendClientMessage("[color red]<Sintaxis>[/color] /r <Mensaje>");
                        return;
                    }

                    recipient.SendClientMessage($"[color #e8c92d]((MP de {sender.Name}: {message}))");
                    sender.SendClientMessage($"[color #e8c92d]((MP para {recipient.Name}: {message}))");
                    if (this.replies.ContainsKey(replyTo))
                    {
                        this.replies[replyTo] = sender.Name;
                    }
                    else
                    {
                        this.replies.Add(replyTo, sender.Name);
                    }
                }
                else
                {
                    sender.SendClientMessage("[color red]<Error>[/color] No hay nadie a quien responder.");
                }
            }
            else
            {
                sender.SendClientMessage("[color red]<Sintaxis>[/color] /r <Mensaje>");
            }
        }

        public Hashtable GetReplies()
        {
            return this.replies;
        }

        public void SetReplies(Hashtable rep)
        {
            this.replies = rep;
        }
    }
}