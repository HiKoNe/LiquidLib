using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Liquid;
using Terraria.ID;

namespace LiquidLib
{
    public static class LiquidLoader
    {
        internal static readonly List<(Entity entity, int liquidType)> wetEntities = new();
        internal static readonly List<Entity> lastWetEntities= new();
        internal static readonly Dictionary<int, ModLiquid> liquids = new();
        internal static readonly List<GlobalLiquid> globalLiquids = new();

        internal static readonly List<LiquidCollision> liquidCollisions = new()
        {
            new LiquidCollision(0, 1).SetTileType(TileID.Obsidian).SetSound(SoundID.LiquidsWaterLava),
            new LiquidCollision(0, 2).SetTileType(TileID.HoneyBlock).SetSound(SoundID.LiquidsHoneyWater),
            new LiquidCollision(1, 2).SetTileType(TileID.CrispyHoneyBlock).SetSound(SoundID.LiquidsHoneyLava),
        };

        public static LiquidCollision CollisionGet(int lqiuid, int lqiuid2)
        {
            LiquidCollision lc = null;
            if (liquidCollisions.Any(l =>
            {
                bool @is = l.Is(lqiuid, lqiuid2, out var liquidCollision);
                lc = liquidCollision;
                return @is;
            }))
                return lc;

            lc = new LiquidCollision(lqiuid, lqiuid2);
            liquidCollisions.Add(lc);
            return lc;
        }

        internal static bool CollisionContains(int lqiuid1, int lqiuid2)
        {
            return liquidCollisions.Any(l => l.Is(lqiuid1, lqiuid2, out _));
        }

        internal static void OnUpdate()
        {
            for (int i = 0; i < wetEntities.Count; i++)
            {
                if (!lastWetEntities.Contains(wetEntities[i].entity))
                {
                    var (entity, liquidType) = wetEntities[i];
                    OnOutLiquid(liquidType, entity);
                    wetEntities.RemoveAt(i);
                }
            }
            lastWetEntities.Clear();
        }

        /// <summary> Global liquid count. </summary>
        public static int LiquidCount => liquids.Count + 3;
        
        /// <summary> Gets the ModLiquid instance with the given type. Returns null if no ModLiquid with the given type exists. </summary>
        public static ModLiquid GetLiquid(int type) => liquids.TryGetValue(type, out var modLiquid) ? modLiquid : null;

        /// <summary> Gets the ModLiquid instance with the given name. Returns null if no ModLiquid with the given type exists. </summary>
        public static ModLiquid GetLiquid(string name)
        {
            foreach (var modLiquid in liquids.Values)
                if (modLiquid.Name == name)
                    return modLiquid;
            return null;
        }

        public static void GetWaterfallLength(int type, ref int waterfallLength)
        {
            if (liquids.TryGetValue(type, out var modLiquid))
                waterfallLength = modLiquid.WaterfallLength;
            else if (type == 0)
                waterfallLength = 10;
            else if (type == 1)
                waterfallLength = 3;
            else if (type == 2)
                waterfallLength = 2;
            else
                waterfallLength = 5;

            foreach (var globalLiquid in globalLiquids)
                globalLiquid.WaterfallLength(type, ref waterfallLength);
        }

        public static float GetOpacity(int type)
        {
            float opacity;
            if (liquids.TryGetValue(type, out var modLiquid))
                opacity = modLiquid.Opacity;
            else
                opacity = ((float[])typeof(LiquidRenderer)
                    .GetField("DEFAULT_OPACITY", BindingFlags.NonPublic | BindingFlags.Static)
                    .GetValue(null))[type];

            foreach (var globalLiquid in globalLiquids)
                globalLiquid.Opacity(type, ref opacity);

            return opacity;
        }

        public static byte GetWaveMaskStrength(int type)
        {
            byte waveMaskStrength = 0;

            if (liquids.TryGetValue(type, out var modLiquid))
                waveMaskStrength = modLiquid.WaveMaskStrength;

            foreach (var globalLiquid in globalLiquids)
                globalLiquid.WaveMaskStrength(type, ref waveMaskStrength);

            return waveMaskStrength;
        }

        public static byte GetViscosityMask(int type)
        {
            byte viscosityMask = 0;

            if (liquids.TryGetValue(type, out var modLiquid))
                viscosityMask = modLiquid.ViscosityMask;
            else if (type == 0)
                viscosityMask = 160;
            else if (type == 1)
                viscosityMask = 200;
            else if (type == 2)
                viscosityMask = 240;

            foreach (var globalLiquid in globalLiquids)
                globalLiquid.ViscosityMask(type, ref viscosityMask);

            return viscosityMask;
        }

        public static Asset<Texture2D> GetLiquidTexture(int type, bool isWaterStyle)
        {
            Asset<Texture2D> texture;
            if (!isWaterStyle && liquids.TryGetValue(type, out var modLiquid))
                texture = modLiquid.Textures[0];
            else
                texture = LiquidRenderer.Instance._liquidTextures[type];

            foreach (var globalLiquid in globalLiquids)
                globalLiquid.LiquidTexture(type, isWaterStyle, ref texture);

            return texture;
        }

        public static Asset<Texture2D> GetFlowTexture(int type)
        {
            Asset<Texture2D> texture;

            if (liquids.TryGetValue(type, out var modLiquid))
                texture = modLiquid.Textures[2];
            else
                texture = TextureAssets.Liquid[type == 2 ? 11 : type];

            foreach (var globalLiquid in globalLiquids)
                globalLiquid.FlowTexture(type, ref texture);

            return texture;
        }

        public static Asset<Texture2D> GetSlopeTexture(int type)
        {
            Asset<Texture2D> texture;

            if (liquids.TryGetValue(type, out var modLiquid))
                texture = modLiquid.Textures[1];
            else
                texture = TextureAssets.LiquidSlope[type == 2 ? 11 : type];

            foreach (var globalLiquid in globalLiquids)
                globalLiquid.SlopeTexture(type, ref texture);

            return texture;
        }

        public static Asset<Texture2D> GetWaterfallTexture(int type)
        {
            Asset<Texture2D> texture;

            if (liquids.TryGetValue(type, out var modLiquid))
                texture = modLiquid.Textures[3];
            else
                texture = ((Asset<Texture2D>[])typeof(WaterfallManager)
                    .GetField("waterfallTexture", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(Main.instance.waterfallManager))[type == 2 ? 14 : type];

            foreach (var globalLiquid in globalLiquids)
                globalLiquid.WaterfallTexture(type, ref texture);

            return texture;
        }

        public static void OnLiquidCollision(int i, int j, Entity entity)
        {
            var type = Main.tile[i, j].LiquidType;

            if (liquids.TryGetValue(type, out var modLiquid))
                modLiquid.OnCollision(i, j, entity);

            foreach (var globalLiquid in globalLiquids)
                globalLiquid.OnCollision(type, i, j, entity);
        }

        public static void OnInLiquid(int type, Entity entity)
        {
            bool flag = false;
            int dustCount = -1;
            int dustType = -1;
            LegacySoundStyle sound = null;

            if (type == 0)
            {
                flag = true;
                dustCount = 50;
                dustType = Dust.dustWater();
                sound = new LegacySoundStyle(19, 0);
            }
            else if (type == 1)
            {
                flag = true;
                dustCount = 20;
                dustType = 35;
                sound = new LegacySoundStyle(19, 1);
            }
            else if (type == 2)
            {
                flag = true;
                dustCount = 20;
                dustType = 152;
                sound = new LegacySoundStyle(19, 1);
            }
            else if (liquids.TryGetValue(type, out var modLiquid))
                if (modLiquid.OnInLiquid(entity))
                {
                    flag = true;
                    dustCount = modLiquid.DustCount;
                    dustType = modLiquid.DustType;
                    sound = modLiquid.Sound;
                }

            foreach (var globalLiquid in globalLiquids)
                if (globalLiquid.OnInLiquid(type, entity))
                {
                    flag = true;
                    globalLiquid.ParticlesAndSound(type, entity, ref dustCount, ref dustType, ref sound);
                }

            if (flag)
                ParticlesAndSound(dustCount, dustType, sound, entity);
        }

        public static void OnOutLiquid(int type, Entity entity)
        {
            bool flag = false;
            int dustCount = -1;
            int dustType = -1;
            LegacySoundStyle sound = null;

            if (type == 0)
            {
                flag = true;
                dustCount = 50;
                dustType = Dust.dustWater();
                sound = new LegacySoundStyle(19, 0);
            }
            else if (type == 1)
            {
                flag = true;
                dustCount = 20;
                dustType = 35;
                sound = new LegacySoundStyle(19, 1);
            }
            else if (type == 2)
            {
                flag = true;
                dustCount = 20;
                dustType = 152;
                sound = new LegacySoundStyle(19, 1);
            }
            else if (liquids.TryGetValue(type, out var modLiquid))
                if (modLiquid.OnOutLiquid(entity))
                {
                    flag = true;
                    dustCount = modLiquid.DustCount;
                    dustType = modLiquid.DustType;
                    sound = modLiquid.Sound;
                }

            foreach (var globalLiquid in globalLiquids)
                if (globalLiquid.OnOutLiquid(type, entity))
                {
                    flag = true;
                    globalLiquid.ParticlesAndSound(type, entity, ref dustCount, ref dustType, ref sound);
                }

            if (flag)
                ParticlesAndSound(dustCount, dustType, sound, entity);
        }
        static void ParticlesAndSound(int dustCount, int dustType, LegacySoundStyle sound, Entity entity)
        {
            if (dustType >= 0)
                for (int i = 0; i < dustCount; i++)
                {
                    int dust = Dust.NewDust(new Vector2(entity.position.X - 6f, entity.position.Y + entity.height / 2f), entity.width + 12, 24, dustType, 0f, 0f, 0, default, 1f);
                    Main.dust[dust].velocity.Y -= 2f;
                    Main.dust[dust].velocity.X *= 2.5f;
                    Main.dust[dust].alpha = 100;
                    Main.dust[dust].noGravity = true;
                }
            if (sound != null)
                SoundEngine.PlaySound(sound, entity.position);
        }

        public static bool OnUpdate(Liquid liquid)
        {
            bool flag = true;

            if (liquids.TryGetValue(Main.tile[liquid.x, liquid.y].LiquidType, out var modLiquid))
                flag = modLiquid.OnUpdate(liquid);

            foreach (var globalLiquid in globalLiquids)
                flag = globalLiquid.OnUpdate(liquid);

            return flag;
        }

        public static int GetDelay(int type)
        {
            int delay = 0;

            if (liquids.TryGetValue(type, out var modLiquid))
                delay = modLiquid.Delay;
            else if (type == 1)
                delay = 5;
            else if (type == 2)
                delay = 10;

            foreach (var globalLiquid in globalLiquids)
                globalLiquid.Delay(type, ref delay);

            return delay;
        }

        public static bool OnBucket(int type, Item item)
        {
            bool flag = true;

            if (liquids.TryGetValue(type, out var modLiquid))
                flag = modLiquid.OnBucket(item);

            foreach (var globalLiquid in globalLiquids)
                flag = globalLiquid.OnBucket(type, item);

            return flag;
        }

        public static void OnTilePlaceByLiquid(int type, int type2)
        {
            if (liquids.TryGetValue(type, out var modLiquid))
                modLiquid.OnTilePlaceByLiquid(type2);
            if (liquids.TryGetValue(type2, out modLiquid))
                modLiquid.OnTilePlaceByLiquid(type);

            foreach (var globalLiquid in globalLiquids)
                globalLiquid.OnTilePlaceByLiquid(type, type2);
        }

        internal static void AddLiquid(ModLiquid modLiquid)
        {
            liquids.Add(modLiquid.Type, modLiquid);
        }

        internal static void Unload()
        {
            liquids.Clear();
            globalLiquids.Clear();
            liquidCollisions.Clear();
        }
    }
}
