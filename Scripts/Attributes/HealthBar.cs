using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] GameObject healthBarImage = null;

        Vector3 _scalingVector;
        bool _healthBarEnabled = false;

        void Awake()
        {
            _scalingVector = new Vector3(1, 1, 1);
            healthBarImage.transform.localScale = _scalingVector;
            transform.GetChild(0).gameObject.SetActive(false);
        }

        public void UpdateHealthBar(GameObject character)
        {
            if (!_healthBarEnabled)
            {
                transform.GetChild(0).gameObject.SetActive(true);
                _healthBarEnabled = true;
            }
            _scalingVector = new Vector3(character.GetComponent<Health>().GetPercentage(), 1, 1);
            healthBarImage.transform.localScale = _scalingVector;
            if (_scalingVector.x <= 0)
            {
                DeactivateHealthBar();
            }
        }

        void DeactivateHealthBar()
        {
            transform.GetChild(0).gameObject.SetActive(false);
            _healthBarEnabled = false;
        }
    }
}