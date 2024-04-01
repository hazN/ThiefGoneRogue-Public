using RPG.Attributes;
using TMPro;
using UnityEngine;

namespace RPG.Combat
{
    public class HealthDisplay : MonoBehaviour
    {
        private Fighter fighter;
        private TextMeshProUGUI text;

        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            text = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            Health health = fighter.GetTarget();
            if (!health) 
            { 
                text.text = "N/A";
                return;
            }
            text.text = string.Format("{0:0}/{1:0}", health.GetHealth(), health.GetMaxHealth());
        }
    }
}