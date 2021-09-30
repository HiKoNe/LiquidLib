using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LiquidLib
{
    internal class LiquidWorld : ModSystem
    {
        const string TAG_LIQUIDS = "liquids";
        TagCompound tag_tiles = null;
        static readonly HashSet<int> list = new();

        public static void PreSaveWorld()
        {
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    var tile = Main.tile[i, j];

                    //Pack
                    if (tile.LiquidType > 2 && tile.LiquidAmount > 0)
                    {
                        tile.IsActive = true;
                        tile.type = LiquidLib.unloadedTileID;
                        tile.frameX = (short)tile.LiquidType;
                        tile.frameY = tile.LiquidAmount;
                        tile.LiquidType = 0;
                        tile.LiquidAmount = 0;
                    }

                    if (tile.type == LiquidLib.unloadedTileID)
                        list.Add(tile.frameX);
                }
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            var t = tag_tiles ?? new();
            foreach (var modLiquid in LiquidLoader.liquids)
            {
                if (list.Contains(modLiquid.Type))
                    t[modLiquid.FullName] = modLiquid.Type;
                else
                    t.Remove(modLiquid.FullName);
            }

            tag[TAG_LIQUIDS] = t;
            tag_tiles = null;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (!tag.ContainsKey(TAG_LIQUIDS))
                return;

            LiquidLoader.ReloadTypes(tag_tiles = tag.GetCompound(TAG_LIQUIDS));

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    var tile = Main.tile[i, j];

                    //Unpack
                    if (tile.type == LiquidLib.unloadedTileID &&
                        LiquidLoader.unloadedLiquids.TryGetValue(tile.frameX, out var name) &&
                        LiquidLoader.liquids.TryGetValue(name, out _))
                    {
                        tile.IsActive = false;
                        tile.type = 0;
                        tile.LiquidType = tile.frameX;
                        tile.LiquidAmount = (byte)tile.frameY;
                        tile.frameX = 0;
                        tile.frameY = 0;
                    }
                }
            }
        }
    }
}