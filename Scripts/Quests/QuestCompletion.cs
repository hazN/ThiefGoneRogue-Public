using RPG.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Quests
{
    public class QuestCompletion : MonoBehaviour
    {
        [SerializeField] Quest quest;
        [SerializeField] string objective;
        [SerializeField] [Min(1)] int amountOfCompletions = 1;
        public UnityEvent questCompletion;
        public void CompleteObjective()
        {
            QuestList questList = GameObject.FindWithTag("Player").GetComponent<QuestList>();
            // Check if quest is in quest list
            if (!questList.HasQuest(quest)) return;
            questList.CompleteObjective(quest, objective, amountOfCompletions);
            // Check if objective is complete
            if (questList.GetStatuses().Where(x => x.GetQuest() == quest).FirstOrDefault().IsObjectiveComplete(objective))
            {
                questCompletion.Invoke();
            }
        }
    }
}