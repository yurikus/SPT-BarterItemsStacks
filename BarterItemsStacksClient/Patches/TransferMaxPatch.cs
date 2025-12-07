using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace BarterItemsStacksClient.Patches
{
    internal class TransferMaxPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(InteractionsHandlerClass), nameof(InteractionsHandlerClass.TransferMax));
        }

        [PatchPrefix]
        public static bool Prefix(InteractionsHandlerClass __instance, Item item, Item targetItem, int count, TraderControllerClass itemController, bool simulate, ref GStruct154<GClass3425> __result)
        {
            if (item.SpawnedInSession != targetItem.SpawnedInSession)
            { 
                __result = new GClass1522("Cannot transfer FIR and non-FIR items");
                return false;
            }

            var itemResource = item.GetItemComponent<ResourceComponent>();
            var targetResource = targetItem.GetItemComponent<ResourceComponent>();

            if (itemResource != null && targetResource != null && itemResource.Value != targetResource.Value)
            {
                __result = new GClass1522("Cannot transfer items with different resource values");
                return false;
            }

            return true;
        }
    }
}
