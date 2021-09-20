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
        /// <summary> The ID of this type of liquid. </summary>
        public int Type { get; private set; }

        /// <summary> The Bucket Item ID of this liquid. </summary>
        public int BucketType { get; internal set; }

        /// <summary> 4 Textures of: <br/> 
        /// 0: LiquidTexture <br/> 
        /// 1: SlopeTexture <br/> 
        /// 2: FlowTexture <br/> 
        /// 3: WaterfallTexture
        /// </summary>
        public Asset<Texture2D>[] Textures { get; internal set; }

        /// <summary> The texture that this type of bucket liquid uses. </summary>
        public abstract string BucketTexture { get; }

        /// <summary> The sprite sheet that this type of liquid uses. </summary>
        public abstract string LiquidTexture { get; }

        /// <summary> The sprite sheet that this type of liquid uses. </summary>
        public abstract string SlopeTexture { get; }

        /// <summary> The sprite sheet that this type of flow liquid uses. </summary>
        public abstract string FlowTexture { get; }

        /// <summary> The sprite sheet that this type of waterfall liquid uses. </summary>
        public abstract string WaterfallTexture { get; }

        /// <summary> [Default: 5] The waterfall length of this liquid. Value: 0 - 10. </summary>
        public int WaterfallLength { get; set; } = 5;

        /// <summary> [Default: 0.0f] The opacity of this liquid. Value: 0f - 1f. </summary>
        public float Opacity { get; set; } = 0.0f;

        /// <summary> [Default: 0] The wave mask strength of this liquid. </summary>
        public byte WaveMaskStrength { get; set; } = 0;

        /// <summary> [Default: 0] The viscosity mask of this liquid. </summary>
        public byte ViscosityMask { get; set; } = 0;

        /// <summary> [Default: -1] <br/> The dust count of this liquid. </summary>
        public int DustCount { get; set; } = -1;

        /// <summary> [Default: -1] <br/> The dust type of this liquid. </summary>
        public int DustType { get; set; } = -1;

        /// <summary> [Default: null] <br/> The sound of this liquid. </summary>
        public LegacySoundStyle Sound { get; set; } = null;

        /// <summary> [Default: 0] <br/> The delay of this liquid. </summary>
        public int Delay { get; set; } = 0;

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

        /// <summary>  </summary>
        public override void SetStaticDefaults() => base.SetStaticDefaults();

        /// <summary>  </summary>
        public virtual void OnCollision(int i, int j, Entity entity)
        {
        }

        /// <summary>  </summary>
        public virtual bool OnInLiquid(Entity entity)
        {
            return true;
        }

        /// <summary>  </summary>
        public virtual bool OnOutLiquid(Entity entity)
        {
            return true;
        }

        /// <summary>  </summary>
        public virtual bool OnUpdate(Liquid liquid)
        {
            return true;
        }

        /// <summary>  </summary>
        public virtual bool OnBucket(Item bucket)
        {
            return true;
        }

        /// <summary>  </summary>
        public virtual void OnTilePlaceByLiquid(int liquidType)
        {

        }
    }
}
