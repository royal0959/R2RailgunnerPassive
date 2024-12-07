using System;
using BepInEx;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Plugin
{
    [BepInDependency(R2API.ContentManagement.R2APIContentManager.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]

    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "royal";
        public const string PluginName = "RailgunnerCritChancePassive";
        public const string PluginVersion = "1.0.0";

        public void Awake()
        {
            // https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/Assets/Localization/
            GameObject railBodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerBody.prefab").WaitForCompletion();

            LanguageAPI.Add("RAILGUNNER_PASSIVE_CRITCHANCE_NAME", "Placeholder");
            LanguageAPI.Add("RAILGUNNER_PASSIVE_CRITCHANCE_DESCRIPTION", $"Can <style=cIsDamage>Critically Strike</style>");

            PassiveItemSkillDef mySkillDef = ScriptableObject.CreateInstance<PassiveItemSkillDef>();

            mySkillDef.passiveItem = null;
            mySkillDef.icon = Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/Railgunner/texCrosshairRailgunSniperCenter.png").WaitForCompletion();
            mySkillDef.skillDescriptionToken = "RAILGUNNER_PASSIVE_CRITCHANCE_DESCRIPTION";
            mySkillDef.skillName = "RAILGUNNER_PASSIVE_CRITCHANCE_NAME";
            mySkillDef.skillNameToken = "RAILGUNNER_PASSIVE_CRITCHANCE_NAME";
            ContentAddition.AddSkillDef(mySkillDef);

            SkillLocator skillLocator = railBodyPrefab.GetComponent<SkillLocator>();


            foreach (GenericSkill skill in railBodyPrefab.GetComponentsInChildren<GenericSkill>())
            {
                if ((skill._skillFamily as ScriptableObject).name.Contains("Passive"))
                {
                    SkillFamily family = skill._skillFamily;
                    Array.Resize(ref family.variants, family.variants.Length + 1);
                    family.variants[^1] = new SkillFamily.Variant
                    {
                        skillDef = mySkillDef,
                        viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false)
                    };
                }
            }
        }
    }
}