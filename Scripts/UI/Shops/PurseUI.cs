using RPG.Inventories;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace RPG.UI.Shops
{
    public class PurseUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI balanceText = null;
        private Purse playerPurse = null;
        private void Awake()
        {
            playerPurse = GameObject.FindGameObjectWithTag("Player").GetComponent<Purse>();
            if (playerPurse == null) Debug.LogError("Player does not have a purse component");
        }
        
        private void OnEnable()
        {
            if (playerPurse == null) return;
            playerPurse.onBalanceChange += UpdateBalanceText;
            UpdateBalanceText();
        }

        private void OnDisable()
        {
            if (playerPurse == null) return;
            playerPurse.onBalanceChange -= UpdateBalanceText;
        }

        private void UpdateBalanceText()
        {
            balanceText.text = playerPurse.GetBalance().ToString() + "c";
        }
    }
}