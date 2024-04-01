using System;
using System.Collections;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Self Targeting", menuName = "RPG/Abilities/Targeting/Self")]
    public class SelfTargeting : TargetingStrategy
    {
        public override void StartTargeting(AbilityData data, Action finished)
        {
            data.SetTargets(new GameObject[] { data.GetUser() });
            data.SetTargetedPoint(data.GetUser().transform);
            data.SetOriginalTarget(data.GetUser().transform.position);
            finished();
        }
    }
}