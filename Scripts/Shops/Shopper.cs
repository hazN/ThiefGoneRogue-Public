using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Shops
{
    public class Shopper : MonoBehaviour
    {
        private Shop activeShop = null;
        public event Action activeShopChanged;
        public void SetActiveShop(Shop shop)
        {
            if (activeShop)
            {
                activeShop.SetShopper(null);
            }
            activeShop = shop;
            if (activeShop)
            {
                activeShop.SetShopper(this);
            }
            activeShopChanged?.Invoke();
        }

        public Shop GetActiveShop()
        {
            return activeShop;
        }
    }
}