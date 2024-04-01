using RPG.Attributes;
using System;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Mana Effect", menuName = "RPG/Abilities/Effects/ManaEffect", order = 0)]
    public class ManaEffect : EffectStrategy
    {
        [SerializeField] private float manaChange;
        [SerializeField][Range(-100, 100)] private float percentageManaChange;

        public override void StartEffect(AbilityData data, Action effectFinisher)
        {
            ManaChange(data);
            effectFinisher();
        }

        private void ManaChange(AbilityData data)
        {
            foreach (GameObject target in data.GetTargets())
            {
                Mana targetMana = target.GetComponent<Mana>();
                if (targetMana == null) continue;
                float finalMana = manaChange + (targetMana.GetMaxMana() * (percentageManaChange / 100));
                targetMana.AddMana(finalMana);
            }
        }

        public override string GetTooltipInfo()
        {
            string tooltip = "";
            if (manaChange != 0)
            { 
                tooltip += $"+{manaChange} mana"; 
            }
            if (percentageManaChange != 0)
            {
                tooltip += $"\n+{percentageManaChange}% mana";
            }

            return tooltip;
        }
    }
}