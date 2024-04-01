using RPG.UI.Quests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestListUI : MonoBehaviour
    {
        public enum ShowQuestType { Active, Completed }
        [SerializeField] QuestItemUI questPrefab;
        private QuestList questList;
        private ShowQuestType showQuestType = ShowQuestType.Active;
        
        private void Start()
        {
            questList = GameObject.FindWithTag("Player").GetComponent<QuestList>();
            questList.onQuestListUpdated += Redraw;
            Redraw();
        }
        private void Redraw()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            foreach (QuestStatus status in questList.GetStatuses())
            {
                QuestItemUI item = null;

                // Show only active quests
                if (showQuestType == ShowQuestType.Active && !status.IsComplete())
                {
                    item = Instantiate<QuestItemUI>(questPrefab, transform);
                }
                // Show only completed quests
                if (showQuestType == ShowQuestType.Completed && status.IsComplete())
                {
                    item = Instantiate<QuestItemUI>(questPrefab, transform);
                }
                
                if (item != null)
                {
                    item.Setup(status);
                }
            }
        }
        private void SetQuestType(ShowQuestType showQuestType)
        {
            this.showQuestType = showQuestType;
            Redraw();
        }

        public void ShowActiveQuests()
        {
            SetQuestType(ShowQuestType.Active);
        }
        public void ShowCompletedQuests()
        {
            SetQuestType(ShowQuestType.Completed);
        }
    }
}