using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : SerializedScriptableObject
    {
        [SerializeField] private const int maxLevel = 99;

        [Title("Character Classes")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "GetClassName()")]
        [SerializeField]
        private ProgressionCharacterClass[] characterClasses;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            foreach (ProgressionCharacterClass progressionClass in characterClasses)
            {
                if (progressionClass.characterClass == characterClass)
                {
                    var result = progressionClass.GetStatProgression(stat);
                    if (result == null)
                    {
                        return 0;
                    }
                    return result[level - 1];
                }
            }
            return 0;
        }

        [System.Serializable]
        private class ProgressionCharacterClass
        {
            [HideInInspector]
            public string className = "New Class";
            [SerializeField] public CharacterClass characterClass;
            [SerializeField] private ProgressionStatFormula[] statFormulas;
            private Dictionary<Stat, float[]> stats;

            public Dictionary<Stat, float[]> GetStats()
            {
                return stats;
            }
            public ProgressionStatFormula[] GetStatFormulas()
            {
                return statFormulas;
            }
            public float[] GetStatProgression(Stat stat)
            {
                if (stats == null)
                {
                    CalculateStatProgressions();
                }
                if (stats == null || !stats.ContainsKey(stat))
                {
                    //Debug.LogError("Invalid stat or stats not calculated yet.");
                    return null;
                }

                return stats[stat];
            }

            public void CalculateStatProgressions()
            {
                stats = new Dictionary<Stat, float[]>();

                foreach (ProgressionStatFormula formula in statFormulas)
                {
                    Stat stat = formula.stat;
                    float[] progression = new float[maxLevel];

                    for (int level = 1; level <= maxLevel; level++)
                    {
                        progression[level - 1] = formula.Calculate(level);
                        //Debug.Log($"{characterClass.ToString()}: {stat.ToString()} - Level {level}: {progression[level - 1]}");
                    }

                    stats.Add(stat, progression);
                }
            }
        }

        [System.Serializable]
        public class ProgressionStatFormula
        {
            [HideInInspector]
            public string statName = "New Stat";
            [SerializeField] public Stat stat;

            [Range(0, 1000)]
            [SerializeField] private float startingValue = 100;

            [Range(0, 1)]
            [SerializeField] private float percentageAdded = 0.0f;

            [Range(0, 1000)]
            [SerializeField] private float absoluteAdded = 10;

            public float Calculate(int level)
            {
                if (level <= 1) return startingValue;
                float c = Calculate(level - 1);
                return c + (c * percentageAdded) + absoluteAdded;
            }
        }

#if UNITY_EDITOR

        [CustomEditor(typeof(Progression))]
        public class ProgressionEditor : Editor
        {
            ProgressionEditor()
            {
                UnityEditor.EditorApplication.update += Refresh;
            }
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                Progression progression = (Progression)target;

                if (GUILayout.Button("Refresh"))
                {
                    Refresh();
                }
                if (GUILayout.Button("Calculate Progression"))
                {
                    CalculateProgression(progression);
                }
            }
            private void Refresh()
            {
                if (EditorApplication.isPlaying) return;
                Progression progression = (Progression)target;
                foreach (var characterClass in progression.characterClasses)
                {
                    characterClass.className = characterClass.characterClass.ToString();
                    foreach (var stat in characterClass.GetStatFormulas())
                    {
                        stat.statName = stat.stat.ToString();
                    }
                }
            }
            private void CalculateProgression(Progression progression)
            {

                foreach (var characterClass in progression.characterClasses)
                {
                    characterClass.CalculateStatProgressions();
                }
            }
        }

#endif
    }
}