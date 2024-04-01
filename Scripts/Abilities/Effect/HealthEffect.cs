using RPG.Attributes;
using RPG.Stats;
using System;
using System.Collections;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Health Effect", menuName = "RPG/Abilities/Effects/HealthEffect", order = 0)]
    public class HealthEffect : EffectStrategy
    {
        [SerializeField] private float healthChange;
        [SerializeField][Range(-100, 100)] private float percentageHealthChange;
        [SerializeField] private bool shouldUseModifiers = true;

        public override void StartEffect(AbilityData data, Action effectFinisher)
        {
            HealthChange(data);
            effectFinisher();
        }

        private void HealthChange(AbilityData data)
        {
            // If negative, then its damage
            if (healthChange < 0)
            {
                float damage = 0;
                if (shouldUseModifiers) {
                    damage = data.GetUser().GetComponent<BaseStats>().GetStat(Stat.Damage);
                }
                float finalDamage = damage + -healthChange;
                finalDamage += finalDamage * (percentageHealthChange / 100);
                foreach (GameObject target in data.GetTargets())
                {
                    Health targetHealth = target.GetComponent<Health>();
                    if (targetHealth == null) continue;
                    targetHealth.TakeDamage(data.GetUser(), finalDamage);
                }
            } // If positive, then its healing
            else
            {
                foreach (GameObject target in data.GetTargets())
                {
                    Health targetHealth = target.GetComponent<Health>();
                    if (targetHealth == null) continue;
                    float finalHeal = healthChange + (targetHealth.GetMaxHealth() * (percentageHealthChange / 100));
                    targetHealth.Heal(finalHeal);
                }
            }
        }
        public override string GetTooltipInfo()
        {
            string tooltip = "";
            if (healthChange < 0)
            {
                tooltip += $"+{-healthChange} damage";
                if (percentageHealthChange != 0)
                {
                    tooltip += $"\n+{-percentageHealthChange}% damage";
                }
            }
            else
            {
                if (healthChange != 0)
                    tooltip += $"+{healthChange} health";
                if (percentageHealthChange != 0)
                {
                    tooltip += $"\n+{percentageHealthChange}% health";
                }
            }
            return tooltip;
        }
    }
}