using RPG.Inventories;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace RPG.Abilities
{
    public class CooldownStore : MonoBehaviour
    {
        private Dictionary<InventoryItem, float> cooldownTimers = new Dictionary<InventoryItem, float>();
        private Dictionary<InventoryItem, float> initialCooldownTimes = new Dictionary<InventoryItem, float>();
        private void Update()
        {
            var keys = new List<InventoryItem>(cooldownTimers.Keys);
            foreach (InventoryItem inventoryItem in keys)
            {
                cooldownTimers[inventoryItem] -= Time.deltaTime;
                if (cooldownTimers[inventoryItem] <= 0)
                {
                    cooldownTimers.Remove(inventoryItem);
                    initialCooldownTimes.Remove(inventoryItem);
                }
            }
        }
        public void StartCooldown(InventoryItem inventoryItem, float cooldownTime)
        {
            cooldownTimers[inventoryItem] = cooldownTime;
            initialCooldownTimes[inventoryItem] = cooldownTime;
        }
        public float GetTimeRemaining(InventoryItem inventoryItem)
        {
            if (!cooldownTimers.ContainsKey(inventoryItem)) return 0;
            return cooldownTimers[inventoryItem];
        }

        public float GetFractionRemaining(InventoryItem inventoryItem)
        {
            if (inventoryItem == null) return 0;
            if (!cooldownTimers.ContainsKey(inventoryItem)) return 0;
            return cooldownTimers[inventoryItem] / initialCooldownTimes[inventoryItem];
        }
    }
}