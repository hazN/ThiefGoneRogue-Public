using RPG.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public class TraitStore : MonoBehaviour, IModifierProvider, ISaveable
    {
        [SerializeField] TraitBonus[] bonusConfig;
        [System.Serializable]
        class TraitBonus
        {
            public Trait trait;
            public Stat stat;
            public float additiveBonusPerPoint = 0;
            public float percentBonusPerPoint = 0;
        }
        Dictionary<Trait, int> assignedPoints = new Dictionary<Trait, int>();
        Dictionary<Trait, int> stagedPoints = new Dictionary<Trait, int>();

        Dictionary<Stat, Dictionary<Trait, float>> additiveBonusCache;
        Dictionary<Stat, Dictionary<Trait, float>> percentBonusCache;

        public event Action onCommit;

        private void Awake()
        {
            additiveBonusCache = new Dictionary<Stat, Dictionary<Trait, float>>();
            percentBonusCache = new Dictionary<Stat, Dictionary<Trait, float>>();
            foreach (TraitBonus bonus in bonusConfig)
            {
                if (!additiveBonusCache.ContainsKey(bonus.stat))
                {
                    additiveBonusCache[bonus.stat] = new Dictionary<Trait, float>();
                }
                if (!percentBonusCache.ContainsKey(bonus.stat))
                {
                    percentBonusCache[bonus.stat] = new Dictionary<Trait, float>();
                }
                additiveBonusCache[bonus.stat][bonus.trait] = bonus.additiveBonusPerPoint;
                percentBonusCache[bonus.stat][bonus.trait] = bonus.percentBonusPerPoint;
            }
        }
        public int GetProposedPoints(Trait trait)
        {
            return GetPoints(trait) + GetStagedPoints(trait);
        }
        public int GetPoints(Trait trait)
        {
            return assignedPoints.ContainsKey(trait) ? assignedPoints[trait] : 0;
        }
        public int GetStagedPoints(Trait trait)
        {
            return stagedPoints.ContainsKey(trait) ? stagedPoints[trait] : 0;
        }
        public void AssignPoints(Trait trait, int points)
        {
            if (!CanAssignPoints(trait, points)) return;
            stagedPoints[trait] = GetStagedPoints(trait) + points;
        }
        public bool CanAssignPoints(Trait trait, int points)
        {
            if (points + GetStagedPoints(trait) < 0) return false;
            if (GetUnassignedPoints() < points) return false;
            return true;
        }
        public int GetUnassignedPoints()
        {
            return GetAssignablePoints() - GetTotalPoints();
        }
        public int GetTotalPoints()
        {
            int total = 0;
            foreach (int points in assignedPoints.Values)
            {
                total += points;
            }
            foreach (int points in stagedPoints.Values)
            {
                total += points;
            }
            return total;
        }

        public void Commit()
        {
            foreach (Trait trait in stagedPoints.Keys)
            {
                assignedPoints[trait] = GetProposedPoints(trait);
            }
            stagedPoints.Clear();
            onCommit?.Invoke();
        }
        public int GetAssignablePoints()
        {
            return (int)GetComponent<BaseStats>().GetStat(Stat.TotalTraitPoints);
        }

        public IEnumerable<(float absoluteModifier, float percentModifier)> GetModifiers(Stat stat)
        {
            if (!percentBonusCache.ContainsKey(stat)) yield break;
            foreach (Trait trait in percentBonusCache[stat].Keys)
            {
                yield return (0, percentBonusCache[stat][trait] * GetPoints(trait));
            }
        }

        public object CaptureState()
        {
            return assignedPoints;
        }

        public void RestoreState(object state)
        {
            assignedPoints = new Dictionary<Trait, int>((Dictionary<Trait, int>)state);
        }
    }
}