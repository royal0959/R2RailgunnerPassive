using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Plugin
{
    internal class CustomItems
    {

        public static ItemDef ItemAttackSpeedWithCrit;

        private const float ATTACK_SPEED_PER_CRIT_CHANCE = 0.005f; // 0.5%


        public static void Init()
        {
            CreateItemAttackSpeedWithCrit();
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        public static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (!sender.inventory)
            {
                return;
            }

            int stack = sender.inventory.GetItemCount(ItemAttackSpeedWithCrit);

            if (stack <= 0)
            {
                return;
            }

            args.attackSpeedMultAdd += sender.crit * ATTACK_SPEED_PER_CRIT_CHANCE;
        }

        private static void CreateItemAttackSpeedWithCrit()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            ItemAttackSpeedWithCrit = new ItemDef
            {
                name = "AttackSpeedWithCrit",
                //tier = ItemTier.NoTier,
                //_itemTierDef = Addressables.LoadAssetAsync<ItemTierDef>("RoR2/Base/Common/Tier3Def.asset").WaitForCompletion(),
                _itemTierDef = null,

                deprecatedTier = ItemTier.NoTier,
                pickupModelPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mystery/PickupMystery.prefab").WaitForCompletion(),
                pickupIconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion(),
                nameToken = "ITEM_ATTACKSPEED_WITH_CRIT_NAME",
                pickupToken = "ITEM_ATTACKSPEED_WITH_CRIT_PICKUP",
                descriptionToken = "ITEM_ATTACKSPEED_WITH_CRIT_DESC",
                loreToken = "ITEM_ATTACKSPEED_WITH_CRIT_LORE",
                tags =
                [
                    ItemTag.WorldUnique,
                    ItemTag.CannotCopy,
                    ItemTag.CannotDuplicate,
                    ItemTag.BrotherBlacklist,
                    ItemTag.CannotSteal,
                ],

                canRemove = false,
                hidden = true
            };
#pragma warning restore CS0618 // Type or member is obsolete

            var displayRules = new ItemDisplayRuleDict(null);

            var itemIndex = new CustomItem(ItemAttackSpeedWithCrit, displayRules);
            ItemAPI.Add(itemIndex);
        }
    }
}
