using RPG.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestStatus 
    {
        private Quest quest;
        //private List<string> completedObjectives = new List<string>();
        private Dictionary<string, int> completedObjectives = new Dictionary<string, int>();
        [System.Serializable]
        private class QuestStatusRecord
        {
            public string questName;
            public Dictionary<string, int> completedObjectives = new Dictionary<string, int>();
        }
        public QuestStatus(Quest quest)
        {
            this.quest = quest;
        }

        public QuestStatus(object objectState)
        {
            QuestStatusRecord record = objectState as QuestStatusRecord;
            quest = Quest.GetByName(record.questName);
            completedObjectives = record.completedObjectives;
        }

        public Quest GetQuest()
        {
            return quest;
        }

        public int GetCompletedObjectivesCount()
        {
            int count = 0;
            foreach (var objective in quest.GetObjectives())
            {
                if (IsObjectiveComplete(objective.reference))
                {
                    count++;
                }
            }
            return count;
        }

        public bool IsObjectiveComplete(string objective)
        {
            return GetObjective(objective) >= quest.GetObjectiveRequiredCompletion(objective);
        }

        public void CompleteObjective(string objective, int amountOfCompletions)
        {
            if (GetObjective(objective) != -1)
            {
                Quest.Objective theObjective = quest.GetObjective(objective);
                for (int i = 0; i < theObjective.precursors.Count(); i++)
                {
                    if (!IsObjectiveComplete(theObjective.precursors[i]))
                    {
                        return;
                    }
                }
                completedObjectives[objective] += amountOfCompletions;
                if (quest.CompleteObjective(objective, amountOfCompletions))
                {
                    NotificationTextSpawner.Spawn("Completed Objective: " + quest.GetObjectiveDescription(objective));
                }
            }
        }

        public object CaptureState()
        {
            QuestStatusRecord record = new QuestStatusRecord();
            record.questName = quest.name;
            record.completedObjectives = completedObjectives;
            return record;
        }

        public bool IsComplete()
        {
            foreach (var objective in quest.GetObjectives())
            {
                if (!IsObjectiveComplete(objective.reference))
                {
                    return false;
                }
            }
            return true;
        }

        public int GetObjectiveCompletionAmount(string reference)
        {
            return completedObjectives[reference];
        }
        private int GetObjective(string reference)
        {
            if (completedObjectives.ContainsKey(reference))
            {
                return completedObjectives[reference];
            }
            else if (quest.HasObjective(reference))
            {
                completedObjectives.Add(reference, 0);
                return completedObjectives[reference];
            }
            return -1;
        }
    }
}