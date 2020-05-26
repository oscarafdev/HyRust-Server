using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.FriendComponent.Commands
{
    class FriendsCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            string lang = LanguageComponent.LanguageComponent.GetPlayerLangOrDefault(pl);
            if (!RustPP.Data.Globals.UserIsLogged(pl))
            {
                char ch = '☢';
                pl.Notice(ch.ToString(),LanguageComponent.LanguageComponent.getMessage("notice_not_logged", lang), 4f);
                return;
            }
            RustPP.Data.Entities.User user = Data.Globals.GetInternalUser(pl);
            if(ChatArguments.Length < 1)
            {
                pl.SendClientMessage($"[color red]<Sintaxis>[/color] {Command} <Opcion>");
                pl.SendClientMessage($"[color #fc038c]<Amigos>[/color] Opciones: [color $ffb3dd] agregar - eliminar - editar");
                return;
            }
            string search = ChatArguments[0];
            if (search == "agregar")
            {
                if (ChatArguments.Length < 2)
                {
                    pl.SendClientMessage($"[color red]<Sintaxis>[/color] {Command} [color $ffb3dd]agregar[/color] <NombreJugador>");
                    return;
                }
                string invite = ChatArguments[1];
                Fougerite.Player recipient = Fougerite.Player.FindByName(invite);
                RustPP.Data.Entities.User recipientUser = null;
                if (recipient == null)
                {
                    recipientUser = Data.Globals.GetUserByName(invite);
                }
                else
                {
                    recipientUser = Data.Globals.GetUserByName(recipient.Name);
                }
                if (recipientUser == null)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] No se encontró un usuario llamado {invite}.");
                    return;
                }
                FriendComponent.AddFriend(user, recipientUser);
            }
            else if (search == "eliminar")
            {
                if (ChatArguments.Length < 2)
                {
                    pl.SendClientMessage($"[color red]<Sintaxis>[/color] /{Command} [color $ffb3dd]eliminar[/color] <NombreJugador>");
                    return;
                }
                string invite = ChatArguments[1];
                Fougerite.Player recipient = Fougerite.Player.FindByName(invite);
                Data.Entities.User recipientUser;
                if (recipient == null)
                {
                    recipientUser = Data.Globals.GetUserByName(invite);
                }
                else
                {
                    recipientUser = Data.Globals.GetUserByName(recipient.Name);
                }
                if (recipientUser == null)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] No se encontró un usuario llamado {invite}.");
                    return;
                }
                FriendComponent.RemoveFriend(user, recipientUser);
            }
            else if (search == "editar")
            {
                if (ChatArguments.Length < 3)
                {
                    pl.SendClientMessage($"[color red]<Sintaxis>[/color] /{Command} [color $ffb3dd]editar[/color] <NombreJugador> <Permiso>");
                    pl.SendClientMessage($"[color #fc038c]<Amigos>[/color] Permisos: [color $ffb3dd] loot - puertas - home - construir");
                    return;
                }
                string invite = ChatArguments[1];
                Fougerite.Player recipient = Fougerite.Player.FindByName(invite);
                Data.Entities.User recipientUser;
                if (recipient == null)
                {
                    recipientUser = Data.Globals.GetUserByName(invite);
                }
                else
                {
                    recipientUser = Data.Globals.GetUserByName(recipient.Name);
                }
                if (recipientUser == null)
                {
                    pl.SendClientMessage($"[color red]<Error>[/color] No se encontró un usuario llamado {invite}.");
                    return;
                }
                if(FriendComponent.IsFriendOf(user, recipientUser))
                {
                    RustPP.Data.Entities.Friend friend = FriendComponent.GetUserFriend(user, recipientUser);
                    string perm = ChatArguments[2];
                    if(perm == "loot")
                    {
                        if(friend.CanLoot)
                        {
                            friend.CanLoot = true;
                            friend.Save();
                            if (recipientUser.Connected == 1)
                            {
                                recipientUser.Player.SendClientMessage($"[color #fc038c]<Amigos>[/color] {user.Name} te dio permisos para abrir sus cajas.");
                            }
                            user.Player.SendClientMessage($"[color #fc038c]<Amigos>[/color] Le diste permisos a [color #fc038c]{recipientUser.Name}[/color] para abrir tus cajas.");
                        } else
                        {
                            friend.CanLoot = false;
                            friend.Save();
                            if (recipientUser.Connected == 1)
                            {
                                recipientUser.Player.SendClientMessage($"[color #fc038c]<Amigos>[/color] {user.Name} te quitó los permisos para abrir sus cajas.");
                            }
                            user.Player.SendClientMessage($"[color #fc038c]<Amigos>[/color] Le quitaste permisos a [color #fc038c]{recipientUser.Name}[/color] para abrir tus cajas.");
                        }
                    }
                    else if (perm == "puertas")
                    {
                        if (friend.CanOpenDoors)
                        {
                            friend.CanOpenDoors = true;
                            friend.Save();
                            if (recipientUser.Connected == 1)
                            {
                                recipientUser.Player.SendClientMessage($"[color #fc038c]<Amigos>[/color] {user.Name} te dio permisos para abrir sus puertas.");
                            }
                            user.Player.SendClientMessage($"[color #fc038c]<Amigos>[/color] Le diste permisos a [color #fc038c]{recipientUser.Name}[/color] para abrir tus puertas.");
                        }
                        else
                        {
                            friend.CanOpenDoors = false;
                            friend.Save();
                            if (recipientUser.Connected == 1)
                            {
                                recipientUser.Player.SendClientMessage($"[color #fc038c]<Amigos>[/color] {user.Name} te quitó los permisos para abrir sus puertas.");
                            }
                            user.Player.SendClientMessage($"[color #fc038c]<Amigos>[/color] Le quitaste permisos a [color #fc038c]{recipientUser.Name}[/color] para abrir tus puertas.");
                        }
                    }
                    else if (perm == "home")
                    {
                        if (friend.CanSetHome)
                        {
                            friend.CanSetHome = true;
                            friend.Save();
                            if (recipientUser.Connected == 1)
                            {
                                recipientUser.Player.SendClientMessage($"[color #fc038c]<Amigos>[/color] {user.Name} te dio permisos para guardar home en su casa.");
                            }
                            user.Player.SendClientMessage($"[color #fc038c]<Amigos>[/color] Le diste permisos a [color #fc038c]{recipientUser.Name}[/color] para guardar home en tu casa.");
                        }
                        else
                        {
                            friend.CanSetHome = false;
                            friend.Save();
                            if (recipientUser.Connected == 1)
                            {
                                recipientUser.Player.SendClientMessage($"[color #fc038c]<Amigos>[/color] {user.Name} te quitó los permisos para guardar home en su casa.");
                            }
                            user.Player.SendClientMessage($"[color #fc038c]<Amigos>[/color] Le quitaste permisos a [color #fc038c]{recipientUser.Name}[/color] para guardar home en tu casa.");
                        }
                    }
                    else if (perm == "construir")
                    {
                        if (friend.CanEditHome)
                        {
                            friend.CanEditHome = true;
                            friend.Save();
                            if (recipientUser.Connected == 1)
                            {
                                recipientUser.Player.SendClientMessage($"[color #fc038c]<Amigos>[/color] {user.Name} te dio permisos para editar la estructura de su casa (construir, destruir).");
                            }
                            user.Player.SendClientMessage($"[color #fc038c]<Amigos>[/color] Le diste permisos a [color #fc038c]{recipientUser.Name}[/color] para editar la estructura de tu casa (construir, destruir).");
                        }
                        else
                        {
                            friend.CanEditHome = false;
                            friend.Save();
                            if (recipientUser.Connected == 1)
                            {
                                recipientUser.Player.SendClientMessage($"[color #fc038c]<Amigos>[/color] {user.Name} te quitó los permisos para editar la estructura de su casa (construir, destruir).");
                            }
                            user.Player.SendClientMessage($"[color #fc038c]<Amigos>[/color] Le quitaste permisos a [color #fc038c]{recipientUser.Name}[/color] para editar la estructura de tu casa (construir, destruir).");
                        }
                    }
                }
                else
                {
                    user.Player.SendClientMessage($"[color #fc038c]<!>[/color] No eres amigo de {recipientUser.Name}.");
                }
                

            }
        }
    }
}
