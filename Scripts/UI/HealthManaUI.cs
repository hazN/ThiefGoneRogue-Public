using RPG.Attributes;
using RPG.Stats;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class HealthManaUI : MonoBehaviour
    {
        private Health health;
        private Mana mana;
        private BaseStats baseStats;
        [SerializeField] private Image healthFill;
        [SerializeField] private Image manaFill;
        [SerializeField] private TextMeshProUGUI levelText;
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            health = player.GetComponent<Health>();
            mana = player.GetComponent<Mana>();
            baseStats = player.GetComponent<BaseStats>();
        }
        private void Update()
        {
            healthFill.fillAmount = health.GetPercentage();
            manaFill.fillAmount = mana.GetMana() / mana.GetMaxMana();
            levelText.text = baseStats.GetLevel().ToString();
        }
    }
}