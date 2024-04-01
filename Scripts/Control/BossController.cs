using RPG.Abilities;
using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control
{
    public class BossController : MonoBehaviour
    {
        [SerializeField] private float chaseDistance = 5f;

        private GameObject player;
        private Fighter fighter;
        private Health health;
        private Mover mover;
        [SerializeField] private Ability[] abilities = null;
        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            player = GameObject.FindWithTag("Player");
        }
        void Update()
        {
            if (health.IsDead()) return;
            if (IsAggravated() && fighter.CanAttack(player))
            {
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            UseAbilities();
            fighter.Attack(player);
        }
        private bool UseAbilities()
        {
            foreach (Ability ability in abilities)
            {
                ability.Use(this.gameObject);
            }
            return false;
        }
        private bool IsAggravated()
        {
            bool isInRange = Vector3.Distance(player.transform.position, transform.position) < chaseDistance;
            return isInRange;
        }

    }
}