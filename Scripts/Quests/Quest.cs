using JetBrains.Annotations;
using RPG.Inventories;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quest")]
    public class Quest : ScriptableObject
    {
        [TextArea(4, 10)] public string QuestSummary = "";
        [SerializeField] List<Objective> objectives = new List<Objective>();
        [SerializeField] List<Reward> rewards = new List<Reward>();

        [System.Serializable]
        public class Reward
        {
            public int amount;
            public InventoryItem item;
            public int experience = 0;
        }
        [System.Serializable]
        public class Objective
        {
            public string reference;
            public string description;
            [Min(1)] public int requiredAmount;
            public string[] precursors;
        }
        private void Awake()
        {
            
        }
        public string GetSummary()
        {
            return QuestSummary;
        }
        public string GetTitle()
        {
            return name;
        }
        public int GetObjectiveCount()
        {
            return objectives.Count;
        }
        public IEnumerable<Objective> GetObjectives()
        {
            return objectives;
        }
        public IEnumerable<Reward> GetRewards()
        {
            return rewards;
        }
        public Objective GetObjective(string reference)
        {
            foreach (Objective objective in objectives)
            {
                if (objective.reference == reference)
                {
                    return objective;
                }
            }
            return null;
        }
        public bool CompleteObjective(string reference, int amountOfCompletions)
        {
            foreach (Objective objective in objectives)
            {
                if (objective.reference == reference)
                {
                    return amountOfCompletions >= objective.requiredAmount;
                }
            }
            return false;
        }
        public string GetObjectiveDescription(string reference)
        {
            foreach (Objective objective in objectives)
            {
                if (objective.reference == reference)
                {
                    return objective.description;
                }
            }
            return "";
        }
        public bool HasObjective(string reference)
        {
           foreach (Objective objective in objectives)
            {
                if (objective.reference == reference)
                {
                    return true;
                }
            }
            return false;
        }
        public int GetObjectiveRequiredCompletion(string reference)
        {
            foreach (Objective objective in objectives)
            {
                if (objective.reference == reference)
                {
                    return objective.requiredAmount;
                }
            }
            return -1;
        }

        private static Dictionary<string, Quest> masterQuestDictionary;

        public static Quest GetByName(string questName)
        {
            PopulateQuestDictionary();
            return masterQuestDictionary.ContainsKey(questName) ? masterQuestDictionary[questName] : null;
        }

        public static void PopulateQuestDictionary()
        {
            if (masterQuestDictionary == null)
            {
                masterQuestDictionary = new Dictionary<string, Quest>();
                foreach (Quest quest in Resources.LoadAll<Quest>(""))
                {
                    if (masterQuestDictionary.ContainsKey(quest.name)) Debug.Log($"There are two {quest.name} quests in the system");
                    else masterQuestDictionary.Add(quest.name, quest);
                }
            }
        }
    }
}