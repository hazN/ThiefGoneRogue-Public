using RPG.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class ExperienceUI : MonoBehaviour
    {
        [SerializeField] private Slider experienceSlider;
        private Experience experience;
        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            experience = player.GetComponent<Experience>();
        }
        private void Update()
        {
            experienceSlider.value = experience.GetExperience() / experience.GetMaxExperience();
        }
    }
}