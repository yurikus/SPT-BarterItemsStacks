using System;
using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace BarterItemsStacksClient.Patches.Quest
{
    public class PlaceItemProtectPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Player.PlayerInventoryController), nameof(Player.PlayerInventoryController.SetupItem));
        }

        [PatchPrefix]
        public static bool Prefix(Player.PlayerInventoryController __instance,
            Item item,
            string zone,
            Vector3 position,
            Quaternion rotation,
            float setupTime,
            Callback callback)
        {
            if (item.StackObjectsCount <= 1)
                return true;
            
            var location = __instance.Inventory.Equipment
                .GetPrioritizedGridsForLoot(item)
                .FindLocationForItem(item);
            
        // No space to split into -> fail without consuming whole stack
            if (location == null)
            {
                
                var noLocationSplit = InteractionsHandlerClass.SplitToNowhere(item, item.StackObjectsCount, __instance, __instance, true);
                callback?.Invoke(noLocationSplit.ToResult());
                return false;
            }

            var split = InteractionsHandlerClass.SplitExact(item, 1, location, __instance, __instance, true);
            
            if (split.Failed)
            {
                callback?.Invoke(split.ToResult());
                return false;
            }
            
            __instance.TryRunNetworkTransaction(split, result =>
                {
                    if (!result.Succeed)
                    {
                        callback?.Invoke(result);
                        return;
                    }

                    Item splitItem = split.Value.ResultItem;
                    if (__instance.TryFindItem(splitItem.Id, out Item currentSplitItem))
                    {
                        splitItem = currentSplitItem;
                    }

                    __instance.SetupItem(splitItem, zone, position, rotation, setupTime, callback);
                }
            );

            return false;
        }
    }
}
