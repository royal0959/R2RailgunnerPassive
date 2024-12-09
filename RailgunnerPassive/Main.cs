using System;
using System.IO;
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

        public static PluginInfo PInfo { get; private set; }

        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "royal";
        public const string PluginName = "RailgunnerCritChancePassive";
        public const string PluginVersion = "1.0.0";

        private Sprite LoadSpriteFromDisk()
        {
            string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Info.Location), "AtomStabilizer.png");
            if (!File.Exists(path))
            {
                Log.Error("File not found at: " + path);
                return null;
            }

            // Read the PNG file data into a byte array
            byte[] imageData = File.ReadAllBytes(path);

            // Create a new Texture2D (the size and format can be left empty since it will be overwritten)
            Texture2D texture = new Texture2D(2, 2);

            // Load the image data into the texture using ImageConversion.LoadImage
            if (ImageConversion.LoadImage(texture, imageData))
            {
                // Create a Sprite from the loaded texture
                Sprite newSprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));  // Pivot at the center

                return newSprite;
            }
            else
            {
                Log.Error("Failed to load texture from image data.");
                return null;
            }
        }

        public void Awake()
        {
            Log.Init(Logger);

            PInfo = Info;

            Asset.Init();
            CustomItems.Init(); 

            // https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/Assets/Localization/
            GameObject railBodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerBody.prefab").WaitForCompletion();

            LanguageAPI.Add("RAILGUNNER_PASSIVE_CRITCHANCE_NAME", "Atom Stabilizer");
            LanguageAPI.Add("RAILGUNNER_PASSIVE_CRITCHANCE_DESCRIPTION", $"Can gain <style=cIsDamage>Critical Strike Chance</style> and cause them normally. Gain <style=cIsDamage>0.5% attack speed</style> for every 1% <style=cIsDamage>Critical Strike Chance</style>.");

            PassiveItemSkillDef atomStabilizerSkill = ScriptableObject.CreateInstance<PassiveItemSkillDef>();
            atomStabilizerSkill.passiveItem = CustomItems.ItemAttackSpeedWithCrit;
            //atomStabilizerSkill.passiveItem = null;

            //atomStabilizerSkill.icon = Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/Railgunner/texCrosshairRailgunSniperCenter.png").WaitForCompletion();
            //atomStabilizerSkill.icon = Asset.mainBundle.LoadAsset<Sprite>("textAtomStabilizerIcon");
            atomStabilizerSkill.icon = LoadSpriteFromDisk();

            atomStabilizerSkill.skillDescriptionToken = "RAILGUNNER_PASSIVE_CRITCHANCE_DESCRIPTION";
            atomStabilizerSkill.skillName = "RAILGUNNER_PASSIVE_CRITCHANCE_NAME";
            atomStabilizerSkill.skillNameToken = "RAILGUNNER_PASSIVE_CRITCHANCE_NAME";
            ContentAddition.AddSkillDef(atomStabilizerSkill);

            SkillLocator skillLocator = railBodyPrefab.GetComponent<SkillLocator>();

            foreach (GenericSkill skill in railBodyPrefab.GetComponentsInChildren<GenericSkill>())
            {
                if ((skill._skillFamily as ScriptableObject).name.Contains("Passive"))
                {
                    SkillFamily family = skill._skillFamily;
                    Array.Resize(ref family.variants, family.variants.Length + 1);
                    family.variants[^1] = new SkillFamily.Variant
                    {
                        skillDef = atomStabilizerSkill,
                        viewableNode = new ViewablesCatalog.Node(atomStabilizerSkill.skillNameToken, false)
                    };
                }
            }
        }
    }
}