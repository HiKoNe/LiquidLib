using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace LiquidLib
{
    internal class LiquidCollisions
    {
        List<LiquidCollision> liquidCollisions = new()
        {
            new LiquidCollision(0, 1) { TileType = TileID.Obsidian, Sound = SoundID.LiquidsWaterLava },
            new LiquidCollision(0, 2) { TileType = TileID.HoneyBlock, Sound = SoundID.LiquidsHoneyWater },
            new LiquidCollision(1, 2) { TileType = TileID.CrispyHoneyBlock, Sound = SoundID.LiquidsHoneyLava },
        };

        public LiquidCollision this[int lqiuid1, int lqiuid2]
        {
            get
            {
                LiquidCollision lc = null;
                if (liquidCollisions.Any(l =>
                {
                    bool @is = l.Is(lqiuid1, lqiuid2, out var liquidCollision);
                    lc = liquidCollision;
                    return @is;
                }))
                    return lc;

                lc = new LiquidCollision(lqiuid1, lqiuid2);
                liquidCollisions.Add(lc);
                return lc;
            }
        }

        public bool Contains(int lqiuid1, int lqiuid2)
        {
            return liquidCollisions.Any(l =>
            {
                return l.Is(lqiuid1, lqiuid2, out _);
            });
        }

        public void Unload() => liquidCollisions.Clear();
    }

    public class LiquidCollision
    {
        int lqiuid1;
        int lqiuid2;
        int tileType;
        LegacySoundStyle sound;

        public int TileType { get => this.tileType; set => this.tileType = value; }
        public LegacySoundStyle Sound { get => this.sound; set => this.sound = value; }

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
