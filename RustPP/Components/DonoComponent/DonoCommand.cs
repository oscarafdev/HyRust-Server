using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RustPP.Components.DonoComponent
{
    class DonoCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var player = Fougerite.Server.Cache[Arguments.argUser.userID];
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (RustPP.Components.AuthComponent.AuthComponent.UserIsLogged(player))
            {
                RustPP.Data.Entities.User usuario = Data.Globals.usersOnline.FindLast(x => x.Name == pl.Name);
                var position = player.PlayerClient.controllable.character.transform.position;
                var direction = player.PlayerClient.controllable.character.eyesRay.direction;
                position.y += player.PlayerClient.controllable.character.stateFlags.crouch ? 0.85f : 1.65f;
                if (Facepunch.MeshBatch.MeshBatchPhysics.Raycast(new Ray(position, direction), out var raycastHit, 10, -1, out var flag, out var meshBatchInstance))
                {
                    var idmain = flag ? meshBatchInstance.idMain : IDBase.GetMain(raycastHit.collider);
                    if (idmain != null)
                    {
                        if (idmain.gameObject.GetComponent<StructureComponent>())
                        {
                            var id = idmain.gameObject.GetComponent<StructureComponent>()._master.ownerID;
                            var user = Fougerite.Player.FindByGameID(id.ToString());
                            player.Message("[color yellow]<!>[/color] Este " + idmain.gameObject.name + " pertenece a " + user.Name);
                            if (usuario.AdminLevel > 1)
                            {
                                player.Message("Ubicación: " + idmain.gameObject.transform.position);
                                player.Message("Vida: " + idmain.gameObject.GetComponent<TakeDamage>().health +
                                               "/" + idmain.gameObject.GetComponent<TakeDamage>().maxHealth);
                            }
                        }
                        if (idmain.gameObject.GetComponent<DeployableObject>())
                        {
                            var id = idmain.gameObject.GetComponent<DeployableObject>().ownerID;
                            var user = Fougerite.Player.FindByGameID(id.ToString());
                            player.Message("[color yellow]<!>[/color] Este " + idmain.gameObject.name + " pertenece a " + user.Name);
                            if (usuario.AdminLevel > 1 )
                            {
                                player.Message("Ubicación: " + idmain.gameObject.transform.position);
                                player.Message("Vida: " + idmain.gameObject.GetComponent<TakeDamage>().health +
                                               "/" + idmain.gameObject.GetComponent<TakeDamage>().maxHealth);
                            }
                        }
                    }
                }
            }
            else
            {
                pl.SendClientMessage("[color red]<Error>[/color] Primero debes estar conectado (Utiliza [color orange] /login[/color])");
            }
            
        }

    }
}
