using System.Collections.Generic;
using Terraria.ModLoader;

namespace LiquidLib
{
	internal class LiquidLib : Mod
	{
        public static LiquidLib Instance { get; private set; }

        public static List<LiquidBucket> BucketToLoad = new();

        public override void Load()
        {
            Instance = this;
            OnHooks.Load();
            ILHooks.Load();

            foreach (var bucket in BucketToLoad)
                AddContent(bucket);
            BucketToLoad.Clear();
        }

        public override void Unload()
        {
            Instance = null;
            OnHooks.Unload();
            ILHooks.Unload();
            LiquidLoader.Unload();
        }
    }
}