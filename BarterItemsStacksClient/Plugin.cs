using BarterItemsStacksClient.Patches;
using BepInEx;
using BepInEx.Logging;
using System.Collections.Generic;

namespace BarterItemsStacksClient
{
    [BepInPlugin("com.slpf.barteritemsstacks", "BarterItemsStacksClient", "1.2.2")]
    [BepInDependency("com.lacyway.mc", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.tyfon.uifixes", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource LogSource;
        public static HashSet<string> StackItemsIDs { get; private set; } = new HashSet<string>();

        private void Awake()
        {
            LogSource = Logger;

            new UpdateItemViewPatch().Enable();
            new MergePatch().Enable();
            new TransferMaxPatch().Enable();
            new HideoutMethod23Patch().Enable();
            new HideoutMethod21Patch().Enable();
            new COCCheckCompatibilityPatch().Enable();
            new CanApplyItemPatch().Enable();
            new RepaitKitStackUsePatch().Enable();
            new PlaceItemTriggerPatch().Enable();
            new ConvertOperationResultToOperationPatch().Enable();

            if (HarmonyLib.AccessTools.TypeByName("MergeConsumables.Patches.ExecutePossibleAction_Patch") != null)
            {
                new ExecutePossibleActionPatch().Enable();
            }

            if (HarmonyLib.AccessTools.TypeByName("UIFixes.SortPatches+StackFirstPatch") != null)
            {
                new StackAllPatch().Enable();
            }
        }
    }
}
