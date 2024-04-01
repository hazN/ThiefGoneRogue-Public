using RPG.Attributes;
using RPG.Core;
using RPG.Inventories;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Ability", menuName = "RPG/Abilities/Ability")]
    public class Ability : ActionItem
    {
        [SerializeField] private TargetingStrategy targetingStrategy;
        [SerializeField] private FilterStrategy[] filterStrategies;
        [SerializeField] private EffectStrategy[] effectStrategies;
        [SerializeField] private float cooldownTime = 0;
        [SerializeField] private float manaCost = 0;

        public override void Use(GameObject user)
        {
            // Check if user has enough mana
            Mana mana = user.GetComponent<Mana>();
            if (mana == null || mana.GetMana() < manaCost) return;

            // Check if ability is on cooldown
            if (user.GetComponent<CooldownStore>().GetTimeRemaining(this) > 0) return;

            // Create new ability data and start targeting
            AbilityData data = new AbilityData(user);
            // If not consumable, start action (potions should not be started as actions)
            if (!isConsumable())
            {
                user.GetComponent<ActionScheduler>().StartAction(data);
            }
            targetingStrategy.StartTargeting(data, () => TargetAcquired(data));
        }

        private void TargetAcquired(AbilityData data)
        {
            // If canceled, return
            if (data.IsCanceled()) return;

            // Consume resources
            if (!data.GetUser().GetComponent<Mana>().UseMana(manaCost)) return;
            data.GetUser().GetComponent<CooldownStore>().StartCooldown(this, cooldownTime);

            // Filter targets based on filter strategies

            foreach (FilterStrategy filterStrategy in filterStrategies)
            {
                data.SetTargets(filterStrategy.Filter(data.GetTargets()));
            }
            // Apply ability effects
            foreach (EffectStrategy effectStrategy in effectStrategies)
            {
                effectStrategy.StartEffect(data, EffectFinished);
            }
        }

        private void EffectFinished()
        {
        }

        public float GetManaCost()
        {
            return manaCost;
        }

        public float GetCooldownTime()
        {
            return cooldownTime;
        }

        public IEnumerable<EffectStrategy> GetEffectStrategies()
        {
            return effectStrategies;
        }
    }
}