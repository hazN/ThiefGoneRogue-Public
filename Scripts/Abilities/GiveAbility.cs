using RPG.Inventories;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    public class GiveAbility : MonoBehaviour
    {
        [SerializeField] private Ability[] Abilities = null;
        [SerializeField] private bool giveOnStart = false;
        GameObject player;
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (giveOnStart)
            {
                Inventory inventory = player.GetComponent<Inventory>();
                foreach (Ability ability in Abilities)
                {
                    inventory.AddToFirstEmptySlot(ability, 1);
                }
            }
        }
        public void Give()
        {
            Inventory inventory = player.GetComponent<Inventory>();
            foreach (Ability ability in Abilities)
            {
                inventory.AddToFirstEmptySlot(ability, 1);
            }
        }
    }
}