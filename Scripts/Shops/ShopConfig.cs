using RPG.Combat;
using RPG.Inventories;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Shops
{
    [CreateAssetMenu(fileName = "Shop Config", menuName = "RPG Project/Shop Config")]
    [System.Serializable]
    public class ShopConfig : SerializedScriptableObject
    {
        public List<StockItemConfig> stockItemConfigs = new List<StockItemConfig>();

        public void Add(StockItemConfig stockItemConfig)
        {
            if (stockItemConfig == null)
            {
                return;
            }
            if (stockItemConfigs.Contains(stockItemConfig))
            {
                return;
            }
            stockItemConfigs.Add(stockItemConfig);
        }

        public bool ContainsConfig(InventoryItem item)
        {
            foreach (var shopItem in stockItemConfigs)
            {
                if (shopItem.item == item)
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<StockItemConfig> GetAllConfigs()
        {
            foreach (var shopItem in stockItemConfigs)
            {
                yield return shopItem;
            }
        }
#if UNITY_EDITOR
        [SerializeField] private InventoryItem[] itemsToPopulate;
        [SerializeField] private int levelToUnlock;
        [ButtonGroup("Button")]
        [GUIColor(0.4f, 0.8f, 1f)]
        [Button(ButtonSizes.Large)]
        private void PopulateShop()
        {
            foreach (InventoryItem item in itemsToPopulate)
            {
                if (item == null) continue;
                // Config is based off amount of modifiers
                StockItemConfig itemConfig = new StockItemConfig();
                // Level is slightly randomized to make it more interesting
                itemConfig.item = item;
                itemConfig.levelToUnlock = levelToUnlock + Random.Range(-1, 2);
                if (item is WeaponConfig)
                {
                    var weapon = item as WeaponConfig;
                    int amtOfModifiers = weapon.GetModifiers().Count();
                    // Randomized values with modifiers having impact
                    itemConfig.initialStock = (int)(1 * Random.Range(1f, 20f) / (1 + amtOfModifiers));
                    itemConfig.maxStock = 999;
                    itemConfig.initialStock = (int)(1 * Random.Range(1f, 5f) / (1 + amtOfModifiers));
                    itemConfig.replenishRate = Random.Range(0, 5);
                    itemConfig.buyingDiscountPercentage = Mathf.Clamp(10 - amtOfModifiers * 2, 0, 100);
                }
                else if (item is StatsEquipableItem)
                {
                    var equipable = item as StatsEquipableItem;
                    int amtOfModifiers = equipable.GetModifiers().Count();
                    // Randomized values with modifiers having impact
                    itemConfig.initialStock = (int)(1 * Random.Range(1f, 20f) / (1 + amtOfModifiers));
                    itemConfig.maxStock = 999;
                    itemConfig.initialStock = (int)(1 * Random.Range(1f, 5f) / (1 + amtOfModifiers));
                    itemConfig.replenishRate = Random.Range(0, 5);
                    itemConfig.buyingDiscountPercentage = Mathf.Clamp(10 - amtOfModifiers * 2, 0, 100);
                }
                Add(itemConfig);
            }
        }
#endif
    }
}