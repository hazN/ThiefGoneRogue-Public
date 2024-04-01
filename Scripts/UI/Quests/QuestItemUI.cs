using RPG.Quests;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace RPG.UI.Quests
{
    public class QuestItemUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] TextMeshProUGUI progress;
        private QuestStatus status;
        public void Setup(QuestStatus status)
        {
            this.status = status;
            title.text = status.GetQuest().GetTitle();
            progress.text = status.GetCompletedObjectivesCount() + "/" + status.GetQuest().GetObjectiveCount();
        }
        public QuestStatus GetQuestStatus()
        {
            return status;
        }
    }
}