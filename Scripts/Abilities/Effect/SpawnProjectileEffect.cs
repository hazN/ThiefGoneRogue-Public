using RPG.Attributes;
using RPG.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Spawn Projectile Effect", menuName = "RPG/Abilities/Effects/Spawn Projectile")]
    public class SpawnProjectileEffect : EffectStrategy
    {
        [SerializeField] private Projectile projectileToSpawn;
        [SerializeField] private float damage;
        [SerializeField] private bool isRightHand;
        [SerializeField] private int maxProjectiles = 3;
        [SerializeField] private bool useTargetPoint = true;
        [SerializeField] private float degreesPerShot = 15;

        public override string GetTooltipInfo()
        {
            return $"Fires {maxProjectiles} projectiles for {damage} damage each";
        }

        public override void StartEffect(AbilityData data, Action finished)
        {
            Fighter fighter = data.GetUser().GetComponent<Fighter>();
            Vector3 spawnPosition = fighter.GetHandTransform(isRightHand).position;
            if (useTargetPoint)
            {
                SpawnProjectilesForTargetPoint(data, spawnPosition);
            }
            else
            {
                SpawnProjectilesForTargets(data, spawnPosition);
            }
            finished();
        }

        private void SpawnProjectilesForTargetPoint(AbilityData data, Vector3 spawnPosition)
        {
            for (int i = 0; i < maxProjectiles; i++)
            {
                // Calculate the rotation offset
                Quaternion rotationOffset = Quaternion.Euler(0, -degreesPerShot * (maxProjectiles - 1) * 0.5f + i * degreesPerShot, 0);

                // Calculate the offset vector based on rotation
                Vector3 offsetVector = rotationOffset * (data.GetTargetedPoint().position - spawnPosition);

                // Apply the offset to the target point
                Vector3 adjustedTarget = spawnPosition + offsetVector;
                adjustedTarget.y = data.GetTargetedPoint().position.y;

                // Spawn the projectile
                Projectile projectile = Instantiate(projectileToSpawn, spawnPosition, rotationOffset);
                projectile.SetTarget(adjustedTarget, data.GetUser(), damage);
            }
        }

        private void SpawnProjectilesForTargets(AbilityData data, Vector3 spawnPosition)
        {
            int projectilesSpawned = 0;
            foreach (GameObject target in data.GetTargets())
            {
                if (projectilesSpawned >= maxProjectiles) break;
                Health health = target.GetComponent<Health>();
                if (health == null && !health.IsDead()) continue;
                Projectile projectile = Instantiate(projectileToSpawn);
                projectile.transform.position = spawnPosition;
                projectile.SetTarget(health, data.GetUser(), damage);
                projectilesSpawned++;
            }
        }
    }
}