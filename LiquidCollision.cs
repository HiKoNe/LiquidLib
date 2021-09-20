using Terraria.Audio;

namespace LiquidLib
{
    public class LiquidCollision
    {
        internal int lqiuid1;
        internal int lqiuid2;
        internal int tileType;
        internal LegacySoundStyle sound;

        /// <summary> Set Tile id for collision liquids. </summary>
        public LiquidCollision SetTileType(int tileType)
        {
            this.tileType = tileType;
            return this;
        }

        /// <summary> Set Sound for collision liquids. </summary>
        public LiquidCollision SetSound(LegacySoundStyle sound)
        {
            this.sound = sound;
            return this;
        }

        internal LiquidCollision(int lqiuid1, int lqiuid2)
        {
            this.lqiuid1 = lqiuid1;
            this.lqiuid2 = lqiuid2;
        }

        internal bool Is(int lqiuid1, int lqiuid2, out LiquidCollision liquidCollision)
        {
            liquidCollision = this;
            return (this.lqiuid1 == lqiuid1 && this.lqiuid2 == lqiuid2) || (this.lqiuid1 == lqiuid2 && this.lqiuid2 == lqiuid1);
        }
    }
}
