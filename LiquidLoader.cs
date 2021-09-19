using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Liquid;

namespace LiquidLib
{
    public static class LiquidLoader
    {
        internal static readonly List<(Entity entity, int liquidType)> wetEntities = new();
        internal static readonly List<Entity> lastWetEntities= new();
        internal static readonly Dictionary<int, ModLiquid> liquids = new();
        internal static readonly List<GlobalLiquid> globalLiquids = new();
        internal static LiquidCollisions liquidCollisions = new LiquidCollisions();

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

        public static void GetWaterfallLength(int type, ref int waterfallLength)
        {
            if (liquids.TryGetValue(type, out var modLiquid))
                waterfallLength = modLiquid.WaterfallLength;
            else
                waterfallLength = ((int[])typeof(LiquidRenderer)
                    .GetField("WATERFALL_LENGTH", BindingFlags.NonPublic | BindingFlags.Static)
                    .GetValue(null))[type];

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
            int soundType = -1;
            int soundStyle = -1;

            if (type == 0)
            {
                flag = true;
                dustCount = 50;
                dustType = Dust.dustWater();
                soundType = 19;
                soundStyle = 0;
            }
            else if (type == 1)
            {
                flag = true;
                dustCount = 20;
                dustType = 35;
                soundType = 19;
                soundStyle = 1;
            }
            else if (type == 2)
            {
                flag = true;
                dustCount = 20;
                dustType = 152;
                soundType = 19;
                soundStyle = 1;
            }
            else if (liquids.TryGetValue(type, out var modLiquid))
                if (modLiquid.OnInLiquid(entity))
                {
                    flag = true;
                    dustCount = modLiquid.DustCount;
                    dustType = modLiquid.DustType;
                    soundType = modLiquid.SoundType;
                    soundStyle = modLiquid.SoundStyle;
                }

            foreach (var globalLiquid in globalLiquids)
                if (globalLiquid.OnInLiquid(type, entity))
                {
                    flag = true;
                    globalLiquid.ParticlesAndSound(type, entity, ref dustCount, ref dustType, ref soundType, ref soundStyle);
                }

            if (flag)
                ParticlesAndSound(dustCount, dustType, soundType, soundStyle, entity);
        }

        public static void OnOutLiquid(int type, Entity entity)
        {
            bool flag = false;
            int dustCount = -1;
            int dustType = -1;
            int soundType = -1;
            int soundStyle = -1;

            if (type == 0)
            {
                flag = true;
                dustCount = 50;
                dustType = Dust.dustWater();
                soundType = 19;
                soundStyle = 0;
            }
            else if (type == 1)
            {
                flag = true;
                dustCount = 20;
                dustType = 35;
                soundType = 19;
                soundStyle = 1;
            }
            else if (type == 2)
            {
                flag = true;
                dustCount = 20;
                dustType = 152;
                soundType = 19;
                soundStyle = 1;
            }
            else if (liquids.TryGetValue(type, out var modLiquid))
                if (modLiquid.OnOutLiquid(entity))
                {
                    flag = true;
                    dustCount = modLiquid.DustCount;
                    dustType = modLiquid.DustType;
                    soundType = modLiquid.SoundType;
                    soundStyle = modLiquid.SoundStyle;
                }

            foreach (var globalLiquid in globalLiquids)
                if (globalLiquid.OnOutLiquid(type, entity))
                {
                    flag = true;
                    globalLiquid.ParticlesAndSound(type, entity, ref dustCount, ref dustType, ref soundType, ref soundStyle);
                }

            if (flag)
                ParticlesAndSound(dustCount, dustType, soundType, soundStyle, entity);
        }
        static void ParticlesAndSound(int dustCount, int dustType, int soundType, int soundStyle, Entity entity)
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
            if (soundType >= 0 && soundStyle >= 0)
                SoundEngine.PlaySound(soundType, (int)entity.position.X, (int)entity.position.Y, soundStyle, 1f, 0f);
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
            liquidCollisions.Unload();
        }
    }
}
