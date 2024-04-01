using RPG.Inventories;
using RPG.UI.Inventories;
using RPG.UI.Shops;
using UnityEngine;

namespace RPG.Shops
{
    public class ShopItemHolder : MonoBehaviour, IItemHolder
    {
        public InventoryItem GetItem()
        {
            return gameObject.GetComponentInParent<RowUI>().GetShopItem().GetInventoryItem();
        }
    }
}