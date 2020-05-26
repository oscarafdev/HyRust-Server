using RustPP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RustPP.Components.UtilityComponent
{
    class FPSCommand : ChatCommand
	{
		public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
		{
			Fougerite.Player pl = Fougerite.Server.Cache[Arguments.argUser.userID];
			string lang = LanguageComponent.LanguageComponent.GetPlayerLangOrDefault(pl);
			if (!RustPP.Data.Globals.UserIsLogged(pl))
			{
				char ch = '☢';
				pl.Notice(ch.ToString(),LanguageComponent.LanguageComponent.getMessage("notice_not_logged", lang), 4f);
				return;
			}
			RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(pl);
			FPS(pl, user.FPS);
		}
		public static void FPS(Fougerite.Player player, bool active)
		{
			RustPP.Data.Entities.User user = RustPP.Data.Globals.GetInternalUser(player);
			if (active)
			{
				player.SendCommand("render.fov 60");
				player.SendCommand("grass.on True");
				player.SendCommand("grass.forceredraw True");
				player.SendCommand("grass.displacement True");
				player.SendCommand("grass.disp_trail_seconds 1");
				player.SendCommand("grass.shadowcast True");
				player.SendCommand("grass.shadowreceive True");
				player.SendCommand("render.level 1");
				player.SendCommand("render.frames 1");
				player.SendCommand("render.vsync True");
				player.SendCommand("footsteps.quality 2");
				player.SendCommand("gfx.ssaa True");
				player.SendCommand("gfx.bloom True");
				player.SendCommand("gfx.grain True");
				player.SendCommand("gfx.ssao True");
				player.SendCommand("gfx.tonemap True");
				player.SendCommand("gfx.shafts True");
				player.SendCommand("water.level 1");
				player.SendCommand("render.distance 1");
				player.SendCommand("terrain.idleinterval 4");
				player.SendCommand("water.reflection True");
				player.SendCommand("config.save");
				player.SendClientMessage("[color cyan]<!>[/color] FPS Desactivado.");
				user.FPS = false;
			}
			else
			{
				player.SendCommand("grass.on false");
				player.SendCommand("grass.forceredraw false");
				player.SendCommand("grass.displacement false");
				player.SendCommand("grass.disp_trail_seconds 99999");
				player.SendCommand("grass.shadowcast false");
				player.SendCommand("grass.shadowreceive false");
				player.SendCommand("render.level 0");
				player.SendCommand("render.frames 0");
				player.SendCommand("render.vsync false");
				player.SendCommand("footsteps.quality 0");
				player.SendCommand("gfx.ssaa false");
				player.SendCommand("gfx.bloom false");
				player.SendCommand("gfx.grain false");
				player.SendCommand("gfx.ssao false");
				player.SendCommand("gfx.damage false");
				player.SendCommand("gfx.tonemap false");
				player.SendCommand("gfx.shafts false");
				player.SendCommand("render.distance 0");
				player.SendCommand("render.fov 60");
				player.SendCommand("water.level -1");
				player.SendCommand("terrain.idleinterval 0");
				player.SendCommand("water.reflection false");
				player.SendCommand("config.save");
				player.SendClientMessage("[color cyan]<!>[/color] FPS Activado.");
				user.FPS = false;
			}
			
		}
    }
}
