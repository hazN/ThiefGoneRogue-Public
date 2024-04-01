using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Trigger Animation Effect", menuName = "RPG/Abilities/Effects/Trigger Animation")]
    public class TriggerAnimationEffect : EffectStrategy
    {
        [SerializeField] string animationTrigger = null;

        public override string GetTooltipInfo()
        {
            return "";
        }

        public override void StartEffect(AbilityData data, Action finished)
        {
            Animator anim = data.GetUser().GetComponent<Animator>();
            if (anim == null) return;
            anim.SetTrigger(animationTrigger);
            finished();
        }
    }
}