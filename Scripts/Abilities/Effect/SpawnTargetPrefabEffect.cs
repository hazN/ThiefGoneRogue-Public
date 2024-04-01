using RPG.Abilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Spawn Target Prefab Effect", menuName = "RPG/Abilities/Effects/Spawn Target Prefab Effect", order = 0)]
    public class SpawnTargetPrefabEffect : EffectStrategy
    {
        [SerializeField] private GameObject prefabToSpawn;
        [SerializeField] private bool shouldUseTargetPoint;
        [SerializeField] private float destroyDelay = -1;
        [SerializeField] private float groundOffset = 0.3f;
        public override void StartEffect(AbilityData data, Action finished)
        {
            data.StartCoroutine(EffectCoroutine(data, finished));
        }

        private GameObject SpawnPrefab(Transform transform)
        {
            GameObject spawnedObject = Instantiate(prefabToSpawn, transform.position, transform.rotation);
            spawnedObject.transform.localScale = transform.localScale / 2f;
            spawnedObject.transform.position = new Vector3(spawnedObject.transform.position.x, spawnedObject.transform.position.y + groundOffset, spawnedObject.transform.position.z);
            return spawnedObject;
        }
        private IEnumerator EffectCoroutine(AbilityData data, Action finished)
        {
            GameObject prefab = null;
            if (shouldUseTargetPoint)
            {
                prefab = SpawnPrefab(data.GetTargetedPoint());
            }
            else
            {
                // TODO: Spawn at each target
            }
            if (destroyDelay > 0)
            {
                yield return new WaitForSeconds(destroyDelay);
                if (prefab != null)
                Destroy(prefab, destroyDelay);

            }
            finished();
        }

        public override string GetTooltipInfo()
        {
            return "";
        }
    }
}