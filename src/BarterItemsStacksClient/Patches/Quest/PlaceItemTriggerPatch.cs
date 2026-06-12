using Comfort.Common;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using EFT.InventoryLogic;

namespace BarterItemsStacksClient.Patches.Quest
{
    internal class PlaceItemTriggerPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GetActionsClass.Class1772), nameof(GetActionsClass.Class1772.method_0));
        }

        [PatchPrefix]
        public static bool Prefix(GetActionsClass.Class1772 __instance, bool successful)
        {
            TraderControllerClass inventoryController = __instance.owner.Player.InventoryController;
            Item itemToConsume = __instance.resultItem;
            if (inventoryController.TryFindItem(__instance.resultItem.Id, out Item currentItem))
            {
                itemToConsume = currentItem;
            }

            if (itemToConsume.StackObjectsCount <= 1)
            {
                return true;
            }

            __instance.owner.Player.vmethod_6(__instance.resultItem.TemplateId, __instance.itemTrigger.Id, successful);
            __instance.owner.CloseObjectivesPanel();

            if (!successful)
            {
                return false;
            }

            InventoryController playerInventoryController = inventoryController as InventoryController;
            if (playerInventoryController == null)
            {
                return false;
            }

            ItemAddress location = playerInventoryController.Inventory.Equipment
                .GetPrioritizedGridsForLoot(itemToConsume)
                .FindLocationForItem(itemToConsume);
            if (location == null)
            {
                return false;
            }

            var splitOne = InteractionsHandlerClass.SplitExact(itemToConsume, 1, location, playerInventoryController, playerInventoryController, true);
            if (splitOne.Failed)
            {
                return false;
            }

            inventoryController.TryRunNetworkTransaction(splitOne, splitResult =>
            {
                if (!splitResult.Succeed)
                {
                    return;
                }

                Item splitItem = splitOne.Value.ResultItem;
                if (inventoryController.TryFindItem(splitItem.Id, out Item currentSplitItem))
                {
                    splitItem = currentSplitItem;
                }

                GStruct153 remove = InteractionsHandlerClass.Remove(splitItem, inventoryController, true);
                Callback callback;
                if ((callback = __instance.callback_0) == null)
                {
                    callback = (__instance.callback_0 = new Callback(__instance.method_1));
                }
                inventoryController.TryRunNetworkTransaction(remove, callback);
            });

            return false;
        }
    }
}
