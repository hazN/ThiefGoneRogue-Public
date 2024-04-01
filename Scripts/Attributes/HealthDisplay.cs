using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        private Health health;
        private TextMeshProUGUI text;
        private void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
            text = GetComponent<TextMeshProUGUI>();
        }
        private void Update()
        {
           text.text = string.Format("{0:0}/{1:0}", health.GetHealth(), health.GetMaxHealth());
        }
    }
}