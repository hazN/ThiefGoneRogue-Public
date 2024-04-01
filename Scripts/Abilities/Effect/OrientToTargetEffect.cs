using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Orient To Target Effect", menuName = "RPG/Abilities/Effects/Orient To Target")]
    public class OrientToTargetEffect : EffectStrategy
    {
        public override string GetTooltipInfo()
        {
            return "";
        }

        public override void StartEffect(AbilityData data, Action finished)
        {
            data.GetUser().transform.LookAt(data.GetTargetedPoint());
            finished();
        }
    }
}