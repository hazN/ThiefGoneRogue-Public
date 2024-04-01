using System;
using System.Collections;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Enemy Targeting", menuName = "RPG/Abilities/Targeting/Enemy")]
    public class EnemyTargeting : TargetingStrategy
    {
        [SerializeField] private float range = 10f;
        public override void StartTargeting(AbilityData data, Action finished)
        {
            // Get Player position
            GameObject player = GameObject.FindWithTag("Player");
            // Check if player is in range
            if (Vector3.Distance(data.GetUser().transform.position, player.transform.position) > range)
            {
                data.SetTargets(null);
                finished();
                return;
            }
            // Set player as target
            data.SetTargets(new GameObject[] { player });
            data.SetTargetedPoint(player.transform);
            data.SetOriginalTarget(player.transform.position);
            finished();
        }
    }
}