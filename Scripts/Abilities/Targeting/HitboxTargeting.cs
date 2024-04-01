using RPG.Control;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Hitbox Targeting", menuName = "RPG/Abilities/Targeting/Hitbox")]
    public class HitboxTargeting : TargetingStrategy
    {
        [SerializeField] private LayerMask excludedLayers;
        [SerializeField] private float groundOffset = 0.5f;
        [SerializeField] private float radius = 2f;
        public override void StartTargeting(AbilityData data, Action finished)
        {
            RaycastHit hit;
            LayerMask includedLayers = ~excludedLayers;
            Ray ray = PlayerController.GetMouseRay();
            if (Physics.Raycast(ray, out hit, 1000, includedLayers))
            {
                // Get direction of ray from player to mouse
                Vector3 direction = hit.point - data.GetUser().transform.position;
                // Do a spherecast in that direction
                data.SetTargets(GetGameObjectsInRadius(data.GetUser().transform.position + direction * 1.2f));
            }
            finished();
        }
        private IEnumerable<GameObject> GetGameObjectsInRadius(Vector3 point)
        {
            RaycastHit[] hits = Physics.SphereCastAll(point, radius, Vector3.up, 0);
            foreach (var hit in hits)
            {
                yield return hit.transform.gameObject;
            }
        }
    }
}