using RPG.Combat;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RPG.Inventories.Editor
{
    [CreateAssetMenu(fileName = "Drop Library Generator", menuName = "RPG/Drop Library Generator")]
    public class DropLibraryGenerator : OdinEditorWindow
    {
        [MenuItem("Tools/Drop Library Generator")]
        private static void OpenWindow()
        {
            GetWindow<DropLibraryGenerator>().Show();
        }
        [SerializeField] private DropLibrary dropLibrary;

        [BoxGroup("Config")]
        [SerializeField] private List<InventoryItem> items = new List<InventoryItem>();

        [BoxGroup("Config")]
        [SerializeField][MinMaxSlider(0, 100, true)] private Vector2 level;

        [BoxGroup("Config")]
        [SerializeField][MinMaxSlider(0, 5, true)] private Vector2 quantity;

        [BoxGroup("Config")]
        [SerializeField] private DropLibrary.DropChance dropChance;

        [ButtonGroup("Button")]
        [GUIColor(0.4f, 0.8f, 1f)]
        [Button(ButtonSizes.Large)]
        private void PopulateDropLibrary()
        {
            if (dropLibrary == null)
            {
                dropLibrary = ScriptableObject.CreateInstance<DropLibrary>();
                AssetDatabase.CreateAsset(dropLibrary, "Assets/Game/Drops/NewDropLibrary.asset");
            }
            foreach (var item in items)
            {
                DropLibrary.DropConfig dropConfig = new DropLibrary.DropConfig();
                dropConfig.item = item;
                dropConfig.minLevel = (int)level.x;
                dropConfig.maxLevel = (int)level.y;
                dropConfig.minQuantity = (int)quantity.x;
                dropConfig.maxQuantity = (int)quantity.y;
                // Weight is based on the amount of modifiers, so that weapons with less modifiers are more likely to drop
                if (item is WeaponConfig)
                {
                    int modifierAmount = ((WeaponConfig)item).GetModifiers().Count();
                    dropConfig.weight = 1f / (modifierAmount + 1);
                }
                else if (item is StatsEquipableItem)
                {
                    int modifierAmount = ((StatsEquipableItem)item).GetModifiers().Count();
                    dropConfig.weight = 1f / (modifierAmount + 1);
                }
                else
                {
                    dropConfig.weight = 1f;
                }
                dropLibrary.AddDropConfig(dropConfig);
            }
            dropLibrary.AddDropChance(dropChance);
        }
    }
}