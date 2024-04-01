using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [Range(0f, 1f)]
        [SerializeField] private float patrolSpeedFraction = 0.2f;

        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] private float suspicionTime = 3f;
        [SerializeField] private float aggroTime = 5f;
        [SerializeField] private float shoutDistance = 5f;
        private bool canShout = true;

        private GameObject player;
        private Fighter fighter;
        private Health health;
        private Mover mover;

        // Guard
        [SerializeField] private PatrolPath patrolPath;

        [SerializeField] private float waypointTolerance = 1f;
        private int currentWaypointIndex = 0;
        private Vector3 guardPosition;
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private float timeSinceAggravated = Mathf.Infinity;
        private bool lookedAround = false;

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            guardPosition = transform.position;
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void Start()
        {
            player = GameObject.FindWithTag("Player");
            if (patrolPath)
                currentWaypointIndex = patrolPath.GetClosestWaypoint(transform.position);
            //guardPosition.ForceInit();
        }

        private void Update()
        {
            if (health.IsDead()) return;

            if (IsAggravated() && fighter.CanAttack(player))
            {
                lookedAround = false;
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                canShout = true;
                SuspicionBehaviour();
            }
            else
            {
                canShout = true;
                lookedAround = false;
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        public void Aggravate()
        {
            if (!(timeSinceAggravated < aggroTime))
            {
                timeSinceAggravated = 0;
            }
        }
        public void Reset()
        {
            timeSinceLastSawPlayer = Mathf.Infinity;
            timeSinceArrivedAtWaypoint = Mathf.Infinity;
            timeSinceAggravated = Mathf.Infinity;
            if (patrolPath)
                currentWaypointIndex = patrolPath.GetClosestWaypoint(transform.position);
            GetComponent<NavMeshAgent>().Warp(guardPosition);
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggravated += Time.deltaTime;
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
            AggravateNearbyEnemies();
        }

        private void SuspicionBehaviour()
        {
            if (lookedAround == false && GetComponent<NavMeshAgent>().velocity.magnitude <= 0.1f)
            {
                GetComponent<Animator>().SetTrigger("lookAround");
                lookedAround = true;
            }
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition;
            if (patrolPath)
            {
                if (AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }
            if (!patrolPath || timeSinceArrivedAtWaypoint > patrolPath.GetWaypointDwellTime(currentWaypointIndex))
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }

        private bool AtWaypoint()
        {
            return Vector3.Distance(transform.position, GetCurrentWaypoint()) < waypointTolerance;
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private void GuardBehaviour()
        {
            mover.StartMoveAction(guardPosition, patrolSpeedFraction);
        }

        private void AggravateNearbyEnemies()
        {
            // Make sure we only shout once
            // Otherwise enemies loop shout and never lose aggro
            if (canShout)
            {
                canShout = false;
            }
            else return;

            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0f);
            foreach (RaycastHit hit in hits)
            {
                AIController enemy = hit.collider.GetComponent<AIController>();
                if (enemy == null || enemy == this) continue;
                enemy.Aggravate();
            }
        }

        private bool IsAggravated()
        {
            bool isAggravated = timeSinceAggravated < aggroTime;
            bool isInRange = Vector3.Distance(player.transform.position, transform.position) < chaseDistance;
            return isInRange || isAggravated;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}