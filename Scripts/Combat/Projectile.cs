using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 1f;
        [SerializeField] private float maxLifetime = 10f;
        [SerializeField] private float posthitLifetime = 2f;
        [SerializeField] private bool isHoming = false;
        [SerializeField] private GameObject hitEffect = null;
        [SerializeField] private GameObject[] destroyOnHit = null;
        [SerializeField] public UnityEvent onHit;
        private Health target = null;
        private Vector3 targetPoint;
        private GameObject instigator = null;
        private float damage = 0;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }
        private void Update()
        {
            if (target != null && this.isHoming && !target.IsDead())
                transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            SetTarget(instigator, damage, target);
        }
        public void SetTarget(Vector3 targetPoint, GameObject instigator, float damage)
        {
            SetTarget(instigator, damage, null, targetPoint);
        }
        public void SetTarget(GameObject instigator, float damage, Health target=null, Vector3 targetPoint=default)
        {
            this.target = target;
            this.targetPoint = targetPoint;
            this.instigator = instigator;
            this.damage = damage;

            Destroy(gameObject, maxLifetime);
        }
        private Vector3 GetAimLocation()
        {
            if (target == null) return targetPoint;

            var collider = target.GetComponent<CapsuleCollider>();
            if (!collider) return target.transform.position;
            return target.transform.position + Vector3.up * collider.height / 1.5f;
        }

        private void OnTriggerEnter(Collider other)
        {
            Health health = other.GetComponent<Health>();
            if (target != null && health != target) return;
            if (health == null || health.IsDead()) return;
            if (other.gameObject == instigator) return;
            CombatTarget combatTarget = other.GetComponent<CombatTarget>();
            if (combatTarget == null || !combatTarget.isActiveAndEnabled)
            {
                if (other.gameObject.tag != "Player" && instigator.tag != "Enemy")
                {
                    return;
                }
            }
            health.TakeDamage(instigator, damage);
            speed = 0f;

            onHit.Invoke();

            if (hitEffect != null)
            {
                // DestroyAfterEffect.cs handles destruction of hitEffect
                var hitVFX = Instantiate(hitEffect, transform.position, transform.rotation);
                hitVFX.transform.rotation *= Quaternion.Euler(0f, 180f, 0f);
            }
            foreach (GameObject destroyThis in destroyOnHit)
            {
                Destroy(destroyThis);
            }

            Destroy(gameObject, posthitLifetime);
        }
    }
}