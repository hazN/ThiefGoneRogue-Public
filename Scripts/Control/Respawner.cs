using Cinemachine;
using RPG.Attributes;
using RPG.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control
{
    public class Respawner : MonoBehaviour
    {
        [SerializeField] private Transform respawnLocation;
        [SerializeField] private float respawnTime = 2f;
        [SerializeField] private float fadeTime = 0.2f;
        private void Awake()
        {
            GetComponent<Health>().onDie.AddListener(Respawn);
        }
        private void Start()
        {
            if (GetComponent<Health>().IsDead())
            {
                Respawn();
            }
        }
        private void Respawn()
        {
            StartCoroutine(RespawnRoutine());
        }
        private IEnumerator RespawnRoutine()
        {
            var savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();
            yield return new WaitForSeconds(respawnTime);
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeTime);
            RespawnPlayer();
            ResetEnemies();
            savingWrapper.Save();
            yield return fader.FadeIn(fadeTime);
        }
        private void RespawnPlayer()
        {
            Vector3 positionDelta = respawnLocation.position - transform.position;
            GetComponent<NavMeshAgent>().Warp(respawnLocation.position);
            Health health = GetComponent<Health>();
            health.Revive();
            ICinemachineCamera vcam = FindObjectOfType<CinemachineBrain>().ActiveVirtualCamera;
            if (vcam.Follow == transform)
            {
                vcam.OnTargetObjectWarped(transform, positionDelta);
            }
        }
        private void ResetEnemies()
        {
            foreach (AIController enemy in FindObjectsOfType<AIController>())
            {
                Health health = enemy.GetComponent<Health>();
                if (health)
                {
                    if (health.IsDead()) continue;
                    enemy.Reset();
                    health.Heal(health.GetMaxHealth() * 0.25f);
                }
            }
        }
    }
}