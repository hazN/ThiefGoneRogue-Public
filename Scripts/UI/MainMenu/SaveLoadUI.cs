using RPG.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.UI
{
    public class SaveLoadUI : MonoBehaviour
    {
        [SerializeField] private Transform contentRoot;
        [SerializeField] GameObject buttonPrefab;

        private void OnEnable()
        {
            PopulateSaveList();
        }
        private void PopulateSaveList()
        {
            // Get the saving wrapper
            var savingWrapper = FindObjectOfType<SavingWrapper>();
            if (!savingWrapper) return;
            // Clear the list
            foreach (Transform child in contentRoot)
            {
                Destroy(child.gameObject);
            }
            // Populate the list
            foreach (var save in savingWrapper.ListSaves())
            {
                var button = Instantiate(buttonPrefab, contentRoot);
                button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = save;
                button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => savingWrapper.LoadGame(save));
            }
        }
    }
}