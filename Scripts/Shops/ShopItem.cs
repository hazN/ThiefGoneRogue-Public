using RPG.Inventories;
using System;
using System.Collections;
using UnityEngine;

namespace RPG.Shops
{
    public class ShopItem
    {
        public InventoryItem item;
        public int availability;
        public int price;
        public int quantityInTransaction;

        public ShopItem(InventoryItem item, int availability, int price, int quantityInTransaction)
        {
            this.item = item;
            this.availability = availability;
            this.price = price;
            this.quantityInTransaction = quantityInTransaction;
        }

        public Sprite GetIcon()
        {
            return item.GetIcon();
        }
        public int GetAvailability()
        {
            return availability;
        }
        public int GetPrice()
        {
            return price;
        }
        public int GetQuantityInTransaction()
        {
            return quantityInTransaction;
        }
        public InventoryItem GetInventoryItem()
        {
            return item;
        }
    }
}