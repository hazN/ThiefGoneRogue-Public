using RPG.Inventories;
using System;
using UnityEngine;

namespace RPG.Shops
{
    [System.Serializable]
    public class StockItemConfig
    {

        [Range(-100, 100)]
        public float buyingDiscountPercentage = 0;
        public int initialStock = 5;

        public InventoryItem item = null;
        public int levelToUnlock = 0;
        public int maxStock = 999;
        public int replenishRate = 5;
        public StockItemConfig(InventoryItem item, int initialStock, int replenishRate, int maxStock, float buyingDiscountPercentage)
        {
            this.item = item;
            this.initialStock = initialStock;
            this.replenishRate = replenishRate;
            this.maxStock = maxStock;
            this.buyingDiscountPercentage = buyingDiscountPercentage;
            this.levelToUnlock = 0;
        }
        public StockItemConfig()
        {
            // Set Defaults
            this.item = null;
            this.initialStock = 5;
            this.replenishRate = 5;
            this.maxStock = 999;
            this.buyingDiscountPercentage = 0;
            this.levelToUnlock = 0;
        }
    }
}