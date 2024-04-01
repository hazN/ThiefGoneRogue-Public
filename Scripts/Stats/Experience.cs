using RPG.Saving;
using System;
using UnityEngine;

namespace RPG.Stats
{
    [RequireComponent(typeof(BaseStats))]
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] private float experiencePoints = 0;
        private BaseStats baseStats;
        private float expToLevelUp = -1;

        private void Awake()
        {
            baseStats = GetComponent<BaseStats>();
            baseStats.onLevelUp += UpdateExperienceToLevelUp;
        }
        private void UpdateExperienceToLevelUp()
        {
            expToLevelUp = baseStats.GetStat(Stat.ExperienceToLevelUp);
        }

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            float expBeforeTryLevelUp = experiencePoints;
            experiencePoints -= baseStats.TryLevelUp(experiencePoints);
        }
        public float GetExperience()
        {
            return experiencePoints;
        }
        public float GetMaxExperience()
        {
            if (expToLevelUp < 0)
            {
                UpdateExperienceToLevelUp();
            }
            return expToLevelUp;
        }
        public object CaptureState()
        {
            return experiencePoints;
        }
        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }
    }
}