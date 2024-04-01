using RPG.Core;
using RPG.Inventories;
using RPG.Saving;
using RPG.Stats;
using RPG.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        [SerializeField]private List<QuestStatus> statuses = new List<QuestStatus>();

        public event Action onQuestListUpdated;

        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest)) return;
            var newStatus = new QuestStatus(quest);
            statuses.Add(newStatus);
            NotificationTextSpawner.Spawn("Received Quest: " + quest.name);
            if (onQuestListUpdated != null)
            {
                onQuestListUpdated();
            }
        }

        public IEnumerable<QuestStatus> GetStatuses()
        {
            return statuses;
        }

        public bool HasQuest(Quest quest)
        {
            return GetQuestStatus(quest) != null;
        }

        public void CompleteObjective(Quest quest, string objective, int amountOfCompletions)
        {
            QuestStatus status = GetQuestStatus(quest);
            if (status != null)
            {
                // Check if objective is already complete
                if (status.IsComplete()) return;
                status.CompleteObjective(objective, amountOfCompletions);
                if (status.IsComplete())
                {
                    NotificationTextSpawner.Spawn("Completed Quest: " + quest.name);
                    GiveReward(status.GetQuest());
                }
                if (onQuestListUpdated != null)
                {
                    onQuestListUpdated();
                }
            }
        }

        private void GiveReward(Quest quest)
        {
            int exp = 0;
            foreach (var reward in quest.GetRewards())
            {
                NotificationTextSpawner.Spawn("Received Reward: " + reward.item.name);
                exp += reward.experience;
                if (reward.item.IsStackable())
                {
                    var added = GetComponent<Inventory>().AddToFirstEmptySlot(reward.item, reward.amount);

                    if (!added) GetComponent<ItemDropper>().DropItem(reward.item, reward.amount);
                }
                else // if not stackable give or drop several units
                {
                    var given = 0;

                    // add all possible to empty slots
                    for (var i = 1; i <= reward.amount; i++)
                    {
                        var added = GetComponent<Inventory>().AddToFirstEmptySlot(reward.item, i);

                        if (!added) break;

                        given++;
                    }

                    // if entire reward is given go to next reward.
                    if (given == reward.amount) continue;

                    // if given less then in reward, drop the difference
                    for (var i = 1; i <= reward.amount; i++)
                    {
                        GetComponent<ItemDropper>().DropItem(reward.item, i);
                    }
                }
            }
            GetComponent<Experience>().GainExperience(exp);
        }

        private QuestStatus GetQuestStatus(Quest quest)
        {
            foreach (var status in statuses)
            {
                if (status.GetQuest() == quest)
                {
                    return status;
                }
            }
            return null;
        }
        public int GetObjectiveCompletionAmount(Quest quest, string reference)
        {
            QuestStatus status = GetQuestStatus(quest);
            if (status == null) return 0;
            return status.GetObjectiveCompletionAmount(reference);
        }

        public object CaptureState()
        {
            List<object> state = new List<object>();
            foreach (var status in statuses)
            {
                state.Add(status.CaptureState());
            }
            return state;
        }

        public void RestoreState(object state)
        {
            List<object> stateList = state as List<object>;
            if (stateList == null) return;

            statuses.Clear();
            foreach (var objectState in stateList)
            {
                statuses.Add(new QuestStatus(objectState));
            }
            onQuestListUpdated?.Invoke();
        }

        public bool? Evaluate(EPredicate predicate, string[] parameters)
        {
            switch (predicate)
            {
                case EPredicate.HasQuest:
                    return HasQuest(Quest.GetByName(parameters[0]));
                case EPredicate.CompletedQuest:
                    QuestStatus status = GetQuestStatus(Quest.GetByName(parameters[0]));
                    if (status == null) return false;
                    return status.IsComplete();
                case EPredicate.CompletedObjective:
                    QuestStatus teststatus = GetQuestStatus(Quest.GetByName(parameters[0]));
                    if (teststatus == null) return false;
                    return teststatus.IsObjectiveComplete(parameters[1]);
            }
            return null;
        }
    }
}