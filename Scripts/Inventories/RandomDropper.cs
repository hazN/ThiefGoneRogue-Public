using RPG.Stats;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper
    {
        [Tooltip("How far can the pickups be scattered from the dropper.")]
        [SerializeField] float scatterDistance = 1.5f;
        [Tooltip("List of all potential drops and their chance to drop. Note: chance is 0-100")]
        [SerializeField] DropLibrary dropLibrary;
        [SerializeField] int numberOfDrops = 1;
        const int ATTEMPTS = 30;

        public void RandomDrop()
        {
            BaseStats baseStats = GetComponent<BaseStats>();
            var drops = dropLibrary.GetRandomDrops(baseStats.GetLevel());
            foreach (var drop in drops)
            {
                DropItem(drop.item, drop.quantity);
            }
        }

        protected override Vector3 GetDropLocation()
        {
            // 30 attempts to find a suitable drop location
            for (int i = 0; i < ATTEMPTS; i++)
            {
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * scatterDistance;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }   
            return transform.position;
        }
    }
}