using RPG.Control;
using RPG.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace RPG.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        private GameObject player;
        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        private void OnEnable()
        {
            if (player == null) return;
            Time.timeScale = 0.0f;
            player.GetComponent<PlayerController>().enabled = false;
        }
        private void OnDisable()
        {
            if (player == null) return;
            Time.timeScale = 1.0f;
            player.GetComponent<PlayerController>().enabled = true;
        }
        public void Save()
        {
            var savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();
        }
        public void Quit()
        {
            var savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();
            savingWrapper.LoadMenu();
        }
    }
}