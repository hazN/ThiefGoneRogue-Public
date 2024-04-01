using RPG.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Buff Effect", menuName = "RPG/Abilities/Effects/Buff Effect")]
    public class ApplyBuffEffect : EffectStrategy
    {
        [SerializeField] private Buff buff;
        public override string GetTooltipInfo()
        {
            return $"+{(buff.GetBuffPercentageModifier() * 100f):N1}% {buff.GetBuffType()}\n";
        }

        public override void StartEffect(AbilityData data, Action finished)
        {
            foreach (var target in data.GetTargets())
            {
                BuffStore store = target.GetComponent<BuffStore>();
                if (store == null) continue;
                store.AddBuff(buff);
            }
        }
    }
}