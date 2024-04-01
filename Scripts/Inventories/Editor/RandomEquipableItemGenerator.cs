using RPG.Combat;
using RPG.Stats;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Inventories.Editor
{
    public class RandomEquipableItemGenerator : OdinEditorWindow
    {
        [MenuItem("Tools/Equipable Item Generator")]
        private static void OpenWindow()
        {
            GetWindow<RandomEquipableItemGenerator>().Show();
        }

        [SerializeField] private StatsEquipableItem defaultConfig;

        [BoxGroup("Config")]
        [SerializeField] private int level = 1;

        [BoxGroup("Config")]
        [SerializeField] private ModifierPreset[] modifiers;

        [BoxGroup("Generate")]
        [SerializeField] private string savePath = "Assets/Game/Wearables/Resources/Default";

        [BoxGroup("Generate")]
        [SerializeField] private int numberOfWearables = 5;

        private int modifierAmount = 0;

        [ButtonGroup("Button")]
        [GUIColor(0.4f, 0.8f, 1f)]
        [Button(ButtonSizes.Large)]
        private void GenerateWearables()
        {
            for (int i = 0; i < numberOfWearables; i++)
            {
                var config = ScriptableObject.CreateInstance<StatsEquipableItem>();
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
                string name = GetRandomName(defaultConfig.name);
                config.SetPickup(defaultConfig.GetPickup());
                config.SetIcon(defaultConfig.GetIcon());
                config.SetDisplayName(name);
                config.SetDescription("");
                config.SetCategory(defaultConfig.GetCategory());
                config.ResetID();
                // Price should be influenced by the level and the modifiers
                config.SetPrice(GetRandomPrice());
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
            // Minimum of 1 modifier, maximum of 5
            int numberOfModifiers = Random.Range(0, 100);
            if (numberOfModifiers < 34)
            {
                numberOfModifiers = 1;
            }
            else if (numberOfModifiers < 74)
            {
                numberOfModifiers = 2;
            }
            else if (numberOfModifiers < 94)
            {
                numberOfModifiers = 3;
            }
            else if (numberOfModifiers < 99)
            {
                numberOfModifiers = 4;
            }
            else
            {
                numberOfModifiers = 5;
            }
            return numberOfModifiers;
        }

        private string GetRandomName(string defaultName)
        {
            List<string[]> adjectiveSets = new List<string[]>
            {
                new string[] { "Simple", "Basic", "Common", "Worn", "Ragged", "Frail", "Aged", "Tattered", "Rusty", "Threadbare", "Weathered", "Plain" }, // 0 modifiers
                new string[] { "Sturdy", "Reinforced", "Reliable", "Solid", "Protective", "Hardened", "Durable", "Balanced", "Robust", "Repaired", "Functional", "Tough" }, // 1 modifier
                new string[] { "Fine", "Polished", "Masterwork", "Elegant", "Immaculate", "Intricate", "Serpentine", "Dazzling", "Adorned", "Ornate", "Elaborate", "Sophisticated" }, // 2 modifiers
                new string[] { "Exquisite", "Radiant", "Majestic", "Pristine", "Magnificent", "Supreme", "Royal", "Enlightened", "Glorious", "Splendid", "Regal", "Imposing" }, // 3 modifiers
                new string[] { "Divine", "Infernal", "Epic", "Mythical", "Legendary", "Godly", "Titanic", "Apocalyptic", "Unyielding", "Ineffable", "Oblivion", "Dread" }, // 4 modifiers
            };


            string[] selectedSet = adjectiveSets[modifierAmount];

            string randomAdjective = selectedSet[Random.Range(0, selectedSet.Length)];
            string randomSuffix = "";

            // 50% chance of adding a suffix for 3 or 4 modifiers
            if (modifierAmount >= 3 && Random.Range(0f, 1f) <= 0.5f)
            {
                string[] suffixes =
                {
                    "of Doom", "of Legends", "from the Abyss", "for Heroes", "of the Ancients",
                    "the Eternal", "the Damned", "the Enigma", "the Celestial", "the Unyielding",
                    "the Phoenix", "the Serpent", "the Void", "the Avenger", "the Reckoning",
                    "the Conqueror", "the Slayer", "the Savior", "the Unforgiven", "the Merciless"
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
    }
}