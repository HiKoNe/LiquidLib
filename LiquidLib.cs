using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LiquidLib
{
	internal class LiquidLib : Mod
	{
        public static LiquidLib Instance { get; private set; }
        public static int bucketsRecipeGroupID;

        public override void Load()
        {
            Instance = this;
            OnHooks.Load();
            ILHooks.Load();
        }

        public override void Unload()
        {
            Instance = null;
            OnHooks.Unload();
            ILHooks.Unload();
            LiquidLoader.Unload();
        }

        public override void AddRecipeGroups() =>
            bucketsRecipeGroupID = RecipeGroup.RegisterGroup("LiquidLib:Buckets",
                new RecipeGroup(() => "Any Buckets", ItemID.WaterBucket, ItemID.LavaBucket, ItemID.HoneyBucket) { IconicItemId = ItemID.WaterBucket });
    }
}