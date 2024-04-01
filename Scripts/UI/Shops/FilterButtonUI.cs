using RPG.Inventories;
using RPG.Shops;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class FilterButtonUI : MonoBehaviour
    {
        [SerializeField] private ItemCategory filter = ItemCategory.None;
        Button button;
        Shop currentShop;
        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(SelectFilter);
        }
        public void SetShop(Shop currentShop)
        {
            this.currentShop = currentShop;
        }
        public void RefreshUI()
        {
            button.interactable = currentShop.GetFilter() != filter;
        }
        private void SelectFilter()
        {
            currentShop.SelectFilter(filter);
        }
    }
}