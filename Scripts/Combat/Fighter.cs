using RPG.Utils;
using RPG.Attributes;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Stats;
using System.Collections.Generic;
using UnityEngine;
using RPG.Inventories;
using System;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        // Weapon Properties
        Equipment equipment;
        [SerializeField] private WeaponConfig defaultWeapon = null;

        private WeaponConfig currentWeaponConfig = null;
        private LazyValue<Weapon> currentWeapon;

        [SerializeField]
        private Transform leftHandTransform = null;

        [SerializeField]
        private Transform rightHandTransform = null;

        // Components
        private Mover mover;

        private ActionScheduler scheduler;
        private Animator anim;
        private BaseStats baseStats;

        // Enemy
        private Health target;

        // IAction Name
        public string Name => "Fighter";

        // Variables
        private float timeSinceLastAttack = Mathf.Infinity;

        private void Awake()
        {
            mover = GetComponent<Mover>();
            scheduler = GetComponent<ActionScheduler>();
            anim = GetComponent<Animator>();
            baseStats = GetComponent<BaseStats>();
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            equipment = GetComponent<Equipment>();
            if (equipment)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }
        }
        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            // Return if no target
            if (!target) return;
            if (target.IsDead())
            {
                target = FindNewTargetInRange();
                if (!target) return;
            }

            // Check if we are in range
            if (target && !GetIsInRange(target.transform))
            {
                mover.MoveTo(target.transform.position, 1f);
            }
            else
            {
                mover.Cancel();
                AttackBehaviour();
            }
        }

        private Health FindNewTargetInRange()
        {
            Health closestTarget = null;
            float closestDistance = Mathf.Infinity;
            foreach(Health health in FindAllTargetsInRange())
            {
                float distanceToTarget = Vector3.Distance(health.transform.position, transform.position);
                if (distanceToTarget < closestDistance)
                {
                    closestTarget = health;
                    closestDistance = distanceToTarget;
                }
            }
            return closestTarget;
        }
        private IEnumerable<Health> FindAllTargetsInRange()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, currentWeaponConfig.GetRange(), Vector3.up);
            foreach (RaycastHit hit in hits)
            {
                Health health = hit.transform.GetComponent<Health>();
                if (health == null || health.IsDead()) continue;
                if (health.gameObject == gameObject) continue;
                yield return health;
            }
        }
        private void UpdateWeapon()
        {
            var weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if (weapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
            else
            {
                EquipWeapon(weapon);
            }
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            return weapon.Spawn(rightHandTransform, leftHandTransform, anim);
        }
        public WeaponConfig GetCurrentWeapon()
        {
            return currentWeaponConfig;
        }

        public Health GetTarget()
        {
            return target;
        }
        public Transform GetHandTransform(bool isRightHand)
        {
            return isRightHand ? rightHandTransform : leftHandTransform;
        }
        private void AttackBehaviour()
        {
            // Look at target
            transform.LookAt(target.transform);
            if (timeSinceLastAttack >= currentWeaponConfig.GetCooldown())
            {
                // Set animation trigger and cooldown
                anim.ResetTrigger("stopAttack");
                anim.SetTrigger("attack");
                timeSinceLastAttack = 0f;
            }
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(targetTransform.position, transform.position) <= currentWeaponConfig.GetRange();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            // Return if target is invalid
            if (!combatTarget) return false;
            // Check if target is either able to be moved to or is in range
            if (!mover.CanMoveTo(combatTarget.transform.position) && !GetIsInRange(combatTarget.transform)) return false;
            // Check if target is alive
            return combatTarget.TryGetComponent(out Health test) ? !test.IsDead() : false;
        }

        // Start Attack through scheduler
        public void Attack(GameObject combatTarget)
        {
            scheduler.StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            anim.ResetTrigger("attack");
            anim.SetTrigger("stopAttack");
            target = null;
            GetComponent<Mover>().Cancel();
        }

        // Animation Event
        private void Hit()
        {
            if (target)
            {
                float damage = baseStats.GetStat(Stat.Damage);

                if (currentWeapon.value)
                {
                    currentWeapon.value.OnHit();
                }

                if (currentWeaponConfig.HasProjectile())
                {
                    currentWeaponConfig.Fire(rightHandTransform, leftHandTransform, target, gameObject, damage);
                }
                else
                {
                    target.TakeDamage(gameObject, damage);
                }
            }
        }

        private void Shoot()
        {
            Hit();
        }

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
    }
}