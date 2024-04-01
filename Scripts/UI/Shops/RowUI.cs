using RPG.Shops;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class RowUI : MonoBehaviour
    {
        [SerializeField] private Image iconField = null;
        [SerializeField] private TextMeshProUGUI nameText = null;
        [SerializeField] private TextMeshProUGUI availabilityText = null;
        [SerializeField] private TextMeshProUGUI priceText = null;
        [SerializeField] private TextMeshProUGUI quantityText = null;
        private Shop currentShop = null;
        private ShopItem currentItem = null;
        public void Setup(Shop currentShop, ShopItem item)
        {
            this.currentShop = currentShop;
            currentItem = item;
            iconField.sprite = item.GetIcon();
            nameText.text = item.item.GetDisplayName();
            availabilityText.text = $"{item.GetAvailability()}";
            priceText.text = $"{item.GetPrice():N0}c";
            quantityText.text = $"{item.GetQuantityInTransaction()}";
        }

        public void Add()
        {
            int amt = 1;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                amt = 10;
            }
            currentShop.AddToTransaction(currentItem.GetInventoryItem(), amt);
        }

        public void Remove()
        {
            int amt = 1;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                amt = 10;
            }
            currentShop.AddToTransaction(currentItem.GetInventoryItem(), -amt);
        }

        public ShopItem GetShopItem()
        {
            return currentItem;
        }
    }
}