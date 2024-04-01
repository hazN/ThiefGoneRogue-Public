using RPG.Control;
using RPG.Inventories;
using RPG.Saving;
using RPG.SceneManagement;
using RPG.Stats;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour, IRaycastable, ISaveable
    {
        [SerializeField] private string shopName = "SHOP";
        [SerializeField] private float sellingDiscountPercentage = 80f;
        [SerializeField] private ShopConfig stock;

        private Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();
        private Dictionary<InventoryItem, int> stockSold = new Dictionary<InventoryItem, int>();
        private Shopper currentShopper = null;
        private bool isBuyingMode = true;
        private ItemCategory shopFilter = ItemCategory.None;

        public event Action onShopChanged;

        private void Awake()
        {
        }

        public void SetShopper(Shopper shopper)
        {
            currentShopper = shopper;
        }

        public IEnumerable<ShopItem> GetFilteredItems()
        {
            foreach (ShopItem shopItem in GetAllItems())
            {
                if (shopFilter == ItemCategory.None || shopItem.GetInventoryItem().GetCategory() == shopFilter)
                {
                    yield return shopItem;
                }
            }
        }

        public IEnumerable<ShopItem> GetAllItems()
        {
            if (IsBuyingMode())
            {
                Dictionary<InventoryItem, int> prices = GetPrices();
                Dictionary<InventoryItem, int> availabilites = GetAvailabilites();
                int shopperLevel = GetShopperLevel();
                foreach (InventoryItem item in availabilites.Keys)
                {
                    // Skip if no stock available
                    if (availabilites[item] <= 0) continue;

                    // Get the item, price, and quantity
                    int price = prices[item];
                    int quantitiyInTransaction = 0;
                    transaction.TryGetValue(item, out quantitiyInTransaction);
                    yield return new ShopItem(item, availabilites[item], price, quantitiyInTransaction);
                }
            }
            else
            {
                foreach (ShopItem shopItem in GetSellableItems())
                {
                    yield return shopItem;
                }
            }
        }

        public void SelectFilter(ItemCategory itemCategory)
        {
            shopFilter = itemCategory;
            onShopChanged?.Invoke();
        }

        public ItemCategory GetFilter()
        {
            return shopFilter;
        }

        public void SetBuyingMode(bool isBuying)
        {
            isBuyingMode = isBuying;
            onShopChanged?.Invoke();
        }

        public bool IsBuyingMode()
        {
            return isBuyingMode;
        }

        public bool CanTransact()
        {
            if (IsTransactionEmpty()) return false;
            if (!HasSufficientFunds()) return false;
            if (!HasInventorySpace()) return false;
            return true;
        }

        public bool HasInventorySpace()
        {
            if (!IsBuyingMode()) return true;

            // Get shopper inventory
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            if (!shopperInventory) { return false; }

            // Create a list of all items in the transaction
            List<InventoryItem> flatItems = new List<InventoryItem>();
            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityInTransaction();
                for (int i = 0; i < quantity; i++)
                {
                    flatItems.Add(item);
                }
            }

            // Check if inventory has space for the items
            return shopperInventory.HasSpaceFor(flatItems);
        }

        public bool HasSufficientFunds()
        {
            if (!IsBuyingMode()) return true;
            return currentShopper.GetComponent<Purse>().GetBalance() >= GetTransactionTotal();
        }

        public bool IsTransactionEmpty()
        {
            return transaction.Count == 0;
        }

        public void ConfirmTransaction()
        {
            // Get shopper inventory
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            Purse shopperPurse = currentShopper.GetComponent<Purse>();
            if (!shopperInventory || !shopperPurse) { return; }

            // Transfer to or from inventory
            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityInTransaction();
                int price = shopItem.GetPrice();
                for (int i = 0; i < quantity; i++)
                {
                    if (IsBuyingMode())
                    {
                        BuyItem(shopperInventory, shopperPurse, item, price);
                    }
                    else
                    {
                        SellItem(shopperInventory, shopperPurse, item, price);
                    }
                }
            }
            //TODO: SavingWrapper.Save();
            onShopChanged?.Invoke();
        }

        public int GetTransactionTotal()
        {
            float total = 0;
            foreach (ShopItem item in GetAllItems())
            {
                total += item.GetPrice() * item.GetQuantityInTransaction();
            }
            return (int)total;
        }

        public void AddToTransaction(InventoryItem item, int quantity)
        {
            if (!transaction.ContainsKey(item))
            {
                transaction[item] = 0;
            }
            var availabilities = GetAvailabilites();
            int availableStock = availabilities[item];
            if (transaction[item] + quantity > availableStock)
            {
                transaction[item] = availableStock;
            }
            else
            {
                transaction[item] += quantity;
            }
            if (transaction[item] <= 0)
            {
                transaction.Remove(item);
            }
            onShopChanged?.Invoke();
        }

        public CursorType GetCursorType()
        {
            return CursorType.Shop;
        }

        public bool HandleRaycast(PlayerController playerController, bool mouseClicked, bool mouseHeld)
        {
            if (mouseClicked)
            {
                playerController.GetComponent<Shopper>().SetActiveShop(this);
            }
            return true;
        }

        public float GetRadius()
        {
            return 2f;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public string GetShopName()
        {
            return shopName;
        }

        private int CountItemsInInventory(InventoryItem item)
        {
            Inventory inventory = currentShopper.GetComponent<Inventory>();
            if (!inventory) { return 0; }
            int total = 0;
            for (int i = 0; i < inventory.GetSize(); i++)
            {
                if (inventory.GetItemInSlot(i) == item)
                {
                    total += inventory.GetNumberInSlot(i);
                }
            }
            return total;
        }

        private int GetPrice(InventoryItem item, float buyingDiscountPercentage)
        {
            // Buying mode
            if (isBuyingMode)
            {
                return Mathf.RoundToInt(item.GetPrice() * (1 - buyingDiscountPercentage / 100));
            }
            // Selling mode
            return Mathf.RoundToInt(item.GetPrice() * (1 - buyingDiscountPercentage / 100) * (sellingDiscountPercentage / 100));
        }

        private void BuyItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, int price)
        {
            if (shopperPurse.GetBalance() < price) { return; }
            bool success = shopperInventory.AddToFirstEmptySlot(item, 1);
            if (success)
            {
                AddToTransaction(item, -1);
                if (!stockSold.ContainsKey(item))
                {
                    stockSold[item] = 0;
                }
                stockSold[item]++;
                shopperPurse.UpdateBalance(-price);
            }
        }

        private void SellItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, int price)
        {
            // Find first slot with item
            int slot = FindFirstItemSlot(shopperInventory, item);
            if (slot == -1) { return; }

            // Remove from inventory and remove from transaction
            AddToTransaction(item, -1);

            shopperInventory.RemoveFromSlot(slot, 1);
            if (!stockSold.ContainsKey(item))
            {
                stockSold[item] = 0;
                // Add to stock config
                stock.Add(new StockItemConfig(item, 0, 0, -1, -20));
            }
            stockSold[item]--;
            shopperPurse.UpdateBalance(price);
        }

        private int FindFirstItemSlot(Inventory shopperInventory, InventoryItem item)
        {
            for (int i = 0; i < shopperInventory.GetSize(); i++)
            {
                if (shopperInventory.GetItemInSlot(i) == item)
                {
                    return i;
                }
            }
            return -1;
        }

        private int GetShopperLevel()
        {
            BaseStats stats = currentShopper.GetComponent<BaseStats>();
            if (stats == null) return 0;

            return stats.GetLevel();
        }

        private Dictionary<InventoryItem, int> GetAvailabilites()
        {
            Dictionary<InventoryItem, int> availabilities = new Dictionary<InventoryItem, int>();
            if (isBuyingMode)
            {
                foreach (StockItemConfig config in GetAvailableConfigs())
                {
                    if (!availabilities.ContainsKey(config.item))
                    {
                        int sold = 0;
                        stockSold.TryGetValue(config.item, out sold);
                        availabilities[config.item] = -sold;
                    }
                    availabilities[config.item] += config.initialStock;
                }
            }
            else
            {
                foreach (ShopItem item in GetSellableItems())
                {
                    availabilities[item.GetInventoryItem()] = CountItemsInInventory(item.GetInventoryItem());
                }
            }
            return availabilities;
        }

        private Dictionary<InventoryItem, int> GetPrices()
        {
            Dictionary<InventoryItem, int> prices = new Dictionary<InventoryItem, int>();
            foreach (StockItemConfig config in GetAvailableConfigs())
            {
                if (isBuyingMode)
                {
                    if (!prices.ContainsKey(config.item))
                    {
                        prices[config.item] = (int)config.item.GetPrice();
                    }

                    prices[config.item] = Mathf.RoundToInt(prices[config.item] * (1 - config.buyingDiscountPercentage / 100));
                }
                else
                {
                    if (!prices.ContainsKey(config.item))
                    {
                        prices[config.item] = (int)config.item.GetPrice();
                    }
                    prices[config.item] = Mathf.RoundToInt(prices[config.item] * (1 - sellingDiscountPercentage / 100));
                }
            }
            return prices;
        }

        private IEnumerable<StockItemConfig> GetAvailableConfigs()
        {
            foreach (StockItemConfig config in stock.GetAllConfigs())
            {
                if (config.levelToUnlock <= GetShopperLevel())
                {
                    yield return config;
                }
            }
        }

        private IEnumerable<ShopItem> GetSellableItems()
        {
            Inventory inventory = currentShopper.GetComponent<Inventory>();
            List<InventoryItem> items = new List<InventoryItem>();

            for (int i = 0; i < inventory.GetSize(); i++)
            {
                InventoryItem item = inventory.GetItemInSlot(i);
                if (item == null) continue;

                // Check if the item already has a slot in the shop
                if (items.Contains(item)) continue;

                // Get the item, price, availability and quantity
                int price = GetPrice(item, 0);
                int availableStock = CountItemsInInventory(item);

                int quantitiyInTransaction = 0;
                transaction.TryGetValue(item, out quantitiyInTransaction);

                // Add item to list
                items.Add(item);

                // Create new shop item
                yield return new ShopItem(item, availableStock, price, quantitiyInTransaction);
            }
        }
        private bool ConfigContains(InventoryItem item)
        {
            return stock.ContainsConfig(item);
        }
        public object CaptureState()
        {
            Dictionary<string, int> saveObject = new Dictionary<string, int>();
            foreach (var pair in stockSold)
            {
                saveObject[pair.Key.GetItemID()] = pair.Value;
            }
            return saveObject;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, int> saveObject = (Dictionary<string, int>)state;
            foreach (var pair in saveObject)
            {
                stockSold[InventoryItem.GetFromID(pair.Key)] = pair.Value;
                if(!ConfigContains(InventoryItem.GetFromID(pair.Key)))
                {
                    stock.Add(new StockItemConfig(InventoryItem.GetFromID(pair.Key), 0, 0, -1, -20));
                }
            }
        }
    }
}