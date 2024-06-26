﻿using System.Collections;
using UnityEngine;

namespace RPG.Combat
{
    public class AggroGroup : MonoBehaviour
    {
        [SerializeField] private Fighter[] fighters;
        [SerializeField] private bool activateOnStart = false;
        private void Start ()
        {
            Activate(activateOnStart);
        }
        public void Activate(bool shouldActivate)
        {
            foreach (var fighter in fighters)
            {
                fighter.gameObject.tag = shouldActivate ? "Enemy" : "Untagged";
                CombatTarget target = fighter.GetComponent<CombatTarget>();
                if (target != null)
                {
                    target.enabled = shouldActivate;
                }
                fighter.enabled = shouldActivate;
            }
        }
    }
}