using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace LiquidLib
{
    public class UnloadedLiquidTile : ModTile
    {
        public override void SetStaticDefaults()
        {
			LiquidLib.unloadedTileID = Type;
			TileID.Sets.DisableSmartCursor[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileSolid[Type] = false;
		}

		public override void MouseOver(int i, int j)
		{
			if (Main.netMode != NetmodeID.SinglePlayer)
				return;

			var tile = Main.tile[i, j];
			if (tile != null && tile.type == Type)
			{
				var player = Main.LocalPlayer;
				player.cursorItemIconEnabled = true;
				player.cursorItemIconID = -1;
				player.cursorItemIconText = "Unloaded Liquid: " + (LiquidLoader.unloadedLiquids.TryGetValue(tile.frameX, out var name) ? name : "Undefined");
			}
		}
    }
}
