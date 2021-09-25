using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LiquidLib
{
    internal class LiquidWorld : ModSystem
    {
        const string TAG_NAME = "LiquidTile";

        public override void SaveWorldData(TagCompound tag)
        {
            var byteArray = new byte[Main.maxTilesX * Main.maxTilesY];
            int index = 0;

            for (int i = 0; i < Main.maxTilesX; i++)
                for (int j = 0; j < Main.maxTilesY; j++)
                    byteArray[index++] = (byte)Main.tile[i, j].LiquidType;

            tag.Add(TAG_NAME, byteArray);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            var byteArray = (byte[])tag[TAG_NAME];
            int index = 0;

            for (int i = 0; i < Main.maxTilesX; i++)
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    var liquidType = byteArray[index++];
                    if (liquidType < LiquidLoader.LiquidCount)
                        Main.tile[i, j].LiquidType = liquidType;
                    else
                        Main.tile[i, j].LiquidType = liquidType;
                }
        }
    }
}