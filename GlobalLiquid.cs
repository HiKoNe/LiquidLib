using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace LiquidLib
{
    public abstract class GlobalLiquid : ModType
    {
        public virtual void WaterfallLength(int type, ref int waterfallLength)
        {
        }

        public virtual void Opacity(int type, ref float opacity)
        {
        }

        public virtual void WaveMaskStrength(int type, ref byte waveMaskStrength)
        {
        }

        public virtual void ViscosityMask(int type, ref byte viscosityMask)
        {
        }

        public virtual void LiquidTexture(int type, int waterStyle, ref Asset<Texture2D> texture)
        {
        }

        public virtual void FlowTexture(int type, ref Asset<Texture2D> texture)
        {
        }

        public virtual void SlopeTexture(int type, ref Asset<Texture2D> texture)
        {
        }

        public virtual void WaterfallTexture(int type, ref Asset<Texture2D> texture)
        {
        }

        public virtual void OnCollision(int type, int i, int j, Entity entity)
        {
        }

        public virtual bool OnInLiquid(int type, Entity entity)
        {
            return true;
        }

        public virtual bool OnOutLiquid(int type, Entity entity)
        {
            return true;
        }

        public virtual void ParticlesAndSound(int type, Entity entity, ref int dustCount, ref int dustType, ref LegacySoundStyle sound)
        {
        }

        public virtual bool OnUpdate(Liquid liquid)
        {
            return true;
        }

        public virtual void Delay(int type, ref int delay)
        {
        }

        public virtual void Drown(int type, ref bool drown)
        {
        }

        public virtual bool OnBucket(int type, Item bucket)
        {
            return true;
        }

        public virtual void OnTilePlaceByLiquid(int i, int j, int liquidType, int liquidType2)
        {
        }

        public virtual void OnCatchFish(int type, Projectile projectile, ref FishingAttempt fisher)
        {
        }

        protected sealed override void Register()
        {
            LiquidLoader.globalLiquids.Add(this);
        }
        public sealed override void SetupContent() => this.SetStaticDefaults();

        public override void SetStaticDefaults() => base.SetStaticDefaults();
    }
}
