using RPG.Combat;
using System;
using System.Collections;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Equip Weapon Effect", menuName = "RPG/Abilities/Effects/Equip Weapon")]
    public class EquipWeaponEffect : EffectStrategy
    {
        [SerializeField] private WeaponConfig weaponConfig;
        [SerializeField] private string animationToEndOn;
        private WeaponConfig oldWeapon;
        private AnimationClip currentAnimation;

        public override string GetTooltipInfo()
        {
            return "";
        }

        public override void StartEffect(AbilityData data, Action finished)
        {
            Fighter fighter = data.GetUser().GetComponent<Fighter>();
            oldWeapon = fighter.GetCurrentWeapon();
            fighter.EquipWeapon(weaponConfig);
            Animator anim = data.GetUser().GetComponent<Animator>();
            data.StartCoroutine(equipOldWeapon(data));
            finished();
        }

        private IEnumerator equipOldWeapon(AbilityData data)
        {
            while (true)
            {
                var anim = data.GetUser().GetComponent<Animator>();
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName(animationToEndOn))
                {
                    // wait half a second
                    yield return new WaitForSeconds(0.75f);
                    Fighter fighter = data.GetUser().GetComponent<Fighter>();
                    fighter.EquipWeapon(oldWeapon);
                    break;
                }

                yield return null;
            }
        }
    }
}