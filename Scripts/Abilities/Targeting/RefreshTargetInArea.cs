using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Refresh Target In Area", menuName = "RPG/Abilities/Targeting/Refresh Target In Area")]
    public class RefreshTargetInArea : EffectStrategy
    {
        [SerializeField] private float radius = 5f;
        [SerializeField] private FilterStrategy filter = null;

        public override string GetTooltipInfo()
        {
            return "";
        }

        public override void StartEffect(AbilityData data, Action finished)
        {
            data.SetTargets(filter.Filter(GetTargetsInArea(data)));
            finished();
        }
        private IEnumerable<GameObject> GetTargetsInArea(AbilityData data)
        {
            RaycastHit[] hits = Physics.SphereCastAll(data.GetOriginalTarget(), radius, Vector3.up, 0);
            foreach (var hit in hits)
            {
                yield return hit.transform.gameObject;
            }
        }
    }
}