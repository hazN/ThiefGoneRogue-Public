using RPG.Shops;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class ShopUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI shopName = null;
        [SerializeField] Transform listRoot = null;
        [SerializeField] RowUI rowUI = null;
        [SerializeField] TextMeshProUGUI totalCost = null;
        [SerializeField] Button confirmButton = null;
        [SerializeField] Button switchButton = null;
        Shopper shopper = null;
        Shop currentShop = null;
        private Color originalTextColor;
        void Start()
        {
            originalTextColor = totalCost.color;
            shopper = GameObject.FindWithTag("Player").GetComponent<Shopper>();
            if (shopper == null)
            { 
                return;
            }
            shopper.activeShopChanged += UpdateUI;
            confirmButton.onClick.AddListener(ConfirmTransaction);
            switchButton.onClick.AddListener(SwitchMode);
            UpdateUI();
        }
        private void UpdateUI()
        {
            // Unsubscribe old shop from event
            if (currentShop != null)
            {
                currentShop.onShopChanged -= RefreshUI;
            }

            // Get new shop
            currentShop = shopper.GetActiveShop();
            gameObject.SetActive(currentShop != null);

            // Set up filter buttons
            foreach (FilterButtonUI button in GetComponentsInChildren<FilterButtonUI>())
            {
                button.SetShop(currentShop);
            }

            if (!currentShop) { return; }
            shopName.text = currentShop.GetShopName();

            // Subscribe new shop to event
            currentShop.onShopChanged += RefreshUI;
            RefreshUI();
        }
        private void RefreshUI()
        {
            foreach (Transform child in listRoot)
            {
                Destroy(child.gameObject);
            }
            foreach (ShopItem item in currentShop.GetFilteredItems())
            {
                RowUI row = Instantiate(rowUI, listRoot);
                row.Setup(currentShop, item);
            }

            bool canTransact = currentShop.CanTransact();
            confirmButton.interactable = canTransact;
            // Change text colour to red if player can't afford transaction
            totalCost.color = (canTransact || currentShop.GetTransactionTotal() == 0) ? originalTextColor : Color.red;
            totalCost.text = $"Total: {currentShop.GetTransactionTotal():N0}c";
            // Change button text depending on mode
            switchButton.GetComponentInChildren<TextMeshProUGUI>().text = currentShop.IsBuyingMode() ? "SWITCH TO SELLING" : "SWITCH TO BUYING";
            confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = currentShop.IsBuyingMode() ? "Buy" : "Sell";

            // Set up filter buttons
            foreach (FilterButtonUI button in GetComponentsInChildren<FilterButtonUI>())
            {
                button.RefreshUI();
            }
        }
        public void Close()
        {
            shopper.SetActiveShop(null);
        }

        public void ConfirmTransaction()
        {
            currentShop.ConfirmTransaction();
        }
        public void SwitchMode()
        {
            currentShop.SetBuyingMode(!currentShop.IsBuyingMode());
        }
    }
}