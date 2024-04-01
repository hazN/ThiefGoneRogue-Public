using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class TraitUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI unassignedPointsText;
        [SerializeField] Button commitButton;
        TraitStore traitStore = null;

        private void Start()
        {
            traitStore = GameObject.FindWithTag("Player").GetComponent<TraitStore>();
            commitButton.onClick.AddListener(traitStore.Commit);
        }
        private void Update()
        {
            unassignedPointsText.text = traitStore.GetUnassignedPoints().ToString();
        }
    }
}