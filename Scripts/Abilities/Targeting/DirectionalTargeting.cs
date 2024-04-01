using RPG.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Directional Targeting", menuName = "RPG/Abilities/Targeting/Directional")]
    public class DirectionalTargeting : TargetingStrategy
    {
        [SerializeField] private LayerMask excludedLayers;
        [SerializeField] private float groundOffset = 0.5f;
        public override void StartTargeting(AbilityData data, Action finished)
        {
            RaycastHit hit;
            LayerMask includedLayers = ~excludedLayers;
            Ray ray = PlayerController.GetMouseRay();
            if (Physics.Raycast(ray, out hit, 1000, includedLayers))
            {
                // make new transform 
                Transform target;
                target = new GameObject().transform;
                target.position = hit.point + ray.direction * groundOffset / ray.direction.y;
                data.SetTargetedPoint(target);
                data.SetOriginalTarget(target.position);
                Destroy(target.gameObject, 10);
            }
            finished();
        }
    }
}