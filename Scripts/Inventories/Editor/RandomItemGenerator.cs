using RPG.Attributes;
using RPG.Combat;
using RPG.Stats;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RPG.Inventories.Editor
{
    [CreateAssetMenu(fileName = "New Random Item Generator", menuName = "RPG/New Random Item Generator")]
    public class RandomItemGenerator : OdinEditorWindow
    {
        [MenuItem("Tools/Weapon Generator")]
        private static void OpenWindow()
        {
            GetWindow<RandomItemGenerator>().Show();
        }

        [SerializeField] private WeaponConfig defaultConfig;
        [SerializeField] private Weapon[] equippedPrefabs;

        [BoxGroup("Config")]
        [SerializeField] private int level = 1;

        [BoxGroup("Config")]
        [SerializeField][MinMaxSlider(0f, 100f, true)] private Vector2 damage;

        [BoxGroup("Config")]
        [SerializeField][MinMaxSlider(0f, 100f, true)] private Vector2 percentageBonus;

        [BoxGroup("Config")]
        [SerializeField][MinMaxSlider(0f, 100f, true)] private Vector2 range;

        [BoxGroup("Config")]
        [SerializeField][MinMaxSlider(0f, 100f, true)] private Vector2 cooldown;

        [BoxGroup("Config")]
        [SerializeField] private Projectile[] projectiles;

        [BoxGroup("Config")]
        [SerializeField] private ModifierPreset[] modifiers;

        [BoxGroup("Config")]
        [SerializeField] private Buff[] buffs;

        [BoxGroup("Generate")]
        [SerializeField] private string savePath = "Assets/Game/Weapons/Resources/Default";

        [BoxGroup("Generate")]
        [SerializeField] private int numberOfWeapons = 5;

        private int modifierAmount = 0;

        [ButtonGroup("Button")]
        [GUIColor(0.4f, 0.8f, 1f)]
        [Button(ButtonSizes.Large)]
        private void GenerateWeapons()
        {
            for (int i = 0; i < numberOfWeapons; i++)
            {
                Weapon weapon = equippedPrefabs[Random.Range(0, equippedPrefabs.Length)];
                var config = ScriptableObject.CreateInstance<WeaponConfig>();
                config.SetFromConfig(defaultConfig);
                // Get random modifiers from the list, could be 0, 1, 2, 3 with a high weight for the lower options
                // 0 has a weight of 34%, 1 has a weight of 40%, 2 has a weight of 20%, 3 has a weight of 5% and 1% for 4
                modifierAmount = GetModifierAmount();
                List<ModifierPreset> alreadyUsedStats = new List<ModifierPreset>();
                for (int j = 0; j < modifierAmount; j++)
                {
                    Modifier modifier = new Modifier();
                    // Make sure we don't use the same ModifierPreset twice
                    int mod = Random.Range(0, modifiers.Length - 1);
                    while (alreadyUsedStats.Contains(modifiers[mod]))
                    {
                        mod = Random.Range(0, modifiers.Length - 1);
                    }
                    alreadyUsedStats.Add(modifiers[mod]);
                    modifier.stat = modifiers[mod].stat;
                    modifier.additiveValue = Random.Range(modifiers[mod].additiveValue.x, modifiers[mod].additiveValue.y);
                    modifier.percentageValue = Random.Range(modifiers[mod].percentageValue.x, modifiers[mod].percentageValue.y);
                    // Use the level to determine the value of the modifier
                    // 50% chance to be additive, 50% chance to be percentage
                    if (modifier.additiveValue != 0)
                    {
                        modifier.additiveValue = modifier.additiveValue * level;
                    }
                    else if (modifier.percentageValue != 0)
                    {
                        // Percentage should not change as much as additive, make sure we are not dividing by a number that is too small
                        modifier.percentageValue = modifier.percentageValue * (Mathf.Max(level / 2, 1));
                    }
                    config.AddModifier(modifier.stat, modifier.additiveValue, modifier.percentageValue);
                }
                // Small chance based on number of modifiers to add a buff
                if (ShouldItemGetBuff(modifierAmount))
                {
                    if (buffs.Length > 0)
                    {
                        Buff buff = buffs[Random.Range(0, buffs.Length)];
                        Buff newBuff = new Buff();
                        // Apply some slight randomness to the percentage modifier
                        float mod = buff.GetBuffPercentageModifier();
                        float percentage = Random.Range(mod - 2f, mod + 2f);
                        newBuff.SetBuff(buff.GetBuffDuration(), percentage, buff.GetBuffType());
                        newBuff.SetSource(buff.GetSource());
                        config.AddBuff(buff);
                    }
                }
                string name = GetRandomName(defaultConfig.name);
                config.SetPickup(defaultConfig.GetPickup());
                config.SetIcon(defaultConfig.GetIcon());
                config.SetDisplayName(name);
                config.SetDescription("");
                config.ResetID();
                config.SetPrice(GetRandomPrice());
                config.SetWeapon(weapon);
                config.SetDamage(Random.Range(damage.x, damage.y));
                config.SetPercentageBonus(Random.Range(percentageBonus.x, percentageBonus.y));
                config.SetRange(Random.Range(range.x, range.y));
                config.SetCooldown(Random.Range(cooldown.x, cooldown.y));
                config.SetRightHanded(defaultConfig.IsRightHanded());
                AssetDatabase.CreateAsset(config, $"{savePath}/{name + " " + Random.Range(0, 999).ToString()}.asset");
            }
        }
        private int GetRandomPrice()
        {
            float price = 0;
            // Get the amount of modifiers
            int amt = modifierAmount;
            // Get the level
            int lvl = level;
            // Get the price of the base item
            float basePrice = defaultConfig.GetPrice();
            // Calculate based off those 3 with some randomness
            price = basePrice + (amt * 10) + (lvl * 10) * Random.Range(-2.5f, 5);
            return Mathf.Max(0, (int)price); // Ensure the price is non-negative
        }
        private int GetModifierAmount()
        {
            int numberOfModifiers = Random.Range(0, 100);
            if (numberOfModifiers < 34)
            {
                numberOfModifiers = 0;
            }
            else if (numberOfModifiers < 74)
            {
                numberOfModifiers = 1;
            }
            else if (numberOfModifiers < 94)
            {
                numberOfModifiers = 2;
            }
            else if (numberOfModifiers < 99)
            {
                numberOfModifiers = 3;
            }
            else
            {
                numberOfModifiers = 4;
            }
            return numberOfModifiers;
        }

        private string GetRandomName(string defaultName)
        {
            List<string[]> adjectiveSets = new List<string[]>
            {
                new string[] { "Shoddy", "Dented", "Rusty", "Weak", "Cracked", "Worn", "Frail", "Aged" }, // 0 modifiers
                new string[] { "Sturdy", "Sharp", "Reliable", "Balanced", "Solid", "Vicious", "Lethal", "Swift" }, // 1 modifier
                new string[] { "Fine", "Polished", "Masterwork", "Precise", "Dazzling", "Immaculate", "Elegant", "Serpentine" }, // 2 modifiers
                new string[] { "Exquisite", "Radiant", "Majestic", "Pristine", "Magnificent", "Supreme", "Royal", "Enlightened" }, // 3 modifiers
                new string[] { "Divine", "Infernal", "Epic", "Mythical", "Legendary", "Godly", "Titanic", "Apocalyptic" } // 4 modifiers
            };

            string[] selectedSet = adjectiveSets[modifierAmount];

            string randomAdjective = selectedSet[Random.Range(0, selectedSet.Length)];
            string randomSuffix = "";

            // 50% chance of adding a suffix for 3 or 4 modifiers
            if (modifierAmount >= 3 && Random.Range(0f, 1f) <= 0.5f)
            {
                string[] suffixes =
                {
                  "of Protection", "of Legends", "from the Abyss", "for Heroes", "of the Ancients",
                  "the Eternal", "of the Guardian", "the Enigma", "of the Celestial", "the Unyielding",
                  "the Phoenix", "of the Sentinel", "the Ward", "of the Avenger", "of the Defender",
                  "of the Conqueror", "of the Slayer", "of the Saviour", "the Unforgiving", "the Merciless"
                };

                randomSuffix = suffixes[Random.Range(0, suffixes.Length)];
            }
            return $"{randomAdjective} {defaultName} {randomSuffix}".Trim(); // Trim to remove extra spaces
        }
        [System.Serializable]
        public struct ModifierPreset
        {
            public Stat stat;
            [Tooltip("Additive value is added to the base value of the stat, percentage value is multiplied with the base value of the stat")]
            public Vector2 additiveValue;
            public Vector2 percentageValue;
        }
        private bool ShouldItemGetBuff(int modifiers)
        {
            // 0 modifiers = 0% chance
            // 1 modifier = 20% chance
            // 2 modifiers = 30% chance
            // 3 modifiers = 40% chance
            // 4 modifiers = 60% chance
            float chance = 0;
            switch (modifiers)
            {
                case 0:
                    chance = 0;
                    break;
                case 1:
                    chance = 0.2f;
                    break;
                case 2:
                    chance = 0.3f;
                    break;
                case 3:
                    chance = 0.4f;
                    break;
                case 4:
                    chance = 0.6f;
                    break;
            }
            return Random.Range(0f, 1f) <= chance;
        }

    }
}