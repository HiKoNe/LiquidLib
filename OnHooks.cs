using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace LiquidLib
{
    internal static class OnHooks
    {

        static event hook_OnInitialize OnInitialize
        {
            add => HookEndpointManager.Add(typeof(Mod).Assembly
                    .GetType("Terraria.ModLoader.UI.UIModItem")
                    .GetMethod("OnInitialize", BindingFlags.Public | BindingFlags.Instance), value);
            remove => HookEndpointManager.Remove(typeof(Mod).Assembly
                    .GetType("Terraria.ModLoader.UI.UIModItem")
                    .GetMethod("OnInitialize", BindingFlags.Public | BindingFlags.Instance), value);
        }
        delegate void hook_OnInitialize(orig_OnInitialize orig, object self);
        delegate void orig_OnInitialize(object self);

        public static void Load()
        {
            On.Terraria.Tile.liquidType += Tile_liquidType;
            On.Terraria.Tile.liquidType_int += Tile_liquidType_int;
            On.Terraria.Tile.lava += Tile_lava;
            On.Terraria.Tile.lava_bool += Tile_lava_bool;
            On.Terraria.Tile.honey += Tile_honey;
            On.Terraria.Tile.honey_bool += Tile_honey_bool;
            HookEndpointManager.Add(typeof(Tile).GetMethod("get_LiquidType"), get_LiquidType);
            HookEndpointManager.Add(typeof(Tile).GetMethod("set_LiquidType"), set_LiquidType);
            On.Terraria.Main.DoUpdate += Main_DoUpdate;
            On.Terraria.Liquid.Update += Liquid_Update;
            On.Terraria.Liquid.LavaCheck += Liquid_LavaCheck;
            On.Terraria.Liquid.HoneyCheck += Liquid_HoneyCheck;
            On.Terraria.NetMessage.CompressTileBlock_Inner += NetMessage_CompressTileBlock_Inner;
            On.Terraria.NetMessage.DecompressTileBlock_Inner += NetMessage_DecompressTileBlock_Inner;
            OnInitialize += OnHooks_OnInitialize;
            On.Terraria.Wiring.XferWater += Wiring_XferWater;
        }

        public static void Unload()
        {
            On.Terraria.Tile.liquidType -= Tile_liquidType;
            On.Terraria.Tile.liquidType_int -= Tile_liquidType_int;
            On.Terraria.Tile.lava -= Tile_lava;
            On.Terraria.Tile.lava_bool -= Tile_lava_bool;
            On.Terraria.Tile.honey -= Tile_honey;
            On.Terraria.Tile.honey_bool -= Tile_honey_bool;
            HookEndpointManager.Remove(typeof(Tile).GetMethod("get_LiquidType"), get_LiquidType);
            HookEndpointManager.Remove(typeof(Tile).GetMethod("set_LiquidType"), set_LiquidType);
            On.Terraria.Main.DoUpdate -= Main_DoUpdate;
            On.Terraria.Liquid.Update -= Liquid_Update;
            On.Terraria.Liquid.LavaCheck -= Liquid_LavaCheck;
            On.Terraria.Liquid.HoneyCheck -= Liquid_HoneyCheck;
            On.Terraria.NetMessage.CompressTileBlock_Inner -= NetMessage_CompressTileBlock_Inner;
            On.Terraria.NetMessage.DecompressTileBlock_Inner -= NetMessage_DecompressTileBlock_Inner;
            OnInitialize -= OnHooks_OnInitialize;
            On.Terraria.Wiring.XferWater -= Wiring_XferWater;
        }

        static byte Tile_liquidType(On.Terraria.Tile.orig_liquidType orig, Tile self) =>
            GetLiquidType(self);

        static void Tile_liquidType_int(On.Terraria.Tile.orig_liquidType_int orig, Tile self, int liquidType) =>
            SetLiquidType(self, (byte)liquidType);

        static bool Tile_lava(On.Terraria.Tile.orig_lava orig, Tile self) =>
            GetLiquidType(self) == LiquidID.Lava;

        static void Tile_lava_bool(On.Terraria.Tile.orig_lava_bool orig, Tile self, bool lava)
        {
            if (lava)
                SetLiquidType(self, LiquidID.Lava);
            else
                SetLiquidType(self, 0);
        }

        static bool Tile_honey(On.Terraria.Tile.orig_honey orig, Tile self) =>
            GetLiquidType(self) == LiquidID.Honey;

        static void Tile_honey_bool(On.Terraria.Tile.orig_honey_bool orig, Tile self, bool honey)
        {
            if (honey)
                SetLiquidType(self, LiquidID.Honey);
            else
                SetLiquidType(self, 0);
        }

        static Func<Tile, int> get_LiquidType =
            (tile) => GetLiquidType(tile);

        static Action<Tile, int> set_LiquidType =
            (tile, type) => SetLiquidType(tile, (byte)type);

        static void Main_DoUpdate(On.Terraria.Main.orig_DoUpdate orig, Main self, ref GameTime gameTime)
        {
            orig(self, ref gameTime);
            LiquidLoader.OnUpdate();
        }

        static void Liquid_Update(On.Terraria.Liquid.orig_Update orig, Liquid self)
        {
            if (LiquidLoader.OnUpdate(self))
                orig(self);
        }

        static void Liquid_HoneyCheck(On.Terraria.Liquid.orig_HoneyCheck orig, int x, int y)
        {
        }

        static void Liquid_LavaCheck(On.Terraria.Liquid.orig_LavaCheck orig, int x, int y)
        {
        }

        static void NetMessage_CompressTileBlock_Inner(On.Terraria.NetMessage.orig_CompressTileBlock_Inner orig, BinaryWriter writer, int xStart, int yStart, int width, int height)
        {
            orig(writer, xStart, yStart, width, height);

            var byteArray = new byte[width * height];
            int index = 0;

            for (int i = xStart; i < xStart + width; i++)
                for (int j = yStart; j < yStart + height; j++)
                    byteArray[index++] = (byte)Main.tile[i, j].LiquidType;
            
            writer.Write(byteArray);
        }

        static void NetMessage_DecompressTileBlock_Inner(On.Terraria.NetMessage.orig_DecompressTileBlock_Inner orig, BinaryReader reader, int xStart, int yStart, int width, int height)
        {
            orig(reader, xStart, yStart, width, height);

            var byteArray = reader.ReadBytes(width * height);
            int index = 0;

            for (int i = xStart; i < xStart + width; i++)
                for (int j = yStart; j < yStart + height; j++)
                    Main.tile[i, j].LiquidType = byteArray[index++];
        }

        static void OnHooks_OnInitialize(orig_OnInitialize orig, object self)
        {
            orig(self);
            var ass = typeof(Mod).Assembly;
            var type_UIModItem = ass.GetType("Terraria.ModLoader.UI.UIModItem");
            string ModName = (string)type_UIModItem.GetProperty("ModName", BindingFlags.Public | BindingFlags.Instance).GetValue(self);

            if (ModLoader.TryGetMod(ModName, out var loadedMod))
            {
                int liquidCount = loadedMod.GetContent<ModLiquid>().Count();

                if (liquidCount > 0)
                {
                    int baseOffset = -40;
                    void ChangeOffset(int modCount)
                    {
                        if (modCount > 0)
                            baseOffset -= 18;
                    }
                    ChangeOffset(loadedMod.GetContent<ModItem>().Count());
                    ChangeOffset(loadedMod.GetContent<ModNPC>().Count());
                    ChangeOffset(loadedMod.GetContent<ModTile>().Count());
                    ChangeOffset(loadedMod.GetContent<ModWall>().Count());
                    ChangeOffset(loadedMod.GetContent<ModBuff>().Count());
                    ChangeOffset(loadedMod.GetContent<ModMount>().Count());

                    var type_UIHoverImage = ass.GetType("Terraria.ModLoader.UI.UIHoverImage");
                    var UIHoverImage = Activator.CreateInstance(type_UIHoverImage, Main.Assets.Request<Texture2D>(TextureAssets.InfoIcon[6].Name), liquidCount + " liquids");
                    var field_Left = type_UIHoverImage.GetField("Left", BindingFlags.Public | BindingFlags.Instance);
                    field_Left.SetValue(UIHoverImage, new StyleDimension { Percent = 1f, Pixels = baseOffset });
                    type_UIModItem.GetMethod("Append", BindingFlags.Public | BindingFlags.Instance).Invoke(self, new object[] { UIHoverImage });
                }
            }
        }

        static void Wiring_XferWater(On.Terraria.Wiring.orig_XferWater orig)
        {
            for (int i = 0; i < Wiring._numInPump; i++)
            {
                int inPumpX = Wiring._inPumpX[i];
                int inPumpY = Wiring._inPumpY[i];
                var inPumpTile = Main.tile[inPumpX, inPumpY];

                if (inPumpTile.LiquidAmount > 0)
                {

                    for (int j = 0; j < Wiring._numOutPump; j++)
                    {
                        int outPumpX = Wiring._outPumpX[j];
                        int outPumpY = Wiring._outPumpY[j];
                        var outPumpTile = Main.tile[outPumpX, outPumpY];

                        if (outPumpTile.LiquidAmount < 255 && (outPumpTile.LiquidType == inPumpTile.LiquidType || outPumpTile.LiquidAmount == 0))
                        {
                            if (outPumpTile.LiquidAmount == 0)
                                outPumpTile.LiquidType = inPumpTile.LiquidType;

                            int toTransfer = inPumpTile.LiquidAmount;
                            if (toTransfer + outPumpTile.LiquidAmount > 255)
                                toTransfer = 255 - outPumpTile.LiquidAmount;

                            inPumpTile.LiquidAmount -= (byte)toTransfer;
                            outPumpTile.LiquidAmount += (byte)toTransfer;

                            WorldGen.SquareTileFrame(outPumpX, outPumpY, true);
                            if (inPumpTile.LiquidAmount == 0)
                            {
                                WorldGen.SquareTileFrame(inPumpX, inPumpY, true);
                                break;
                            }
                        }
                    }
                }
            }
        }

        //            HL
        //           7654_3210 - Pos
        // 1000_0000_0000_0000 - sTileHeader
        //           0110_0000 - bTileHeader
        //           1110_0000 - bTileHeader3
        //           0011_1111 - Liquid Byte (64)
        static byte GetLiquidType(Tile tile)
        {
            byte result = 0;
            result.SetBit(0, tile.bTileHeader.IsBit(5));
            result.SetBit(1, tile.bTileHeader.IsBit(6));
            result.SetBit(2, tile.sTileHeader.IsBit(15));
            result.SetBit(3, tile.bTileHeader3.IsBit(5));
            result.SetBit(4, tile.bTileHeader3.IsBit(6));
            result.SetBit(5, tile.bTileHeader3.IsBit(7));
            return result;
        }
        static void SetLiquidType(Tile tile, byte v)
        {
            tile.bTileHeader.SetBit(5, v.IsBit(0));
            tile.bTileHeader.SetBit(6, v.IsBit(1));
            tile.sTileHeader.SetBit(15, v.IsBit(2));
            tile.bTileHeader3.SetBit(5, v.IsBit(3));
            tile.bTileHeader3.SetBit(6, v.IsBit(4));
            tile.bTileHeader3.SetBit(7, v.IsBit(5));
        }

        static void SetBit(this ref byte b, int pos, bool v)
        {
            if (v)
                b = (byte)(b | 1 << pos);
            else
                b = (byte)(b & ~(1 << pos));
        }
        static bool IsBit(this byte b, int pos) =>
            (b & 1 << pos) != 0;

        static void SetBit(this ref ushort b, int pos, bool v)
        {
            if (v)
                b = (ushort)(b | 1 << pos);
            else
                b = (ushort)(b & ~(1 << pos));
        }
        static bool IsBit(this ushort b, int pos) =>
            (b & 1 << pos) != 0;
    }
}
