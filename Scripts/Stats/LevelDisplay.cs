using System.Collections;
using TMPro;
using UnityEngine;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        private BaseStats baseStats;
        private TextMeshProUGUI text;
        private void Awake()
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            text = GetComponent<TextMeshProUGUI>();
        }
        private void Update()
        {
            text.text = string.Format("{0}", baseStats.GetLevel());
        }
    }
}