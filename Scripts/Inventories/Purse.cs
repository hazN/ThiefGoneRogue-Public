using RPG.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    public class Purse : MonoBehaviour, ISaveable, IItemStore
    {
        [SerializeField] private float startingBalance = 100f;
        private float balance = 0f;
        public event Action onBalanceChange;

        private void Awake()
        {
            balance = startingBalance;
        }
        public void UpdateBalance(float amount)
        {
            balance += amount;
            onBalanceChange?.Invoke();
        }
        public float GetBalance()
        {
            return balance;
        }

        public object CaptureState()
        {
            return balance;
        }

        public void RestoreState(object state)
        {
            balance = (float)state;
        }

        public int AddItems(InventoryItem item, int number)
        {
            if (item is CurrencyItem currencyItem)
            {
                UpdateBalance(currencyItem.GetPrice() * number);
                return number;
            }
            return 0;
        }
    }
}