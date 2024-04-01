using System.Collections;
using UnityEngine;

namespace RPG.Inventories
{
    public class ItemEvent : MonoBehaviour
    {
        [SerializeField] InventoryItem item = null;
        [SerializeField] int number = 1;
        public void AddItem()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            var inventory = player.GetComponent<Inventory>();
            if (inventory == null) return;
            inventory.AddToFirstEmptySlot(item, number);
        }
        public void RemoveItem()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            var inventory = player.GetComponent<Inventory>();
            if (inventory == null) return;
            int slot = FindFirstItemSlot(inventory, item);
            if (slot == -1) { return; }
            inventory.RemoveFromSlot(slot, 1);
        }
        private int FindFirstItemSlot(Inventory inventory, InventoryItem item)
        {
            for (int i = 0; i < inventory.GetSize(); i++)
            {
                if (inventory.GetItemInSlot(i) == item)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}