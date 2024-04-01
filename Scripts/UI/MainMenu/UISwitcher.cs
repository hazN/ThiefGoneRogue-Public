using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI
{
    public class UISwitcher : MonoBehaviour
    {
        [SerializeField] GameObject defaultDisplay = null;
        private void Start()
        {
            SwitchTo(defaultDisplay);
        }
        public void SwitchTo(GameObject ToDisplay)
        {
            if (ToDisplay.transform.parent != transform) return;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(child.gameObject == ToDisplay);
            }
        }
    }
}