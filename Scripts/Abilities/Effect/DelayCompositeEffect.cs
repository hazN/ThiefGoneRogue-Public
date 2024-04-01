using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Delay Composite Effect", menuName = "RPG/Abilities/Effects/Delay Composite Effect")]

    public class DelayCompositeEffect : EffectStrategy
    {
        [SerializeField] private float delay = 0f;
        [SerializeField] private EffectStrategy[] delayedEffects = null;
        [SerializeField] private bool abortIfCanceled = false;
        public override void StartEffect(AbilityData data, Action finished)
        {
            data.StartCoroutine(DelayedEffect(data, finished));
        }

        private IEnumerator DelayedEffect(AbilityData data, Action finished)
        {
            yield return new WaitForSeconds(delay);
            if (abortIfCanceled && data.IsCanceled()) yield break;
            foreach (var effect in delayedEffects)
            {
                effect.StartEffect(data, finished);
            }
        }

        public IEnumerable<EffectStrategy> GetEffects()
        {
            return delayedEffects;
        }

        public override string GetTooltipInfo()
        {
            string tooltip = "";
            foreach (EffectStrategy effect in delayedEffects)
            {
                if (effect.GetTooltipInfo() != "")
                tooltip += effect.GetTooltipInfo() + "\n";
            }
            return tooltip;
        }
    }
}