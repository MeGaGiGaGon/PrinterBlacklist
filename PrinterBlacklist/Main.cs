﻿using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PrinterBlacklist
{
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    public class Main : BaseUnityPlugin
    {
        public const string ModGuid = "com.GiGaGon.PrinterBlacklist";
        public const string ModName = "PrinterBlacklist";
        public const string ModVer = "1.0.2";
        public System.Random rng = new System.Random();
        public static ConfigEntry<string> Myconfig { get; set; }
        private void Awake()
        {
            Myconfig = Config.Bind<string>("Config", "BannedItems", "CritGlasses,Bear,BleedOnHit,BossDamageBonus,Clover,ChainLightning,BeetleGland", "What items to ban. Format in comma seperated values, capitals are important, no spaces, use in code names. To get them, open the console and run item_list. Works for white, green, red, and boss items, all mixed in one string. Example: CritGlasses,Bear,BleedOnHit,BossDamageBonus,Clover,ChainLightning,BeetleGland");
            var BannedItemsRaw = Myconfig.Value;
            var BannedItemsList = BannedItemsRaw.Split(',').ToList();
            var BannedItems = BannedItemsList.Select(x => "ItemIndex." + x).ToList();

            List<String> DuplicatorList = new List<string> { "Duplicator(Clone)", "DuplicatorLarge(Clone)", "DuplicatorMilitary(Clone)", "DuplicatorWild(Clone)" };

            On.RoR2.ShopTerminalBehavior.GenerateNewPickupServer += (orig, self) =>
            {
                orig(self);
                var item = self.pickupIndex.ToString();

                if (DuplicatorList.Contains(self.name.ToString()) && BannedItems.Contains(item))
                {

                    var list = new List<RoR2.PickupIndex>();
                    switch (self.itemTier.ToString())
                    {
                        case "Tier1":
                            list = RoR2.Run.instance.availableTier1DropList; break;
                        case "Tier2":
                            list = RoR2.Run.instance.availableTier2DropList; break;
                        case "Tier3":
                            list = RoR2.Run.instance.availableTier3DropList; break;
                        case "Boss":
                            list = RoR2.Run.instance.availableBossDropList; break;
                    }

                GenerateItem:
                    var newitem = list[Math.Abs(RoR2.Run.instance.runRNG.nextInt % (list.Count - 1))];
                    if (BannedItems.Contains(newitem.ToString())) goto GenerateItem;
                    self.pickupIndex = newitem;
                    self.SetPickupIndex(newitem, self.hidden);
                }
            };
        }
    }
}
