using Fougerite;
using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.EconomyComponent.Commands
{
    class SellCommand : ChatCommand
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
            if (ChatArguments.Length < 2)
            {
                pl.SendClientMessage("[color red]<!>[/color] Escribe el tipo de evento, clan o exp.");
                pl.SendClientMessage("[color red]<Sintaxis>[/color] /evento <tipo> <Rate>");
                return;
            }
            RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
            if(user.AdminLevel == 6)
            {
                string invite = ChatArguments[0];
                if(invite == "clan")
                {
                    string exp = ChatArguments[1];
                    int rate = Int32.Parse(exp);
                    Data.Globals.EventoExpClan = rate;
                    if(rate != 1)
                    {
                        Server.GetServer().SendMessageForAll($"[color blue]<!>[/color][color cyan] Se activo el evento EXP CLAN x{rate}");
                    }
                    
                }
                else if(invite == "exp")
                {
                    string exp = ChatArguments[1];
                    int rate = Int32.Parse(exp);
                    Data.Globals.EventoExp = rate;
                    if (rate != 1)
                    {
                        Server.GetServer().SendMessageForAll($"[color blue]<!>[/color][color cyan] Se activo el evento EXP x{rate}");
                    }
                    
                    
                }
                
            }
            
        }
        
    }
}
