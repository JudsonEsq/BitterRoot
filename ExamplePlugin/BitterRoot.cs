using BepInEx;
using ItemLib;
using RoR2;
using R2API;
using UnityEngine;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace FirstMod
{


    //This attribute specifies that we have a dependency on R2API, as we're using it to add Bandit to the game.
    //You don't need this if you're not using R2API in your plugin, it's just to tell BepInEx to initialize R2API before this plugin so it's safe to use R2API.
    [BepInDependency("com.bepis.r2api")]

    [BepInDependency(ItemLibPlugin.ModGuid)]

    //This attribute is required, and lists metadata for your plugin.
    //The GUID should be a unique ID for this plugin, which is human readable (as it is used in places like the config). I like to use the java package notation, which is "com.[your name here].[your plugin name here]"
    //The name is the name of the plugin that's displayed on load, and the version number just specifies what version the plugin is.
    [BepInPlugin("dev.JudsonEsq.BitterestOfRoots", "Bitter Root", "0.0.1")]

    
    public class BitterestOfRoots : BaseUnityPlugin 
    {
        private const string ModVer = "0.0.1";
        private const string ModName = "Bitter Root";
        public const string ModGuid = "dev.JudsonEsq.BitterestOfRoots";
        

        void Start()
        {
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        void CharacterBody_RecalculateStats(ILContext il)
        {

            ILCursor rootCursor = new ILCursor(il);
            rootCursor.GotoNext(
                x => x.MatchLdcR4(1),       //actual address at this point is ldc.r4
                x => x.MatchStloc(32)       //actual address at this point is stloc.s
                );
            rootCursor.Index += 2;
            rootCursor.Emit(OpCodes.Ldloc, 32);
            rootCursor.Emit(OpCodes.Ldarg, 0);
            rootCursor.EmitDelegate<Func<float, RoR2.CharacterBody, float>>(
                (currentMultiplier, self) =>
                {
                    if (self.inventory)
                        currentMultiplier += self.inventory.GetItemCount((ItemIndex)ItemLib.ItemLib.GetItemId("Bitter Root")) * 0.08f;
                    return currentMultiplier;

                }
                );
            rootCursor.Emit(OpCodes.Stloc, 32);
            

            //Old Code Museum; admissions 10 roots
            //float oldHealth = self.maxHealth;
            //self.SetPropertyValue<float>("maxHealth", self.maxHealth * (1f + 0.08f * self.inventory.GetItemCount(rootID)));
            //self.healthComponent.Heal(self.maxHealth - oldHealth, default(ProcChainMask), false);

        }

        //Various tags for BepIn
        
        public static AssetBundle bitterBundle;
        public static GameObject model;
        public static UnityEngine.Object icon;

        //Left null, as this item does not need to be displayed
        private static ItemDisplayRule[] _itemDisplayRules;

        [Item(ItemAttribute.ItemType.Item)]
        public static ItemLib.CustomItem Ginger()
        {
            bitterBundle = AssetBundle.LoadFromFile(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/bitterbundle");
            
            icon = bitterBundle.LoadAsset<UnityEngine.Object>("Assets/superiorRoot.png");
            model = bitterBundle.LoadAsset<GameObject>("Assets/rootModel.prefab");
            

            ItemDef Ginger = new ItemDef
            {
                tier = ItemTier.Tier1,
                pickupModelPath = "",
                pickupIconPath = "",
                nameToken = "Bitter Root",
                pickupToken = "8% increased maximum health",
                descriptionToken = "oof ouch owie it's bitter o no"
            };
            _itemDisplayRules = null;

            return new ItemLib.CustomItem(Ginger, model, icon, _itemDisplayRules);

        }
    }
}
