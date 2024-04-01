using RPG.Quests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] Transform objectiveContainer;
        [SerializeField] GameObject objectivePrefab;
        [SerializeField] GameObject objectiveIncompletePrefab;
        [SerializeField] TextMeshProUGUI rewardText;
        [SerializeField] TextMeshProUGUI summaryText;
        public void Setup(QuestStatus quest)
        {
            title.text = quest.GetQuest().GetTitle();
            summaryText.text = quest.GetQuest().GetSummary();
            foreach (Transform child in objectiveContainer)
            {
                Destroy(child.gameObject);
            }
            foreach (var objective in quest.GetQuest().GetObjectives())
            {
                if (!PrecursorsComplete(quest, objective))
                    continue;
                GameObject prefab = objectiveIncompletePrefab;
                if (quest.IsObjectiveComplete(objective.reference))
                {
                    prefab = objectivePrefab;
                }
                GameObject objectiveInstance = Instantiate(prefab, objectiveContainer);
                TextMeshProUGUI objectiveText = objectiveInstance.GetComponentInChildren<TextMeshProUGUI>();
                objectiveText.text = objective.description;
                if (objective.requiredAmount > 1) 
                {
                    int objectiveCompletion = GameObject.FindWithTag("Player")?.GetComponent<QuestList>()?.GetObjectiveCompletionAmount(quest.GetQuest(), objective.reference) ?? 0;
                    objectiveText.text += " (" + objectiveCompletion + "/" + objective.requiredAmount + ")"; 
                }
            }
            rewardText.text = GetRewardText(quest.GetQuest());
        }

        private string GetRewardText(Quest quest)
        {
            string rewardText = "";
            int exp = 0;
            foreach (var reward in quest.GetRewards())
            {
                exp += reward.experience;
                if (rewardText != "")
                {
                    rewardText += ", ";
                }
                if (reward.amount > 1)
                {
                    rewardText += reward.amount + " ";
                }
                if (reward.item != null)
                {
                    rewardText += reward.item.GetDisplayName();
                }
                else if (reward.experience > 0)
                {
                    rewardText += reward.experience + " exp";
                }
            }
            if (rewardText == "")
            {
                rewardText = "No reward";
            }
            rewardText += ".";
            return exp + " exp, " + rewardText;
        }

        private bool PrecursorsComplete(QuestStatus quest, Quest.Objective objective)
        {
            for (int i = 0; i < objective.precursors.Count(); i++)
            {
                if (!quest.IsObjectiveComplete(objective.precursors[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}