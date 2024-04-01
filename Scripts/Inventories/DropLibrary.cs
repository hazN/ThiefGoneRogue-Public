using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(fileName = "DropLibrary", menuName = "RPG/Inventory/Drop Library", order = 0)]
    public class DropLibrary : ScriptableObject
    {
        [SerializeField] private List<DropConfig> potentialDrops = new List<DropConfig>();
        [SerializeField] private List<DropChance> dropChances = new List<DropChance>();
        [System.Serializable]
        public class DropConfig
        {
            public InventoryItem item;
            public float weight;
            public int minLevel;
            public int maxLevel;
            public int minQuantity;
            public int maxQuantity;
            public int GetRandomQuantity()
            {
                if (!item.IsStackable()) return 1;
                return Random.Range(minQuantity, maxQuantity + 1);
            }
        }
        [System.Serializable]
        public struct DropChance
        {
            public float chance;
            public int minLevel;
            public int maxLevel;
            public int minDrops;
            public int maxDrops;
        }

        public struct Dropped
        {
            public InventoryItem item;
            public int quantity;
        }
        public void AddDropConfig(DropConfig drop)
        {
            potentialDrops.Add(drop);
        }
        public void RemoveDropConfig(DropConfig drop)
        {
            potentialDrops.Remove(drop);
        }
        public void AddDropChance(DropChance drop)
        {
            dropChances.Add(drop);
        }
        public void RemoveDropChance(DropChance drop)
        {
            dropChances.Remove(drop);
        }
        public IEnumerable<Dropped> GetRandomDrops(int level)
        {
            if (!ShouldRandomDrop(level)) yield break;
            DropChance dropChance = GetDropChance(level);
            for (int i = 0; i < GetRandomQuantity(dropChance); i++)
            {
                yield return GetRandomDrop(level);
            }
        }
        private bool ShouldRandomDrop(int level)
        {
            float dropChance = GetDropChance(level).chance;
            return Random.Range(0f, 100f) <= dropChance;
        }

        private DropChance GetDropChance(int level)
        {
            foreach (DropChance dropChance in dropChances)
            {
                if (IsBetween(level, dropChance))
                {
                    return dropChance;
                }
            }
            return default;
        }

        private int GetRandomQuantity(DropConfig drop)
        {
            return Random.Range(drop.minQuantity, drop.maxQuantity + 1);
        }
        private int GetRandomQuantity(DropChance drop)
        {
            return Random.Range(drop.minDrops, drop.maxDrops + 1);
        }
        private Dropped GetRandomDrop(int level)
        {
            DropConfig drop = SelectRandomItem(level);
            if (drop == null) return new Dropped();
            int quantity = GetRandomQuantity(drop);
            return new Dropped
            {
                item = drop.item,
                quantity = quantity
            };
        }
        private DropConfig SelectRandomItem(int level)
        {
            float totalWeight = GetTotalWeight(level);
            float randomRoll = Random.Range(0f, totalWeight);
            float chanceTotal = 0;
            foreach (DropConfig drop in potentialDrops)
            {
                if (!IsBetween(level, drop)) continue;
                chanceTotal += drop.weight;
                if (chanceTotal >= randomRoll)
                {
                    return drop;
                }
            }
            return null;
        }
        private float GetTotalWeight(int level)
        {
            float total = 0;
            foreach (DropConfig drop in potentialDrops)
            {
                if (IsBetween(level, drop))
                {
                    total += drop.weight;
                }
            }
            return total;
        }
        private bool IsBetween(int value, DropConfig drop)
        {
            if (value >= drop.minLevel && value <= drop.maxLevel)
            {
                return true;
            }
            return false;
        }
        private bool IsBetween(int value, DropChance drop)
        {
            if (value >= drop.minLevel && value <= drop.maxLevel)
            {
                return true;
            }
            return false;
        }
    }
}