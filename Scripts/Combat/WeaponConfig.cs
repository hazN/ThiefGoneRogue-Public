using RPG.Attributes;
using RPG.Inventories;
using RPG.Stats;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/New Weapon", order = 0)]
    public class WeaponConfig : EquipableItem, IModifierProvider
    {
        [SerializeField, AssetsOnly, InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private AnimatorOverrideController animatorOverride = null;

        [SerializeField, MinValue(0f), Range(0f, 50f)]
        private float damage = 5f;

        [SerializeField, MinValue(0f), Range(0f, 100f)]
        private float percentageBonus = 0f;

        [SerializeField, MinValue(0f), Range(0f, 20f)]
        private float range = 2f;

        [SerializeField, MinValue(0f), Range(0f, 10f)]
        private float cooldown = 1f;

        [SerializeField]
        [LabelWidth(100)]
        [HorizontalGroup("WeaponSettings", Width = 0.5f)]
        [VerticalGroup("WeaponSettings/Row2")]
        private bool isRightHanded = true;

        [SerializeField]
        [VerticalGroup("WeaponSettings/Row2")]
        [HideLabel]
        private Projectile projectile = null;

        [SerializeField]
        private Weapon equippedPrefab = null;
        [SerializeField] private List<Modifier> modifiers = new List<Modifier>();

        private const string weaponName = "Weapon";

        public Weapon GetWeaponPrefab() => equippedPrefab;

        public AnimatorOverrideController GetAnimatorOverride() => animatorOverride;

        public float GetDamage() => damage;
        public float GetPercentageBonus() => percentageBonus;
        public float GetRange() => range;

        public float GetCooldown() => cooldown;
        public void SetDamage(float damage)
        {
            this.damage = damage;
        }
        public void SetPercentageBonus(float percentageBonus)
        {
            this.percentageBonus = percentageBonus;
        }
        public void SetRange(float range)
        {
            this.range = range;
        }
        public void SetCooldown(float cooldown)
        {
            this.cooldown = cooldown;
        }
        public void SetProjectile(Projectile projectile)
        {
            this.projectile = projectile;
        }
        public void SetWeapon(Weapon weapon) 
        {
            equippedPrefab = weapon;
        }
        public void AddModifier(Stat stat, float additive, float percentage)
        {
            Modifier modifier = new Modifier(stat, additive, percentage);
            modifiers.Add(modifier);
        }
        public void RemoveModifier(Modifier modifier)
        {
            modifiers.Remove(modifier);
        }
        public IEnumerable<Modifier> GetModifiers()
        {
            return modifiers;
        }
        public void SetRightHanded(bool isRightHanded)
        {
            this.isRightHanded = isRightHanded;
        }
        public bool IsRightHanded()
        {
            return isRightHanded;
        }
        public void SetFromConfig(WeaponConfig config)
        {
            damage = config.GetDamage();
            percentageBonus = config.GetPercentageBonus();
            range = config.GetRange();
            cooldown = config.GetCooldown();
            projectile = config.projectile;
            equippedPrefab = config.GetWeaponPrefab();
            animatorOverride = config.GetAnimatorOverride();
        }
        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);
            Weapon weapon = null;
            if (equippedPrefab != null)
            {
                Transform handTransform = GetHandTransform(rightHand, leftHand);
                weapon = Instantiate(equippedPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
                animator.runtimeAnimatorController = animatorOverride;
            else if (overrideController != null) 
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;

            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (!oldWeapon)
                oldWeapon = leftHand.Find(weaponName);
            if (!oldWeapon) return;

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetHandTransform(Transform rightHand, Transform leftHand)
        {
            return isRightHanded ? rightHand : leftHand;
        }

        public void Fire(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float finalDamage)
        {
            if (!HasProjectile()) return;
            Projectile projectileInstance = Instantiate(projectile, GetHandTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, finalDamage);
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }
        public IEnumerable<(float absoluteModifier, float percentModifier)> GetModifiers(Stat stat)
        {
            foreach (var modifier in modifiers)
            {
                if (modifier.stat == stat)
                {
                    yield return (modifier.additiveValue, modifier.percentageValue);
                }
                if (stat == Stat.Damage)
                {
                    yield return (damage, percentageBonus);
                }
            }
        }
        public IEnumerable<Modifier> GetAllModifiers()
        {
            foreach (var modifier in modifiers)
            {
                yield return modifier;
            }
        }
    }
}