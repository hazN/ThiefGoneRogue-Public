using RPG.Core;
using RPG.Saving;
using System;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        [Range(1, 99)][SerializeField] private int level = 1;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private Progression progression;
        [SerializeField] private GameObject levelUpParticleEffect;
        [SerializeField] private bool shouldUseModifiers = false;
        public event Action onLevelUp;
        public float GetStat(Stat stat)
        {
            // Item 1 = Absolute Modifier, Item 2 = Percentage Modifier
            var modifiers = GetTotalModifiers(stat);
            return (GetBaseStat(stat) + modifiers.Item1) * (1f + modifiers.Item2);
        }
        public (float, float) GetModifiers(Stat stat)
        {
            return GetTotalModifiers(stat);
        }
        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, level);
        }

        public int GetLevel()
        {
            return level;
        }

        public float TryLevelUp(float experience)
        {
            float expTolevelup = GetStat(Stat.ExperienceToLevelUp);
            if (expTolevelup <= experience)
            {
                level++;
                onLevelUp();
                LevelUpEffect();
                return expTolevelup;
            }
            return 0;
        }
        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, gameObject.transform);
        }
        // Item1: Absolute Modifier (e.g. +5) Item2: Percentage Modifier (e.g. +5%)
        private (float, float) GetTotalModifiers(Stat stat)
        {
            if (!shouldUseModifiers) return (0, 0);
            float absoluteTotal = 0;
            float percentageTotal = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach ((float absoluteModifier, float percentageModifier) in provider.GetModifiers(stat))
                {
                    absoluteTotal += absoluteModifier;
                    percentageTotal += percentageModifier;
                }
            }
            return (absoluteTotal, percentageTotal / 100f);
        }
        public object CaptureState()
        {
            return level;
        }

        public void RestoreState(object state)
        {
            level = (int)state;
        }

        public bool? Evaluate(EPredicate predicate, string[] parameters)
        {
            if (predicate == EPredicate.HasLevel)
            {
                if (int.TryParse(parameters[0], out int testLevel))
                {
                    return level >= testLevel;
                }
            }
            return null;
        }
        public void ForceChangeLevel(int newLevel)
        {
            level = newLevel;
            onLevelUp?.Invoke();
        }
    }
}