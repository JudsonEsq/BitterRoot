using BepInEx;
using ItemLib;
using RoR2;
using UnityEngine;
using R2API.Utils;
using System.Reflection;

namespace FirstMod
{


    //This attribute specifies that we have a dependency on R2API, as we're using it to add Bandit to the game.
    //You don't need this if you're not using R2API in your plugin, it's just to tell BepInEx to initialize R2API before this plugin so it's safe to use R2API.
    [BepInDependency("com.bepis.r2api")]

    //This attribute is required, and lists metadata for your plugin.
    //The GUID should be a unique ID for this plugin, which is human readable (as it is used in places like the config). I like to use the java package notation, which is "com.[your name here].[your plugin name here]"
    //The name is the name of the plugin that's displayed on load, and the version number just specifies what version the plugin is.
    [BepInPlugin("dev.JudsonEsq.BitterestOfRoots", "Bitter Root", "0.0.1")]

    
    public class BitterestOfRoots : BaseUnityPlugin 
    {
        private const string ModVer = "0.0.1";
        private const string ModName = "Bitter Root";
        public const string ModGuid = "dev.JudsonEsq.BitterestOfRoots";
        

        public void Awake()
        {
            ItemIndex rootID = (ItemIndex)ItemLib.ItemLib.GetItemId("Bitter Root");

            On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
            {
                orig(self);
                if (self.inventory)
                {
                    self.SetFieldValue<float>("maxHealth", self.maxHealth * (1f + 0.08f * self.inventory.GetItemCount(rootID)));
                }
            };
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;
                Ginger();
                PickupDropletController.CreatePickupDroplet(PickupIndex.Find("ItemIndex.Count"), transform.position, transform.forward * 20f);
            }
        }
        //Various tags for BepIn
        

        
        public static AssetBundle bitterBundle;
        public static GameObject model;
        public static Object icon;

        //Left null, as this item does not need to be displayed
        private static ItemDisplayRule[] _itemDisplayRules;

        [Item(ItemAttribute.ItemType.Item)]
        public static CustomItem Ginger()
        {
            bitterBundle = AssetBundle.LoadFromFile(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/bitterbundle");
            
            icon = bitterBundle.LoadAsset<UnityEngine.Object>("Assets/superiorRoot.png");
            model = bitterBundle.LoadAsset<UnityEngine.GameObject>("Assets/SuperiorRoot.fbx");
            

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

            return new CustomItem(Ginger, model, icon, _itemDisplayRules);

        }
    }
}
