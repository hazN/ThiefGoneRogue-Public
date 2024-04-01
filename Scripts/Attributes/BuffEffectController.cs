using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class BuffEffectController : MonoBehaviour
    {
        [SerializeField] private GameObject poisonEffect;
        [SerializeField] private GameObject burnEffect;
        [SerializeField] private GameObject freezeEffect;

        private Dictionary<Buff.Buffs, bool> activeEffects = new Dictionary<Buff.Buffs, bool>();
        public void SetBuff(bool active, Buff.Buffs buff)
        {
            if (activeEffects.ContainsKey(buff))
            {
                activeEffects[buff] = active;
            }
            else
            {
                activeEffects.Add(buff, active);
            }
            UpdateEffects();
        }

        private void UpdateEffects()
        {
            poisonEffect.SetActive(activeEffects.ContainsKey(Buff.Buffs.Poison) && activeEffects[Buff.Buffs.Poison]);
            burnEffect.SetActive(activeEffects.ContainsKey(Buff.Buffs.Burn) && activeEffects[Buff.Buffs.Burn]);
            freezeEffect.SetActive(activeEffects.ContainsKey(Buff.Buffs.Freeze) && activeEffects[Buff.Buffs.Freeze]);
        }
    }
}