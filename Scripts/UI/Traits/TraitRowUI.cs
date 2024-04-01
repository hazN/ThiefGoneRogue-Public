using RPG.Stats;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace RPG.UI
{
    public class TraitRowUI : MonoBehaviour
    {
        [SerializeField] private Trait trait;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Button minusButton;
        [SerializeField] private Button plusButton;
        private TraitStore traitStore = null;

        private void Start()
        {
            traitStore = GameObject.FindWithTag("Player").GetComponent<TraitStore>();
            minusButton.onClick.AddListener(() => Allocate(-1));
            plusButton.onClick.AddListener(() => Allocate(1));
        }

        private void Update()
        {
            minusButton.interactable = traitStore.CanAssignPoints(trait, -1);
            plusButton.interactable = traitStore.CanAssignPoints(trait, 1);
            valueText.text = traitStore.GetProposedPoints(trait).ToString();
        }

        public void Allocate(int points)
        {
            if (Input.GetKey(KeyCode.LeftShift))
                points *= 5;
            traitStore.AssignPoints(trait, points);
        }
    }
}