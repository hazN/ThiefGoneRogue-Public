using RPG.Attributes;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] private float maxSpeed = 6f;
        [SerializeField] private float maxTravelDistance = 40f;
        private NavMeshAgent agent;
        private Animator anim;
        private ActionScheduler scheduler;
        private float previousSpeed = 0f;
        public string Name => "Mover";

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
            scheduler = GetComponent<ActionScheduler>();
            GetComponent<BuffStore>().buffEvent.AddListener(UpdateSpeed);
        }
        private void Start()
        {
            UpdateSpeed();
            var traitStore = GetComponent<TraitStore>();
            if (traitStore != null)
                traitStore.onCommit += UpdateSpeed;
        }
        private void Update()
        {
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedModifier)
        {
            scheduler.StartAction(this);
            MoveTo(destination, speedModifier);
        }
        public bool CanMoveTo(Vector3 target)
        {
            NavMeshPath path = new NavMeshPath();
            // Check if the path is valid
            if (NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path))
            {
                // Return false if the path is incomplete or too long
                if (path.status != NavMeshPathStatus.PathComplete)
                    return false;
                if (GetPathLength(path) > maxTravelDistance)
                    return false;
            } // Return false if the path is invalid
            else return false;
            // Otherwise return true
            return true;
        }

        public void MoveTo(Vector3 destination, float speedModifier)
        {
            agent.destination = destination;
            agent.speed = maxSpeed * Mathf.Clamp01(speedModifier);
            agent.isStopped = false;
        }

        public void Cancel()
        {
            agent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            // Get the velocity of the agent and send to animator for the blend tree
            Vector3 velocity = agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            anim.SetFloat("forwardSpeed", speed);
        }
        private float GetPathLength(NavMeshPath path)
        {
            float totalLength = 0f;
            if (path.corners.Length < 2) return totalLength;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                totalLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return totalLength;
        }
        // Animation Event
        private void FootL()
        {
            Footstep();
        }

        // Animation Event
        private void FootR()
        {
            Footstep();
        }
        private void Footstep()
        {

        }
        private void UpdateSpeed()
        {
            var baseStats = GetComponent<BaseStats>();
            if (baseStats == null) return;
            float speed = baseStats.GetStat(Stat.Speed);
            if (speed != 0)
            {
                maxSpeed = speed;
            }
            float freeze = GetComponent<BuffStore>().GetBuffValue(Buff.Buffs.Freeze);
            if (freeze != 0)
            {
                maxSpeed -= maxSpeed * freeze;
            }
        }
        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            agent.enabled = false;
            transform.position = position.ToVector3();
            agent.enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}