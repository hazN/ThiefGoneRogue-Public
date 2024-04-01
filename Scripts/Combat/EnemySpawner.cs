using RPG.Attributes;
using RPG.Stats;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace RPG.Combat
{
    public class EnemySpawner : MonoBehaviour
    {
        [System.Serializable]
        public class SpawnInfo
        {
            public GameObject enemyPrefab;
            public float weight;
        }
        public enum SpawnType { SceneLoad, Wave, OnCommand}
        [BoxGroup("Enemy Settings")]
        [SerializeField] private SpawnType spawnType;
        [BoxGroup("Enemy Settings")]
        [SerializeField] private SpawnInfo[] enemyPrefabs;
        [BoxGroup("Enemy Settings")]
        [SerializeField] private int enemiesToSpawn = 1;
        [BoxGroup("Enemy Settings")]
        [SerializeField] private int maxAmount = 1;
        [BoxGroup("Enemy Settings")]
        [Tooltip("Total amount of enemies that can be spawned in this spawner, keep at 0 if there is no max.")]
        [SerializeField] private int totalMaxAmount = 0;
        [BoxGroup("Enemy Settings")]
        [Tooltip("Keep at 0 to have the level match the players level.")]
        [SerializeField] private Vector2 levelRange;
        [BoxGroup("Enemy Settings")]
        [SerializeField] private float spawnDelay = 1;
        [BoxGroup("Enemy Settings")]
        [SerializeField] private float spawnRadius = 1;
        [BoxGroup("Particle Settings")]
        [SerializeField] private GameObject spawnEffect = null;
        [BoxGroup("Particle Settings")]
        [SerializeField] private float spawnEffectDuration = 3;
        private int currentAmount = 0;
        private int totalAmount = 0;
        private float spawnTimer = Mathf.Infinity;
        private int playerLevel = 0;
        BaseStats playerStats;
        private void Start()
        {
            playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
            playerLevel = playerStats.GetLevel();
            playerStats.onLevelUp += UpdateLevel;
            if (spawnType == SpawnType.SceneLoad)
            {
                for (int i = 0; i < enemiesToSpawn; i++)
                {
                    StartCoroutine(SpawnEnemy());
                }
            }
        }
        private void Update()
        {
            spawnTimer += Time.deltaTime;
            if (spawnType == SpawnType.Wave)
            {
                if (totalAmount >= totalMaxAmount && totalMaxAmount != 0) return;
                if (spawnTimer >= spawnDelay && currentAmount < maxAmount)
                {
                    for (int i = 0; i < enemiesToSpawn; i++)
                    {
                        StartCoroutine(SpawnEnemy());
                        currentAmount++;
                        totalAmount++;
                        if (currentAmount >= maxAmount) break;
                    }
                    spawnTimer = 0;
                }
            }
        }
        public void SpawnWave(int numEnemiesToSpawn)
        {
            if (spawnType == SpawnType.OnCommand)
            {
                for (int i = 0; i < numEnemiesToSpawn; i++)
                {
                    StartCoroutine(SpawnEnemy());
                }
            }
        }
        public IEnumerator SpawnEnemy()
        { 
            // Use navmesh to spawn enemy at random location within spawn radius
            Vector3 spawnPosition = transform.position + UnityEngine.Random.insideUnitSphere * spawnRadius;
            NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, spawnRadius, NavMesh.AllAreas);
            if (spawnEffect != null)
            {
                GameObject effect = Instantiate(spawnEffect, hit.position, Quaternion.identity);
                Destroy(effect, spawnEffectDuration);
                yield return new WaitForSeconds(spawnEffectDuration);
            }
            if (enemyPrefabs.Length == 0) throw new Exception("No enemy prefabs assigned to spawner.");

            GameObject enemy = Instantiate(PickRandomEnemy(), hit.position, Quaternion.identity);
            // Either set enemy level to match player level or set it to a random level within the range
            if (levelRange == Vector2.zero) enemy.GetComponent<BaseStats>().ForceChangeLevel(playerLevel);
            else enemy.GetComponent<BaseStats>().ForceChangeLevel((int)UnityEngine.Random.Range(levelRange.x, levelRange.y));
            enemy.transform.parent = transform;
            enemy.GetComponent<Health>().onDie.AddListener(OnDeath);
            yield return null;
        }
        public IEnumerator SpawnEnemy(Vector3 position)
        {
            // Use navmesh to spawn enemy at random location within spawn radius
            Vector3 spawnPosition = position;
            NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, spawnRadius, NavMesh.AllAreas);
            if (spawnEffect != null)
            {
                GameObject effect = Instantiate(spawnEffect, hit.position, Quaternion.identity);
                Destroy(effect, spawnEffectDuration);
                yield return new WaitForSeconds(spawnEffectDuration);
            }
            if (enemyPrefabs.Length == 0) throw new Exception("No enemy prefabs assigned to spawner.");
            GameObject enemy = Instantiate(PickRandomEnemy(), hit.position, Quaternion.identity);
            // Either set enemy level to match player level or set it to a random level within the range
            if (levelRange == Vector2.zero) enemy.GetComponent<BaseStats>().ForceChangeLevel(playerLevel);
            else enemy.GetComponent<BaseStats>().ForceChangeLevel((int)UnityEngine.Random.Range(levelRange.x, levelRange.y));
            enemy.transform.parent = transform;
            enemy.GetComponent<Health>().onDie.AddListener(OnDeath);
            yield return null;
        }
        private void OnDeath()
        {
            currentAmount--;
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
            // Draw small sphere at spawn position
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
        private GameObject PickRandomEnemy()
        {
            if (enemyPrefabs.Length == 0) throw new Exception("No enemy prefabs assigned to spawner.");
            float totalWeight = 0;
            foreach (SpawnInfo enemy in enemyPrefabs)
            {
                totalWeight += enemy.weight;
            }
            float randomWeight = Random.Range(0, totalWeight);
            float currentWeight = 0;
            foreach (SpawnInfo enemy in enemyPrefabs)
            {
                currentWeight += enemy.weight;
                if (currentWeight >= randomWeight) return enemy.enemyPrefab;
            }
            return null;
        }
        private void UpdateLevel()
        {
            playerLevel = playerStats.GetLevel();
        }
    }

}