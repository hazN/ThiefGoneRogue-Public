using System.Collections;
using TMPro;
using UnityEngine;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        private Experience experience;
        private BaseStats stats;
        private TextMeshProUGUI text;
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            experience = player.GetComponent<Experience>();
            stats = player.GetComponent<BaseStats>();
            text = GetComponent<TextMeshProUGUI>();
        }
        private void Update()
        {
            text.text = string.Format("{0:0}/{1:0}", experience.GetExperience(), stats.GetStat(Stat.ExperienceToLevelUp));
        }
    }
}