using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace LiquidLib
{
    public abstract class ModLiquid : ModType
    {
        public int Type { get; private set; }

        public int BucketType { get; internal set; }

        public Asset<Texture2D>[] Textures { get; internal set; }

        public abstract string BucketTexture { get; }

        public abstract string LiquidTexture { get; }

        public abstract string SlopeTexture { get; }

        public abstract string FlowTexture { get; }

        public abstract string WaterfallTexture { get; }

        public int WaterfallLength { get; set; } = 5;

        public float Opacity { get; set; } = 0.0f;

        public byte WaveMaskStrength { get; set; } = 0;

        public byte ViscosityMask { get; set; } = 0;

        public int DustCount { get; set; } = -1;

        public int DustType { get; set; } = -1;

        public LegacySoundStyle Sound { get; set; } = null;

        public int Delay { get; set; } = 0;

        public bool Drown { get; set; } = true;

        public sealed override void SetupContent()
        {
            Textures = new Asset<Texture2D>[4];
            Textures[0] = ModContent.Request<Texture2D>(LiquidTexture);
            Textures[1] = ModContent.Request<Texture2D>(SlopeTexture);
            Textures[2] = ModContent.Request<Texture2D>(FlowTexture);
            Textures[3] = ModContent.Request<Texture2D>(WaterfallTexture);
            this.SetStaticDefaults();
        }

        protected sealed override void Register()
        {
            Type = LiquidLoader.LiquidCount;
            if (Type > 63)
                throw new Exception("Fluids Limit Reached. (Max: 64)");
            LiquidLoader.AddLiquid(this);
            this.Mod.AddContent(new LiquidBucket(this));
            LiquidLib.Instance.Logger.Info("Register new Liquid: " + this.Name + ", By: " + this.Mod.Name + ", Type: " + Type);
        }

        public override void SetStaticDefaults() => base.SetStaticDefaults();

        public virtual void OnCollision(int i, int j, Entity entity)
        {
        }

        public virtual bool OnInLiquid(Entity entity)
        {
            return true;
        }

        public virtual bool OnOutLiquid(Entity entity)
        {
            return true;
        }

        public virtual bool OnUpdate(Liquid liquid)
        {
            return true;
        }

        public virtual bool OnBucket(Item bucket)
        {
            return true;
        }

        public virtual void OnTilePlaceByLiquid(int i, int j, int liquidType)
        {
        }
    }
}
