using RPG.Sounds;
using System;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Sound Effect", menuName = "RPG/Abilities/Effects/Sound Effect")]
    public class SoundEffect : EffectStrategy
    {
        [SerializeField] private SoundEffectSO soundEffect = null;

        public override string GetTooltipInfo()
        {
            return "";
        }

        public override void StartEffect(AbilityData data, Action finished)
        {
            // play sound effect
            soundEffect.Play();
            finished();
        }
    }
}