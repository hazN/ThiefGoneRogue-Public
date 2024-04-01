using System;
using System.Collections;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Repeat Composite Effect", menuName = "RPG/Abilities/Effects/RepeatCompositeEffect", order = 0)]
    public class RepeatCompositeEffect : EffectStrategy
    {
        [Min(0)][SerializeField] private int duration = 5;
        [Min(0)][SerializeField] private int tickTime = 1;
        [SerializeField] private EffectStrategy[] repeatEffects = null;

        public override void StartEffect(AbilityData data, Action finished)
        {
            data.StartCoroutine(RepeatEffect(data, finished));
        }

        private IEnumerator RepeatEffect(AbilityData data, Action finished)
        {
            for (int i = 0; i < duration; i++)
            {
                foreach (EffectStrategy effect in repeatEffects)
                {
                    effect.StartEffect(data, () => { });
                }
                yield return new WaitForSeconds(tickTime);
            }
            finished();
        }

        public override string GetTooltipInfo()
        {
            string tooltip = "";
            // Get total time
            int totalTime = duration * tickTime;
            foreach (EffectStrategy effect in repeatEffects)
            {
                if (effect.GetTooltipInfo() != "")
                {
                    tooltip += $"{effect.GetTooltipInfo()} for {totalTime} seconds\n";
                }
            }
            return tooltip;
        }
    }
}