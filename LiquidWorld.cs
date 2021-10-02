using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LiquidLib
{
    internal class LiquidWorld : ModSystem
    {
        const string TAG_UNLOADED_LIQUIDS = "unloaded_liquids";
        const string TAG_TYPES = "types";
        readonly HashSet<int> list = new();
        readonly HashSet<int> list2 = new();

        public override void SaveWorldData(TagCompound tag)
        {
            var byteArray = new byte[Main.maxTilesX * Main.maxTilesY];
            int index = 0;
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    var tile = Main.tile[i, j];

                    byteArray[index++] = (byte)tile.LiquidType;
                    list.Add(tile.LiquidType);

                    if (tile.type == LiquidLib.unloadedTileID)
                        list2.Add(tile.frameX);
                }
            }

            var t = new TagCompound();

            foreach (var liquidType in list)
                if (LiquidLoader.liquids.TryGetValue(liquidType, out var modLiquid))
                    t[modLiquid.FullName] = liquidType;

            foreach (var liquidType in list2)
                if (LiquidLoader.unloadedLiquids.TryGetValue(liquidType, out var name))
                    t[name] = liquidType;

            tag[TAG_UNLOADED_LIQUIDS] = t;
            tag[TAG_TYPES] = byteArray;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (!tag.ContainsKey(TAG_TYPES) || !tag.ContainsKey(TAG_UNLOADED_LIQUIDS))
                return;

            LiquidLoader.ReloadTypes(tag.GetCompound(TAG_UNLOADED_LIQUIDS));

            var byteArray = (byte[])tag[TAG_TYPES];
            int index = 0;

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    var tile = Main.tile[i, j];
                    tile.LiquidType = byteArray[index++];

                    if (!LiquidLoader.liquids.TryGetValue(tile.LiquidType, out _) &&
                        LiquidLoader.unloadedLiquids.TryGetValue(tile.LiquidType, out _)) //Pack
                    {
                        tile.IsActive = true;
                        tile.type = LiquidLib.unloadedTileID;
                        tile.frameX = (short)tile.LiquidType;
                        tile.frameY = tile.LiquidAmount;
                        tile.LiquidType = 0;
                        tile.LiquidAmount = 0;
                    }
                    else if (tile.type == LiquidLib.unloadedTileID) //UnPack
                    {
                        if (LiquidLoader.unloadedLiquids.TryGetValue(tile.frameX, out var name) &&
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
}