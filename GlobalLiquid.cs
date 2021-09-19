using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace LiquidLib
{
    public abstract class GlobalLiquid : ModType
    {
        /// <summary>  </summary>
        public LiquidCollision LiquidCollision(int liquidType, int liquidType2) =>
            LiquidLoader.liquidCollisions[liquidType, liquidType2];

        /// <summary>  </summary>
        public virtual void WaterfallLength(int type, ref int waterfallLength)
        {
        }

        /// <summary>  </summary>
        public virtual void Opacity(int type, ref float opacity)
        {
        }

        /// <summary>  </summary>
        public virtual void LiquidTexture(int type, bool isWaterStyle, ref Asset<Texture2D> texture)
        {
        }

        /// <summary>  </summary>
        public virtual void FlowTexture(int type, ref Asset<Texture2D> texture)
        {
        }

        /// <summary>  </summary>
        public virtual void SlopeTexture(int type, ref Asset<Texture2D> texture)
        {
        }

        /// <summary>  </summary>
        public virtual void WaterfallTexture(int type, ref Asset<Texture2D> texture)
        {
        }

        /// <summary>  </summary>
        public virtual void OnCollision(int type, int i, int j, Entity entity)
        {
        }

        /// <summary>  </summary>
        public virtual bool OnInLiquid(int type, Entity entity)
        {
            return true;
        }

        /// <summary>  </summary>
        public virtual bool OnOutLiquid(int type, Entity entity)
        {
            return true;
        }

        /// <summary>  </summary>
        public virtual void ParticlesAndSound(int type, Entity entity, ref int dustCount, ref int dustType, ref int soundType, ref int soundStyle)
        {
        }

        /// <summary>  </summary>
        public virtual bool OnUpdate(Liquid liquid)
        {
            return true;
        }

        /// <summary>  </summary>
        public virtual void Delay(int type, ref int delay)
        {
        }

        /// <summary>  </summary>
        public virtual bool OnBucket(int type, Item bucket)
        {
            return true;
        }

        /// <summary>  </summary>
        public virtual void OnTilePlaceByLiquid(int liquidType, int liquidType2)
        {

        }

        protected sealed override void Register()
        {
            LiquidLoader.globalLiquids.Add(this);
        }
        public sealed override void SetupContent() => this.SetStaticDefaults();

        /// <summary>  </summary>
        public override void SetStaticDefaults() => base.SetStaticDefaults();
    }
}
